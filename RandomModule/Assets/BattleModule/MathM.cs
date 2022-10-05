public static partial class MathM
{
    public static float Min(float a, float b)
    {
        return a < b ? a : b;
    }

    public static float Max(float a, float b)
    {
        return a > b ? a : b;
    }

    public static float Mid3(float a, float b, float c)
    {
        if(a >= b)
            if(b >= c)
                return b;
            else if(a <= c)
                return a;
            else
                return c;
        else if(c < a)
            return a;
        else if(b > c)
            return c;
        else
            return b;
    }
}