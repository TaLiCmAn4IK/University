using System;
using System.Collections.Generic;
using System.Linq;
using static System.Formats.Asn1.AsnWriter;

// Перечисление для типов музыкальных инструментов
public enum InstrumentType
{
    String,
    Wind,
    Percussion
}

// Структура для характеристик инструмента
public struct InstrumentSpecs
{
    public double Weight { get; set; }
    public int YearOfManufacture { get; set; }
    public string Manufacturer { get; set; }

    public override string ToString()
    {
        return $"Вес: {Weight}кг, Год: {YearOfManufacture}, Производитель: {Manufacturer}";
    }
}

// Интерфейс для музыкальных инструментов
public interface IMusicalInstrument
{
    string Name { get; set; }
    void Play();
    bool Tune();
}

// Базовый partial класс для музыкальных инструментов
public abstract partial class MusicalInstrument
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Material { get; set; }
    public InstrumentType Type { get; set; }
    public InstrumentSpecs Specs { get; set; }

    protected MusicalInstrument(string name, decimal price, string material, InstrumentType type, InstrumentSpecs specs)
    {
        Name = name;
        Price = price;
        Material = material;
        Type = type;
        Specs = specs;
    }

    public abstract void MakeSound();

    public virtual bool Tune()
    {
        Console.WriteLine($"{Name} настраивается...");
        return true;
    }

    public override string ToString()
    {
        return $"Тип: {GetType().Name}, Название: {Name}, Цена: {Price} руб., {Specs}";
    }
}

// Вторая часть partial класса
public partial class MusicalInstrument
{
    public virtual double CalculateMaintenanceCost()
    {
        return (double)(Price * 0.05m);
    }

    public bool IsVintage()
    {
        return Specs.YearOfManufacture < 2000;
    }
}

// Класс для струнных инструментов
public class StringInstrument : MusicalInstrument, IMusicalInstrument
{
    public int StringCount { get; set; }

    public StringInstrument(string name, decimal price, string material, int stringCount, InstrumentSpecs specs)
        : base(name, price, material, InstrumentType.String, specs)
    {
        StringCount = stringCount;
    }

    public override void MakeSound()
    {
        Console.WriteLine($"{Name} издает струнный звук с {StringCount} струнами");
    }

    public void Play()
    {
        Console.WriteLine($"{Name} играет");
    }

    bool IMusicalInstrument.Tune()
    {
        Console.WriteLine($"{Name} настраивается через интерфейс");
        return true;
    }

    public override string ToString()
    {
        return base.ToString() + $", Струн: {StringCount}";
    }
}

// Класс для духовых инструментов
public class WindInstrument : MusicalInstrument, IMusicalInstrument
{
    public int KeyCount { get; set; }

    public WindInstrument(string name, decimal price, string material, int keyCount, InstrumentSpecs specs)
        : base(name, price, material, InstrumentType.Wind, specs)
    {
        KeyCount = keyCount;
    }

    public override void MakeSound()
    {
        Console.WriteLine($"{Name} издает духовой звук");
    }

    public void Play()
    {
        Console.WriteLine($"{Name} играет с {KeyCount} клавишами");
    }

    bool IMusicalInstrument.Tune()
    {
        Console.WriteLine($"{Name} настраивается через интерфейс");
        return true;
    }

    public override string ToString()
    {
        return base.ToString() + $", Клавиш: {KeyCount}";
    }
}

// Sealed класс для ударных инструментов
public sealed class PercussionInstrument : MusicalInstrument, IMusicalInstrument
{
    public bool IsTuned { get; set; }

    public PercussionInstrument(string name, decimal price, string material, bool isTuned, InstrumentSpecs specs)
        : base(name, price, material, InstrumentType.Percussion, specs)
    {
        IsTuned = isTuned;
    }

    public override void MakeSound()
    {
        Console.WriteLine($"{Name} издает ударный звук");
    }

    public void Play()
    {
        Console.WriteLine($"{Name} играет");
    }

    bool IMusicalInstrument.Tune()
    {
        Console.WriteLine($"{Name} настраивается через интерфейс");
        return IsTuned;
    }

