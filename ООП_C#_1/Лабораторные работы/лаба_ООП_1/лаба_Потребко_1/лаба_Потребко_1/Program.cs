using System;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        // 1. Типы
        Console.WriteLine("1. Типы");

        // a. Примитивные типы с вводом/выводом
        Console.WriteLine("1a. Примитивные типы");

        Console.Write("Введите значение bool (true/false): ");
        bool b = bool.Parse(Console.ReadLine());

        Console.Write("Введите значение byte (0-255): ");
        byte by = byte.Parse(Console.ReadLine());

        Console.Write("Введите значение sbyte (-128 до 127): ");
        sbyte sb = sbyte.Parse(Console.ReadLine());

        Console.Write("Введите значение short (-32768 до 32767): ");
        short sh = short.Parse(Console.ReadLine());

        Console.Write("Введите значение ushort (0 до 65535): ");
        ushort ush = ushort.Parse(Console.ReadLine());

        Console.Write("Введите значение int: ");
        int i = int.Parse(Console.ReadLine());

        Console.Write("Введите значение uint: ");
        uint ui = uint.Parse(Console.ReadLine());

        Console.Write("Введите значение long: ");
        long l = long.Parse(Console.ReadLine());

        Console.Write("Введите значение ulong: ");
        ulong ul = ulong.Parse(Console.ReadLine());

        Console.Write("Введите значение float: ");
        float f = float.Parse(Console.ReadLine());

        Console.Write("Введите значение double: ");
        double d = double.Parse(Console.ReadLine());

        Console.Write("Введите значение decimal: ");
        decimal dec = decimal.Parse(Console.ReadLine());

        Console.Write("Введите один символ: ");
        char c = Console.ReadLine()[0];

        Console.WriteLine($"\nВы ввели: bool={b}, byte={by}, sbyte={sb}, short={sh}, ushort={ush}");
        Console.WriteLine($"int={i}, uint={ui}, long={l}, ulong={ul}, float={f}");
        Console.WriteLine($"double={d}, decimal={dec}, char={c}");

        // b. Приведения
        Console.WriteLine("\n--- 1b. Приведения типов ---");

        // Неявные приведения
        int intValue = 100;
        double doubleValue = intValue;
        byte byteValue = 100;
        short byteToShort = byteValue;
        char charValue = 'A';
        int charToInt = charValue;
        ushort ushortValue = 500;
        int ushortToInt = ushortValue;
        float floatValue = 3.14f;
        double floatToDouble = floatValue;
        int intToLong = 100;
        long longValue = intToLong;

        // Явные приведения
        double doubleValue2 = 123.456;
        int doubleToInt = (int)doubleValue2;
        char intToChar = (char)charToInt;
        float floatValue2 = 3.99f;
        int floatToInt = (int)floatValue2;
        long longValue2 = 1000;
        int longToInt = (int)longValue2;
        decimal decimalValue = 999.99m;
        int decimalToInt = (int)decimalValue;

        // Преобразования с помощью Convert
        double convertDouble = 456.789;
        int convertedInt = Convert.ToInt32(convertDouble);
        string numberString = "123";
        int fromString = Convert.ToInt32(numberString);

        Console.WriteLine("Неявные приведения:");
        Console.WriteLine($"int {intValue} -> double {doubleValue}");
        Console.WriteLine($"byte {byteValue} -> short {byteToShort}");
        Console.WriteLine($"char '{charValue}' -> int {charToInt}");
        Console.WriteLine($"ushort {ushortValue} -> int {ushortToInt}");
        Console.WriteLine($"float {floatValue} -> double {floatToDouble}");
        Console.WriteLine($"int {intToLong} -> long {longValue}");

        Console.WriteLine("\nЯвные приведения:");
        Console.WriteLine($"double {doubleValue2} -> int {doubleToInt}");
        Console.WriteLine($"int {charToInt} -> char '{intToChar}'");
        Console.WriteLine($"float {floatValue2} -> int {floatToInt}");
        Console.WriteLine($"long {longValue2} -> int {longToInt}");
        Console.WriteLine($"decimal {decimalValue} -> int {decimalToInt}");

        Console.WriteLine("\nПреобразования с помощью Convert:");
        Console.WriteLine($"Convert.ToInt32({convertDouble}) = {convertedInt} (округление)");
        Console.WriteLine($"Convert.ToInt32(\"{numberString}\") = {fromString}");

        // c. Упаковка и распаковка
        Console.WriteLine("\n--- 1c. Упаковка и распаковка ---");
        int boxedValue = 42;
        object boxed = boxedValue; // упаковка
        int unboxed = (int)boxed; // распаковка
        Console.WriteLine($"Исходное: {boxedValue}, упакованное: {boxed}, распакованное: {unboxed}");

        // d. Неявно типизированная переменная
        Console.WriteLine("\n--- 1d. Неявно типизированная переменная ---");
        var implicitVar = 10;
        Console.WriteLine($"var implicitVar = 10; // Тип: {implicitVar.GetType()}, Значение: {implicitVar}");

        // e. Nullable переменная
        Console.WriteLine("\n--- 1e. Nullable переменная ---");
        int? nullableInt = null;
        Console.WriteLine($"int? nullableInt = null; // Значение: {nullableInt}");
        nullableInt = 5;
        Console.WriteLine($"nullableInt = 5; // Значение: {nullableInt}");
        Console.WriteLine($"nullableInt.HasValue: {nullableInt.HasValue}");
        Console.WriteLine($"nullableInt.Value: {nullableInt.Value}");

        // f. Ошибка с var
        Console.WriteLine("\n--- 1f. Ошибка с var ---");
        var errorVar = 10;
        Console.WriteLine($"var errorVar = 10; // Работает - тип int");
        // errorVar = "string"; // Раскомментировать для ошибки:
        Console.WriteLine("// errorVar = \"string\"; // Ошибка: нельзя изменить тип после объявления");

        // 2. Строки
        Console.WriteLine("\n=== 2. Строки ===");

        // a. Строковые литералы
        Console.WriteLine("--- 2a. Строковые литералы ---");
        string literal1 = "hello";
        string literal2 = "hello";
        string literal3 = "HELLO";
        Console.WriteLine($"literal1 = \"{literal1}\", literal2 = \"{literal2}\", literal3 = \"{literal3}\"");
        Console.WriteLine($"literal1 == literal2: {literal1 == literal2}");
        Console.WriteLine($"literal1 == literal3: {literal1 == literal3}");
        Console.WriteLine($"string.Equals(literal1, literal2): {string.Equals(literal1, literal2)}");
        Console.WriteLine($"string.Equals(literal1, literal3, StringComparison.OrdinalIgnoreCase): {string.Equals(literal1, literal3, StringComparison.OrdinalIgnoreCase)}");

        // b. Операции со строками
        Console.WriteLine("\n--- 2b. Операции со строками ---");
        string str1 = "Hello";
        string str2 = "World";
        string str3 = "C# Programming";
        string str4 = "Hel";

        // Сцепление
        string concat = str1 + " " + str2;
        Console.WriteLine($"Сцепление: \"{str1}\" + \" {str2}\" = \"{concat}\"");

        // Копирование
        string copy = string.Copy(str4);
        Console.WriteLine($"Копирование: string.Copy(\"{str1}\") = \"{copy}\"");

        // Подстрока
        string substring = str3.Substring(0, 5);
        Console.WriteLine($"Подстрока: \"{str1}\".Substring(0, 5) = \"{substring}\"");

        string substrin_1 = str1.Substring(0, 2);

        // Разделение на слова
        string text = "C# is a great programming language";
        string[] words = text.Split(' ');
        Console.WriteLine($"Разделение: \"{text}\".Split(' ')");
        foreach (var word in words)
        {
            Console.WriteLine($"  - {word}");
        }

        // Вставка
        string inserted = str1.Insert(2, "TEST");
        Console.WriteLine($"Вставка: \"{str1}\".Insert(2, \"TEST\") = \"{inserted}\"");

        // Удаление
        string removed = str3.Remove(3, 2);
        Console.WriteLine($"Удаление: \"{str3}\".Remove(3, 2) = \"{removed}\"");

        // Интерполирование
        string interpolated = $"{str1} {str2}! Today is {DateTime.Now:yyyy-MM-dd}";
        Console.WriteLine($"Интерполирование: $\"{str1} {str2}! Today is {{DateTime.Now:yyyy-MM-dd}}\" = \"{interpolated}\"");

        // c. Пустая и null строка
        Console.WriteLine("\n--- 2c. Пустая и null строка ---");
        string empty = "";
        string nullString = null;
        string whitespace = "   ";

        Console.WriteLine($"string.IsNullOrEmpty(null): {string.IsNullOrEmpty(nullString)}");
        Console.WriteLine($"string.IsNullOrEmpty(\"\"): {string.IsNullOrEmpty(empty)}");
        Console.WriteLine($"string.IsNullOrEmpty(\"   \"): {string.IsNullOrEmpty(whitespace)}");
        Console.WriteLine($"string.IsNullOrWhiteSpace(\"   \"): {string.IsNullOrWhiteSpace(whitespace)}");

        // Демонстрация других операций
        Console.WriteLine($"empty.Length: {empty.Length}");
        Console.WriteLine($"empty.ToUpper(): {empty.ToUpper()}");

        // Попытка вызова метода на null строке
        try
        {
            Console.WriteLine(nullString.ToUpper());
        }
        catch (NullReferenceException ex)
        {
            Console.WriteLine($"Ошибка при вызове nullString.ToUpper(): {ex.Message}");
        }

        // Защита от null
        if (!string.IsNullOrEmpty(nullString))
        {
            Console.WriteLine(nullString.ToUpper());
        }
        else
        {
            Console.WriteLine("Строка null или пустая, вызов ToUpper() опасен.");
        }

        // d. StringBuilder
        Console.WriteLine("\n--- 2d. StringBuilder ---");
        StringBuilder sbBuilder = new StringBuilder("Hello World");
        Console.WriteLine($"Исходная строка: {sbBuilder}");
        sbBuilder.Remove(3, 6);
        sbBuilder.Remove(5, 6); // Удаление " World"
        Console.WriteLine($"После Remove(5, 6): {sbBuilder}");

        sbBuilder.Append("!"); // Добавление в конец
        Console.WriteLine($"После Append(\"!\"): {sbBuilder}");

        sbBuilder.Insert(0, "Start "); // Вставка в начало
        Console.WriteLine($"После Insert(0, \"Start \"): {sbBuilder}");

        sbBuilder.Replace("Start", "Begin"); // Замена
        Console.WriteLine($"После Replace(\"Start\", \"Begin\"): {sbBuilder}");

        // 3. Массивы
        Console.WriteLine("\n3. Массивы");

        // a. Двумерный массив
        Console.WriteLine("3a. Двумерный массив");
        int[,] matrix = {
            { 1, 2, 3, 4 },
            { 5, 6, 7, 8 },
            { 9, 10, 11, 12 }
        };

        Console.WriteLine("Двумерный массив (матрица 3x4):");
        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            for (int col = 0; col < matrix.GetLength(1); col++)
            {
                Console.Write($"{matrix[row, col],3}");
            }
            Console.WriteLine();
        }

        // b. Одномерный массив строк
        Console.WriteLine("\n--- 3b. Одномерный массив строк ---");
        string[] stringArray = { "apple", "banana", "cherry", "date" };

        Console.WriteLine("Исходный массив:");
        foreach (var item in stringArray)
        {
            Console.WriteLine($"  - {item}");
        }
        Console.WriteLine($"Длина массива: {stringArray.Length}");

        // Изменение элемента
        Console.Write("Введите позицию для изменения (0-3): ");
        int position = int.Parse(Console.ReadLine());
        Console.Write("Введите новое значение: ");
        string newValue = Console.ReadLine();

        if (position >= 0 && position < stringArray.Length)
        {
            stringArray[position] = newValue;
            Console.WriteLine("Массив после изменения:");
            foreach (var item in stringArray)
            {
                Console.WriteLine($"  - {item}");
            }
        }
        else
        {
            Console.WriteLine("Неверная позиция!");
        }

        // c. Ступенчатый массив
        Console.WriteLine("\n--- 3c. Ступенчатый массив ---");
        double[][] jagged = new double[3][];
        jagged[0] = new double[2];
        jagged[1] = new double[3];
        jagged[2] = new double[4];

        Console.WriteLine("Введите значения для ступенчатого массива:");
        for (int row = 0; row < jagged.Length; row++)
        {
            Console.WriteLine($"Строка {row} ({jagged[row].Length} элементов):");
            for (int col = 0; col < jagged[row].Length; col++)
            {
                Console.Write($"  Элемент [{row}][{col}]: ");
                jagged[row][col] = double.Parse(Console.ReadLine());
            }
        }

        Console.WriteLine("Ступенчатый массив:");
        for (int row = 0; row < jagged.Length; row++)
        {
            Console.Write($"Строка {row}: ");
            for (int col = 0; col < jagged[row].Length; col++)
            {
                Console.Write($"{jagged[row][col]} ");
            }
            Console.WriteLine();
        }

        // d. Неявно типизированные переменные
        Console.WriteLine("\n--- 3d. Неявно типизированные переменные ---");
        var implicitArray = new[] { 1, 2, 3, 4, 5 };
        var implicitString = "Hello World";

        Console.WriteLine($"var implicitArray = new[] {{ 1, 2, 3, 4, 5 }}; // Тип: {implicitArray.GetType()}");
        Console.WriteLine($"var implicitString = \"Hello World\"; // Тип: {implicitString.GetType()}");

        // 4. Кортежи
        Console.WriteLine("\n=== 4. Кортежи ===");

        // a. Кортеж из 5 элементов
        Console.WriteLine("--- 4a. Кортеж из 5 элементов ---");
        var tuple = (123, "hello", 'A', "world", 100UL);
        Console.WriteLine($"Кортеж: {tuple}");

        // b. Вывод кортежа
        Console.WriteLine("\n--- 4b. Вывод кортежа ---");
        Console.WriteLine($"Целиком: {tuple}");
        Console.WriteLine($"Выборочно (1, 3, 4 элементы): {tuple.Item1}, {tuple.Item3}, {tuple.Item4}");

        // c. Распаковка кортежа
        Console.WriteLine("\n--- 4c. Распаковка кортежа ---");

        // Способ 1: Явное указание типов
        (int num1, string word1, char sym1, string word2, ulong ulVal1) = tuple;
        Console.WriteLine($"Способ 1: {num1}, {word1}, {sym1}, {word2}, {ulVal1}");

        // Способ 2: Неявное указание типов (var)
        var (num2, word3, sym2, word4, ulVal2) = tuple;
        Console.WriteLine($"Способ 2: {num2}, {word3}, {sym2}, {word4}, {ulVal2}");

        // Способ 3: Использование _
        var (num3, _, sym3, _, ulVal3) = tuple;
        Console.WriteLine($"Способ 3 (с _): {num3}, {sym3}, {ulVal3}");

        // d. Сравнение кортежей
        Console.WriteLine("\n--- 4d. Сравнение кортежей ---");
        var tuple2 = (123, "hello", 'A', "world", 100UL);
        var tuple3 = (456, "test", 'B', "string", 200UL);

        Console.WriteLine($"tuple == tuple2: {tuple == tuple2}");
        Console.WriteLine($"tuple == tuple3: {tuple == tuple3}");
        Console.WriteLine($"tuple.Equals(tuple2): {tuple.Equals(tuple2)}");
        Console.WriteLine($"tuple.Equals(tuple3): {tuple.Equals(tuple3)}");

        // 5. Локальная функция
        Console.WriteLine("\n=== 5. Локальная функция ===");

        (int max, int min, int sum, char firstChar) ProcessArray(int[] arr, string str)
        {
            if (arr == null || arr.Length == 0)
                throw new ArgumentException("Массив не может быть пустым");

            if (string.IsNullOrEmpty(str))
                throw new ArgumentException("Строка не может быть пустой");

            int max = arr[0];
            int min = arr[0];
            int sum = 0;

            foreach (var num in arr)
            {
                if (num > max) max = num;
                if (num < min) min = num;
                sum += num;
            }

            return (max, min, sum, str[0]);
        }

        int[] numbers = { 10, 5, 8, 3, 15, 7 };
        string textForFunction = "Hello";

        var result = ProcessArray(numbers, textForFunction);
        Console.WriteLine($"Массив: [{string.Join(", ", numbers)}]");
        Console.WriteLine($"Строка: \"{textForFunction}\"");
        Console.WriteLine($"Результат: Max={result.max}, Min={result.min}, Sum={result.sum}, FirstChar={result.firstChar}");

        // 6. checked/unchecked
        Console.WriteLine("\n=== 6. checked/unchecked ===");

        void CheckedFunction()
        {
            Console.WriteLine("--- checked блок ---");
            checked
            {
                try
                {
                    int max = int.MaxValue;
                    Console.WriteLine($"int.MaxValue = {max}");
                    int willThrow = max + 1; // Выбросит OverflowException
                    Console.WriteLine($"Этот текст не будет напечатан: {willThrow}");
                }
                catch (OverflowException ex)
                {
                    Console.WriteLine($"В checked блоке поймано исключение: {ex.Message}");
                }
            }
        }

        void UncheckedFunction()
        {
            Console.WriteLine("--- unchecked блок ---");
            unchecked
            {
                int max = int.MaxValue;
                int overflow = max + 1; // Переполнение без исключения
                Console.WriteLine($"int.MaxValue = {max}");
                Console.WriteLine($"max + 1 = {overflow} (переполнение)");
            }
        }

        CheckedFunction();
        UncheckedFunction();

        Console.WriteLine("\n=== Завершение программы ===");
        Console.ReadLine();
    }
}