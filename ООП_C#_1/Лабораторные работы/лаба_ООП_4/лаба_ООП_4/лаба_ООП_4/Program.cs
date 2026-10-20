using System;
using System.Collections.Generic;

// Интерфейс для музыкальных инструментов
public interface IMusicalInstrument
{
    string Name { get; set; }
    void Play(); 
    bool Tune(); 
}

// Абстрактный базовый класс для музыкальных инструментов
public abstract class MusicalInstrument
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Material { get; set; }

    protected MusicalInstrument(string name, decimal price, string material)
    {
        Name = name;
        Price = price;
        Material = material;
    }

    public virtual void DisplayInfo() 
    {
        Console.WriteLine($"Инструмент: {Name}, Материал: {Material}, Цена: {Price} руб.");
    }
    public abstract void MakeSound(); 
    
    public virtual bool Tune() 
    {
        Console.WriteLine($"{Name} настраивается...");
        return true;
    }

    public override string ToString()
    {
        return $"Тип: {GetType().Name}, Название: {Name}, Материал: {Material}, Цена: {Price} руб.";
    }

    public override bool Equals(object obj) 
    {
        if (obj is MusicalInstrument other)
        {
            return Name == other.Name && Material == other.Material && Price == other.Price;
        }
        return false;
    }

    public override int GetHashCode() 
    {
        return HashCode.Combine(Name, Material, Price);
    }
}

// Класс для струнных инструментов
public class StringInstrument : MusicalInstrument, IMusicalInstrument
{
    public int StringCount { get; set; }
    public string BowType { get; set; }

    public StringInstrument(string name, decimal price, string material, int stringCount, string bowType)
        : base(name, price, material)
    {
        StringCount = stringCount;
        BowType = bowType;
    }

    public override void MakeSound()
    {
        Console.WriteLine($"{Name} издает струнный звук с {StringCount} струнами");
    }

    public void Play() 
    {
        Console.WriteLine($"{Name} играет с использованием {BowType}");
    }

    bool IMusicalInstrument.Tune() 
    {
        Console.WriteLine($"{Name} настраивается через интерфейс (точная настройка струн)");
        return true;
    }

    public override bool Tune() 
    {
        Console.WriteLine($"{Name} настраивается через базовый класс (общая настройка)");
        return true;
    }

    public override string ToString() 
    {
        return base.ToString() + $", Количество струн: {StringCount}, Тип смычка: {BowType}";
    }
}

// Класс для духовых инструментов
public class WindInstrument : MusicalInstrument, IMusicalInstrument // W наслед от M и реализ интерфейс
{
    public string WindType { get; set; }
    public int KeyCount { get; set; }

    public WindInstrument(string name, decimal price, string material, string windType, int keyCount)
        : base(name, price, material)
    {
        WindType = windType;
        KeyCount = keyCount;
    }

    public override void MakeSound()
    {
        Console.WriteLine($"{Name} издает духовой звук типа {WindType}");
    }

    public void Play() 
    {
        Console.WriteLine($"{Name} играет с использованием {KeyCount} клавиш");
    }

    bool IMusicalInstrument.Tune() 
    {
        Console.WriteLine($"{Name} настраивается через интерфейс (настройка амбушюра)");
        return true;
    }

    public override bool Tune() 
    {
        Console.WriteLine($"{Name} настраивается через базовый класс (регулировка мундштука)");
        return true;
    }

    public override string ToString() 
    {
        return base.ToString() + $", Тип дыхания: {WindType}, Количество клавиш: {KeyCount}";
    }
}

// Sealed класс для ударных инструментов
public sealed class PercussionInstrument : MusicalInstrument, IMusicalInstrument
{
    public string StrikeType { get; set; }
    public bool IsTuned { get; set; }

    public PercussionInstrument(string name, decimal price, string material, string strikeType, bool isTuned)
        : base(name, price, material)
    {
        StrikeType = strikeType;
        IsTuned = isTuned;
    }

    public override void MakeSound() 
    {
        Console.WriteLine($"{Name} издает ударный звук типа {StrikeType}");
    }

    public void Play()
    {
        Console.WriteLine($"{Name} играет с использованием {StrikeType} ударов");
    }

