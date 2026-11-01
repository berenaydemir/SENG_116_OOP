using System;

namespace CampusParking;

public class Ticket
{
    public int Number { get; }
    public Vehicle Vehicle { get; }
    public ParkingSpot Spot { get; }
    public int EntryMinute { get; }
    public bool IsOpen { get; private set; }
    public decimal Fee { get; private set; }

    // Dışarıdan doğrudan oluşturulamaz, yalnızca kütüphane içinden (Garage) oluşturulabilir
    internal Ticket(int number, Vehicle vehicle, ParkingSpot spot, int entryMinute)
    {
        if (number < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(number), "Ticket number must be positive.");
        }

        if (entryMinute < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(entryMinute), "Entry minute cannot be negative.");
        }

        Number = number;
        Vehicle = vehicle ?? throw new ArgumentNullException(nameof(vehicle));
        Spot = spot ?? throw new ArgumentNullException(nameof(spot));
        EntryMinute = entryMinute;
        IsOpen = true;
        Fee = 0m;
    }

    internal void Close(int exitMinute)
    {
        if (!IsOpen)
        {
            throw new InvalidOperationException("Ticket is already closed.");
        }

        if (exitMinute < EntryMinute)
        {
            throw new ArgumentOutOfRangeException(nameof(exitMinute), "Exit minute cannot be earlier than entry minute.");
        }

        int duration = exitMinute - EntryMinute;
        Fee = Tariff.FeeFor(duration);
        IsOpen = false;
    }
}