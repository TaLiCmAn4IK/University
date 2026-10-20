using System;

namespace BusLab
{
    public partial class Bus
    {
        // Статическое поле для подсчета созданных объектов
        private static int _busCount = 0;

        // Поле только для чтения (ID на основе хэша)
        private readonly int _id;

        // Поле-константа
        private const string VEHICLE_TYPE = "Автобус";

        // Закрытые поля
        private string _driverName;
        private int _busNumber;
        private int _routeNumber;
        private string _brand;
        private int _startYear;
        private double _mileage;

        // Статический конструктор
        static Bus()
        {
            Console.WriteLine("Статический конструктор класса Bus вызван");
            _busCount = 0;
        }

        // Закрытый конструктор
        private Bus(int id)
        {
            _id = id;
            _busCount++;
        }

        // Конструктор без параметров
        public Bus() : this(CalculateHash("default"))
        {
            _driverName = "Неизвестно";
            _busNumber = 0;
            _routeNumber = 0;
            _brand = "Неизвестно";
            _startYear = DateTime.Now.Year;
            _mileage = 0;
        }

        // Конструктор с параметрами по умолчанию
        public Bus(string driverName, int busNumber = 1, int routeNumber = 1)
            : this(CalculateHash(driverName + busNumber + routeNumber))
        {
            DriverName = driverName;
            BusNumber = busNumber;
            RouteNumber = routeNumber;
            _brand = "Неизвестно";
            _startYear = DateTime.Now.Year;
            _mileage = 0;
        }

        // Конструктор со всеми параметрами
        public Bus(string driverName, int busNumber, int routeNumber,
                  string brand, int startYear, double mileage)
            : this(CalculateHash(driverName + busNumber + routeNumber + brand + startYear + mileage))
        {
            DriverName = driverName;
            BusNumber = busNumber;
            RouteNumber = routeNumber;
            Brand = brand;
            StartYear = startYear;
            Mileage = mileage;
        }

        // Свойства с get и set
        public int Id => _id; // Только get (readonly)

        public string DriverName
        {
            get => _driverName;
            set => _driverName = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentException("ФИО водителя не может быть пустым");
        }

        public int BusNumber
        {
            get => _busNumber;
            set => _busNumber = value > 0 ? value : throw new ArgumentException("Номер автобуса должен быть положительным");
        }

        public int RouteNumber
        {
            get => _routeNumber;
            set => _routeNumber = value > 0 ? value : throw new ArgumentException("Номер маршрута должен быть положительным");
        }

        public string Brand
        {
            get => _brand;
            set => _brand = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentException("Марка не может быть пустой");
        }

        public int StartYear
        {
            get => _startYear;
            set
            {
                int currentYear = DateTime.Now.Year;
                if (value >= 1950 && value <= currentYear)
                    _startYear = value;
                else
                    throw new ArgumentException($"Год начала эксплуатации должен быть между 1950 и {currentYear}");
            }
        }

        public double Mileage
        {
            get => _mileage;
            private set // Ограниченный доступ по set
            {
                if (value >= 0)
                    _mileage = value;
                else
                    throw new ArgumentException("Пробег не может быть отрицательным");
            }
        }

        // Метод с ref и out параметрами
        public void UpdateMileage(ref double currentMileage, double additionalMileage, out double newMileage)
        {
            if (additionalMileage < 0)
                throw new ArgumentException("Добавляемый пробег не может быть отрицательным");

            currentMileage += additionalMileage;
            _mileage = currentMileage;
            newMileage = _mileage;
        }

        // Метод для вывода возраста автобуса
        public int GetBusAge()
        {
            return DateTime.Now.Year - _startYear;
        }

        // Статический метод вывода информации о классе
        public static void PrintClassInfo()
        {
            Console.WriteLine($"=== Информация о классе Bus ===");
            Console.WriteLine($"Тип транспортного средства: {VEHICLE_TYPE}");
            Console.WriteLine($"Количество созданных автобусов: {_busCount}");
            Console.WriteLine($"Дата вывода информации: {DateTime.Now}");
        }

        // Переопределение методов класса Object
        public override bool Equals(object obj)
        {
            if (obj is Bus other)
            {
                return _id == other._id &&
                       _driverName == other._driverName &&
                       _busNumber == other._busNumber &&
                       _routeNumber == other._routeNumber &&
                       _brand == other._brand &&
                       _startYear == other._startYear &&
                       Math.Abs(_mileage - other._mileage) < 0.001;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _id;
        }

        public override string ToString()
        {
            return $"Автобус #{_id}: {_brand}, маршрут {_routeNumber}, водитель {_driverName}, " +
                   $"год начала {_startYear}, пробег {_mileage} км, возраст {GetBusAge()} лет";
        }

        // Приватный метод для вычисления хэша
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

        // Статический метод для вызова закрытого конструктора
        public static Bus CreateBusWithCustomId(int customId)
        {
            return new Bus(customId);
        }
    }

