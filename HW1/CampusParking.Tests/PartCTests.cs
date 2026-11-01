using CampusParking;
using System.Reflection;

namespace CampusParking.Tests;

// HW1 Part C (Week 4): object relationships, composition, arrays.
// Given test file: do not modify.
public class PartCTests
{
    private static readonly ParameterCard Card = ParameterCard.Mine;

    [Fact]
    public void NewGarage_HasNumberedFreeSpots()

    {
        Garage garage = new Garage("North Gate", 3);

        Assert.Equal("North Gate", garage.Name);
        Assert.Equal(3, garage.Capacity);
        Assert.Equal(3, garage.FreeSpotCount());
        Assert.Equal(1, garage.GetSpot(1).Number);
        Assert.Equal(3, garage.GetSpot(3).Number);
        Assert.True(garage.GetSpot(2).IsFree());
    }

    [Fact]
    public void Garage_RejectsInvalidArguments()
    {
        Assert.Throws<ArgumentException>(() => new Garage("  ", 3));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Garage("North Gate", 0));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    public void GetSpot_OutsideTheGarage_Throws(int number)
    {
        Garage garage = new Garage("North Gate", 3);

        Assert.Throws<ArgumentOutOfRangeException>(() => garage.GetSpot(number));
    }

    [Fact]
    public void Garage_OwnsItsSpots()
    {
        Garage north = new Garage("North Gate", 2);
        Garage south = new Garage("South Gate", 2);

        Assert.Same(north.GetSpot(1), north.GetSpot(1));
        Assert.NotSame(north.GetSpot(1), south.GetSpot(1));
    }

    [Fact]
    public void Enter_UsesTheSpotOfYourCardAndIssuesATicket()
    {
        Garage garage = new Garage("North Gate", 3);
        Vehicle ada = new Vehicle("06 ABC 123", "Ada");
        Vehicle bora = new Vehicle("34 XY 99", "Bora");

        Ticket? first = garage.Enter(ada, 10);
        Ticket? second = garage.Enter(bora, 15);

        Assert.NotNull(first);
        Assert.NotNull(second);
        int firstSpot = Card.SpotFilled(1, 3);
        Assert.Equal(Card.FirstTicketNumber, first.Number);
        Assert.Equal(Card.FirstTicketNumber + 1, second.Number);
        Assert.Same(ada, first.Vehicle);
        Assert.Same(garage.GetSpot(firstSpot), first.Spot);
        Assert.Same(garage.GetSpot(Card.SpotFilled(2, 3)), second.Spot);
        Assert.Same(ada, garage.GetSpot(firstSpot).Occupant);
        Assert.Equal(10, first.EntryMinute);
        Assert.True(first.IsOpen);
        Assert.Equal(0m, first.Fee);
        Assert.Equal(1, garage.FreeSpotCount());
    }

    [Fact]
    public void Enter_WhenFull_ReturnsNull()
    {
        Garage garage = new Garage("North Gate", 1);
        garage.Enter(new Vehicle("06 ABC 123", "Ada"), 0);

        Ticket? ticket = garage.Enter(new Vehicle("34 XY 99", "Bora"), 5);

        Assert.Null(ticket);
        Assert.Equal(0, garage.FreeSpotCount());
    }

    [Fact]
    public void Enter_SamePlateTwice_ReturnsNull()
    {
        Garage garage = new Garage("North Gate", 3);
        garage.Enter(new Vehicle("06 ABC 123", "Ada"), 0);

        // A different object, but the same plate (written in small letters).
        Ticket? again = garage.Enter(new Vehicle("06 abc 123", "Ada"), 5);

        Assert.Null(again);
        Assert.Equal(2, garage.FreeSpotCount());
    }

    [Fact]
    public void Enter_ReusesAFreedSpot_ButNotATicketNumber()
    {
        Garage garage = new Garage("North Gate", 2);
        Ticket? a = garage.Enter(new Vehicle("06 ABC 123", "Ada"), 0);
        garage.Enter(new Vehicle("34 XY 99", "Bora"), 5);
        Assert.NotNull(a);
        garage.Exit(a, 20);

        Ticket? c = garage.Enter(new Vehicle("35 DE 35", "Cem"), 25);

        Assert.NotNull(c);
        Assert.Same(a.Spot, c.Spot);
        Assert.Equal(Card.FirstTicketNumber + 2, c.Number);
    }

    [Fact]
    public void TicketNumbers_AreCountedPerGarage()
    {
        Garage north = new Garage("North Gate", 3);
        Garage south = new Garage("South Gate", 3);

        Ticket? n1 = north.Enter(new Vehicle("06 ABC 123", "Ada"), 0);
        Ticket? s1 = south.Enter(new Vehicle("34 XY 99", "Bora"), 0);
        Ticket? n2 = north.Enter(new Vehicle("35 DE 35", "Cem"), 0);

        Assert.NotNull(n1);
        Assert.NotNull(s1);
        Assert.NotNull(n2);
        Assert.Equal(Card.FirstTicketNumber, n1.Number);
        Assert.Equal(Card.FirstTicketNumber, s1.Number);
        Assert.Equal(Card.FirstTicketNumber + 1, n2.Number);
    }