    public override string ToString()
    {
        return base.ToString() + $", Настраиваемый: {IsTuned}";
    }
}

// Класс-контейнер для хранения музыкальных инструментов
public class MusicStore
{
    private List<MusicalInstrument> instruments; // колл объ
    public decimal Budget { get; private set; }
    public int MaxCapacity { get; private set; }

    public MusicStore(decimal budget, int maxCapacity)
    {
        instruments = new List<MusicalInstrument>();
        Budget = budget;
        MaxCapacity = maxCapacity;
    }

    public bool AddInstrument(MusicalInstrument instrument)
    {
        if (instruments.Count >= MaxCapacity)
        {
            Console.WriteLine("Достигнуто максимальное количество инструментов");
            return false;
        }

        if (GetTotalCost() + instrument.Price > Budget)
        {
            Console.WriteLine("Превышен бюджет");
            return false;
        }

        instruments.Add(instrument);
        return true;
    }

    public bool RemoveInstrumentAt(int index)
    {
        if (index >= 0 && index < instruments.Count)
        {
            instruments.RemoveAt(index);
            return true;
        }
        return false;
    }

    public void PrintAllInstruments()
    {
        Console.WriteLine($"=== Музыкальный магазин (Бюджет: {Budget} руб., Вместимость: {MaxCapacity}) ===");
        Console.WriteLine($"Всего инструментов: {instruments.Count}, Общая стоимость: {GetTotalCost()} руб.");

        for (int i = 0; i < instruments.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {instruments[i]}");
        }
        Console.WriteLine();
    }

    public decimal GetTotalCost()
    {
        return instruments.Sum(instr => instr.Price);
    }

    public int GetInstrumentCount()
    {
        return instruments.Count;
    }

    public decimal GetRemainingBudget()
    {
        return Budget - GetTotalCost();
    }

    public List<MusicalInstrument> GetInstruments()
    {
        return new List<MusicalInstrument>(instruments);
    }
}

// Класс-контроллер для управления музыкальным магазином
public class StoreController
{
    private MusicStore store; // работает с контейнером

    public StoreController(MusicStore store) // конструктор 
    {
        this.store = store;
    }
    //фильтрация по диапазону цен
    public List<MusicalInstrument> FindInstrumentsByPriceRange(decimal minPrice, decimal maxPrice)
    {
        var instruments = store.GetInstruments();
        return instruments.Where(instr => instr.Price >= minPrice && instr.Price <= maxPrice).ToList();
    }
    //сортировка по цене
    public List<MusicalInstrument> SortInstrumentsByPrice(bool ascending = true)
    {
        var instruments = store.GetInstruments();
        return ascending ?
            instruments.OrderBy(instr => instr.Price).ToList() :
            instruments.OrderByDescending(instr => instr.Price).ToList();
    }
    //фильтрация по типу инструмента
    public List<MusicalInstrument> FindInstrumentsByType(InstrumentType type)
    {
        var instruments = store.GetInstruments();
        return instruments.Where(instr => instr.Type == type).ToList();
    }
    public void PrintStoreStatistics()
    {
        var instruments = store.GetInstruments();

        Console.WriteLine("=== Статистика музыкального магазина ===");
        Console.WriteLine($"Общее количество инструментов: {instruments.Count}");
        Console.WriteLine($"Общая стоимость: {store.GetTotalCost()} руб.");
        Console.WriteLine($"Остаток бюджета: {store.GetRemainingBudget()} руб.");

        var byType = instruments.GroupBy(instr => instr.Type)
                               .Select(g => new { Type = g.Key, Count = g.Count() });

        foreach (var group in byType)
        {
            Console.WriteLine($"{group.Type}: {group.Count} шт.");
        }
        Console.WriteLine();
    }
}

