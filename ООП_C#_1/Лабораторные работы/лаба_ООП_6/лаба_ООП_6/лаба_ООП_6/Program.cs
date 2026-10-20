using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

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

// Иерархия классов исключений
public class MusicStoreException : Exception
{
    public MusicStoreException(string message) : base(message) { }
    public MusicStoreException(string message, Exception inner) : base(message, inner) { }
}

public class InvalidInstrumentException : MusicStoreException
{
    public InvalidInstrumentException(string message) : base(message) { }
}

public class StoreCapacityException : MusicStoreException
{
    public int CurrentCapacity { get; }
    public int MaxCapacity { get; }

    public StoreCapacityException(string message, int current, int max) : base(message)
    {
        CurrentCapacity = current;
        MaxCapacity = max;
    }
}

public class BudgetExceededException : MusicStoreException
{
    public decimal CurrentBudget { get; }
    public decimal RequiredAmount { get; }

    public BudgetExceededException(string message, decimal current, decimal required) : base(message)
    {
        CurrentBudget = current;
        RequiredAmount = required;
    }
}

public class InstrumentNotFoundException : MusicStoreException
{
    public string InstrumentName { get; }

    public InstrumentNotFoundException(string message, string name) : base(message)
    {
        InstrumentName = name;
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
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название инструмента не может быть пустым");
        if (price <= 0)
            throw new ArgumentException("Цена должна быть положительной");
        if (specs.Weight <= 0)
            throw new ArgumentException("Вес должен быть положительным");
        if (specs.YearOfManufacture < 1900 || specs.YearOfManufacture > DateTime.Now.Year)
            throw new ArgumentException($"Год производства должен быть между 1900 и {DateTime.Now.Year}");

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
   
        int randomFactor = new Random().Next(0, 10);
        if (randomFactor == 0)
        {

            int zero = 0;
            double result = (double)(Price * 0.05m) / zero;
            return result;
        }
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
        if (stringCount <= 0)
            throw new ArgumentException("Количество струн должно быть положительным");

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
        if (keyCount < 0)
            throw new ArgumentException("Количество клавиш не может быть отрицательным");

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
    private List<MusicalInstrument> instruments;
    public decimal Budget { get; private set; }
    public int MaxCapacity { get; private set; }

    public MusicStore(decimal budget, int maxCapacity)
    {
        if (budget <= 0)
            throw new ArgumentException("Бюджет должен быть положительным");
        if (maxCapacity <= 0)
            throw new ArgumentException("Вместимость должна быть положительной");

        instruments = new List<MusicalInstrument>();
        Budget = budget;
        MaxCapacity = maxCapacity;
    }

    public bool AddInstrument(MusicalInstrument instrument)
    {
        // Использование Assert для проверки условий
        Debug.Assert(instrument != null, "Инструмент не может быть null");
        Debug.Assert(instrument.Price > 0, "Цена инструмента должна быть положительной");

        if (instrument == null)
            throw new ArgumentNullException(nameof(instrument), "Инструмент не может быть null");

        if (instruments.Count >= MaxCapacity)
        {
            throw new StoreCapacityException(
                $"Достигнуто максимальное количество инструментов. Текущее: {instruments.Count}, Максимальное: {MaxCapacity}",
                instruments.Count, MaxCapacity);
        }

        decimal totalAfterAdd = GetTotalCost() + instrument.Price;
        if (totalAfterAdd > Budget)
        {
            throw new BudgetExceededException(
                $"Превышен бюджет при добавлении {instrument.Name}. Текущий бюджет: {Budget}, Требуется: {totalAfterAdd}",
                Budget, totalAfterAdd);
        }

        instruments.Add(instrument);
        return true;
    }

    public bool RemoveInstrumentAt(int index)
    {
        if (index < 0 || index >= instruments.Count)
        {
            throw new IndexOutOfRangeException(
                $"Индекс {index} вне диапазона. Допустимый диапазон: 0-{instruments.Count - 1}");
        }

        instruments.RemoveAt(index);
        return true;
    }

    public MusicalInstrument FindInstrumentByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название инструмента не может быть пустым");

        var instrument = instruments.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (instrument == null)
        {
            throw new InstrumentNotFoundException($"Инструмент с названием '{name}' не найден", name);
        }

        return instrument;
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
    private MusicStore store;

    public StoreController(MusicStore store)
    {
        this.store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public List<MusicalInstrument> FindInstrumentsByPriceRange(decimal minPrice, decimal maxPrice)
    {
        if (minPrice < 0 || maxPrice < 0)
            throw new ArgumentException("Цены не могут быть отрицательными");
        if (minPrice > maxPrice)
            throw new ArgumentException("Минимальная цена не может быть больше максимальной");

        var instruments = store.GetInstruments();
        return instruments.Where(instr => instr.Price >= minPrice && instr.Price <= maxPrice).ToList();
    }

    public List<MusicalInstrument> SortInstrumentsByPrice(bool ascending = true)
    {
        var instruments = store.GetInstruments();
        return ascending ?
            instruments.OrderBy(instr => instr.Price).ToList() :
            instruments.OrderByDescending(instr => instr.Price).ToList();
    }

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
        if (someObj == null)
            throw new ArgumentNullException(nameof(someObj), "Объект для печати не может быть null");

        Console.WriteLine($"Тип: {someObj.GetType().Name}");
        Console.WriteLine(someObj.ToString());
    }
}

class Program
{
    static void DemonstrateExceptionHandling()
    {
        Console.WriteLine("=== Демонстрация обработки исключений ===\n");

        try
        {
            // 1. Исключение при создании инструмента с неверными данными
            Console.WriteLine("1. Попытка создать инструмент с неверными данными:");
            var invalidInstrument = new StringInstrument("", -100, "Дерево", 4,
                new InstrumentSpecs { Weight = 0.5, YearOfManufacture = 2020, Manufacturer = "Test" });
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"   Поймано ArgumentException: {ex.Message}");
        }

