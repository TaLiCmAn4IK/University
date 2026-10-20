using DAL003;
using System;
using System.IO;
using System.Linq;

namespace Test_DAL003;

class Program
{
    static void Main(string[] args)
    {
        string basePath = Path.Combine(Directory.GetCurrentDirectory(), "Database");

        Console.WriteLine($"Базовая директория: {basePath}");
        Console.WriteLine(new string('-', 50));

        using (IRepository repository = Repository.Create(basePath))
        {
            // 1. Все знаменитости
            Console.WriteLine("1. Все знаменитости:");
            foreach (Person celebrity in repository.GetAllPersons())
            {
                Console.WriteLine($"   Id = {celebrity.Id}, FirstName = {celebrity.FirstName}, " +
                    $"Surname = {celebrity.Surname}, PhotoPath = {celebrity.PhotoPath}");
            }
            Console.WriteLine();

            // 2. Поиск по ID (существующие)
            Console.WriteLine("2. Поиск по ID:");

            Person? celebrity1 = repository.GetPersonById(1);
            if (celebrity1 != null)
                Console.WriteLine($"   Id = 1: {celebrity1.FirstName} {celebrity1.Surname}");

            Person? celebrity3 = repository.GetPersonById(3);
            if (celebrity3 != null)
                Console.WriteLine($"   Id = 3: {celebrity3.FirstName} {celebrity3.Surname}");

            Person? celebrity7 = repository.GetPersonById(7);
            if (celebrity7 != null)
                Console.WriteLine($"   Id = 7: {celebrity7.FirstName} {celebrity7.Surname}");
            Console.WriteLine();

            // 3. Поиск по ID (несуществующий)
            Console.WriteLine("3. Поиск по несуществующему ID (222):");
            Person? celebrity222 = repository.GetPersonById(222);
            Console.WriteLine(celebrity222 == null ? "   Not Found 222" : $"   Найден: {celebrity222.FirstName}");
            Console.WriteLine();

            // 4. Поиск по фамилии (все)
            Console.WriteLine("4. Поиск по фамилии (GetPersonsBySurname):");

            Console.WriteLine("   Поиск 'Chomsky':");
            foreach (Person c in repository.GetPersonsBySurname("Chomsky"))
                Console.WriteLine($"      {c.FirstName} {c.Surname}");

            Console.WriteLine("   Поиск 'Knuth':");
            foreach (Person c in repository.GetPersonsBySurname("Knuth"))
                Console.WriteLine($"      {c.FirstName} {c.Surname}");

            Console.WriteLine("   Поиск 'XXXX' (несуществующая):");
            var xxxxResults = repository.GetPersonsBySurname("XXXX").ToList();
            if (xxxxResults.Any())
                foreach (Person c in xxxxResults)
                    Console.WriteLine($"      {c.FirstName} {c.Surname}");
            else
                Console.WriteLine("      Ничего не найдено");
            Console.WriteLine();

            // 5. Поиск по имени (GetPersonByFirstName)
            Console.WriteLine("5. Поиск по имени (GetPersonByFirstName):");

            Person? byFirstName1 = repository.GetPersonByFirstName("Noam");
            Console.WriteLine($"   Поиск 'Noam': {(byFirstName1 != null ? $"{byFirstName1.FirstName} {byFirstName1.Surname}" : "Not Found")}");

            Person? byFirstName2 = repository.GetPersonByFirstName("TIM");
            Console.WriteLine($"   Поиск 'TIM': {(byFirstName2 != null ? $"{byFirstName2.FirstName} {byFirstName2.Surname}" : "Not Found")}");

            Person? byFirstName3 = repository.GetPersonByFirstName("Alexander");
            Console.WriteLine($"   Поиск 'Alexander': {(byFirstName3 != null ? $"{byFirstName3.FirstName} {byFirstName3.Surname}" : "Not Found")}");
            Console.WriteLine();

            // 6. Поиск по фамилии (GetPersonBySurname) - первый найденный
            Console.WriteLine("6. Поиск по фамилии (GetPersonBySurname) - первый найденный:");

            Person? bySurname1 = repository.GetPersonBySurname("Chomsky");
            Console.WriteLine($"   Поиск 'Chomsky': {(bySurname1 != null ? $"{bySurname1.FirstName} {bySurname1.Surname}" : "Not Found")}");

            Person? bySurname2 = repository.GetPersonBySurname("KNUTH");
            Console.WriteLine($"   Поиск 'KNUTH': {(bySurname2 != null ? $"{bySurname2.FirstName} {bySurname2.Surname}" : "Not Found")}");

            Person? bySurname3 = repository.GetPersonBySurname("Pushkin");
            Console.WriteLine($"   Поиск 'Pushkin': {(bySurname3 != null ? $"{bySurname3.FirstName} {bySurname3.Surname}" : "Not Found")}");
            Console.WriteLine();

            // 7. Получение пути к фото по ID
            Console.WriteLine("7. Пути к фотографиям:");

            Person? photo1 = repository.GetPhotoPathById(1);
            Console.WriteLine($"   Photo для Id=1: {photo1?.PhotoPath ?? "Not Found"}");

            Person? photo6 = repository.GetPhotoPathById(6);
            Console.WriteLine($"   Photo для Id=6: {photo6?.PhotoPath ?? "Not Found"}");

            Person? photo222 = repository.GetPhotoPathById(222);
            Console.WriteLine($"   Photo для Id=222: {photo222?.PhotoPath ?? "Not Found"}");
        }

        Console.WriteLine(new string('-', 50));
        Console.WriteLine("Тестирование завершено.");
        Console.ReadLine();
    }
}