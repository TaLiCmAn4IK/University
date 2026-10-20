namespace DAL004;

public record Person(int Id, string FirstName, string Surname, string PhotoPath)
{
    public override string ToString() => $"{FirstName} {Surname}";
}