        try
        {
            // 2. Исключение при переполнении магазина
            Console.WriteLine("\n2. Попытка переполнить магазин:");
            var smallStore = new MusicStore(10000, 2);
            var instrument1 = new StringInstrument("Скрипка", 5000, "Дерево", 4,
                new InstrumentSpecs { Weight = 0.5, YearOfManufacture = 2020, Manufacturer = "Test1" });
            var instrument2 = new WindInstrument("Труба", 3000, "Латунь", 3,
                new InstrumentSpecs { Weight = 1.2, YearOfManufacture = 2018, Manufacturer = "Test2" });
            var instrument3 = new PercussionInstrument("Барабан", 2000, "Дерево", true,
                new InstrumentSpecs { Weight = 15.0, YearOfManufacture = 2019, Manufacturer = "Test3" });

            smallStore.AddInstrument(instrument1);
            smallStore.AddInstrument(instrument2);
            smallStore.AddInstrument(instrument3); // Должно выбросить StoreCapacityException
        }
        catch (StoreCapacityException ex)
        {
            Console.WriteLine($"   Поймано StoreCapacityException: {ex.Message}");
            Console.WriteLine($"   Текущая вместимость: {ex.CurrentCapacity}, Максимальная: {ex.MaxCapacity}");
        }

        try
        {
            // 3. Исключение при превышении бюджета
            Console.WriteLine("\n3. Попытка превысить бюджет:");
            var poorStore = new MusicStore(5000, 10);
            var expensiveInstrument = new StringInstrument("Дорогая скрипка", 10000, "Дерево", 4,
                new InstrumentSpecs { Weight = 0.5, YearOfManufacture = 2020, Manufacturer = "Luxury" });

            poorStore.AddInstrument(expensiveInstrument); // Должно выбросить BudgetExceededException
        }
        catch (BudgetExceededException ex)
        {
            Console.WriteLine($"   Поймано BudgetExceededException: {ex.Message}");
            Console.WriteLine($"   Текущий бюджет: {ex.CurrentBudget}, Требуется: {ex.RequiredAmount}");
        }

