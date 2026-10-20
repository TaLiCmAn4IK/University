using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Lab7Generics
{
    // Обобщённый интерфейс
    public interface IMyCollection<T> : IEnumerable<T>
    {
        void Add(T item);
        bool Remove(Predicate<T> match);
        IEnumerable<T> Find(Func<T, bool> predicate);
        void Print();
        int Count { get; }
    }

    // Базовый пользовательский класс 
    public class Person : IComparable<Person>
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public Person() { }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public int CompareTo(Person other)
        {
            if (other == null) return 1;
            return Age.CompareTo(other.Age);
        }

        public override string ToString()
        {
            return $"{Name} ({Age})";
        }
    }

    // Производный класс – пользовательский тип для обобщения
    public class Student : Person, IComparable<Student>
    {
        public string Group { get; set; }

        public Student() { }

        public Student(string name, int age, string group)
            : base(name, age)
        {
            Group = group;
        }

        public int CompareTo(Student other)
        {
            if (other == null) return 1;
            int nameCmp = string.Compare(Name, other.Name, StringComparison.Ordinal);
            if (nameCmp != 0) return nameCmp;
            return Age.CompareTo(other.Age);
        }

        public override string ToString()
        {
            return $"{Name}, {Age} лет, гр. {Group}";
        }
    }

    // Обобщённая коллекция
    public class CollectionType<T> : IMyCollection<T>, IEquatable<CollectionType<T>>
        where T : IComparable<T>//
    {
        private readonly List<T> _items = new List<T>();

        public int Count => _items.Count;

        // Индексатор с обработкой исключений и finally
        public T this[int index]
        {
            get
            {
                try
                {
                    if (index < 0 || index >= _items.Count)
                        throw new ArgumentOutOfRangeException(nameof(index),
                            "Индекс выходит за границы коллекции");
                    return _items[index];
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Console.WriteLine($"Ошибка доступа по индексу: {ex.Message}");
                    throw;
                }
                finally
                {
                    Console.WriteLine("Операция доступа по индексу завершена (finally).");
                }
            }
        }

        // Реализация интерфейса

        public void Add(T item)
        {
            try
            {
                _items.Add(item);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении элемента: {ex.Message}");
                throw;
            }
            finally
            {
                Console.WriteLine("Операция Add завершена (finally).");
            }
        }

        public bool Remove(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            try
            {
                int removed = _items.RemoveAll(match);
                if (removed == 0)
                    throw new InvalidOperationException("Элемент, удовлетворяющий предикату, не найден.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении: {ex.Message}");
                return false;
            }
            finally
            {
                Console.WriteLine("Операция Remove завершена (finally).");
            }
        }

        public IEnumerable<T> Find(Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            foreach (var item in _items)
                if (predicate(item))
                    yield return item;
        }

        public void Print()
        {
            Console.WriteLine(ToString());
        }

        // Перегрузки операторов

        public static CollectionType<T> operator --(CollectionType<T> collection)
        {
            if (collection == null || collection._items.Count <= 1)
                return collection;

            var result = new CollectionType<T>();
            for (int i = collection._items.Count - 1; i >= 0; i--)
                result._items.Add(collection._items[i]);
            return result;
        }

        public static CollectionType<T> operator +(CollectionType<T> a, CollectionType<T> b)
        {
            var result = new CollectionType<T>();
            if (a != null)
                result._items.AddRange(a._items);
            if (b != null)
                result._items.AddRange(b._items);
            return result;
        }

        public static CollectionType<T> operator <(CollectionType<T> a, CollectionType<T> b)
        {
            return a + b;
        }

        public static CollectionType<T> operator >(CollectionType<T> a, CollectionType<T> b)
        {
            return b + a;
        }

        public static bool operator ==(CollectionType<T> a, CollectionType<T> b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            if (a._items.Count != b._items.Count) return false;

            for (int i = 0; i < a._items.Count; i++)
                if (!EqualityComparer<T>.Default.Equals(a._items[i], b._items[i]))
                    return false;

            return true;
        }

        public static bool operator !=(CollectionType<T> a, CollectionType<T> b) => !(a == b);

        public bool Equals(CollectionType<T> other) => this == other;

        public override bool Equals(object obj) => Equals(obj as CollectionType<T>);

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var item in _items)
                hash = hash * 23 + (item?.GetHashCode() ?? 0);
            return hash;
        }

        public override string ToString()
        {
            return $"CollectionType<{typeof(T).Name}>[{string.Join(", ", _items)}]";
        }

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // Сохранение / загрузка JSON

        public void SaveToJson(string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_items, options);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении в файл: {ex.Message}");
                throw;
            }
            finally
            {
                Console.WriteLine("Операция SaveToJson завершена (finally).");
            }
        }
        // загрузка из файла
        public static CollectionType<T> LoadFromJson(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("Файл не найден.", filePath);

                string json = File.ReadAllText(filePath);
                var list = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();

                var collection = new CollectionType<T>();
                foreach (var item in list)
                    collection._items.Add(item);

                return collection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке из файла: {ex.Message}");
                throw;
            }
            finally
            {
                Console.WriteLine("Операция LoadFromJson завершена (finally).");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Коллекция int ===");
            var intCol = new CollectionType<int>();
            intCol.Add(10);
            intCol.Add(-3);
            intCol.Add(7);
            intCol.Add(5);
            intCol.Print();

            Console.WriteLine("Элементы > 0:");
            foreach (var x in intCol.Find(v => v > 0))
                Console.WriteLine(x);

            intCol.Remove(v => v < 0);
            Console.WriteLine("После удаления отрицательных:");
            intCol.Print();

            var reversedInt = --intCol;
            Console.WriteLine("Реверс (--intCol):");
            reversedInt.Print();

            Console.WriteLine("\n=== Коллекция double ===");
            var doubleCol = new CollectionType<double>();
            doubleCol.Add(1.5);
            doubleCol.Add(3.2);
            doubleCol.Add(0.7);
            doubleCol.Print();

            Console.WriteLine("\n=== Коллекция string ===");
            var stringCol1 = new CollectionType<string>();
            stringCol1.Add("Hello");
            stringCol1.Add("World");

            var stringCol2 = new CollectionType<string>();
            stringCol2.Add("C#");
            stringCol2.Add("Generics");

            Console.WriteLine("stringCol1:");
            stringCol1.Print();
            Console.WriteLine("stringCol2:");
            stringCol2.Print();

            var concat = stringCol1 + stringCol2;
            Console.WriteLine("stringCol1 + stringCol2:");
            concat.Print();

            Console.WriteLine($"stringCol1 == stringCol2: {stringCol1 == stringCol2}");

            Console.WriteLine("\n=== Коллекция Student (пользовательский тип) ===");
            var students = new CollectionType<Student>();
            students.Add(new Student("Иванов И.И.", 19, "ИС-21"));
            students.Add(new Student("Петров П.П.", 20, "ИС-21"));
            students.Add(new Student("Сидорова А.А.", 18, "ИС-22"));
            students.Print();

            Console.WriteLine("Студенты старше 18:");
            foreach (var s in students.Find(st => st.Age > 18))
                Console.WriteLine(s);

            string path = "students.json";
            students.SaveToJson(path);

            Console.WriteLine("\nКоллекция после загрузки из файла:");
            // загр из json
            var loadedStudents = CollectionType<Student>.LoadFromJson(path);
            loadedStudents.Print();

            Console.WriteLine("\n=== Конец демонстрации ===");
            Console.ReadKey();
        }
    }
}
