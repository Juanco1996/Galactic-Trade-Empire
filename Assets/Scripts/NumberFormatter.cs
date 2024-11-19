
public static class NumberFormatter
{
    public static string FormatNumber(float number)
    {
        const float tolerance = 0.0001f; // Tolerance for floating-point comparison

        if (UnityEngine.Mathf.Abs(number) < 1_000_000f)
        {
            if (UnityEngine.Mathf.Abs(number % 1) < tolerance)
            {
                return number.ToString("N0");
            }
            else
            {
                return number.ToString("N2");
            }
        }
        else
        {
            return number.ToString("0.##E+0");
        }
    }

    // Overload for double precision numbers
    public static string FormatNumber(double number)
    {
        return FormatNumber((float)number);
    }
}