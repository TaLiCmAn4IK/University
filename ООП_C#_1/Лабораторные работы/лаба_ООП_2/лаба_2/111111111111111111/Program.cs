using System;

// Первая часть partial класса Bus
public partial class Bus
{
    // === СТАТИЧЕСКОЕ ПОЛЕ (количество созданных объектов) ===
    private static int _busCount;

    // === ПОЛЕ-КОНСТРАНТА ===
    private const string VEHICLE_TYPE = "Автобус";

    // === ЗАКРЫТЫЕ ПОЛЯ КЛАССА ===
    private string _driverName;
    private int _busNumber;
    private int _routeNumber;
    private string _brand;
    private int _startYear;
    private double _mileage;

    // === ПОЛЕ ТОЛЬКО ДЛЯ ЧТЕНИЯ (ID на основе хэша) ===
    private readonly int _id;

    // === СТАТИЧЕСКИЙ КОНСТРУКТОР ===
    static Bus()
    {
        Console.WriteLine("Вызван статический конструктор класса Bus");
        _busCount = 0;
    }

    // === ЗАКРЫТЫЙ КОНСТРУКТОР ===
    private Bus(int id)
    {
        _id = id;
        _busCount++;
    }

    // === КОНСТРУКТОР БЕЗ ПАРАМЕТРОВ ===
    public Bus() : this(CalculateHash("default" + DateTime.Now.Ticks))
    {
        _driverName = "Неизвестно";
        _busNumber = 0;
        _routeNumber = 0;
        _brand = "Неизвестно";
        _startYear = DateTime.Now.Year;
        _mileage = 0.0;
    }

    // === КОНСТРУКТОР С ПАРАМЕТРАМИ ПО УМОЛЧАНИЮ ===
    public Bus(string driverName, int busNumber = 1, int routeNumber = 1)
        : this(CalculateHash(driverName + busNumber + routeNumber + DateTime.Now.Ticks))
    {
        DriverName = driverName;
        BusNumber = busNumber;
        RouteNumber = routeNumber;
        _brand = "Неизвестно";
        _startYear = DateTime.Now.Year;
        _mileage = 0.0;
    }

    // === КОНСТРУКТОР СО ВСЕМИ ПАРАМЕТРАМИ ===
    public Bus(string driverName, int busNumber, int routeNumber,
               string brand, int startYear, double mileage)
        : this(CalculateHash(driverName + busNumber + routeNumber + brand + startYear + mileage + DateTime.Now.Ticks))
    {
        DriverName = driverName;
        BusNumber = busNumber;
        RouteNumber = routeNumber;
        Brand = brand;
        StartYear = startYear;
        Mileage = mileage;
    }

    // === СВОЙСТВА (GET, SET) ДЛЯ ВСЕХ ПОЛЕЙ КЛАССА ===

    public int Id
    {
        get { return _id; }
    }

    public string DriverName
    {
        get { return _driverName; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("ФИО водителя не может быть пустым");
            _driverName = value;
        }
    }

    public int BusNumber
    {
        get { return _busNumber; }
        set
        {
            if (value <= 0)
                throw new ArgumentException("Номер автобуса должен быть положительным");
            _busNumber = value;
        }
    }

    public int RouteNumber
    {
        get { return _routeNumber; }
        set
        {
            if (value <= 0)
                throw new ArgumentException("Номер маршрута должен быть положительным");
            _routeNumber = value;
        }
    }

    public string Brand
    {
        get { return _brand; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Марка не может быть пустой");
            _brand = value;
        }
    }

    public int StartYear
    {
        get { return _startYear; }
        set
        {
            int currentYear = DateTime.Now.Year;
            if (value < 1950 || value > currentYear)
                throw new ArgumentException($"Год начала эксплуатации должен быть между 1950 и {currentYear}");
            _startYear = value;
        }
    }

    // Свойство с ОГРАНИЧЕННЫМ ДОСТУПОМ по SET
    public double Mileage
    {
        get { return _mileage; }
        private set
        {
            if (value < 0)
                throw new ArgumentException("Пробег не может быть отрицательным");
            _mileage = value;
        }
    }

    // === МЕТОД С REF И OUT ПАРАМЕТРАМИ ===
    public bool UpdateMileageWithValidation(ref double currentMileage, double additionalMileage, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (currentMileage < 0)
        {
            errorMessage = "Текущий пробег не может быть отрицательным";
            return false;
        }

        if (additionalMileage < 0)
        {
            errorMessage = "Добавляемый пробег не может быть отрицательным";
            return false;
        }

        currentMileage += additionalMileage;
        Mileage = currentMileage;

        return true;
    }