        try
        {
            // 4. Исключение при работе с несуществующим индексом
            Console.WriteLine("\n4. Попытка удалить инструмент по неверному индексу:");
            var store = new MusicStore(10000, 5);
            store.RemoveInstrumentAt(10); // Должно выбросить IndexOutOfRangeException
        }
        catch (IndexOutOfRangeException ex)
        {
            Console.WriteLine($"   Поймано IndexOutOfRangeException: {ex.Message}");
        }

        try
        {
            // 5. Исключение при поиске несуществующего инструмента
            Console.WriteLine("\n5. Попытка найти несуществующий инструмент:");
            var store = new MusicStore(10000, 5);
            store.FindInstrumentByName("Несуществующий инструмент"); // Должно выбросить InstrumentNotFoundException
        }
        catch (InstrumentNotFoundException ex)
        {
            Console.WriteLine($"   Поймано InstrumentNotFoundException: {ex.Message}");
            Console.WriteLine($"   Искомый инструмент: {ex.InstrumentName}");
        }

        try
        {
            // 6. Исключение деления на ноль (моделируется в CalculateMaintenanceCost)
            Console.WriteLine("\n6. Моделирование деления на ноль:");
            var instrument = new StringInstrument("Тест", 1000, "Дерево", 4,
                new InstrumentSpecs { Weight = 1.0, YearOfManufacture = 2020, Manufacturer = "Test" });

            // Многократные вызовы для вероятного деления на ноль
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    double cost = instrument.CalculateMaintenanceCost();
                    Console.WriteLine($"   Стоимость обслуживания: {cost:F2}");
                }
                catch (DivideByZeroException ex)
                {
                    Console.WriteLine($"   Поймано DivideByZeroException: {ex.Message}");
                    // Проброс исключения выше по стеку
                    throw;
                }
            }
        }
        catch (DivideByZeroException)
        {
            Console.WriteLine("   Исключение перехвачено на верхнем уровне после проброса");
        }

        try
        {
            // 7. Исключение при работе с null
            Console.WriteLine("\n7. Работа с null объектом:");
            Printer printer = new Printer();
            printer.IAmPrinting(null); // Должно выбросить ArgumentNullException
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"   Поймано ArgumentNullException: {ex.Message}");
        }
    }

    static void Main(string[] args)
    {
        Console.WriteLine("=== Лабораторная работа 6 - Обработка исключений ===\n");

        try
        {
            // Основная логика программы
            DemonstrateExceptionHandling();

            // Нормальная работа программы
            Console.WriteLine("\n=== Нормальная работа программы ===");

            var musicStore = new MusicStore(50000, 10);
            var controller = new StoreController(musicStore);

            var violin = new StringInstrument("Скрипка", 15000, "Дерево", 4,
                new InstrumentSpecs { Weight = 0.5, YearOfManufacture = 2020, Manufacturer = "Stradivarius Co" });

            var trumpet = new WindInstrument("Труба", 8000, "Латунь", 3,
                new InstrumentSpecs { Weight = 1.2, YearOfManufacture = 2018, Manufacturer = "Yamaha" });

            musicStore.AddInstrument(violin);
            musicStore.AddInstrument(trumpet);
            musicStore.PrintAllInstruments();

            // Демонстрация Assert в работе
            Console.WriteLine("=== Демонстрация Assert ===");
            var testInstrument = new StringInstrument("Тест", 1000, "Дерево", 6,
                new InstrumentSpecs { Weight = 2.0, YearOfManufacture = 2021, Manufacturer = "Test" });
            // инст разр 
            Debug.Assert(testInstrument.Price > 0, "Цена инструмента должна быть положительной");
            Console.WriteLine("Assert проверка пройдена успешно");

        }
        catch (MusicStoreException ex)
        {
            Console.WriteLine($"Поймано пользовательское исключение: {ex.GetType().Name}");
            Console.WriteLine($"Сообщение: {ex.Message}");
            Console.WriteLine($"Стек вызовов: {ex.StackTrace}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Поймано общее исключение: {ex.GetType().Name}");
            Console.WriteLine($"Сообщение: {ex.Message}");
            Console.WriteLine($"Стек вызовов: {ex.StackTrace}");
        }
        finally
        {
            Console.WriteLine("\n=== Блок finally ===");

            Console.WriteLine("\n=== Программа завершена ===");
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}