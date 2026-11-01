using System;

namespace CampusParking;

public class Vehicle
{
    public string Plate { get; }
    public string Owner { get; }

    public Vehicle(string plate, string owner)
    {
        if (string.IsNullOrWhiteSpace(plate))
        {
            throw new ArgumentException("Plate cannot be empty.", nameof(plate));
        }

        if (string.IsNullOrWhiteSpace(owner))
        {
            throw new ArgumentException("Owner cannot be empty.", nameof(owner));
        }

        Plate = NormalizePlate(plate);
        Owner = owner.Trim();
    }

    public static string NormalizePlate(string plate)
    {
        if (string.IsNullOrWhiteSpace(plate))
        {
            throw new ArgumentException("Plate cannot be empty.", nameof(plate));
        }

        return plate.Replace(" ", "").ToUpperInvariant();
    }

    public string Describe()
    {
        return $"{Plate} ({Owner})";
    }
}