    // === СТАТИЧЕСКИЙ МЕТОД вывода информации о классе ===
    public static void PrintClassInfo()
    {
        Console.WriteLine("=== ИНФОРМАЦИЯ О КЛАССЕ BUS ===");
        Console.WriteLine($"Тип транспортного средства: {VEHICLE_TYPE}");
        Console.WriteLine($"Количество созданных объектов: {_busCount}");
        Console.WriteLine($"Дата вывода информации: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
    }

    // === МЕТОД ДЛЯ ВЫЧИСЛЕНИЯ ВОЗРАСТА АВТОБУСА ===
    public int GetBusAge()
    {
        return DateTime.Now.Year - _startYear;
    }

    // === ПРИВАТНЫЙ МЕТОД ДЛЯ ВЫЧИСЛЕНИЯ ХЭША ===
    private static int CalculateHash(string input)
    {
        unchecked
        {
            int hash = 17;
            foreach (char c in input)
            {
                hash = hash * 31 + c;
            }
            return Math.Abs(hash);
        }
    }

    // === ВАРИАНТЫ ВЫЗОВА ЗАКРЫТОГО КОНСТРУКТОРА ===
    public static Bus CreateBusWithCustomId(int customId)
    {
        return new Bus(customId);
    }

    // === ПУБЛИЧНЫЙ МЕТОД ДЛЯ ИЗМЕНЕНИЯ ПРОБЕГА ===
    public void AddMileage(double km)
    {
        if (km < 0)
            throw new ArgumentException("Пробег не может быть отрицательным");

        Mileage += km;
    }

    // Метод с OUT параметрами
    public void GetBusStatistics(out int age, out double mileagePerYear, out bool needsMaintenance)
    {
        age = GetBusAge();
        mileagePerYear = age > 0 ? _mileage / age : _mileage;
        needsMaintenance = (_mileage > 100000) || (age > 10);
    }
}

// Вторая часть partial класса Bus
public partial class Bus
{
    // === ПЕРЕОПРЕДЕЛЕНИЕ МЕТОДОВ КЛАССА OBJECT ===

    public override bool Equals(object obj)
    {
        if (obj == null || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }

        Bus other = (Bus)obj;

        return _driverName == other._driverName &&
               _busNumber == other._busNumber &&
               _routeNumber == other._routeNumber &&
               _brand == other._brand &&
               _startYear == other._startYear &&
               Math.Abs(_mileage - other._mileage) < 0.001;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (_driverName?.GetHashCode() ?? 0);
            hash = hash * 23 + _busNumber.GetHashCode();
            hash = hash * 23 + _routeNumber.GetHashCode();
            hash = hash * 23 + (_brand?.GetHashCode() ?? 0);
            hash = hash * 23 + _startYear.GetHashCode();
            hash = hash * 23 + _mileage.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        return $"Bus [ID: {_id}, Марка: {_brand}, №{_busNumber}, Маршрут: {_routeNumber}, " +
               $"Водитель: {_driverName}, Год: {_startYear}, Пробег: {_mileage} км, " +
               $"Возраст: {GetBusAge()} лет]";
    }

    // Дополнительные методы для partial класса
    public bool EqualsByNumberAndRoute(Bus other)
    {
        if (other == null) return false;
        return _busNumber == other._busNumber && _routeNumber == other._routeNumber;
    }

    public string GetShortInfo()
    {
        return $"{_brand} №{_busNumber} (маршрут {_routeNumber}) - {_driverName}";
    }

    // Статические операторы для сравнения
    public static bool operator ==(Bus left, Bus right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null || right is null)
            return false;
        return left.Equals(right);
    }

    public static bool operator !=(Bus left, Bus right)
    {
        return !(left == right);
    }
}

