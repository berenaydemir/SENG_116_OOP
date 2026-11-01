using CampusParking;
namespace CampusParking.Tests;

// HW1 Part A (Week 2): classes, objects, constructors, identity.
// Given test file: do not modify.
public class PartATests
{
    private static readonly ParameterCard Card = ParameterCard.Mine;

    [Fact]
    public void A00_StudentNumber_IsSetAndParameterCardIsWritten()
    {
        Assert.True(ParameterCard.IsValidNumber(StudentInfo.Number),
            "Write your student number (digits only) in CampusParking/StudentInfo.cs.");

        string? folder = AppContext.BaseDirectory;
        while (folder != null && !File.Exists(Path.Combine(folder, "HW1.slnx")))
        {
            folder = Path.GetDirectoryName(folder);
        }
        Assert.NotNull(folder);   // HW1.slnx must be in the folder above the test project
        File.WriteAllText(Path.Combine(folder, "MyParameters.txt"), Card.ToString());
    }

    [Fact]
    public void Vehicle_StoresPlateAndOwner()
    {
        Vehicle car = new Vehicle("06 ABC 123", "Ada");

        // Part A stores the plate as given; from Part B on it is normalized.
        Assert.Contains(car.Plate, new[] { "06 ABC 123", Card.Plate("06 ABC 123") });
        Assert.Equal("Ada", car.Owner);
    }

    [Fact]
    public void Vehicle_Describe_FollowsYourCard()
    {
        Vehicle car = new Vehicle("06 ABC 123", "Ada");

        Assert.Equal(Card.VehicleText(car.Plate, "Ada"), car.Describe());
    }

    [Fact]
    public void TwoVehicles_WithTheSamePlate_AreTwoDifferentObjects()
    {
        Vehicle first = new Vehicle("06 ABC 123", "Ada");
        Vehicle second = new Vehicle("06 ABC 123", "Ada");

        Assert.NotSame(first, second);
        Assert.Equal(first.Plate, second.Plate);
    }

    [Fact]
    public void NewSpot_HasItsNumberAndIsFree()
    {
        ParkingSpot spot = new ParkingSpot(3);

        Assert.Equal(3, spot.Number);
        Assert.True(spot.IsFree());
        Assert.Null(spot.Occupant);
    }

    [Fact]
    public void TryPark_OnFreeSpot_ParksTheVehicle()
    {
        ParkingSpot spot = new ParkingSpot(3);
        Vehicle car = new Vehicle("06 ABC 123", "Ada");

        bool parked = spot.TryPark(car);

        Assert.True(parked);
        Assert.False(spot.IsFree());
        Assert.Same(car, spot.Occupant);
    }

    [Fact]
    public void TryPark_OnOccupiedSpot_ReturnsFalseAndKeepsTheFirstVehicle()
    {
        ParkingSpot spot = new ParkingSpot(3);
        Vehicle first = new Vehicle("06 ABC 123", "Ada");
        Vehicle second = new Vehicle("34 XY 99", "Bora");
        spot.TryPark(first);

        bool parked = spot.TryPark(second);

        Assert.False(parked);
        Assert.Same(first, spot.Occupant);
    }

    [Fact]
    public void Leave_ReturnsTheVehicleAndFreesTheSpot()
    {
        ParkingSpot spot = new ParkingSpot(3);
        Vehicle car = new Vehicle("06 ABC 123", "Ada");
        spot.TryPark(car);

        Vehicle? left = spot.Leave();

        Assert.Same(car, left);
        Assert.True(spot.IsFree());
        Assert.Null(spot.Occupant);
    }

    [Fact]
    public void Leave_OnFreeSpot_ReturnsNull()
    {
        ParkingSpot spot = new ParkingSpot(3);

        Assert.Null(spot.Leave());
        Assert.True(spot.IsFree());
    }

    [Fact]
    public void Describe_FreeSpot_FollowsYourCard()
    {
        ParkingSpot spot = new ParkingSpot(3);

        Assert.Equal(Card.SpotText(3, null), spot.Describe());
    }

    [Fact]
    public void Describe_OccupiedSpot_FollowsYourCard()
    {
        ParkingSpot spot = new ParkingSpot(3);
        Vehicle car = new Vehicle("06 ABC 123", "Ada");
        spot.TryPark(car);

        Assert.Equal(Card.SpotText(3, car.Plate), spot.Describe());
    }
}