    // Вторая часть partial класса
    public partial class Bus
    {
        // Дополнительные методы можно добавить здесь
        public string GetShortInfo()
        {
            return $"{_brand} №{_busNumber} (маршрут {_routeNumber})";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ЛАБОРАТОРНАЯ РАБОТА - КЛАСС BUS ===\n");

            // 2) Создание нескольких объектов
            Console.WriteLine("1. СОЗДАНИЕ ОБЪЕКТОВ:");

            // Конструктор без параметров
            Bus bus1 = new Bus();
            Console.WriteLine($"bus1: {bus1}");

            // Конструктор с параметрами по умолчанию
            Bus bus2 = new Bus("Иванов И.И.", 101);
            Console.WriteLine($"bus2: {bus2}");

            // Конструктор со всеми параметрами
            Bus bus3 = new Bus("Петров П.П.", 205, 15, "Mercedes", 2018, 125000.5);
            Console.WriteLine($"bus3: {bus3}");

            Bus bus4 = new Bus("Сидоров С.С.", 308, 22, "Volvo", 2020, 80000.0);
            Bus bus5 = new Bus("Козлов К.К.", 412, 15, "MAN", 2015, 200000.0);

            // Вызов закрытого конструктора через статический метод
            Bus customBus = Bus.CreateBusWithCustomId(999999);
            customBus.DriverName = "Специальный водитель";
            customBus.BusNumber = 999;
            customBus.RouteNumber = 99;
            Console.WriteLine($"customBus: {customBus}");

            Console.WriteLine();

            // 2) Вызов свойств и методов
            Console.WriteLine("2. РАБОТА СО СВОЙСТВАМИ И МЕТОДАМИ:");

            // Тестирование свойств
            bus2.Brand = "Scania";
            Console.WriteLine($"bus2 после изменения марки: {bus2.Brand}");

            // Тестирование метода с ref и out параметрами
            double currentMileage = bus3.Mileage;
            bus3.UpdateMileage(ref currentMileage, 1500.5, out double newMileage);
            Console.WriteLine($"bus3 - старый пробег: {bus3.Mileage - 1500.5}, новый пробег: {newMileage}");

            // Вызов метода возраста
            Console.WriteLine($"Возраст bus4: {bus4.GetBusAge()} лет");
            Console.WriteLine($"Возраст bus5: {bus5.GetBusAge()} лет");

            Console.WriteLine();

            // 2) Сравнение объектов и проверка типа
            Console.WriteLine("3. СРАВНЕНИЕ ОБЪЕКТОВ И ПРОВЕРКА ТИПА:");

            Bus bus6 = new Bus("Петров П.П.", 205, 15, "Mercedes", 2018, 126501.0);
            Console.WriteLine($"bus3.Equals(bus6): {bus3.Equals(bus6)}");
            Console.WriteLine($"bus3.GetHashCode(): {bus3.GetHashCode()}");
            Console.WriteLine($"bus6.GetHashCode(): {bus6.GetHashCode()}");
            Console.WriteLine($"Тип bus3: {bus3.GetType()}");

            Console.WriteLine();

            // 3) Создание массива объектов и выполнение заданий
            Console.WriteLine("4. МАССИВ ОБЪЕКТОВ И ВЫПОЛНЕНИЕ ЗАДАНИЙ:");

            Bus[] buses = { bus1, bus2, bus3, bus4, bus5, bus6 };

            // a) Список автобусов для заданного номера маршрута
            int targetRoute = 15;
            Console.WriteLine($"\na) Автобусы маршрута №{targetRoute}:");
            foreach (var bus in buses)
            {
                if (bus.RouteNumber == targetRoute)
                {
                    Console.WriteLine($"   - {bus}");
                }
            }

            // b) Список автобусов, которые эксплуатируются больше заданного срока
            int maxAge = 5;
            Console.WriteLine($"\nb) Автобусы старше {maxAge} лет:");
            foreach (var bus in buses)
            {
                if (bus.GetBusAge() > maxAge)
                {
                    Console.WriteLine($"   - {bus.GetShortInfo()}, возраст: {bus.GetBusAge()} лет");
                }
            }

            Console.WriteLine();

            // 4) Создание и вывод анонимного типа
            Console.WriteLine("5. АНОНИМНЫЙ ТИП:");

            var anonymousBus = new
            {
                Driver = "Анонимный В.В.",
                BusNum = 777,
                Route = 99,
                Brand = "Anonymous",
                StartYear = 2022,
                Mileage = 10000.0
            };

            Console.WriteLine($"Анонимный объект: {anonymousBus}");
            Console.WriteLine($"Водитель: {anonymousBus.Driver}, Маршрут: {anonymousBus.Route}");

            Console.WriteLine();

            // 5) Статический метод вывода информации о классе
            Console.WriteLine("6. СТАТИЧЕСКАЯ ИНФОРМАЦИЯ О КЛАССЕ:");
            Bus.PrintClassInfo();
        }
    }
}





