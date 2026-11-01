namespace CampusParking;

public class ParkingGarage
{
    public readonly int Capacity;
    private readonly ParkingSpot[] _spots;

    public ParkingGarage(int capacity)
    {
        Capacity = capacity;
        _spots = new ParkingSpot[capacity];
        for (int i = 0; i < capacity; i++)
        {
            _spots[i] = new ParkingSpot(i + 1);
        }
    }

    public ParkingSpot? FindSpotForVehicle(string plate)
    {
        string cleanedPlate = plate.Replace(" ", "").ToUpperInvariant();
        for (int i = 0; i < _spots.Length; i++)
        {
            if (!_spots[i].IsFree() && _spots[i].Occupant!.Plate == cleanedPlate)
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
}