    bool IMusicalInstrument.Tune() 
    {
        if (IsTuned)
        {
            Console.WriteLine($"{Name} настраивается через интерфейс (настройка мембраны)");
            return true;
        }
        Console.WriteLine($"{Name} не требует настройки");
        return false;
    }

    public override bool Tune() 
    {
        Console.WriteLine($"{Name} настраивается через базовый класс (общая проверка)");
        return IsTuned;
    }

    public override string ToString() 
    {
        return base.ToString() + $", Тип удара: {StrikeType}, Настраиваемый: {IsTuned}";
    }
}

// Класс Printer для демонстрации полиморфизма
public class Printer
{
    public void IAmPrinting(IMusicalInstrument someObj) 
    {
        if (someObj is IMusicalInstrument musicalInstrument)
        {
            Console.WriteLine($"1{musicalInstrument}") ;
        }
        else if (someObj is StringInstrument stringInstrument)
        {
            Console.WriteLine($"1{stringInstrument}");
        }
        else
        {
            Console.WriteLine("Неизвестный тип");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Лабораторная работа по ООП ===");
        Console.WriteLine("=== Иерархия музыкальных инструментов ===\n");

        // Создание объектов различных классов
        var violin = new StringInstrument("Скрипка", 15000, "Дерево", 4, "Смычковый");
        var trumpet = new WindInstrument("Труба", 8000, "Латунь", "Медный духовой", 3);
        var drum = new PercussionInstrument("Барабан", 5000, "Дерево и кожа", "Ударный", true);
        var flute = new WindInstrument("Флейта", 3000, "Дерево", "Деревянный духовой", 0);

        // Работа с объектами через ссылки на абстрактные классы и интерфейсы
        MusicalInstrument[] instruments = { violin, trumpet, drum, flute };
        IMusicalInstrument[] musicInstruments = { violin, trumpet, drum, flute };

        // Демонстрация работы с операторами is и as
        Console.WriteLine("=== Использование операторов is и as ==="); // для бп и пт
        foreach (var instrument in instruments)
        {
            if (instrument is StringInstrument stringInstrument) //пт + прив
            {
                Console.WriteLine($"{instrument.Name} - это струнный инструмент с {stringInstrument.StringCount} струнами");
            }

            if (instrument is WindInstrument windInstrument)
            {
                Console.WriteLine($"{instrument.Name} - это духовой инструмент с {windInstrument.KeyCount} клавишами");
            }

            var percussion = instrument as PercussionInstrument;
            if (percussion != null)
            {
                Console.WriteLine($"{instrument.Name} - это ударный инструмент типа {percussion.StrikeType}");
            }
        }

        // Вызов одноименных методов с разной реализацией
        Console.WriteLine("\n=== Вызов одноименных методов ===");
        foreach (var instrument in musicInstruments)
        {
            ((IMusicalInstrument)instrument).Tune();
            if (instrument is MusicalInstrument musicalInstrument)
            {
                musicalInstrument.Tune(); // Вызов через базовый класс
            }
            Console.WriteLine();
        }

        // Демонстрация переопределенного метода ToString()
        Console.WriteLine("=== Информация об инструментах через ToString() ===");
        foreach (var instrument in instruments)
        {
            Console.WriteLine(instrument.ToString());
        }

        // Работа с классом Printer
        Console.WriteLine("\n=== Демонстрация полиморфного метода IAmPrinting ===");
        var printer = new Printer();

        // Создание массива с разнотипными объектами
        object[] objects = { violin, trumpet, drum, flute, "не инструмент", 123 };

        foreach (var obj in objects)
        {
            if (obj is IMusicalInstrument musicalObj)
            {
                printer.IAmPrinting(musicalObj); 
            }
            else
            {
                Console.WriteLine($"Объект {obj} не является музыкальным инструментом");
            }
        }

        // Демонстрация других переопределенных методов Object
        Console.WriteLine("\n=== Демонстрация Equals и GetHashCode ===");
        var violin2 = new StringInstrument("Скрипка", 15000, "Дерево", 4, "Смычковый");
        Console.WriteLine($"violin.Equals(violin2): {violin.Equals(violin2)}");
        Console.WriteLine($"Хэш-код скрипки: {violin.GetHashCode()}");
        Console.WriteLine($"Хэш-код второй скрипки: {violin2.GetHashCode()}");

        Console.WriteLine("\n=== Программа завершена ===");
        Console.ReadKey();
    }
}