// Класс Printer для демонстрации полиморфизма
public class Printer
{
    public void IAmPrinting(IMusicalInstrument someObj)
    {
        Console.WriteLine($"Тип: {someObj.GetType().Name}");
        Console.WriteLine(someObj.ToString());
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Лабораторная работа 5 ===\n");

        // Создание инструментов
        var violin = new StringInstrument("Скрипка", 15000, "Дерево", 4,
            new InstrumentSpecs { Weight = 0.5, YearOfManufacture = 2020, Manufacturer = "Stradivarius Co" });

        var trumpet = new WindInstrument("Труба", 8000, "Латунь", 3,
            new InstrumentSpecs { Weight = 1.2, YearOfManufacture = 2018, Manufacturer = "Yamaha" });

        var drum = new PercussionInstrument("Барабан", 5000, "Дерево", true,
            new InstrumentSpecs { Weight = 15.0, YearOfManufacture = 2019, Manufacturer = "Pearl" });

        var guitar = new StringInstrument("Гитара", 12000, "Дерево", 6,
            new InstrumentSpecs { Weight = 2.1, YearOfManufacture = 1995, Manufacturer = "Fender" });
        var piano = new StringInstrument("Пианино", 30000, "Дерево", 88,
            new InstrumentSpecs { Weight = 250.0, YearOfManufacture = 2021, Manufacturer = "Yamaha" });

        // Создание контейнера и контроллера
        var musicStore = new MusicStore(50000, 10); // контейнер
        var musicBook = new MusicStore(10000, 10);
        var controller = new StoreController(musicStore);
        var controller_1 = new StoreController(musicBook);
        controller_1.PrintStoreStatistics();
        musicBook.AddInstrument(piano);
        musicBook.AddInstrument(guitar);


        musicStore.AddInstrument(trumpet);
        musicStore.AddInstrument(drum);
        musicStore.AddInstrument(guitar);

        // Вывод всех обьектов
        musicStore.PrintAllInstruments();

        // Демонстрация работы контроллера
        Console.WriteLine("=== Инструменты в диапазоне 4000-10000 руб. ===");
        var instrumentsInRange = controller.FindInstrumentsByPriceRange(4000, 10000);
        foreach (var instrument in instrumentsInRange)
        {
            Console.WriteLine(instrument);
        }

        Console.WriteLine("\n=== Сортировка по цене (по убыванию) ===");
        var sortedByPrice = controller.SortInstrumentsByPrice(false);
        foreach (var instrument in sortedByPrice)
        {
            Console.WriteLine($"{instrument.Name} - {instrument.Price} руб.");
        }

        Console.WriteLine("\n=== Струнные инструменты ===");
        var stringInstruments = controller.FindInstrumentsByType(InstrumentType.String);
        foreach (var instrument in stringInstruments)
        {
            Console.WriteLine(instrument);
        }

        // Статистика
        controller.PrintStoreStatistics();

        // Демонстрация partial класса
        Console.WriteLine("=== Методы partial класса ===");
        foreach (var instrument in musicStore.GetInstruments())
        {
            Console.WriteLine($"{instrument.Name}:");
            Console.WriteLine($"  Обслуживание: {instrument.CalculateMaintenanceCost():F2} руб.");
            Console.WriteLine($"  Винтажный: {(instrument.IsVintage() ? "Да" : "Нет")}");
            instrument.MakeSound(); 
            instrument.Tune();       
        }

        // Демонстрация работы с бюджетом и вместимостью
        Console.WriteLine($"\nОсталось места: {musicStore.MaxCapacity - musicStore.GetInstrumentCount()}");
        Console.WriteLine($"Остаток бюджета: {musicStore.GetRemainingBudget()} руб.");

        // Демонстрация полиморфизма через Printer
        Console.WriteLine("\n=== Демонстрация полиморфного метода IAmPrinting ===");
        var printer = new Printer();
        var musicalInstruments = new IMusicalInstrument[] { violin, trumpet, drum, guitar };

        foreach (var instrument in musicalInstruments)
        {
            printer.IAmPrinting(instrument);
        }

        // Демонстрация одноименных методов
        Console.WriteLine("=== Одноименные методы ===");
        foreach (var instrument in musicalInstruments)
        {
            Console.WriteLine($"--- {instrument.Name} ---");
            ((IMusicalInstrument)instrument).Tune();
            if (instrument is MusicalInstrument musicalInstrument)
            {
                musicalInstrument.Tune();
            }
            Console.WriteLine();
        }

        Console.WriteLine("\n=== Программа завершена ===");
        Console.ReadKey();
    }
    
}