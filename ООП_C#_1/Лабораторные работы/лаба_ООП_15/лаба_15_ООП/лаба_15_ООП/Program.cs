using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LabParallel
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== 1. Долгая задача (поиск простых чисел) ===");
            LongTaskWithStatus();

            Console.WriteLine();
            Console.WriteLine("=== 2. Та же задача с CancellationToken ===");
            LongTaskWithCancellation();

            Console.WriteLine();
            Console.WriteLine("=== 3. Три задачи с результатом + четвертая ===");
            TasksWithResults();

            Console.WriteLine();
            Console.WriteLine("=== 4. Продолжения задач ===");
            Continuations();

            Console.WriteLine();
            Console.WriteLine("=== 5. Parallel.For и Parallel.ForEach ===");
            ParallelLoops();

            Console.WriteLine();
            Console.WriteLine("=== 6. Parallel.Invoke ===");
            ParallelInvokeDemo();

            Console.WriteLine();
            Console.WriteLine("=== 7. BlockingCollection: 5 поставщиков и 10 покупателей ===");
            BlockingCollectionDemo();

            Console.WriteLine();
            Console.WriteLine("=== 8. async/await пример ===");
            await AsyncAwaitDemo();

            Console.WriteLine("Готово, нажмите Enter.");
            Console.ReadLine();
        }

        // ---------- 1. Долгая задача на основе Task + Stopwatch ----------

        static void LongTaskWithStatus()
        {
            var sw = Stopwatch.StartNew(); // измеряем время

            // Ищем простые числа простым решетом
            Task<long> task = Task.Run(() => CountPrimes(2, 2_000_000));

            Console.WriteLine($"ID задачи: {task.Id}");

            while (!task.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(200); // имитация проверки статуса
            }

            sw.Stop();
            Console.WriteLine();
            Console.WriteLine($"Статус: {task.Status}, найдено простых чисел: {task.Result}, время: {sw.ElapsedMilliseconds} мс");
        }

        static long CountPrimes(int from, int to)
        {
            var size = to + 1;
            bool[] composite = new bool[size];
            long count = 0;

            for (int i = 2; i * i <= to; i++)
            {
                if (!composite[i])
                {
                    for (int j = i * i; j <= to; j += i)
                        composite[j] = true;
                }
            }

            for (int i = from; i <= to; i++)
                if (!composite[i]) count++;

            return count;
        }

        // ---------- 2. Та же задача, но с отменой ----------

        static void LongTaskWithCancellation()
        {
            var cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            Task<long> task = Task.Run(() => CountPrimesCancelable(2, 10_000_000, token), token);

            // Отменяем через 1 секунду
            Task.Delay(1000).ContinueWith(_ => cts.Cancel());

            try
            {
                Console.WriteLine($"Запущена задача {task.Id}, ждём результат/отмену...");
                long result = task.Result;
                Console.WriteLine($"Задача успела завершиться, результат: {result}");
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                    Console.WriteLine($"Исключение: {e.Message}");
            }
        }

        static long CountPrimesCancelable(int from, int to, CancellationToken token)
        {
            bool[] composite = new bool[to + 1];
            long count = 0;

            for (int i = 2; i * i <= to; i++)
            {
                token.ThrowIfCancellationRequested(); // проверяем отмену

                if (!composite[i])
                {
                    for (int j = i * i; j <= to; j += i)
                    {
                        composite[j] = true;
                        if (j % 100_000 == 0)
                            token.ThrowIfCancellationRequested();
                    }
                }
            }

            for (int i = from; i <= to; i++)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                if (!composite[i]) count++;
            }

            return count;
        }

        // ---------- 3. Три задачи с результатом + четвертая ----------

        static void TasksWithResults()
        {
            Task<int> t1 = Task.Run(() => 2 + 3);
            Task<int> t2 = Task.Run(() => 10 * 5);
            Task<int> t3 = Task.Run(() => 100 / 4);

            Task<int[]> all = Task.WhenAll(t1, t2, t3); // ждем три задачи

            int[] res = all.Result;
            int sum = res.Sum();

            Console.WriteLine($"Результаты: {res[0]}, {res[1]}, {res[2]}");
            Console.WriteLine($"Четвертая задача считает сумму: {sum}");
        }

        // ---------- 4. Продолжения задач (ContinueWith и awaiter) ----------

        static void Continuations()
        {
            Task<int> first = Task.Run(() => 5 * 5);

            // Вариант 1: ContinueWith
            Task second = first.ContinueWith(t =>
            {
                Console.WriteLine($"ContinueWith: первый результат = {t.Result}, квадрат = {t.Result * t.Result}");
            });

            second.Wait();

            // Вариант 2: через awaiter вручную
            Task<int> another = Task.Run(() => 7 * 3);
            var awaiter = another.GetAwaiter(); // получаем awaiter
            awaiter.OnCompleted(() =>
            {
                int result = awaiter.GetResult();
                Console.WriteLine($"GetAwaiter/GetResult: результат = {result}");
            });

            another.Wait();
        }

        // ---------- 5. Parallel.For и Parallel.ForEach ----------

        static void ParallelLoops()
        {
            int[] data = Enumerable.Range(0, 1_000_000).ToArray();

            // Обычный for
            var sw = Stopwatch.StartNew();
            long sum1 = 0;
            for (int i = 0; i < data.Length; i++)
                sum1 += data[i];
            sw.Stop();
            Console.WriteLine($"Обычный for, сумма = {sum1}, время = {sw.ElapsedMilliseconds} мс");

            // Parallel.For
            sw.Restart();
            long sum2 = 0;
            object locker = new object();
            Parallel.For(0, data.Length, i =>
            {
                lock (locker)
                {
                    sum2 += data[i];
                }
            }); // распараллеленный цикл
            sw.Stop();
            Console.WriteLine($"Parallel.For,   сумма = {sum2}, время = {sw.ElapsedMilliseconds} мс");

            // Parallel.ForEach
            sw.Restart();
            long sum3 = 0;
            Parallel.ForEach(data, x =>
            {
                lock (locker)
                {
                    sum3 += x;
                }
            }); // перебор коллекции в параллельных задачах
            sw.Stop();
            Console.WriteLine($"Parallel.ForEach, сумма = {sum3}, время = {sw.ElapsedMilliseconds} мс");
        }

        // ---------- 6. Parallel.Invoke ----------

        static void ParallelInvokeDemo()
        {
            Parallel.Invoke(
                () => SimpleWork("A"),
                () => SimpleWork("B"),
                () => SimpleWork("C")
            ); // параллельный запуск нескольких делегатов
        }

        static void SimpleWork(string name)
        {
            Console.WriteLine($"Начало {name}");
            Thread.Sleep(500);
            Console.WriteLine($"Конец  {name}");
        }

        // ---------- 7. BlockingCollection: 5 поставщиков и 10 покупателей ----------

        static void BlockingCollectionDemo()
        {
            using BlockingCollection<string> storage = new BlockingCollection<string>(); // потокобезопасная очередь

            // 5 поставщиков
            Task[] suppliers = new Task[5];
            for (int i = 0; i < suppliers.Length; i++)
            {
                int supplierId = i + 1;
                suppliers[i] = Task.Run(() =>
                {
                    for (int j = 1; j <= 5; j++)
                    {
                        string item = $"Товар_{supplierId}_{j}";
                        storage.Add(item); // блокирующее добавление
                        Console.WriteLine($"Поставщик {supplierId} привёз: {item}");
                        Thread.Sleep(100 + supplierId * 50); // своя скорость
                    }
                });
            }

            // 10 покупателей
            Task[] customers = new Task[10];
            for (int i = 0; i < customers.Length; i++)
            {
                int customerId = i + 1;
                customers[i] = Task.Run(() =>
                {
                    while (!storage.IsCompleted)
                    {
                        string item;
                        if (storage.TryTake(out item, 500)) // пытаемся взять с тайм-аутом
                        {
                            Console.WriteLine($"Покупатель {customerId} купил: {item}");
                        }
                        else
                        {
                            Console.WriteLine($"Покупатель {customerId} не дождался товара и ушёл.");
                            break;
                        }
                    }
                });
            }

            // Ждём поставщиков и закрываем склад
            Task.WaitAll(suppliers);
            storage.CompleteAdding(); // больше товаров не будет

            Task.WaitAll(customers);
        }

        // ---------- 8. async/await ----------

        static async Task AsyncAwaitDemo()
        {
            Console.WriteLine("Начало асинхронного метода");
            int value = await SlowDoubleAsync(10); // ожидание задачи
            Console.WriteLine($"Результат: {value}");
        }

        static async Task<int> SlowDoubleAsync(int x)
        {
            await Task.Delay(1000);
            return x * 2;
        }
    }
}
