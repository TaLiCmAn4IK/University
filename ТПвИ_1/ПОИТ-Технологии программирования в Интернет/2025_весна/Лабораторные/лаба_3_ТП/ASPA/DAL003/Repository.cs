using System.Text.Json;

namespace DAL003;

public class Repository : IRepository
{
    private readonly List<Person> _persons;
    private readonly string _basePath;

    public string BasePath => _basePath;

    private Repository(string basePath)
    {
        _basePath = basePath;
        // Формируем полный путь к JSON файлу
        string jsonPath = Path.Combine(basePath, IRepository.JSONFileName);

        if (!File.Exists(jsonPath))
        {
            throw new FileNotFoundException($"JSON файл не найден: {jsonPath}");
        }

        string jsonContent = File.ReadAllText(jsonPath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _persons = JsonSerializer.Deserialize<List<Person>>(jsonContent, options) //превращаем JSON строку в список объектов Person
            ?? new List<Person>(); // Теперь в _persons лежат все знаменитости из JSON файла
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

    public IEnumerable<Person> GetAllPersons()
    {
        return _persons;
    }

    public Person? GetPhotoPathById(int id)
    {
        return _persons.FirstOrDefault(p => p.Id == id);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}