namespace CubeTrainer.Cube.Kociemba.Common;

internal static class Utils
{
    public static int NChooseK(int n, int k)
    {
        return Factorial(n) / Factorial(k) / Factorial(n - k);
    }

    private static int Factorial(int n)
    {
        if (n <= 1)
        {
            return 1;
        }

        var result = 1;
        for (int i = 2; i <= n; i++)
        {
            result *= i;
        }

        return result;
    }
}