using System.Runtime.InteropServices;
using HW0.App;

Console.WriteLine("=== SENG 116 - HW0 ===");
Console.WriteLine($".NET runtime : {RuntimeInformation.FrameworkDescription}");
Console.WriteLine($"Operating sys. : {RuntimeInformation.OSDescription}");

Console.Write("Full name : ");
string name = Console.ReadLine() ?? "";

Console.Write("Student number : ");
long number = long.Parse(Console.ReadLine() ?? "");

var card = new StudentCard(name, number);
Console.WriteLine(card.Summary());

int digitSum = NumberTools.DigitSum(number);
Console.WriteLine($"Digit sum of the student number : {digitSum}");
Console.WriteLine($"Is the digit sum prime? : {NumberTools.IsPrime(digitSum)}");
Console.WriteLine($"Primes up to the digit sum : {NumberTools.CountPrimes(digitSum)}");