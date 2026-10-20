using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab8_DelegatesEventsLambda
{
    // Данные для события "обновление"
    public sealed class UpgradeEventArgs : EventArgs
    {
        public UpgradeEventArgs(int oldVersion, int newVersion, string reason)
        {
            OldVersion = oldVersion;
            NewVersion = newVersion;
            Reason = reason;
        }

        public int OldVersion { get; }
        public int NewVersion { get; }
        public string Reason { get; }
    }

    // Данные для события "работа"
    public sealed class WorkEventArgs : EventArgs
    {
        public WorkEventArgs(string taskName, int effortPoints)
        {
            TaskName = taskName;
            EffortPoints = effortPoints;
        }

        public string TaskName { get; }
        public int EffortPoints { get; }
    }

    // Класс Пользователь с событиями upgrade и work
    public class User
    {
        public User(string name, int version = 1)
        {
            Name = name;
            Version = version;
        }

        public string Name { get; }
        public int Version { get; private set; }
        public int Experience { get; private set; }

        // События
        public event EventHandler<UpgradeEventArgs>? Upgrade;
        public event EventHandler<WorkEventArgs>? Work;

        // Метод, который "делает работу" и поднимает событие Work
        public void DoWork(string taskName, int effortPoints)
        {
            Experience += effortPoints;              // меняем состояние объекта
            Work?.Invoke(this, new WorkEventArgs(taskName, effortPoints));
        }

        // Метод, который "обновляет версию" и поднимает событие Upgrade
        public void DoUpgrade(string reason)
        {
            int old = Version;
            Version++;                               // меняем состояние объекта
            Upgrade?.Invoke(this, new UpgradeEventArgs(old, Version, reason));
        }


        public override string ToString() => $"{Name}: версия={Version}, опыт={Experience}";
    }

    internal class Program
    {
        // Демонстрация множественного делегата (multicast) на примере логгера
        private static Action<string> СоздатьЛоггер()
        {
            Action<string> log = msg => Console.WriteLine($"[ЛОГ] {msg}");
            log += msg => Console.WriteLine($"[ВРЕМЯ] {DateTime.Now:HH:mm:ss} {msg}");
            return log;
        }

        // Пять обработчиков строки (Func<string,string>) для конвейера
        private static List<Func<string, string>> СоздатьКонвейерСтроки()
        {
            Func<string, string> УбратьЛишниеПробелы =
                s => string.Join(" ", (s ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries));

            Func<string, string> УдалитьПунктуацию =
                s => new string((s ?? "").Where(ch => !char.IsPunctuation(ch)).ToArray());

            Func<string, string> ЗаменитьЦифрыНаРешётку =
                s => new string((s ?? "").Select(ch => char.IsDigit(ch) ? '#' : ch).ToArray());

            Func<string, string> ВВерхнийРегистр =
                s => (s ?? "").ToUpperInvariant();

            Func<string, string> ДобавитьПрефикс =
                s => "[Обработано] " + (s ?? "");

            return new List<Func<string, string>>
            {
                УбратьЛишниеПробелы,
                УдалитьПунктуацию,
                ЗаменитьЦифрыНаРешётку,
                ВВерхнийРегистр,
                ДобавитьПрефикс
            };
        }

        private static string ПрогнатьКонвейер(string input, IEnumerable<Func<string, string>> шаги)
        {
            string текущее = input;
            foreach (var шаг in шаги)
                текущее = шаг(текущее);
            return текущее;
        }

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // ===== Часть 1. Делегаты + события =====
            Console.WriteLine("=== ЧАСТЬ 1: СОБЫТИЯ (Upgrade/Work) ===");

            var log = СоздатьЛоггер();

            // Создаём несколько объектов "Пользователь"
            var users = new List<User>
            {
                new User("Алиса", version: 1),
                new User("Боб", version: 2),
                new User("Чарли", version: 1),
                new User("Диана", version: 3)
            };

            // Подписываем "произвольным образом" (не одинаково для всех)
            users[0].Work += (s, e) => log($"{((User)s!).Name} выполняет: {e.TaskName} (+{e.EffortPoints} опыта)");
            users[0].Upgrade += (s, e) => log($"{((User)s!).Name} обновился: {e.OldVersion} -> {e.NewVersion} ({e.Reason})");

            users[1].Work += (s, e) =>
            {
                var u = (User)s!;
                Console.WriteLine($"Обработчик Боба: задача '{e.TaskName}', суммарный опыт={u.Experience}");
            };
            users[1].Upgrade += (s, e) =>
                Console.WriteLine($"Обработчик Боба: обновление по причине '{e.Reason}'");

            // У Чарли два обработчика на Work (видно, что вызовется оба)
            users[2].Work += (s, e) => Console.WriteLine($"Чарли #1: сделал '{e.TaskName}'");
            users[2].Work += (s, e) => Console.WriteLine($"Чарли #2: начислено {e.EffortPoints} опыта");
            users[2].Upgrade += (s, e) => Console.WriteLine($"Чарли обновился до версии {e.NewVersion}");

            // У Дианы только Upgrade (Work без подписчиков — программа не падает)
            users[3].Upgrade += (s, e) => Console.WriteLine($"Диана: новая версия {e.NewVersion}");

            // Инициируем события через методы
            users[0].DoWork("Исправить баги", 10);
            users[0].DoUpgrade("Патч безопасности");

            users[1].DoWork("Написать отчёт", 5);
            users[1].DoUpgrade("Обновление функционала");

            users[2].DoWork("Рефакторинг", 7);
            users[2].DoUpgrade("Крупный релиз");

            users[3].DoWork("Работа без слушателей", 3);
            users[3].DoUpgrade("Хотфикс");

            Console.WriteLine();
            Console.WriteLine("Состояние объектов после наступления событий:");
            users.ForEach(u => Console.WriteLine(u));

            // ===== Часть 2. Action/Func/Predicate + лямбды =====
            Console.WriteLine();
            Console.WriteLine("=== ЧАСТЬ 2: КОНВЕЙЕР ОБРАБОТКИ СТРОКИ ===");

            Predicate<string> СтрокаНеПуста = s => !string.IsNullOrWhiteSpace(s);
            Action<string> Печать = s => Console.WriteLine(s);

            var шаги = СоздатьКонвейерСтроки();

            string input = "  Привет, мир!!! версия 2.0  ";
            if (СтрокаНеПуста(input))
            {
                string result = ПрогнатьКонвейер(input, шаги);
                Печать("Было:   " + input);
                Печать("Стало:  " + result);
            }
            else
            {
                Печать("Входная строка пустая — обрабатывать нечего.");
            }

            Console.WriteLine();
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}
