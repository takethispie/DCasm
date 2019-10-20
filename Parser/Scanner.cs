using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Token
    {
        public int charPos; // token position in characters in the source text (starting at 0)
        public int col; // token column (starting at 1)
        public int kind; // token kind
        public int line; // token line (starting at 1)
        public Token next; // ML 2005-03-11 Tokens are kept in linked list
        public int pos; // token position in bytes in the source text (starting at 0)
        public string val; // token value
    }

//-----------------------------------------------------------------------------------
// Buffer
//-----------------------------------------------------------------------------------
    public class Buffer
    {
        // This Buffer supports the following cases:
        // 1) seekable stream (file)
        //    a) whole stream in buffer
        //    b) part of stream in buffer
        // 2) non seekable stream (network, console)

        public const int EOF = char.MaxValue + 1;
        private const int MIN_BUFFER_LENGTH = 1024; // 1KB
        private const int MAX_BUFFER_LENGTH = MIN_BUFFER_LENGTH * 64; // 64KB
        private byte[] buf; // input buffer
        private int bufLen; // length of buffer
        private int bufPos; // current position in buffer
        private int bufStart; // position of first byte in buffer relative to input stream
        private int fileLen; // length of input stream (may change if the stream is no file)
        private readonly bool isUserStream; // was the stream opened by the user?
        private Stream stream; // input stream (seekable)

        public Buffer(Stream s, bool isUserStream)
        {
            stream = s;
            this.isUserStream = isUserStream;

            if (stream.CanSeek)
            {
                fileLen = (int) stream.Length;
                bufLen = Math.Min(fileLen, MAX_BUFFER_LENGTH);
                bufStart = int.MaxValue; // nothing in the buffer so far
            }
            else
            {
                fileLen = bufLen = bufStart = 0;
            }

            buf = new byte[bufLen > 0 ? bufLen : MIN_BUFFER_LENGTH];
            if (fileLen > 0) Pos = 0; // setup buffer to position 0 (start)
            else bufPos = 0; // index 0 is already after the file, thus Pos = 0 is invalid
            if (bufLen == fileLen && stream.CanSeek) Close();
        }

        protected Buffer(Buffer b)
        {
            // called in UTF8Buffer constructor
            buf = b.buf;
            bufStart = b.bufStart;
            bufLen = b.bufLen;
            fileLen = b.fileLen;
            bufPos = b.bufPos;
            stream = b.stream;
            // keep destructor from closing the stream
            b.stream = null;
            isUserStream = b.isUserStream;
        }

        public int Pos
        {
            get => bufPos + bufStart;
            set
            {
                if (value >= fileLen && stream != null && !stream.CanSeek)
                    while (value >= fileLen && ReadNextStreamChunk() > 0)
                        ;

                if (value < 0 || value > fileLen)
                    throw new FatalError("buffer out of bounds access, position: " + value);

                if (value >= bufStart && value < bufStart + bufLen)
                {
                    // already in buffer
                    bufPos = value - bufStart;
                }
                else if (stream != null)
                {
                    // must be swapped in
                    stream.Seek(value, SeekOrigin.Begin);
                    bufLen = stream.Read(buf, 0, buf.Length);
                    bufStart = value;
                    bufPos = 0;
                }
                else
                {
                    // set the position to the end of the file, Pos will return fileLen.
                    bufPos = fileLen - bufStart;
                }
            }
        }

        ~Buffer()
        {
            Close();
        }

        protected void Close()
        {
            if (!isUserStream && stream != null)
            {
                stream.Close();
                stream = null;
            }
        }

        public virtual int Read()
        {
            if (bufPos < bufLen) return buf[bufPos++];

            if (Pos < fileLen)
            {
                Pos = Pos; // shift buffer start to Pos
                return buf[bufPos++];
            }

            if (stream != null && !stream.CanSeek && ReadNextStreamChunk() > 0)
                return buf[bufPos++];
            return EOF;
        }

        public int Peek()
        {
            var curPos = Pos;
            var ch = Read();
            Pos = curPos;
            return ch;
        }

        // beg .. begin, zero-based, inclusive, in byte
        // end .. end, zero-based, exclusive, in byte
        public string GetString(int beg, int end)
        {
            var len = 0;
            var buf = new char[end - beg];
            var oldPos = Pos;
            Pos = beg;
            while (Pos < end) buf[len++] = (char) Read();
            Pos = oldPos;
            return new string(buf, 0, len);
        }

        // Read the next chunk of bytes from the stream, increases the buffer
        // if needed and updates the fields fileLen and bufLen.
        // Returns the number of bytes read.
        private int ReadNextStreamChunk()
        {
            var free = buf.Length - bufLen;
            if (free == 0)
            {
                // in the case of a growing input stream
                // we can neither seek in the stream, nor can we
                // foresee the maximum length, thus we must adapt
                // the buffer size on demand.
                var newBuf = new byte[bufLen * 2];
                Array.Copy(buf, newBuf, bufLen);
                buf = newBuf;
                free = bufLen;
            }

            var read = stream.Read(buf, bufLen, free);
            if (read > 0)
            {
                fileLen = bufLen = bufLen + read;
                return read;
            }

            // end of stream reached
            return 0;
        }
    }

