using CampusParking;
using System.Security.Cryptography;
using System.Text;

namespace CampusParking.Tests;

// HW1 parameter card. Given file: do not modify.
// Every student's Campus Parking is configured a little differently. This class
// computes your configuration from StudentInfo.Number; the tests use it to know
// which results to expect, and the test A00 writes it to HW1/MyParameters.txt.
public sealed class ParameterCard
{
    private const string CourseKey = "SENG116-2026-HW1-c7Q2xV9p";

    public static ParameterCard Mine => For(StudentInfo.Number);

    public string StudentNumber { get; }
    public int DescribeStyle { get; }          // 0, 1 or 2
    public bool PlateKeepsSpaces { get; }
    public int FreeMinutes { get; }
    public int UnitMinutes { get; }
    public decimal UnitRate { get; }
    public decimal MaxFee { get; }
    public bool HighestFreeSpotFirst { get; }
    public int FirstTicketNumber { get; }

    private ParameterCard(string number, byte[] h)
    {
        StudentNumber = number;
        DescribeStyle = h[0] % 3;
        PlateKeepsSpaces = h[1] % 2 == 0;
        FreeMinutes = new[] { 15, 20, 30, 45 }[h[2] % 4];
        UnitMinutes = h[3] % 2 == 0 ? 60 : 30;
        UnitRate = UnitMinutes == 60 ? new[] { 15m, 20m, 25m, 30m }[h[4] % 4]
                                     : new[] { 8m, 10m, 12m, 15m }[h[4] % 4];
        MaxFee = new[] { 100m, 120m, 140m, 160m }[h[5] % 4];
        HighestFreeSpotFirst = h[6] % 2 == 1;
        FirstTicketNumber = 1 + 100 * (h[7] % 10);
    }

    public static bool IsValidNumber(string number)
    {
        if (number.Length < 6 || number.Length > 12) return false;
        foreach (char c in number)
        {
            if (c < '0' || c > '9') return false;
        }
        return true;
    }

    public static ParameterCard For(string number)
    {
        byte[] h = SHA256.HashData(Encoding.UTF8.GetBytes(CourseKey + "|" + number.Trim()));
        return new ParameterCard(number.Trim(), h);
    }

    // ----- expected results, computed from the card -----

    public string Plate(string raw)
    {
        string p = raw.Trim().ToUpperInvariant();
        return PlateKeepsSpaces ? p : p.Replace(" ", "");
    }

    public string VehicleText(string plate, string owner) => DescribeStyle switch
    {
        0 => $"{plate} ({owner})",
        1 => $"{owner} / {plate}",
        _ => $"{plate} - {owner}",
    };

    public string SpotText(int number, string? plate) => DescribeStyle switch
    {
        0 => plate == null ? $"Spot {number}: free" : $"Spot {number}: {plate}",
        1 => plate == null ? $"P{number} [free]" : $"P{number} [{plate}]",
        _ => plate == null ? $"#{number} empty" : $"#{number} {plate}",
    };

    public decimal Fee(int minutes)
    {
        if (minutes <= FreeMinutes) return 0m;
        int units = (minutes + UnitMinutes - 1) / UnitMinutes;
        decimal fee = units * UnitRate;
        return fee > MaxFee ? MaxFee : fee;
    }

    // The k-th spot (k = 1, 2, ...) that an empty garage fills.
    public int SpotFilled(int k, int capacity) => HighestFreeSpotFirst ? capacity - k + 1 : k;

    public override string ToString()
    {
        var s = new StringBuilder();
        s.AppendLine("SENG 116 - HW1 parameter card");
        s.AppendLine($"Student number          : {StudentNumber}");
        s.AppendLine();
        s.AppendLine("Part A");
        s.AppendLine($"  Vehicle.Describe()     : {VehicleText("06 ABC 123", "Ada")}");
        s.AppendLine($"  ParkingSpot.Describe() : {SpotText(3, null)}");
        s.AppendLine($"                           {SpotText(3, "06 ABC 123")}");
        s.AppendLine();
        s.AppendLine("Part B");
        s.AppendLine($"  Plate format           : " + (PlateKeepsSpaces
            ? "capital letters, spaces kept"
            : "capital letters, all spaces removed"));
        s.AppendLine($"                           \" 06 abc 123 \" -> \"{Plate(" 06 abc 123 ")}\"");
        s.AppendLine($"  Tariff.FreeMinutes     : {FreeMinutes}");
        s.AppendLine($"  Tariff.UnitMinutes     : {UnitMinutes}");
        s.AppendLine($"  Tariff.UnitRate        : {UnitRate}");
        s.AppendLine($"  Tariff.MaxFee          : {MaxFee}");
        s.AppendLine("  Example fees           :");
        foreach (int m in new[] { FreeMinutes, FreeMinutes + 1, 61, 150, 600 })
        {
            s.AppendLine($"      {m,4} min -> {Fee(m),4} TL");
        }
        s.AppendLine();
        s.AppendLine("Part C");
        s.AppendLine("  Garage.Enter uses      : " + (HighestFreeSpotFirst
            ? "the free spot with the HIGHEST number"
            : "the free spot with the LOWEST number"));
        s.AppendLine($"  First ticket number    : {FirstTicketNumber}");
        return s.ToString();
    }
}
