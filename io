module
{
    function DrawPixel
        set $13 2
        lsh $14 $13 16
        add $14 $14 2

        out $11 $14
        add $14 $14 1
        out $12 $14
        add $14 $14 1
        out $10 $14
        sub $14 $14 3
        out $10 $14
    end
}