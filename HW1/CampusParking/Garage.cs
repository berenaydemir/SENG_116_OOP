using System;
using System.Collections.Generic;

namespace CampusParking;

public class Garage
{
    public string Name { get; }
    public int Capacity { get; }
    private readonly ParkingSpot[] _spots;
    private readonly List<Ticket> _allTickets;
    private int _nextTicketNumber;

    public Garage(string name, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Garage name cannot be empty.", nameof(name));
        }

        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be at least 1.");
        }

        Name = name.Trim();
        Capacity = capacity;
        _spots = new ParkingSpot[capacity];
        for (int i = 0; i < capacity; i++)
        {
            _spots[i] = new ParkingSpot(i + 1);
        }

        _allTickets = new List<Ticket>();
        _nextTicketNumber = 301;
    }

    public ParkingSpot GetSpot(int spotNumber)
    {
        if (spotNumber < 1 || spotNumber > Capacity)
        {
            throw new ArgumentOutOfRangeException(nameof(spotNumber), "Invalid spot number.");
        }

        return _spots[spotNumber - 1];
    }

    public ParkingSpot? FindSpotOf(string plate)
    {
        string normalized = Vehicle.NormalizePlate(plate);
        for (int i = 0; i < _spots.Length; i++)
        {
            if (!_spots[i].IsFree() && _spots[i].Occupant!.Plate == normalized)
            {
                return _spots[i];
            }
        }
        return null;
    }

    public int FreeSpotCount()
    {
        int count = 0;
        for (int i = 0; i < _spots.Length; i++)
        {
            if (_spots[i].IsFree())
            {
                count++;
            }
        }
        return count;
    }

    public Ticket? Enter(Vehicle vehicle, int entryMinute)
    {
        if (vehicle == null)
        {
            throw new ArgumentNullException(nameof(vehicle));
        }

        if (entryMinute < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(entryMinute), "Entry minute cannot be negative.");
        }

        if (FindSpotOf(vehicle.Plate) != null)
        {
            return null;
        }

        for (int i = _spots.Length - 1; i >= 0; i--)
        {
            if (_spots[i].IsFree())
            {
                _spots[i].TryPark(vehicle);
                var ticket = new Ticket(_nextTicketNumber++, vehicle, _spots[i], entryMinute);
                _allTickets.Add(ticket);
                return ticket;
            }
        }

        return null;
    }

    public decimal Exit(Ticket ticket, int exitMinute)
    {
        if (ticket == null)
        {
            throw new ArgumentNullException(nameof(ticket));
        }

        if (!_allTickets.Contains(ticket))
        {
            throw new ArgumentException("This ticket does not belong to this garage.", nameof(ticket));
        }

        if (!ticket.IsOpen)
        {
            throw new InvalidOperationException("Ticket is already closed.");
        }

        ticket.Close(exitMinute);
        ticket.Spot.Leave();

        return ticket.Fee;
    }

    public string Describe()
    {
        return $"{Name}: {FreeSpotCount()} of {Capacity} spots free";
    }
}