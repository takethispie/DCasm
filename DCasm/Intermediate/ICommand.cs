using System;
namespace DCasm
{
    public interface ICommand
    {
        ICommand Next { get; set; }

        bool IsFinal { get; set; }
        void Execute();
    }
}
