using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Lab9_Collections
{
    // Класс по варианту: Интернет‑ресурс
    public class InternetResource
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public int Popularity { get; set; } // условный рейтинг

        public InternetResource(int id, string url, string title, int popularity)
        {
            Id = id;
            Url = url;
            Title = title;
            Popularity = popularity;
        }

        public override string ToString()
        {
            return $"[{Id}] {Title} ({Url}), popularity = {Popularity}";
        }
    }

    // Класс‑обёртка, реализующий IList<InternetResource>
    public class InternetResourceList : IList<InternetResource>
    {
        private readonly List<InternetResource> _inner = new();

        public InternetResource this[int index]
        {
            get => _inner[index];
            set => _inner[index] = value;
        }

        public int Count => _inner.Count;
        public bool IsReadOnly => false;

        public void Add(InternetResource item) => _inner.Add(item);
        public void Clear() => _inner.Clear();
        public bool Contains(InternetResource item) => _inner.Contains(item);
        public void CopyTo(InternetResource[] array, int arrayIndex) => _inner.CopyTo(array, arrayIndex);
        public IEnumerator<InternetResource> GetEnumerator() => _inner.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int IndexOf(InternetResource item) => _inner.IndexOf(item);
        public void Insert(int index, InternetResource item) => _inner.Insert(index, item);
        public bool Remove(InternetResource item) => _inner.Remove(item);
        public void RemoveAt(int index) => _inner.RemoveAt(index);

    }

    class Program
    {
        static void Main()
        {
            // 1) Работа с собственным классом + IList<T>
            var resources = new InternetResourceList
            {
                new InternetResource(1, "https://example.com", "Example", 100),
                new InternetResource(2, "https://google.com",  "Google",  1000),
                new InternetResource(3, "https://github.com",  "GitHub",  800)
            };

            // Добавление
            resources.Add(new InternetResource(4, "https://learn.microsoft.com", "MS Docs", 600));

            // Удаление
            resources.RemoveAt(0);

            // 2) Универсальная коллекция (int) + вторая коллекция =====
            GenericCollectionsDemo();

            // 3) ObservableCollection<InternetResource> =====
            ObservableDemo();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        // П.2: универсальная коллекция по варианту (IList<T>) и ConcurrentDictionary<TKey,TValue>
        static void GenericCollectionsDemo()
        {
            Console.WriteLine("=== Generic collections demo ===");

            // a) Универсальная коллекция на основе List<int> как IList<int>
            IList<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
            PrintCollection(numbers, "Initial IList<int>:");

            // b) Удалить n последовательных элементов
            int n = 3;
            int startIndex = 2;
            for (int i = 0; i < n && startIndex < numbers.Count; i++)
            {
                numbers.RemoveAt(startIndex);
            }
            PrintCollection(numbers, $"After removing {n} elements starting at index {startIndex}:");

            // c) Добавить элементы всеми доступными способами для List<T>
            numbers.Add(100);
            numbers.Insert(0, 200);
            ((List<int>)numbers).AddRange(new[] { 300, 400 });
            PrintCollection(numbers, "After various adds:");

            // d) Вторая коллекция – ConcurrentDictionary<int,int> (из таблицы)
            var dict = new ConcurrentDictionary<int, int>();
            int key = 1;
            foreach (var value in numbers)
            {
                dict.TryAdd(key, value);
                key++;
            }

            Console.WriteLine("ConcurrentDictionary<int,int> filled from IList<int>:");
            foreach (var kv in dict)
                Console.WriteLine($"  key={kv.Key}, value={kv.Value}");
            Console.WriteLine();

            // f) Поиск заданного значения во второй коллекции
            int target = 300;
            bool foundValue = false;
            foreach (var kv in dict)
            {
                if (kv.Value == target)
                {
                    foundValue = true;
                    Console.WriteLine($"Found value {target} at key {kv.Key}");
                    break;
                }
            }
            if (!foundValue)
                Console.WriteLine($"Value {target} not found in ConcurrentDictionary");
            Console.WriteLine();
        }

        static void PrintCollection<T>(IEnumerable<T> col, string header)
        {
            Console.WriteLine(header);
            foreach (var x in col)
                Console.Write(x + " ");
            Console.WriteLine();
            Console.WriteLine();
        }

        // П.3: ObservableCollection<InternetResource> + обработчик CollectionChanged [web:41][web:50]
        static void ObservableDemo()
        {
            Console.WriteLine("=== ObservableCollection<InternetResource> demo ===");

            var observable = new ObservableCollection<InternetResource>();
            observable.CollectionChanged += Resources_CollectionChanged;

            // Добавление
            observable.Add(new InternetResource(10, "https://site1.com", "Site 1", 10));
            observable.Add(new InternetResource(11, "https://site2.com", "Site 2", 20));

            // Удаление
            observable.RemoveAt(0);

            // Ещё добавление
            observable.Add(new InternetResource(12, "https://site3.com", "Site 3", 30));
        }

        private static void Resources_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("CollectionChanged fired: " + e.Action);

            if (e.NewItems != null)
            {
                foreach (InternetResource item in e.NewItems)
                    Console.WriteLine("  Added: " + item);
            }

            if (e.OldItems != null)
            {
                foreach (InternetResource item in e.OldItems)
                    Console.WriteLine("  Removed: " + item);
            }

            Console.WriteLine();
        }
    }
}
