namespace DAL004;

public interface IRepository : IDisposable
{
    static string JSONFileName { get; set; } = "people.json";

    string BasePath { get; }

    Person? GetPersonById(int id);
    Person? GetPersonByFirstName(string firstName);
    Person? GetPersonBySurname(string surname);
    IEnumerable<Person> GetPersonsBySurname(string surname);
    IEnumerable<Person> GetPersonsByName(string surname);
    IEnumerable<Person> GetAllPersons();
    Person? GetPhotoPathById(int id);

    // Новые методы
    int? AddPerson(Person person);
    bool DeletePersonById(int id);
    bool UpdatePersonById(int id, Person updatedPerson);
    void SaveChanges();
}