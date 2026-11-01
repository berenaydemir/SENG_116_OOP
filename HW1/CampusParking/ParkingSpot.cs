using System;

namespace CampusParking;

public class ParkingSpot
{
    public int Number { get; }
    public Vehicle? Occupant { get; private set; }

    public ParkingSpot(int number)
    {
        if (number < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(number), "Spot number must be at least 1.");
        }

        Number = number;
        Occupant = null;
    }

    public bool IsFree()
    {
        return Occupant == null;
    }

    public bool TryPark(Vehicle vehicle)
    {
        if (IsFree())
        {
            Occupant = vehicle;
            return true;
        }
        return false;
    }

    public Vehicle? Leave()
    {
        if (Occupant == null)
        {
            return null;
        }

        Vehicle leaving = Occupant;
        Occupant = null;
        return leaving;
    }

    public string Describe()
    {
        if (IsFree())
        {
            return $"Spot {Number}: free";
        }
        return $"Spot {Number}: {Occupant!.Plate}";
    }
}