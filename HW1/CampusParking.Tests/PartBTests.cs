using CampusParking;
using System.Globalization;
using System.Reflection;

namespace CampusParking.Tests;

// HW1 Part B (Week 3): encapsulation, properties, validation, static members.
// Given test file: do not modify.
public class PartBTests
{
    private static readonly ParameterCard Card = ParameterCard.Mine;

    [Theory]
    [InlineData("06 ABC 123")]
    [InlineData("06 abc 123")]
    [InlineData("  34 xy 99  ")]
    [InlineData("35DE35")]
    public void NormalizePlate_FollowsYourCard(string input)
    {
        Assert.Equal(Card.Plate(input), Vehicle.NormalizePlate(input));
    }

    [Fact]
    public void NormalizePlate_GivesTheSameResultOnATurkishComputer()
    {
        CultureInfo original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("tr-TR");

            Assert.Equal(Card.Plate("34 BIL 42"), Vehicle.NormalizePlate("34 bil 42"));
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }

    [Fact]
    public void Vehicle_StoresTheNormalizedPlate()
    {
        Vehicle car = new Vehicle("  06 abc 123 ", "Ada");

        Assert.Equal(Card.Plate("06 ABC 123"), car.Plate);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Vehicle_RejectsAnEmptyPlate(string plate)
    {
        Assert.Throws<ArgumentException>(() => new Vehicle(plate, "Ada"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Vehicle_RejectsAnEmptyOwner(string owner)
    {
        Assert.Throws<ArgumentException>(() => new Vehicle("06 ABC 123", owner));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-4)]
    public void ParkingSpot_RejectsANumberBelowOne(int number)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ParkingSpot(number));
    }

    [Fact]
    public void Tariff_HasTheConstantsOfYourCard()
    {
        Assert.Equal(Card.FreeMinutes, Tariff.FreeMinutes);
        Assert.Equal(Card.UnitMinutes, Tariff.UnitMinutes);
        Assert.Equal(Card.UnitRate, Tariff.UnitRate);
        Assert.Equal(Card.MaxFee, Tariff.MaxFee);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(14)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(20)]
    [InlineData(21)]
    [InlineData(30)]
    [InlineData(31)]
    [InlineData(45)]
    [InlineData(46)]
    [InlineData(60)]
    [InlineData(61)]
    [InlineData(90)]
    [InlineData(91)]
    [InlineData(150)]
    [InlineData(600)]
    [InlineData(900)]
    public void Tariff_FeeFor_FollowsTheRules(int minutes)
    {
        Assert.Equal(Card.Fee(minutes), Tariff.FeeFor(minutes));
    }

    [Fact]
    public void Tariff_RejectsANegativeStay()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Tariff.FeeFor(-1));
    }

    // ----- design checks: these read the structure of your classes -----

    [Fact]
    public void Tariff_IsAStaticClass()
    {
        // The compiler marks a static class as both abstract and sealed.
        Assert.True(typeof(Tariff).IsAbstract && typeof(Tariff).IsSealed);
    }

    [Theory]
    [InlineData(typeof(Vehicle))]
    [InlineData(typeof(ParkingSpot))]
    public void State_IsNotPubliclyWritable(Type type)
    {
        DesignChecks.AssertNoPublicFieldsOrSetters(type);
    }
}

// Helper used by the design checks of Parts B and C.
public static class DesignChecks
{
    public static void AssertNoPublicFieldsOrSetters(Type type)
    {
        foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            Assert.Fail($"{type.Name}.{field.Name} is a public field; make it private or a property.");
        }
        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.SetMethod != null && property.SetMethod.IsPublic)
            {
                Assert.Fail($"{type.Name}.{property.Name} has a public setter.");
            }
        }
    }
}