//-----------------------------------------------------------------------------------
// UTF8Buffer
//-----------------------------------------------------------------------------------
    public class UTF8Buffer : Buffer
    {
        public UTF8Buffer(Buffer b) : base(b)
        {
        }

        public override int Read()
        {
            int ch;
            do
            {
                ch = base.Read();
                // until we find a utf8 start (0xxxxxxx or 11xxxxxx)
            } while (ch >= 128 && (ch & 0xC0) != 0xC0 && ch != EOF);

            if (ch < 128 || ch == EOF)
            {
                // nothing to do, first 127 chars are the same in ascii and utf8
                // 0xxxxxxx or end of file character
            }
            else if ((ch & 0xF0) == 0xF0)
            {
                // 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx
                var c1 = ch & 0x07;
                ch = base.Read();
                var c2 = ch & 0x3F;
                ch = base.Read();
                var c3 = ch & 0x3F;
                ch = base.Read();
                var c4 = ch & 0x3F;
                ch = (((((c1 << 6) | c2) << 6) | c3) << 6) | c4;
            }
            else if ((ch & 0xE0) == 0xE0)
            {
                // 1110xxxx 10xxxxxx 10xxxxxx
                var c1 = ch & 0x0F;
                ch = base.Read();
                var c2 = ch & 0x3F;
                ch = base.Read();
                var c3 = ch & 0x3F;
                ch = (((c1 << 6) | c2) << 6) | c3;
            }
            else if ((ch & 0xC0) == 0xC0)
            {
                // 110xxxxx 10xxxxxx
                var c1 = ch & 0x1F;
                ch = base.Read();
                var c2 = ch & 0x3F;
                ch = (c1 << 6) | c2;
            }

            return ch;
        }
    }

