namespace HW0.App;

public static class NumberTools
{
    public static int DigitSum(long number)
    {
        long n = Math.Abs(number);
        int sum = 0;
        while (n > 0)
        {
            sum += (int)(n % 10);
            n /= 10;
        }
        return sum;
    }

    public static bool IsPrime(int number)
    {
        if (number <= 1) return false;
        if (number == 2) return true;
        if (number % 2 == 0) return false;

        for (int i = 3; i * i <= number; i += 2)
        {
            if (number % i == 0) return false;
        }
        return true;
    }

    public static int CountPrimes(int upperBound)
    {
        if (upperBound < 2) return 0;

        int count = 0;
        for (int i = 2; i <= upperBound; i++)
        {
            if (IsPrime(i))
            {
                count++;
            }
        }
        return count;
    }
}
