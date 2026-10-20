using System.Text.Json;

namespace DAL004;

public class Repository : IRepository
{
    private readonly List<Person> _persons; 
    private readonly string _basePath;
    private readonly string _jsonPath;

    public string BasePath => _basePath;

    private Repository(string basePath)
    {
        _basePath = basePath;
        _jsonPath = Path.Combine(basePath, IRepository.JSONFileName);
        _persons = LoadData();
    }

    private List<Person> LoadData()
    {
        if (!File.Exists(_jsonPath))
        {
            throw new FileNotFoundException($"JSON файл не найден: {_jsonPath}");
        }

        string jsonContent = File.ReadAllText(_jsonPath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<List<Person>>(jsonContent, options)
               ?? new List<Person>();
    }

    private void SaveToFile()
    {
        string json = JsonSerializer.Serialize(_persons, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_jsonPath, json);
    }

    public static Repository Create(string basePath)
    {
        return new Repository(basePath);
    }

    public Person? GetPersonById(int id)
    {
        return _persons.FirstOrDefault(p => p.Id == id);
    }

    public Person? GetPersonByFirstName(string firstName)
    {
        return _persons.FirstOrDefault(p =>
            p.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase));
    }

    public Person? GetPersonBySurname(string surname)
    {
        return _persons.FirstOrDefault(p =>
            p.Surname.Equals(surname, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Person> GetPersonsBySurname(string surname)
    {
        return _persons.Where(p =>
            p.Surname.Equals(surname, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Person> GetPersonsByName(string surname)
    {
        return _persons.Where(p =>
            p.Surname.StartsWith(surname, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Person> GetAllPersons()
    {
        return _persons;
    }

    public Person? GetPhotoPathById(int id)
    {
        return _persons.FirstOrDefault(p => p.Id == id);
    }

    // Новые методы
    public int? AddPerson(Person person)
    {
        int newId = _persons.Count > 0 ? _persons.Max(p => p.Id) + 1 : 1;
        var newPerson = person with { Id = newId };
        _persons.Add(newPerson);
        return newId;
    }

    public bool DeletePersonById(int id)
    {
        var person = _persons.FirstOrDefault(p => p.Id == id);
        if (person == null)
            return false;

        _persons.Remove(person);
        return true;
    }

    public bool UpdatePersonById(int id, Person updatedPerson)
    {
        var existing = _persons.FirstOrDefault(p => p.Id == id);
        if (existing == null)
            return false;

        _persons.Remove(existing);
        _persons.Add(updatedPerson with { Id = id });
        return true;
    }

    public void SaveChanges()
    {
        SaveToFile();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}