//-----------------------------------------------------------------------------------
// Scanner
//-----------------------------------------------------------------------------------
    public class Scanner
    {
        private const char EOL = '\n';
        private const int eofSym = 0; /* pdt */
        private const int maxT = 29;
        private const int noSym = 29;
        private static readonly Dictionary<int, int> start; // maps first token character to start state


        public Buffer buffer; // scanner buffer
        private int ch; // current input character
        private int charPos; // position by unicode characters starting with 0
        private int col; // column number of current character
        private int line; // line number of current character
        private int oldEols; // EOLs that appeared in a comment;
        private int pos; // byte position of current character
        private Token pt; // current peek token

        private Token t; // current token
        private int tlen; // length of current token

        private Token tokens; // list of tokens already peeked (first token is a dummy)

        private char[] tval = new char[128]; // text of current token

        static Scanner()
        {
            start = new Dictionary<int, int>(128);
            for (var i = 65; i <= 90; ++i) start[i] = 1;
            for (var i = 97; i <= 122; ++i) start[i] = 1;
            for (var i = 48; i <= 57; ++i) start[i] = 4;
            start[36] = 5;
            start[62] = 12;
            start[60] = 13;
            start[61] = 8;
            start[Buffer.EOF] = -1;
        }

        public Scanner(string fileName)
        {
            try
            {
                Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                buffer = new Buffer(stream, false);
                Init();
            }
            catch (IOException)
            {
                throw new FatalError("Cannot open file " + fileName);
            }
        }

        public Scanner(Stream s)
        {
            buffer = new Buffer(s, true);
            Init();
        }

        private void Init()
        {
            pos = -1;
            line = 1;
            col = 0;
            charPos = -1;
            oldEols = 0;
            NextCh();
            if (ch == 0xEF)
            {
                // check optional byte order mark for UTF-8
                NextCh();
                var ch1 = ch;
                NextCh();
                var ch2 = ch;
                if (ch1 != 0xBB || ch2 != 0xBF)
                    throw new FatalError(string.Format("illegal byte order mark: EF {0,2:X} {1,2:X}", ch1, ch2));
                buffer = new UTF8Buffer(buffer);
                col = 0;
                charPos = -1;
                NextCh();
            }

            pt = tokens = new Token(); // first token is a dummy
        }

        private void NextCh()
        {
            if (oldEols > 0)
            {
                ch = EOL;
                oldEols--;
            }
            else
            {
                pos = buffer.Pos;
                // buffer reads unicode chars, if UTF8 has been detected
                ch = buffer.Read();
                col++;
                charPos++;
                // replace isolated '\r' by '\n' in order to make
                // eol handling uniform across Windows, Unix and Mac
                if (ch == '\r' && buffer.Peek() != '\n') ch = EOL;
                if (ch == EOL)
                {
                    line++;
                    col = 0;
                }
            }
        }

        private void AddCh()
        {
            if (tlen >= tval.Length)
            {
                var newBuf = new char[2 * tval.Length];
                Array.Copy(tval, 0, newBuf, 0, tval.Length);
                tval = newBuf;
            }

            if (ch != Buffer.EOF)
            {
                tval[tlen++] = (char) ch;
                NextCh();
            }
        }


        private bool Comment0()
        {
            int level = 1, pos0 = pos, line0 = line, col0 = col, charPos0 = charPos;
            NextCh();
            if (ch == '/')
            {
                NextCh();
                for (;;)
                    if (ch == 10)
                    {
                        level--;
                        if (level == 0)
                        {
                            oldEols = line - line0;
                            NextCh();
                            return true;
                        }

                        NextCh();
                    }
                    else if (ch == Buffer.EOF)
                    {
                        return false;
                    }
                    else
                    {
                        NextCh();
                    }
            }

            buffer.Pos = pos0;
            NextCh();
            line = line0;
            col = col0;
            charPos = charPos0;
            return false;
        }

        private bool Comment1()
        {
            int level = 1, pos0 = pos, line0 = line, col0 = col, charPos0 = charPos;
            NextCh();
            if (ch == '*')
            {
                NextCh();
                for (;;)
                    if (ch == '*')
                    {
                        NextCh();
                        if (ch == '/')
                        {
                            level--;
                            if (level == 0)
                            {
                                oldEols = line - line0;
                                NextCh();
                                return true;
                            }

                            NextCh();
                        }
                    }
                    else if (ch == '/')
                    {
                        NextCh();
                        if (ch == '*')
                        {
                            level++;
                            NextCh();
                        }
                    }
                    else if (ch == Buffer.EOF)
                    {
                        return false;
                    }
                    else
                    {
                        NextCh();
                    }
            }

            buffer.Pos = pos0;
            NextCh();
            line = line0;
            col = col0;
            charPos = charPos0;
            return false;
        }


        private void CheckLiteral()
        {
            switch (t.val)
            {
                case "program":
                    t.kind = 4;
                    break;
                case "module":
                    t.kind = 5;
                    break;
                case "import":
                    t.kind = 6;
                    break;
                case "li":
                    t.kind = 7;
                    break;
                case "lui":
                    t.kind = 8;
                    break;
                case "add":
                    t.kind = 9;
                    break;
                case "sub":
                    t.kind = 10;
                    break;
                case "div":
                    t.kind = 11;
                    break;
                case "mul":
                    t.kind = 12;
                    break;
                case "mov":
                    t.kind = 13;
                    break;
                case "out":
                    t.kind = 14;
                    break;
                case "in":
                    t.kind = 15;
                    break;
                case "lw":
                    t.kind = 16;
                    break;
                case "sw":
                    t.kind = 17;
                    break;
                case "function":
                    t.kind = 18;
                    break;
                case "end":
                    t.kind = 19;
                    break;
                case "call":
                    t.kind = 20;
                    break;
                case "if":
                    t.kind = 21;
                    break;
                case "then":
                    t.kind = 22;
                    break;
                case "else":
                    t.kind = 23;
                    break;
            }
        }

        private Token NextToken()
        {
            while (ch == ' ' ||
                   ch >= 9 && ch <= 10 || ch == 13
            ) NextCh();
            if (ch == '/' && Comment0() || ch == '/' && Comment1()) return NextToken();
            var recKind = noSym;
            var recEnd = pos;
            t = new Token();
            t.pos = pos;
            t.col = col;
            t.line = line;
            t.charPos = charPos;
            int state;
            state = start.ContainsKey(ch) ? start[ch] : 0;
            tlen = 0;
            AddCh();

            switch (state)
            {
                case -1:
                {
                    t.kind = eofSym;
                    break;
                } // NextCh already done
                case 0:
                {
                    if (recKind != noSym)
                    {
                        tlen = recEnd - t.pos;
                        SetScannerBehindT();
                    }

                    t.kind = recKind;
                    break;
                } // NextCh already done
                case 1:
                    recEnd = pos;
                    recKind = 1;
                    if (ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z')
                    {
                        AddCh();
                        goto case 1;
                    }
                    else if (ch == '.')
                    {
                        AddCh();
                        goto case 2;
                    }
                    else
                    {
                        t.kind = 1;
                        t.val = new string(tval, 0, tlen);
                        CheckLiteral();
                        return t;
                    }
                case 2:
                    if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z')
                    {
                        AddCh();
                        goto case 3;
                    }
                    else
                    {
                        goto case 0;
                    }
                case 3:
                    recEnd = pos;
                    recKind = 1;
                    if (ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z')
                    {
                        AddCh();
                        goto case 3;
                    }
                    else
                    {
                        t.kind = 1;
                        t.val = new string(tval, 0, tlen);
                        CheckLiteral();
                        return t;
                    }
                case 4:
                    recEnd = pos;
                    recKind = 2;
                    if (ch >= '0' && ch <= '9')
                    {
                        AddCh();
                        goto case 4;
                    }
                    else
                    {
                        t.kind = 2;
                        break;
                    }
                case 5:
                    if (ch >= '4' && ch <= '9')
                    {
                        AddCh();
                        goto case 6;
                    }
                    else if (ch >= '0' && ch <= '3')
                    {
                        AddCh();
                        goto case 7;
                    }
                    else
                    {
                        goto case 0;
                    }
                case 6:
                {
                    t.kind = 3;
                    break;
                }
                case 7:
                    recEnd = pos;
                    recKind = 3;
                    if (ch >= '0' && ch <= '9')
                    {
                        AddCh();
                        goto case 6;
                    }
                    else
                    {
                        t.kind = 3;
                        break;
                    }
                case 8:
                    if (ch == '=')
                    {
                        AddCh();
                        goto case 9;
                    }
                    else
                    {
                        goto case 0;
                    }
                case 9:
                {
                    t.kind = 26;
                    break;
                }
                case 10:
                {
                    t.kind = 27;
                    break;
                }
                case 11:
                {
                    t.kind = 28;
                    break;
                }
                case 12:
                    recEnd = pos;
                    recKind = 24;
                    if (ch == '=')
                    {
                        AddCh();
                        goto case 10;
                    }
                    else
                    {
                        t.kind = 24;
                        break;
                    }
                case 13:
                    recEnd = pos;
                    recKind = 25;
                    if (ch == '=')
                    {
                        AddCh();
                        goto case 11;
                    }
                    else
                    {
                        t.kind = 25;
                        break;
                    }
            }

            t.val = new string(tval, 0, tlen);
            return t;
        }

        private void SetScannerBehindT()
        {
            buffer.Pos = t.pos;
            NextCh();
            line = t.line;
            col = t.col;
            charPos = t.charPos;
            for (var i = 0; i < tlen; i++) NextCh();
        }

        // get the next token (possibly a token already seen during peeking)
        public Token Scan()
        {
            if (tokens.next == null)
            {
                return NextToken();
            }

            pt = tokens = tokens.next;
            return tokens;
        }

        // peek for the next token, ignore pragmas
        public Token Peek()
        {
            do
            {
                if (pt.next == null) pt.next = NextToken();
                pt = pt.next;
            } while (pt.kind > maxT); // skip pragmas

            return pt;
        }

        // make sure that peeking starts at the current scan position
        public void ResetPeek()
        {
            pt = tokens;
        }
    } // end Scanner
}