    [Theory]
    [InlineData(20)]
    [InlineData(95)]
    [InlineData(400)]
    public void Exit_ChargesTheTariffAndFreesTheSpot(int stay)
    {
        Garage garage = new Garage("North Gate", 3);
        Ticket? ticket = garage.Enter(new Vehicle("06 ABC 123", "Ada"), 100);
        Assert.NotNull(ticket);

        decimal fee = garage.Exit(ticket, 100 + stay);

        Assert.Equal(Card.Fee(stay), fee);
        Assert.Equal(fee, ticket.Fee);
        Assert.False(ticket.IsOpen);
        Assert.True(ticket.Spot.IsFree());
        Assert.Equal(3, garage.FreeSpotCount());
    }

    [Fact]
    public void Exit_WithAClosedTicket_Throws()
    {
        Garage garage = new Garage("North Gate", 3);
        Ticket? ticket = garage.Enter(new Vehicle("06 ABC 123", "Ada"), 0);
        Assert.NotNull(ticket);
        garage.Exit(ticket, 50);

        Assert.Throws<InvalidOperationException>(() => garage.Exit(ticket, 60));
    }

    [Fact]
    public void Exit_BeforeEntry_Throws()
    {
        Garage garage = new Garage("North Gate", 3);
        Ticket? ticket = garage.Enter(new Vehicle("06 ABC 123", "Ada"), 100);
        Assert.NotNull(ticket);

        Assert.Throws<ArgumentOutOfRangeException>(() => garage.Exit(ticket, 99));
        Assert.True(ticket.IsOpen);
    }

    [Fact]
    public void Exit_WithATicketOfAnotherGarage_Throws()
    {
        Garage north = new Garage("North Gate", 2);
        Garage south = new Garage("South Gate", 5);
        Ticket?[] tickets = new Ticket?[5];
        string[] plates = { "06 ABC 123", "34 XY 99", "35 DE 35", "06 KL 456", "01 AB 01" };
        for (int i = 0; i < 5; i++)
        {
            tickets[i] = south.Enter(new Vehicle(plates[i], "Owner"), 0);
        }
        Ticket? atSpot1 = FindTicketAt(tickets, 1);
        Ticket? atSpot3 = FindTicketAt(tickets, 3);
        Assert.NotNull(atSpot1);
        Assert.NotNull(atSpot3);

        // North also has a spot 1, but it is a different object.
        Assert.Throws<ArgumentException>(() => north.Exit(atSpot1, 30));
        // North has no spot 3 at all.
        Assert.Throws<ArgumentException>(() => north.Exit(atSpot3, 30));

        Assert.True(atSpot1.IsOpen);
        Assert.False(south.GetSpot(1).IsFree());
    }

    [Fact]
    public void FindSpotOf_FindsAParkedVehicleByItsPlate()
    {
        Garage garage = new Garage("North Gate", 3);
        garage.Enter(new Vehicle("06 ABC 123", "Ada"), 0);
        garage.Enter(new Vehicle("34 XY 99", "Bora"), 0);

        ParkingSpot? spot = garage.FindSpotOf(" 34 xy 99");

        Assert.NotNull(spot);
        Assert.Equal(Card.SpotFilled(2, 3), spot.Number);
        Assert.Null(garage.FindSpotOf("35 DE 35"));
    }

    [Fact]
    public void Describe_ShowsNameAndFreeSpots()
    {
        Garage garage = new Garage("North Gate", 3);
        garage.Enter(new Vehicle("06 ABC 123", "Ada"), 0);

        Assert.Equal("North Gate: 2 of 3 spots free", garage.Describe());
    }

    private static Ticket? FindTicketAt(Ticket?[] tickets, int spotNumber)
    {
        foreach (Ticket? ticket in tickets)
        {
            if (ticket != null && ticket.Spot.Number == spotNumber)
            {
                return ticket;
            }
        }
        return null;
    }

    // ----- design checks: these read the structure of your classes -----

    [Theory]
    [InlineData(typeof(Garage))]
    [InlineData(typeof(Ticket))]
    public void State_IsNotPubliclyWritable(Type type)
    {
        DesignChecks.AssertNoPublicFieldsOrSetters(type);
    }

    [Fact]
    public void Ticket_CanOnlyBeCreatedAndClosedInsideTheLibrary()
    {
        BindingFlags instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        ConstructorInfo[] constructors = typeof(Ticket).GetConstructors(instance);
        MethodInfo? close = typeof(Ticket).GetMethod("Close", instance);

        Assert.NotEmpty(constructors);
        foreach (ConstructorInfo constructor in constructors)
        {
            Assert.True(constructor.IsAssembly, "Ticket constructors must be internal.");
        }
        Assert.NotNull(close);
        Assert.True(close.IsAssembly, "Ticket.Close must be internal.");
    }
}
