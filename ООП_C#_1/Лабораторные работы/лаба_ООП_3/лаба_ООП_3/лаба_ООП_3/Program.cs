using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Класс однонаправленного списка
public class List<T> : IEnumerable<T>
{
    private class Node
    {
        public T Data { get; set; }
        public Node Next { get; set; }

        public Node(T data)
        {
            Data = data;
            Next = null;
        }
    }

    private Node head;
    public int Count { get; private set; }

    public List()
    {
        head = null;
        Count = 0;
    }

    public void Add(T data)
    {
        var newNode = new Node(data);

        if (head == null)
        {
            head = newNode;
        }
        else
        {
            var current = head;
            while (current.Next != null)
            {
                current = current.Next;
            }
            current.Next = newNode;
        }
        Count++;
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();

            var current = head;
            for (int i = 0; i < index; i++)
            {
                current = current.Next;
            }
            return current.Data;
        }
    }

    public static List<T> operator --(List<T> list)
    {
        if (list == null || list.Count <= 1)
            return list;

        var inverted = new List<T>();
        var stack = new Stack<T>();

        foreach (var item in list)
        {
            stack.Push(item);
        }

        while (stack.Count > 0)
        {
            inverted.Add(stack.Pop());
        }

        return inverted;
    }

    public static List<T> operator +(List<T> list1, List<T> list2)
    {
        var result = new List<T>();

        if (list1 != null)
        {
            foreach (var item in list1)
            {
                result.Add(item);
            }
        }

        if (list2 != null)
        {
            foreach (var item in list2)
            {
                result.Add(item);
            }
        }

        return result;
    }

    public static bool operator ==(List<T> list1, List<T> list2)
    {
        if (ReferenceEquals(list1, list2)) return true;
        if (list1 is null || list2 is null) return false;
        if (list1.Count != list2.Count) return false;

        using (var e1 = list1.GetEnumerator())
        using (var e2 = list2.GetEnumerator())
        {
            while (e1.MoveNext() && e2.MoveNext())
            {
                if (!EqualityComparer<T>.Default.Equals(e1.Current, e2.Current))
                    return false;
            }
        }
        return true;
    }

    public static bool operator !=(List<T> list1, List<T> list2)
    {
        return !(list1 == list2);
    }

    public static List<T> operator <(List<T> list1, List<T> list2)
    {
        return list1 + list2;
    }

    public static List<T> operator >(List<T> list1, List<T> list2)
    {
        return list2 + list1;
    }

    public IEnumerator<T> GetEnumerator()
    {
        var current = head;
        while (current != null)
        {
            yield return current.Data;
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override bool Equals(object obj)
    {
        return this == (obj as List<T>);
    }

    public override int GetHashCode()
    {
        int hash = 17;
        foreach (var item in this)
        {
            hash = hash * 23 + (item?.GetHashCode() ?? 0);
        }
        return hash;
    }

    public override string ToString()
    {
        return $"List[{string.Join(", ", this)}]";
    }

    public class Developer
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Department { get; set; }

        public Developer(string name, string id, string department)
        {
            Name = name;
            Id = id;
            Department = department;
        }

        public override string ToString()
        {
            return $"Developer[Name: {Name}, ID: {Id}, Department: {Department}]";
        }

        public override bool Equals(object obj)
        {
            return obj is Developer other &&
                   Name == other.Name &&
                   Id == other.Id &&
                   Department == other.Department;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Id, Department);
        }
    }
}

// Статический класс StatisticOperations
public static class StatisticOperations
{
    // 1) Сумма элементов списка (для числовых типов)
    public static T Sum<T>(List<T> list) where T : struct
    {
        if (list == null || list.Count == 0)
            return default(T);

        dynamic sum = default(T);
        foreach (var item in list)
        {
            sum += (dynamic)item;
        }
        return (T)sum;
    }

    // 2) Разница между максимальным и минимальным элементами
    public static T DifferenceBetweenMaxAndMin<T>(List<T> list) where T : IComparable
    {
        if (list == null || list.Count == 0)
            return default(T);

        T min = list[0];
        T max = list[0];

        foreach (var item in list)
        {
            if (((IComparable)item).CompareTo(min) < 0)
                min = item;
            if (((IComparable)item).CompareTo(max) > 0)
                max = item;
        }

        dynamic result = max;
        result -= min;
        return (T)result;
    }

    // 3) Подсчет количества элементов
    public static int CountElements<T>(List<T> list)
    {
        return list?.Count ?? 0;
    }

    // Методы расширения для типа string

    // 1) Подсчет количества цифр в строке
    public static int CountDigits(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0;

        int count = 0;
        foreach (char c in str)
        {
            if (char.IsDigit(c))
                count++;
        }
        return count;
    }

    // 2) Проверка на палиндром
    public static bool IsPalindrome(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return true;

        string cleanStr = new string(str.Where(char.IsLetterOrDigit).ToArray()).ToLower();
        int left = 0;
        int right = cleanStr.Length - 1;

        while (left < right)
        {
            if (cleanStr[left] != cleanStr[right])
                return false;
            left++;
            right--;
        }
        return true;
    }

    // Методы расширения для типа List<T>

    // 1) Проверка на наличие отрицательных элементов (для числовых типов)
    public static bool HasNegativeElements<T>(this List<T> list) where T : struct
    {
        if (list == null)
            return false;

        foreach (var item in list)
        {
            dynamic value = item;
            if (value < 0)
                return true;
        }
        return false;
    }

