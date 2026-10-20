using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace SerializationLab
{
    // ==================== 1. КЛАСС Student ====================
    [Serializable]
    public class Student
    {
        public string Name { get; set; }
        public int Age { get; set; }

        [NonSerialized]
        private string password = "secret123";

        public List<string> Courses { get; set; }

        public Student() { }

        public Student(string name, int age, List<string> courses)
        {
            Name = name;
            Age = age;
            Courses = courses;
        }

        public void Display()
        {
            Console.WriteLine($"Студент: {Name}, Возраст: {Age}");
            Console.WriteLine($"Курсы: {string.Join(", ", Courses)}");
            Console.WriteLine($"Пароль (не сериализуется): {password}");
            Console.WriteLine();
        }
    }

    // ==================== 2. ОБЩИЙ ИНТЕРФЕЙС СЕРИАЛИЗАТОРА ====================
    public interface ISerializer
    {
        void Serialize<T>(T obj, string fileName);
        T Deserialize<T>(string fileName);
    }

    // ==================== 3. РЕАЛИЗАЦИЯ СЕРИАЛИЗАТОРОВ ====================

    // 3.1 JSON сериализатор 
    public class JsonSerializer : ISerializer
    {
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public void Serialize<T>(T obj, string fileName)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(obj, _options); // превращает обьект в json строку
            File.WriteAllText(fileName, json, Encoding.UTF8);
        }

        public T Deserialize<T>(string fileName)
        {
            string json = File.ReadAllText(fileName, Encoding.UTF8); 
            return System.Text.Json.JsonSerializer.Deserialize<T>(json, _options);
        }
    }

    // 3.2 XML сериализатор 
    public class XmlSerializer : ISerializer
    {
        public void Serialize<T>(T obj, string fileName)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                serializer.Serialize(writer, obj);
            }
        }

        public T Deserialize<T>(string fileName)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (var reader = new StreamReader(fileName, Encoding.UTF8))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }




    // ==================== 4. МЕНЕДЖЕР ДЛЯ РАБОТЫ С СЕРИАЛИЗАТОРАМИ ====================
    public class SerializationManager
    {
        private readonly Dictionary<string, ISerializer> _serializers;

        public SerializationManager()
        {
            _serializers = new Dictionary<string, ISerializer>
            {
                { "JSON", new JsonSerializer() },
                { "XML", new XmlSerializer() },

            };
        }

        public void TestAllFormats(Student student)
        {
            Console.WriteLine("=== ТЕСТИРОВАНИЕ ВСЕХ ФОРМАТОВ СЕРИАЛИЗАЦИИ ===\n");

            Console.WriteLine("Исходный объект:");
            student.Display();

            foreach (var serializer in _serializers)
            {
                Console.WriteLine($"\n--- {serializer.Key} сериализация ---");

                string fileName = $"student.{serializer.Key.ToLower()}";
                if (serializer.Key == "XML") fileName += ".xml";
                else fileName += ".json";

                try
                {
                    // Сериализация
                    serializer.Value.Serialize(student, fileName);
                    Console.WriteLine($"✓ Сериализовано в {fileName}");
                    // Показ содержимого файла
                    string fileContent = File.ReadAllText(fileName, Encoding.UTF8);
                    Console.WriteLine("Содержимое файла:");
                    Console.WriteLine(new string('-', 40));

                    if (fileContent.Length > 300)
                        Console.WriteLine(fileContent.Substring(0, 300) + "...");
                    else
                        Console.WriteLine(fileContent);

                    Console.WriteLine(new string('-', 40));

                    // Десериализация
                    Student deserialized = serializer.Value.Deserialize<Student>(fileName);
                    Console.WriteLine("✓ Десериализованный объект:");
                    deserialized.Display();

                    // Проверка размера файла
                    FileInfo fileInfo = new FileInfo(fileName);
                    Console.WriteLine($"Размер файла: {fileInfo.Length} байт");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Ошибка: {ex.Message}");
                }
            }
        }

        public void TestCollection(List<Student> students)
        {
            Console.WriteLine("\n=== СЕРИАЛИЗАЦИЯ КОЛЛЕКЦИИ ОБЪЕКТОВ ===\n");

            JsonSerializer jsonSerializer = new JsonSerializer();

            // Сериализация коллекции
            jsonSerializer.Serialize(students, "students_collection.json");
            Console.WriteLine("✓ Коллекция сериализована в students_collection.json");

            // Десериализация коллекции
            List<Student> deserializedStudents = jsonSerializer.Deserialize<List<Student>>("students_collection.json");
            Console.WriteLine($"✓ Десериализовано {deserializedStudents.Count} студентов:");

            foreach (var s in deserializedStudents)
            {
                s.Display();
            }
        }
    }

    // ==================== 5. XPATH И LINQ TO XML ====================
    // язык запросов для навигации по xml
    public class XmlTools
    {
        public static void DemonstrateXPath()
        {
            Console.WriteLine("\n=== ДЕМОНСТРАЦИЯ XPATH ===\n");

            // Создаем XML документ
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("Students");
            xmlDoc.AppendChild(root);

            // Добавляем студентов
            var students = new[]
            {
                new { Name = "Анна", Age = 21, Course = "Математика" },
                new { Name = "Петр", Age = 22, Course = "Программирование" },
                new { Name = "Мария", Age = 20, Course = "Дизайн" }
            };

            foreach (var s in students)
            {
                XmlElement studentElem = xmlDoc.CreateElement("Student");

                XmlElement nameElem = xmlDoc.CreateElement("Name");
                nameElem.InnerText = s.Name;
                studentElem.AppendChild(nameElem);

                XmlElement ageElem = xmlDoc.CreateElement("Age");
                ageElem.InnerText = s.Age.ToString();
                studentElem.AppendChild(ageElem);

                XmlElement courseElem = xmlDoc.CreateElement("Course");
                courseElem.InnerText = s.Course;
                studentElem.AppendChild(courseElem);

                root.AppendChild(studentElem);
            }

            xmlDoc.Save("students.xml");
            Console.WriteLine("✓ XML документ создан: students.xml");

            // XPath запрос 1: Все имена студентов
            Console.WriteLine("\n1. Все имена студентов (XPath: //Student/Name):");
            XmlNodeList nameNodes = xmlDoc.SelectNodes("//Student/Name");
            foreach (XmlNode node in nameNodes)
            {
                Console.WriteLine($"   - {node.InnerText}");
            }
            // XPath запрос 2: Студенты старше 20 лет
            Console.WriteLine("\n2. Студенты старше 20 лет (XPath: //Student[Age > 20]):");
            XmlNodeList oldStudents = xmlDoc.SelectNodes("//Student[Age > 20]");
            foreach (XmlNode node in oldStudents)
            {
                string name = node.SelectSingleNode("Name")?.InnerText ?? "";
                string age = node.SelectSingleNode("Age")?.InnerText ?? "";
                Console.WriteLine($"   - {name}, {age} лет");
            }

            // XPath запрос 3: Студенты на курсе "Программирование"
            Console.WriteLine("\n3. Студенты на курсе 'Программирование' (XPath: //Student[Course='Программирование']):");
            XmlNodeList programmingStudents = xmlDoc.SelectNodes("//Student[Course='Программирование']");
            foreach (XmlNode node in programmingStudents)
            {
                string name = node.SelectSingleNode("Name")?.InnerText ?? "";
                Console.WriteLine($"   - {name}");
            }
        }

        public static void DemonstrateLinqToXml()
        {
            Console.WriteLine("\n=== ДЕМОНСТРАЦИЯ LINQ TO XML ===\n");

            // Создаем XML документ через LINQ to XML
            XDocument xdoc = new XDocument(
                new XElement("University",
                    new XElement("Faculty",
                        new XAttribute("name", "ФИТ"),
                        new XElement("Group",
                            new XAttribute("number", "101"),
                            new XElement("Student",
                                new XElement("Name", "Анна"),
                                new XElement("Age", "21"),
                                new XElement("Courses",
                                    new XElement("Course", "C#"),
                                    new XElement("Course", "Базы данных")
                                )
                            ),
                            new XElement("Student",
                                new XElement("Name", "Петр"),
                                new XElement("Age", "22"),
                                new XElement("Courses",
                                    new XElement("Course", "C#"),
                                    new XElement("Course", "Алгоритмы")
                                )
                            ),
                            new XElement("Student",
                                new XElement("Name", "Мария"),
                                new XElement("Age", "20"),
                                new XElement("Courses",
                                    new XElement("Course", "Дизайн")
                                )
                            )
                        )
                    )
                )
            );

            xdoc.Save("university.xml");
            Console.WriteLine("✓ XML документ создан через LINQ to XML: university.xml");

            // LINQ запрос 1: Все студенты
            Console.WriteLine("\n1. Все студенты:");
            var allStudents = from student in xdoc.Descendants("Student")
                              select new
                              {
                                  Name = student.Element("Name")?.Value,
                                  Age = student.Element("Age")?.Value
                              };

            foreach (var student in allStudents)
            {
                Console.WriteLine($"   - {student.Name}, {student.Age} лет");
            }

            // LINQ запрос 2: Студенты, изучающие C#
            Console.WriteLine("\n2. Студенты, изучающие C#:");
            var csharpStudents = from student in xdoc.Descendants("Student")
                                 where student.Element("Courses")
                                     .Descendants("Course")
                                     .Any(c => c.Value == "C#")
                                 select student.Element("Name")?.Value;
            foreach (var name in csharpStudents)
            {
                Console.WriteLine($"   - {name}");
            }

            // LINQ запрос 3: Средний возраст студентов
            Console.WriteLine("\n3. Статистика:");
            var ages = xdoc.Descendants("Student")
                         .Select(s => int.Parse(s.Element("Age")?.Value ?? "0"))
                         .ToList();

            if (ages.Count > 0)
            {
                Console.WriteLine($"   Количество студентов: {ages.Count}");
                Console.WriteLine($"   Средний возраст: {ages.Average():F1} лет");
                Console.WriteLine($"   Минимальный возраст: {ages.Min()} лет");
                Console.WriteLine($"   Максимальный возраст: {ages.Max()} лет");
            }

            // LINQ запрос 4: Создание нового XML с фильтрацией
            Console.WriteLine("\n4. Создание нового XML с студентами старше 20 лет:");

            XDocument newDoc = new XDocument(
                new XElement("SeniorStudents",
                    from student in xdoc.Descendants("Student")
                    where int.Parse(student.Element("Age")?.Value ?? "0") > 20
                    select new XElement("Student",
                        student.Element("Name"),
                        student.Element("Age")
                    )
                )
            );

            newDoc.Save("senior_students.xml");
            Console.WriteLine("✓ Новый XML документ создан: senior_students.xml");
        }
    }

    // ==================== 6. ОСНОВНАЯ ПРОГРАММА ====================
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Console.WriteLine("=== ЛАБОРАТОРНАЯ РАБОТА ПО СЕРИАЛИЗАЦИИ ===\n");


            try
            {
                // 1. Создаем тестовые данные
                List<string> courses = new List<string> { "C# Программирование", "Базы данных", "ООП", "Алгоритмы" };
                Student student = new Student("Иван Иванов", 20, courses);

                // 2. Тестируем сериализацию во всех форматах
                SerializationManager manager = new SerializationManager();
                manager.TestAllFormats(student);

                // 3. Тестируем сериализацию коллекции
                List<Student> students = new List<Student>
                {
                    new Student("Анна Петрова", 21, new List<string> {"Математика", "Физика"}),
                    new Student("Петр Сидоров", 22, new List<string> {"Программирование", "Сети"}),
                    new Student("Мария Иванова", 20, new List<string> {"Дизайн", "Рисование"})
                };

                manager.TestCollection(students);

                // 4. Демонстрация XPath
                XmlTools.DemonstrateXPath();

                // 5. Демонстрация LINQ to XML
                XmlTools.DemonstrateLinqToXml();


            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОШИБКА: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }

            Console.WriteLine("\n" + new string('=', 70));
            Console.WriteLine("Нажмите Enter для выхода...");
            Console.ReadLine();
        }
    }
}