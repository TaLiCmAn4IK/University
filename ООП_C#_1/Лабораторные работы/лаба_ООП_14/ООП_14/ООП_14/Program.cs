using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Lab14
{
    class Program
    {
        // Для синхронизации вывода в файлы
        static readonly object fileLock = new object();

        // Для 4b.ii – поочерёдный вывод
        static AutoResetEvent oddEvent = new AutoResetEvent(true);   // первым пойдёт нечётный
        static AutoResetEvent evenEvent = new AutoResetEvent(false); // чётный ждёт разрешения

        static Random rnd = new Random();

        static void Main(string[] args)
        {
            Task1();
            Task2();
            Task3();
            Task4();
            Task5();

            Console.WriteLine("\nНажмите Enter для выхода...");
            Console.ReadLine();
        }

        // 1. Информация о процессах
        static void Task1()
        {
            Console.WriteLine("=== Задание 1 ===");

            Process[] processes = Process.GetProcesses();   // получаем список всех процессов ОС
            foreach (Process p in processes)
            {
                try
                {
                    Console.WriteLine($"ID: {p.Id}");
                    Console.WriteLine($"Имя: {p.ProcessName}");
                    Console.WriteLine($"Приоритет: {p.BasePriority}");
                    Console.WriteLine($"Время запуска: {p.StartTime}");
                    Console.WriteLine($"Состояние (отвечает): {p.Responding}");
                    Console.WriteLine($"Время процессора: {p.TotalProcessorTime}");
                    Console.WriteLine($"Память: {p.WorkingSet64 / 1024} КБ"); // объём физ.памяти
                    Console.WriteLine(new string('-', 40));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при получении информации: {ex.Message}");
                }
            }
        }

        // 2. Исследование домена
        static void Task2()
        {
            Console.WriteLine("\n=== Задание 2 ===");

            AppDomain current = AppDomain.CurrentDomain; // получаем текущий домен
            Console.WriteLine($"Текущий домен: {current.FriendlyName}"); 
            Console.WriteLine($"Базовый каталог: {current.BaseDirectory}");
            Console.WriteLine($"ApplicationBase: {current.SetupInformation.ApplicationBase}");

            Console.WriteLine("\nСписок загруженных сборок:");
            foreach (Assembly asm in current.GetAssemblies()) 
            {
                Console.WriteLine("  " + asm.FullName); // возвращает все assemblies
            }
        }

        // 3. Поиск простых чисел в отдельном потоке
        static void Task3()
        {
            Console.WriteLine("\n=== Задание 3 ===");
            Console.Write("Введите n для поиска простых чисел: ");

            if (!int.TryParse(Console.ReadLine(), out int n) || n < 2)
            {
                Console.WriteLine("Некорректное n.");
                return;
            }

            Thread primeThread = new Thread(() => FindPrimes(n));
            primeThread.Name = "PrimeFinder";
            primeThread.Priority = ThreadPriority.Normal;

            Console.WriteLine($"Создан поток: {primeThread.Name}, приоритет: {primeThread.Priority}, ID: {primeThread.ManagedThreadId}");
            Console.WriteLine($"Начальное состояние: {primeThread.ThreadState}");

            primeThread.Start();
            Thread.Sleep(100);
            Console.WriteLine($"Состояние после старта: {primeThread.ThreadState}");

            primeThread.Join();  // ждём завершения
            Console.WriteLine($"Состояние после завершения: {primeThread.ThreadState}");
        }

        static void FindPrimes(int n)
        {
            using (StreamWriter writer = new StreamWriter("primes.txt"))
            {
                for (int i = 2; i <= n; i++)
                {
                    if (IsPrime(i))
                    {
                        Console.WriteLine($"Простое число: {i}");
                        writer.WriteLine(i);
                        Thread.Sleep(50);
                    }
                }
            }
        }

        static bool IsPrime(int x)
        {
            if (x < 2) return false;
            for (int d = 2; d * d <= x; d++)
                if (x % d == 0) return false;
            return true;
        }
        
        // 4. Два потока: чётные и нечётные числа
        static void Task4()
        {
            Console.WriteLine("\n=== Задание 4 ===");
            Console.Write("Введите n: ");

            if (!int.TryParse(Console.ReadLine(), out int n) || n < 1)
            {
                Console.WriteLine("Некорректное n.");
                return;
            }

            // 4a. Два потока с разными приоритетами и разной скоростью
            Thread evenThread = new Thread(() => PrintNumbersToFile("numbers.txt", n, true));
            Thread oddThread = new Thread(() => PrintNumbersToFile("numbers.txt", n, false));

            evenThread.Name = "EvenThread";
            oddThread.Name = "OddThread";

            evenThread.Priority = ThreadPriority.AboveNormal;
            oddThread.Priority = ThreadPriority.Normal;

            // 4b.i. Сначала чётные, потом нечётные
            Console.WriteLine("\n--- 4b.i: сначала чётные, потом нечётные ---");
            File.WriteAllText("numbers.txt", ""); // очистка файла
            evenThread.Start();
            evenThread.Join();
            oddThread.Start();
            oddThread.Join();

            // 4b.ii. Поочерёдный вывод с синхронизацией AutoResetEvent - показывает синхронизацию потоков 
            Console.WriteLine("\n--- 4b.ii: поочерёдный вывод ---");
            File.WriteAllText("numbers_sync.txt", "");

            Thread oddSync = new Thread(() => PrintNumbersSync(n, false));
            Thread evenSync = new Thread(() => PrintNumbersSync(n, true));

            oddSync.Name = "OddSync";
            evenSync.Name = "EvenSync";

            oddSync.Start();
            evenSync.Start();

            oddSync.Join();
            evenSync.Join();
        }

        // Общая печать в файл (4a, 4b.i)
        static void PrintNumbersToFile(string fileName, int n, bool isEven)
        {
            using (StreamWriter writer = new StreamWriter(fileName, true))
            {
                int start = isEven ? 2 : 1;
                for (int i = start; i <= n; i += 2)
                {
                    lock (fileLock)
                    {
                        Console.WriteLine($"{Thread.CurrentThread.Name}: {i}");
                        writer.WriteLine($"{Thread.CurrentThread.Name}: {i}");
                    }

                    Thread.Sleep(isEven ? 100 : 150); // разная скорость потоков
                }
            }
        }

        // Поочерёдная печать в numbers_sync.txt: 1,2,3,4,…
        static void PrintNumbersSync(int n, bool isEven)
        {
            int start = isEven ? 2 : 1;

            for (int i = start; i <= n; i += 2)
            {
                if (isEven)
                {
                    // ждём, пока нечётный поток закончит и даст сигнал evenEvent
                    evenEvent.WaitOne();
                }
                else
                {
                    // ждём стартового или следующего сигнала от чётного потока
                    oddEvent.WaitOne();
                }

                lock (fileLock)
                {
                    using (StreamWriter writer = new StreamWriter("numbers_sync.txt", true))
                    {
                        Console.WriteLine($"{Thread.CurrentThread.Name}: {i}");
                        writer.WriteLine(i);
                    }
                }

                if (isEven)
                {
                    // после чётного снова разрешаем нечётному
                    oddEvent.Set();
                }
                else
                {
                    // после нечётного разрешаем чётному
                    evenEvent.Set();
                }

                Thread.Sleep(50);
            }
        }

        // 5. Таймер
        static void Task5()
        {
            Console.WriteLine("\n=== Задание 5 ===");
            Console.WriteLine("Таймер запущен. Каждые 2 секунды выводится случайное число.");
            Console.WriteLine("Нажмите Enter для остановки...");

            TimerCallback callback = new TimerCallback(PrintRandom);
            using (Timer timer = new Timer(callback, null, 0, 2000))
            {
                Console.ReadLine();   // ждём, пока пользователь не нажмёт Enter
            }

            Console.WriteLine("Таймер остановлен.");
        }

        static void PrintRandom(object state)
        {
            // статический Random, чтобы числа не повторялись при частом вызове
            Console.WriteLine($"Случайное число: {rnd.Next(1, 100)}");
        }
    }
}