    // 2) Удаление дубликатов
    public static List<T> RemoveDuplicates<T>(this List<T> list)
    {
        if (list == null)
            return new List<T>();

        var result = new List<T>();
        var seen = new HashSet<T>();

        foreach (var item in list)
        {
            if (seen.Add(item))
            {
                result.Add(item);
            }
        }

        return result;
    }
}

// Тестирование
class Program
{
    static void Main()
    {
        Console.WriteLine("=== Тестирование класса List ===\n");

        TestIntegerOperations();
        TestStringOperations();
        TestDeveloperClass();
        TestStatisticOperations();
        TestExtensionMethods();

        Console.WriteLine("\n=== Все тесты завершены ===");
        Console.ReadKey();
    }

    static void TestIntegerOperations()
    {
        Console.WriteLine("1. Тестирование операторов с целыми числами:");

        var list1 = new List<int>();
        list1.Add(1);
        list1.Add(2);
        list1.Add(3);

        var list2 = new List<int>();
        list2.Add(4);
        list2.Add(5);

        var list3 = new List<int>();
        list3.Add(1);
        list3.Add(2);
        list3.Add(3);

        Console.WriteLine($"list1: {list1}");
        Console.WriteLine($"list2: {list2}");
        Console.WriteLine($"list3: {list3}");

        var inverted = --list1;
        Console.WriteLine($"--list1: {inverted}");

        var combined = list1 + list2;
        Console.WriteLine($"list1 + list2: {combined}");

        Console.WriteLine($"list1 == list3: {list1 == list3}");
        Console.WriteLine($"list1 == list2: {list1 == list2}");

        var appended = list1 < list2;
        Console.WriteLine($"list1 < list2: {appended}");
    }

    static void TestStringOperations()
    {
        Console.WriteLine("\n2. Тестирование операторов со строками:");

        var strList1 = new List<string>();
        strList1.Add("Hello");
        strList1.Add("World");

        var strList2 = new List<string>();
        strList2.Add("C#");
        strList2.Add("Programming");

        Console.WriteLine($"strList1: {strList1}");
        Console.WriteLine($"strList2: {strList2}");

        var inverted = --strList1;
        Console.WriteLine($"--strList1: {inverted}");

        var combined = strList1 + strList2;
        Console.WriteLine($"strList1 + strList2: {combined}");
    }

    static void TestDeveloperClass()
    {
        Console.WriteLine("\n3. Тестирование вложенного класса Developer:");

        var dev1 = new List<string>.Developer("Иван Петров", "DEV001", "Backend");
        var dev2 = new List<string>.Developer("Мария Сидорова", "DEV002", "Frontend");

        Console.WriteLine($"Разработчик 1: {dev1}");
        Console.WriteLine($"Разработчик 2: {dev2}");
    }

    static void TestStatisticOperations()
    {
        Console.WriteLine("\n4. Тестирование StatisticOperations:");

        // Тестирование с целыми числами
        var intList = new List<int>();
        intList.Add(5);
        intList.Add(2);
        intList.Add(8);
        intList.Add(1);
        intList.Add(9);
        intList.Add(3);

        Console.WriteLine($"Список целых: {intList}");

        // Вызов методов через StatisticOperations
        Console.WriteLine($"Сумма: {StatisticOperations.Sum(intList)}");
        Console.WriteLine($"Разница макс-мин: {StatisticOperations.DifferenceBetweenMaxAndMin(intList)}");
        Console.WriteLine($"Количество элементов: {StatisticOperations.CountElements(intList)}");

        // Тестирование с числами с плавающей точкой
        var doubleList = new List<double>();
        doubleList.Add(1.5);
        doubleList.Add(3.7);
        doubleList.Add(2.1);
        doubleList.Add(5.8);
        doubleList.Add(0.5);

        Console.WriteLine($"\nСписок double: {doubleList}");
        Console.WriteLine($"Сумма: {StatisticOperations.Sum(doubleList)}");
        Console.WriteLine($"Разница макс-мин: {StatisticOperations.DifferenceBetweenMaxAndMin(doubleList)}");

        // Тестирование с отрицательными числами
        var negativeList = new List<int>();
        negativeList.Add(-5);
        negativeList.Add(2);
        negativeList.Add(-8);
        negativeList.Add(1);
        negativeList.Add(9);

        Console.WriteLine($"\nСписок с отрицательными: {negativeList}");
        Console.WriteLine($"Есть отрицательные: {negativeList.HasNegativeElements()}");

        // Тестирование удаления дубликатов
        var duplicateList = new List<int>();
        duplicateList.Add(1);
        duplicateList.Add(2);
        duplicateList.Add(2);
        duplicateList.Add(3);
        duplicateList.Add(4);
        duplicateList.Add(4);
        duplicateList.Add(4);
        duplicateList.Add(5);

        Console.WriteLine($"\nСписок с дубликатами: {duplicateList}");
        Console.WriteLine($"Без дубликатов: {duplicateList.RemoveDuplicates()}");
    }

    static void TestExtensionMethods()
    {
        Console.WriteLine("\n5. Тестирование методов расширения для string:");

        // Тестирование методов для string
        string testStr1 = "Hello123World45";
        string testStr2 = "А роза упала на лапу Азора";
        string testStr3 = "racecar";

        Console.WriteLine($"Строка '{testStr1}' содержит цифр: {testStr1.CountDigits()}");
        Console.WriteLine($"Строка '{testStr2}' является палиндромом: {testStr2.IsPalindrome()}");
        Console.WriteLine($"Строка '{testStr3}' является палиндромом: {testStr3.IsPalindrome()}");
    }
}