using System;

namespace CampusParking;

public static class Tariff
{
    public const int FreeMinutes = 30;
    public const int UnitMinutes = 60;
    public const decimal UnitRate = 30m;
    public const decimal MaxFee = 140m;

    public static decimal FeeFor(int minutes)
    {
        if (minutes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minutes), "Stay minutes cannot be negative.");
        }

        if (minutes <= FreeMinutes)
        {
            return 0m;
        }

        int units = (minutes + UnitMinutes - 1) / UnitMinutes;
        decimal fee = units * UnitRate;

        if (fee > MaxFee)
        {
            return MaxFee;
        }

        return fee;
    }
}