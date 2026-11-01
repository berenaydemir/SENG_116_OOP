using HW0.App;
using Xunit;

namespace HW0.Tests;

public class NumberToolsTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(7, 7)]
    [InlineData(12345, 15)]
    [InlineData(-907, 16)]
    public void DigitSum_ReturnsSumOfDigits(long number, int expected)
    {
        Assert.Equal(expected, NumberTools.DigitSum(number));
    }

    [Theory]
    [InlineData(-5, false)]
    [InlineData(0, false)]
    [InlineData(1, false)]
    [InlineData(2, true)]
    [InlineData(9, false)]
    [InlineData(97, true)]
    public void IsPrime_DetectsPrimes(int n, bool expected)
    {
        Assert.Equal(expected, NumberTools.IsPrime(n));
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 1)]
    [InlineData(10, 4)]
    [InlineData(100, 25)]
    public void CountPrimes_CountsPrimesUpToBound(int upperBound, int expected)
    {
        Assert.Equal(expected, NumberTools.CountPrimes(upperBound));
    }
}