// Класс Program с методом Main
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("ЛАБОРАТОРНАЯ РАБОТА - КЛАСС BUS");
        Console.WriteLine("================================\n");

        // ЗАДАНИЕ 2: Создание нескольких объектов и работа с ними
        Console.WriteLine("ЗАДАНИЕ 2: СОЗДАНИЕ ОБЪЕКТОВ И РАБОТА С НИМИ");
        Console.WriteLine("============================================\n");

        // 2.1 Создание объектов разными конструкторами
        Console.WriteLine("2.1 СОЗДАНИЕ ОБЪЕКТОВ РАЗНЫМИ КОНСТРУКТОРАМИ:");

        // Конструктор без параметров
        Bus bus1 = new Bus();
        Console.WriteLine("bus1 создан конструктором без параметров");
        bus1.DriverName = "Иванов";
        // Конструктор с параметрами по умолчанию
        Bus bus2 = new Bus("Иванов И.И.", 101);
        Console.WriteLine("bus2 создан конструктором с параметрами по умолчанию");

        // Конструктор со всеми параметрами
        Bus bus3 = new Bus("Петров П.П.", 205, 22, "Mercedes", 2018, 125000.5);
        Console.WriteLine("bus3 создан конструктором со всеми параметрами");

        Bus bus4 = new Bus("Сидоров С.С.", 308, 45, "Volvo", 2021, 30000.0);
        Bus bus5 = new Bus("Козлов К.К.", 412, 15, "MAN", 2015, 200000.0);

        Console.WriteLine("\n" + new string('-', 50));

        // 2.2 Вызов свойств
        Console.WriteLine("2.2 ВЫЗОВ СВОЙСТВ:");

        bus1.DriverName = "Алексеев А.А.";
        bus1.BusNumber = 105;
        bus1.RouteNumber = 10;
        bus1.Brand = "ЛиАЗ";
        bus1.StartYear = 2020;
        bus1.AddMileage(50000);

        Console.WriteLine($"bus1 - Водитель: {bus1.DriverName}, Номер: {bus1.BusNumber}");
        Console.WriteLine($"bus2 - Марка: {bus2.Brand}, Маршрут: {bus2.RouteNumber}");
        Console.WriteLine($"bus3 - Год: {bus3.StartYear}, Пробег: {bus3.Mileage}");

        Console.WriteLine("\n" + new string('-', 50));

        // 2.3 Вызов методов
        Console.WriteLine("2.3 ВЫЗОВ МЕТОДОВ:");

        Console.WriteLine($"Возраст bus3: {bus3.GetBusAge()} лет");
        Console.WriteLine($"Возраст bus5: {bus5.GetBusAge()} лет");

        // Метод с ref и out параметрами
        double currentMileage = bus1.Mileage;
        bool success = bus1.UpdateMileageWithValidation(ref currentMileage, 1500.75, out string errorMessage);
        Console.WriteLine($"Обновление пробега bus1: {success}, новый пробег: {bus1.Mileage} км");

        bus3.GetBusStatistics(out int age, out double avgMileage, out bool maintenance);
        Console.WriteLine($"Статистика bus3: возраст {age} лет, средний пробег {avgMileage:F2} км/год");

        Console.WriteLine("\n" + new string('-', 50));

        // 2.4 Сравнение объектов
        Console.WriteLine("2.4 СРАВНЕНИЕ ОБЪЕКТОВ:");

        Bus bus6 = new Bus("Иванов И.И.", 101, 15, "ЛиАЗ", 2020, 51500.75);
        Console.WriteLine($"bus1.Equals(bus6): {bus1.Equals(bus6)}");
        Console.WriteLine($"bus1 == bus6: {bus1 == bus6}");
        Console.WriteLine($"bus1 != bus2: {bus1 != bus2}");
        Console.WriteLine($"bus1.EqualsByNumberAndRoute(bus6): {bus1.EqualsByNumberAndRoute(bus6)}");

        Console.WriteLine("\n" + new string('-', 50));

        // 2.5 Проверка типа созданного объекта
        Console.WriteLine("2.5 ПРОВЕРКА ТИПА СОЗДАННОГО ОБЪЕКТА:");

        Console.WriteLine($"Тип bus1: {bus1.GetType()}");
        Console.WriteLine($"Тип bus2: {bus2.GetType()}");
        Console.WriteLine($"bus1 является Bus: {bus1 is Bus}");
        Console.WriteLine($"bus2 является object: {bus2 is object}");

        Console.WriteLine("\n" + new string('-', 50));

        // 2.6 Работа с закрытым конструктором
        Console.WriteLine("2.6 ВЫЗОВ ЗАКРЫТОГО КОНСТРУКТОРА:");

        Bus customBus = Bus.CreateBusWithCustomId(999999);
        customBus.DriverName = "Специальный водитель";
        customBus.BusNumber = 999;
        customBus.RouteNumber = 99;
        customBus.Brand = "Special";
        customBus.StartYear = 2023;
        customBus.AddMileage(1000);
        Console.WriteLine($"Автобус создан через закрытый конструктор: {customBus}");

        Console.WriteLine("\n" + new string('-', 50));

        // 2.7 Переопределенные методы Object
        Console.WriteLine("2.7 ПЕРЕОПРЕДЕЛЕННЫЕ МЕТОДЫ OBJECT:");

        Console.WriteLine($"bus1.ToString(): {bus1}");
        Console.WriteLine($"bus1.GetHashCode(): {bus1.GetHashCode()}");
        Console.WriteLine($"bus6.GetHashCode(): {bus6.GetHashCode()}");
        Console.WriteLine($"Хэш-коды равны: {bus1.GetHashCode() == bus6.GetHashCode()}");

        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("ЗАДАНИЕ 3: МАССИВ ОБЪЕКТОВ И ВЫПОЛНЕНИЕ ЗАДАНИЯ ИЗ ВАРИАНТА");
        Console.WriteLine("==========================================================\n");

        // 3.1 Создание массива объектов
        Bus[] buses = { bus1, bus2, bus3, bus4, bus5, bus6, customBus };

        Console.WriteLine("Создан массив из 7 автобусов:");
        for (int i = 0; i < buses.Length; i++)
        {
            Console.WriteLine($"  [{i}] {buses[i].GetShortInfo()}");
        }

        Console.WriteLine("\n" + new string('-', 50));

        // 3.2 Выполнение задания из варианта (курсивом в таблице)
        Console.WriteLine("ЗАДАНИЕ ИЗ ВАРИАНТА:");
        Console.WriteLine("a) список автобусов для заданного номера маршрута");
        Console.WriteLine("b) список автобусов, которые эксплуатируются больше заданного срока\n");

        // a) Список автобусов для заданного номера маршрута
        int targetRoute = 15;
        Console.WriteLine($"a) Автобусы маршрута №{targetRoute}:");
        bool foundAny = false;
        foreach (var bus in buses)
        {
            if (bus.RouteNumber == targetRoute)
            {
                Console.WriteLine($"   - {bus.GetShortInfo()}, пробег: {bus.Mileage} км");
                foundAny = true;
            }
        }
        if (!foundAny)
        {
            Console.WriteLine("   Автобусы данного маршрута не найдены");
        }

        Console.WriteLine();

        // b) Список автобусов, которые эксплуатируются больше заданного срока
        int minAge = 5;
        Console.WriteLine($"b) Автобусы старше {minAge} лет:");
        foundAny = false;
        foreach (var bus in buses)
        {
            int busAge = bus.GetBusAge();
            if (busAge > minAge)
            {
                Console.WriteLine($"   - {bus.GetShortInfo()}, возраст: {busAge} лет, пробег: {bus.Mileage} км");
                foundAny = true;
            }
        }
        if (!foundAny)
        {
            Console.WriteLine($"   Автобусы старше {minAge} лет не найдены");
        }

        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("ЗАДАНИЕ 4: СОЗДАНИЕ И ВЫВОД АНОНИМНОГО ТИПА");
        Console.WriteLine("===========================================\n");

        // 4.1 Создание анонимного типа по образцу класса Bus
        var anonymousBus1 = new
        {
            Driver = "Анонимный В.В.",
            BusNumber = 777,
            RouteNumber = 88,
            Brand = "Anonymous",
            StartYear = 2022,
            Mileage = 25000.5
        };

        var anonymousBus2 = new
        {
            Driver = "Безымянный Б.Б.",
            BusNumber = 888,
            RouteNumber = 77,
            Brand = "NoName",
            StartYear = 2023,
            Mileage = 15000.0
        };

        // 4.2 Вывод анонимных типов
        Console.WriteLine("Созданы анонимные объекты по образцу класса Bus:");
        Console.WriteLine($"Анонимный автобус 1: {anonymousBus1}");
        Console.WriteLine($"Анонимный автобус 2: {anonymousBus2}");

        Console.WriteLine("\nДетальная информация:");
        Console.WriteLine($"Автобус 1 - Водитель: {anonymousBus1.Driver}, " +
                         $"Номер: {anonymousBus1.BusNumber}, Маршрут: {anonymousBus1.RouteNumber}, " +
                         $"Марка: {anonymousBus1.Brand}, Год: {anonymousBus1.StartYear}, " +
                         $"Пробег: {anonymousBus1.Mileage} км");

        Console.WriteLine($"Автобус 2 - Водитель: {anonymousBus2.Driver}, " +
                         $"Номер: {anonymousBus2.BusNumber}, Маршрут: {anonymousBus2.RouteNumber}, " +
                         $"Марка: {anonymousBus2.Brand}, Год: {anonymousBus2.StartYear}, " +
                         $"Пробег: {anonymousBus2.Mileage} км");

        // 4.3 Демонстрация свойств анонимных типов
        Console.WriteLine("\nСвойства анонимных типов:");
        Console.WriteLine($"Тип anonymousBus1: {anonymousBus1.GetType()}");
        Console.WriteLine($"Тип anonymousBus2: {anonymousBus2.GetType()}");
        Console.WriteLine($"Типы одинаковы: {anonymousBus1.GetType() == anonymousBus2.GetType()}");

        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("ФИНАЛЬНАЯ ИНФОРМАЦИЯ");
        Console.WriteLine("====================\n");

        // Финальная статистика
        Bus.PrintClassInfo();

        Console.WriteLine($"\nВсего создано объектов: {buses.Length}");
        Console.WriteLine("Программа успешно завершена!");

        Console.ReadLine();
    }
}