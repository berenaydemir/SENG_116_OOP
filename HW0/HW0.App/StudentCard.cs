namespace HW0.App;

public class StudentCard
{
    public string Name { get; }
    public long Number { get; }

    public StudentCard(string name, long number)
    {
        Name = name;
        Number = number;
    }

    public string Summary()
    {
        return $"{Name} ({Number})";
    }
}
