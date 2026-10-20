using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab10_LinqVariant12
{
    class Program
    {
        static void Main()
        {
            string[] months =
            {
                "January","February","March","April","May","June",
                "July","August","September","October","November","December"
            };

            int n = 5; 

            // длина строки = n
            var monthsLengthN = months.Where(m => m.Length == n);

            var winterSummerMonths = months.Where(m =>
                   m == "December" || m == "January" || m == "February"
                || m == "June" || m == "July" || m == "August");

            // в алфавитном порядке
            var monthsOrdered = months.OrderBy(m => m);

            // месяцы, содержащие букву 'u' и длиной не менее 4
            var monthsWithU = months
                .Where(m => m.Contains('u') && m.Length >= 4);

            Console.WriteLine("Months length = n:");
            foreach (var m in monthsLengthN) Console.WriteLine(m);

            Console.WriteLine("\nWinter & Summer months:");
            foreach (var m in winterSummerMonths) Console.WriteLine(m);

            Console.WriteLine("\nMonths ordered:");
            foreach (var m in monthsOrdered) Console.WriteLine(m);

            Console.WriteLine("\nMonths with 'u' and length >= 4:");
            foreach (var m in monthsWithU) Console.WriteLine(m);

          
            // 2. Коллекция автобусов
            List<Bus> buses = new List<Bus>
            {
                new Bus { Number = 1,   RouteNumber = 10, Mileage = 150_000, YearsInService = 3 },
                new Bus { Number = 2,   RouteNumber = 10, Mileage = 220_000, YearsInService = 7 },
                new Bus { Number = 3,   RouteNumber = 15, Mileage =  90_000, YearsInService = 1 },
                new Bus { Number = 4,   RouteNumber = 15, Mileage = 310_000, YearsInService = 10 },
                new Bus { Number = 5,   RouteNumber = 20, Mileage = 180_000, YearsInService = 5 },
                new Bus { Number = 6,   RouteNumber = 20, Mileage =  60_000, YearsInService = 2 },
                new Bus { Number = 7,   RouteNumber = 25, Mileage = 400_000, YearsInService = 12 },
                new Bus { Number = 8,   RouteNumber = 25, Mileage = 270_000, YearsInService = 8 },
                new Bus { Number = 9,   RouteNumber = 30, Mileage = 130_000, YearsInService = 4 },
                new Bus { Number = 10,  RouteNumber = 30, Mileage =  50_000, YearsInService = 1 }
            };

            int routeFilter = 10;            
            int minYears = 5;              

 
            var busesForRoute = from b in buses // все автобусы
                                where b.RouteNumber == routeFilter //только авт нуж марш
                                select b;

            // список автобусов, которые эксплуатируются больше заданного срока
            var busesWithBigLife = buses
                .Where(b => b.YearsInService > minYears);

            // минимальный по пробегу автобус
            var minMileageBus = buses
                .OrderBy(b => b.Mileage)
                .FirstOrDefault();

            // последние два автобуса максимальные по пробегу
            var topTwoMileageBuses = buses
                .OrderByDescending(b => b.Mileage)
                .Take(2);

            // упорядоченный список автобусов по номеру
            var busesOrderedByNumber = buses
                .OrderBy(b => b.Number);

            Console.WriteLine("\nBuses for route " + routeFilter + ":");
            foreach (var b in busesForRoute) Console.WriteLine(b);

            Console.WriteLine("\nBuses with life > " + minYears + " years:");
            foreach (var b in busesWithBigLife) Console.WriteLine(b);

            Console.WriteLine("\nBus with minimal mileage:");
            Console.WriteLine(minMileageBus);

            Console.WriteLine("\nTop 2 buses by mileage:");
            foreach (var b in topTwoMileageBuses) Console.WriteLine(b);

            Console.WriteLine("\nBuses ordered by number:");
            foreach (var b in busesOrderedByNumber) Console.WriteLine(b);

            // Пример: все автобусы маршрутов 10 и 20, старше 3 лет,
            // упорядочить по пробегу, сгруппировать по номеру маршрута
            // и посчитать средний пробег по каждой группе.

            var query =
                from b in buses
                where (b.RouteNumber == 10 || b.RouteNumber == 20)   
                      && b.YearsInService > 3
                orderby b.Mileage                                   
                group b by b.RouteNumber into g                      
                select new                                        
                {
                    Route = g.Key,
                    AverageMileage = g.Average(b => b.Mileage),   
                    Count = g.Count()
                };

            Console.WriteLine("\nCustom query (routes 10 & 20, >3 years):");
            foreach (var item in query)
            {
                Console.WriteLine($"Route {item.Route}: avg mileage = {item.AverageMileage}, count = {item.Count}");
            }

            var drivers = new List<Driver>
            {
                new Driver { Name = "Ivanov",  BusNumber = 1 },
                new Driver { Name = "Petrov",  BusNumber = 2 },
                new Driver { Name = "Sidorov", BusNumber = 3 },
                new Driver { Name = "Kuznetsov", BusNumber = 5 },
                new Driver { Name = "Smirnov",  BusNumber = 7 },
            };

            var busesWithDrivers =
                from b in buses // бер авт
                join d in drivers on b.Number equals d.BusNumber //для каж авт находим вод у которого BusNumber совпадает с number
                select new
                {
                    BusNo = b.Number,
                    b.RouteNumber,
                    b.Mileage,
                    d.Name
                };

            Console.WriteLine("\nBuses with drivers (join):");
            foreach (var x in busesWithDrivers)
            {
                Console.WriteLine($"Bus {x.BusNo}, route {x.RouteNumber}, mileage {x.Mileage}, driver {x.Name}");
            }
        }
    }

    public class Bus
    {
        public int Number { get; set; }          // Номер автобуса
        public int RouteNumber { get; set; }     // Номер маршрута
        public int Mileage { get; set; }         // Пробег, км
        public int YearsInService { get; set; }  // Срок эксплуатации, лет

        public override string ToString()
        {
            return $"Bus #{Number}, route {RouteNumber}, mileage {Mileage}, years {YearsInService}";
        }
    }

    public class Driver
    {
        public string Name { get; set; }
        public int BusNumber { get; set; }
    }
}
