using DAL004;

// Устанавливаем имя JSON-файла
IRepository.JSONFileName = "people.json";

// Создаём репозиторий, указывая папку с данными
using (IRepository repository = Repository.Create("Database"))
{
    // Локальная функция для вывода всех записей
    void Print(string label)
    {
        Console.WriteLine($"--- {label} ---");
        var all = repository.GetAllPersons().ToList();
        foreach (Person person in all)
        {
            Console.WriteLine($"Id = {person.Id}, FirstName = {person.FirstName}, " +
                              $"Surname = {person.Surname}, PhotoPath = {person.PhotoPath}");
        }
        Console.WriteLine();
    }

    // 1. Вывод всех записей
    Print("start");

    // 2. Тестирование добавления записей
    Console.WriteLine("=== Добавление записей ===");
    int? testDel1 = repository.AddPerson(new Person(0, "TestDel1", "TestDel1", "/Photo/TestDel1.jpg"));
    int? testDel2 = repository.AddPerson(new Person(0, "TestDel2", "TestDel2", "/Photo/TestDel2.jpg"));
    int? testUpd1 = repository.AddPerson(new Person(0, "TestUpd1", "TestUpd1", "/Photo/TestUpd1.jpg"));
    int? testUpd2 = repository.AddPerson(new Person(0, "TestUpd2", "TestUpd2", "/Photo/TestUpd2.jpg"));
    repository.SaveChanges();
    Console.WriteLine($"Добавлены записи: ID = {testDel1}, {testDel2}, {testUpd1}, {testUpd2}\n");
    Print("add 4");

    // 3. Тестирование удаления записей
    Console.WriteLine("=== Удаление записей ===");
    if (testDel1 != null)
        Console.WriteLine(repository.DeletePersonById((int)testDel1) ? $"  delete {testDel1} (успешно)" : $"delete {testDel1} (ошибка)");
    if (testDel2 != null)
        Console.WriteLine(repository.DeletePersonById((int)testDel2) ? $"  delete {testDel2} (успешно)" : $"delete {testDel2} (ошибка)");
    Console.WriteLine(repository.DeletePersonById(1000) ? "  delete 1000 (успешно)" : "delete 1000 (ошибка)");
    repository.SaveChanges();
    Console.WriteLine();
    Print("del 2");

    // 4. Тестирование обновления записей
    Console.WriteLine("=== Обновление записей ===");
    if (testUpd1 != null)
        Console.WriteLine(repository.UpdatePersonById((int)testUpd1, new Person(0, "Updated1", "Updated1", "/Photo/Updated1.jpg"))
            ? $"  update {testUpd1} (успешно)" : $"update {testUpd1} (ошибка)");
    if (testUpd2 != null)
        Console.WriteLine(repository.UpdatePersonById((int)testUpd2, new Person(0, "Updated2", "Updated2", "/Photo/Updated2.jpg"))
            ? $"  update {testUpd2} (успешно)" : $"update {testUpd2} (ошибка)");
    Console.WriteLine(repository.UpdatePersonById(1000, new Person(0, "Updated1000", "Updated1000", "/Photo/Updated1000.jpg"))
        ? "  update 1000 (успешно)" : "update 1000 (ошибка)");
    repository.SaveChanges();
    Console.WriteLine();
    Print("upd 2");

    Console.WriteLine("=== Тестирование завершено ===");
}