using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Lab11_Reflection
{
    // 1. Статический класс Reflector
    public static class Reflector
    {
        // a) Определение имени сборки, в которой определён класс
        public static string GetAssemblyName(string typeName)
        {
            Type type = Type.GetType(typeName, throwOnError: true);
            return type.Assembly.FullName;
        }

        // b) Есть ли публичные конструкторы
        public static bool HasPublicConstructors(string typeName)
        {
            Type type = Type.GetType(typeName, throwOnError: true);
            return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Any();
        }

        // c) Все общедоступные публичные методы (возвращает IEnumerable<string>)
        public static IEnumerable<string> GetPublicMethods(string typeName)
        {
            Type type = Type.GetType(typeName, throwOnError: true);
            return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                       .Select(m => m.ToString());
        }

        // d) Информация о полях и свойствах (возвращает IEnumerable<string>(MethodInfo, PropertyInfo, FieldInfo, ConstructorInfo))
        public static IEnumerable<string> GetFieldsAndProperties(string typeName)
        {
            Type type = Type.GetType(typeName, throwOnError: true);

            foreach (var field in type.GetFields(
                         BindingFlags.Public | BindingFlags.NonPublic |
                         BindingFlags.Instance | BindingFlags.Static))
            {
                yield return $"Field: {field.FieldType.Name} {field.Name}";
            }

            foreach (var prop in type.GetProperties(
                         BindingFlags.Public | BindingFlags.NonPublic |
                         BindingFlags.Instance | BindingFlags.Static))
            {
                yield return $"Property: {prop.PropertyType.Name} {prop.Name}";
            }
        }

        // e) Все реализованные классом интерфейсы (возвращает IEnumerable<string>)
        public static IEnumerable<string> GetInterfaces(string typeName)
        {
            Type type = Type.GetType(typeName, throwOnError: true);
            return type.GetInterfaces().Select(i => i.FullName);
        }

        // f) Выводит по имени класса имена методов, которые содержат
        // заданный тип параметра (тип параметра приходит как имя типа)
        public static IEnumerable<string> GetMethodsWithParameterType(string typeName, string parameterTypeName)
        {
            Type type = Type.GetType(typeName, throwOnError: true);
            Type paramType = Type.GetType(parameterTypeName, throwOnError: true);

            return type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                   BindingFlags.Instance | BindingFlags.Static)
                       .Where(m => m.GetParameters().Any(p => p.ParameterType == paramType))
                       .Select(m => m.Name);
        }

        // g) Метод Invoke: вызывает метод класса
        // Параметры: имя класса, имя метода, способ получения аргументов и файл
        // Вариант 1 – аргументы читаются из текстового файла (по одному на строку)
        // Вариант 2 – аргументы генерируются (здесь примитивный генератор)
        public static object Invoke(
            string typeName,
            string methodName,
            bool readArgumentsFromFile,
            string? argsFilePath = null)
        {
            Type type = Type.GetType(typeName, throwOnError: true);

            // берём метод (для простоты — первый с таким именем)
            MethodInfo method = type.GetMethods(
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == methodName)
                ?? throw new MissingMethodException(typeName, methodName);

            object? instance = null;
            if (!method.IsStatic)
            {
                instance = Activator.CreateInstance(type);
            }

            object?[] args = BuildArguments(method, readArgumentsFromFile, argsFilePath);
            return method.Invoke(instance, args);
        }

        // Вспомогательный метод построения аргументов для Invoke
        private static object?[] BuildArguments(
            MethodInfo method,
            bool fromFile,
            string? filePath)
        {
            var parameters = method.GetParameters();
            object?[] args = new object?[parameters.Length];

            if (fromFile)
            {
                if (filePath == null || !File.Exists(filePath))
                    throw new FileNotFoundException("Файл с аргументами не найден", filePath);

                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length < parameters.Length)
                    throw new ArgumentException("В файле недостаточно строк для всех параметров");

                for (int i = 0; i < parameters.Length; i++)
                {
                    args[i] = Convert.ChangeType(lines[i], parameters[i].ParameterType);
                }
            }
            else
            {
                // Простейший генератор значений по типам
                var rnd = new Random();
                for (int i = 0; i < parameters.Length; i++)
                {
                    Type t = parameters[i].ParameterType;
                    if (t == typeof(int))
                        args[i] = rnd.Next(1, 100);
                    else if (t == typeof(double))
                        args[i] = rnd.NextDouble() * 100;
                    else if (t == typeof(string))
                        args[i] = "auto_arg_" + i;
                    else if (t == typeof(bool))
                        args[i] = rnd.Next(0, 2) == 0;
                    else if (t.IsEnum)
                    {
                        Array values = Enum.GetValues(t);
                        args[i] = values.GetValue(rnd.Next(values.Length));
                    }
                    else
                    {
                        // Для сложных типов пробуем Activator.CreateInstance
                        args[i] = Activator.CreateInstance(t);
                    }
                }
            }

            return args;
        }

        // Запись всей информации о типе в файл (текстовый формат)
        public static void DumpTypeInfoToFile(string typeName, string filePath)
        {
            using var writer = new StreamWriter(filePath, append: false);

            writer.WriteLine($"Type: {typeName}");
            writer.WriteLine($"Assembly: {GetAssemblyName(typeName)}");
            writer.WriteLine($"Has public ctors: {HasPublicConstructors(typeName)}");
            writer.WriteLine();

            writer.WriteLine("Public methods:");
            foreach (var m in GetPublicMethods(typeName))
                writer.WriteLine("  " + m);

            writer.WriteLine();
            writer.WriteLine("Fields and properties:");
            foreach (var fp in GetFieldsAndProperties(typeName))
                writer.WriteLine("  " + fp);

            writer.WriteLine();
            writer.WriteLine("Interfaces:");
            foreach (var i in GetInterfaces(typeName))
                writer.WriteLine("  " + i);
        }

        // ---------- 2. Обобщённый метод Create ----------
        // Создаёт объект переданного типа (на основе имеющихся публичных конструкторов)
        public static T Create<T>(params object?[] args)
        {
            Type type = typeof(T);

            var ctor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                           .FirstOrDefault(c =>
                           {
                               var ps = c.GetParameters();
                               if (ps.Length != args.Length) return false;
                               for (int i = 0; i < ps.Length; i++)
                               {
                                   if (args[i] == null)
                                   {
                                       if (ps[i].ParameterType.IsValueType) return false;
                                   }
                                   else if (!ps[i].ParameterType.IsInstanceOfType(args[i]))
                                   {
                                       return false;
                                   }
                               }
                               return true;
                           });

            if (ctor == null)
                throw new MissingMethodException(
                    $"Не найден подходящий публичный конструктор для типа {type.FullName}");

            return (T)ctor.Invoke(args);
        }
    }

    // ---------- Пользовательские типы для демонстрации ----------
    public interface IExample
    {
        void DoWork(int x);
    }

    public class SampleClass : IExample
    {
        public int Value { get; set; }
        private string _name;

        public SampleClass()
        {
            _name = "default";
        }

        public SampleClass(int value, string name)
        {
            Value = value;
            _name = name;
        }

        public void DoWork(int x)
        {
            Value += x;
        }

        private int SecretMethod(string prefix, int repeat)
        {
            return (prefix + _name).Length * repeat;
        }

        public override string ToString() => $"{_name}: {Value}";
    }

    public class AnotherClass
    {
        public double X { get; set; }
        public bool Flag { get; set; }

        public AnotherClass() { }

        public AnotherClass(double x, bool flag)
        {
            X = x;
            Flag = flag;
        }

        public double Add(double a, double b) => a + b;

        public void PrintInfo(string title)
        {
            Console.WriteLine($"{title}: X={X}, Flag={Flag}");
        }
    }

    // ---------- Точка входа ----------
    internal class Program
    {
        private static void Main()
        {
            // Выберите типы для исследования
            string sampleTypeName = typeof(SampleClass).AssemblyQualifiedName!;
            string anotherTypeName = typeof(AnotherClass).AssemblyQualifiedName!;
            string standardTypeName = typeof(List<int>).AssemblyQualifiedName!;

            // 1. Исследование типов и запись в файл
            Reflector.DumpTypeInfoToFile(sampleTypeName, "SampleClassInfo.txt");
            Reflector.DumpTypeInfoToFile(anotherTypeName, "AnotherClassInfo.txt");
            Reflector.DumpTypeInfoToFile(standardTypeName, "ListOfIntInfo.txt");

            Console.WriteLine("Информация о типах записана в файлы.");

            // 2. Методы с параметром заданного типа (например, int)
            Console.WriteLine("\nМетоды SampleClass с параметром int:");
            foreach (var name in Reflector.GetMethodsWithParameterType(
                         sampleTypeName,
                         typeof(int).AssemblyQualifiedName!))
            {
                Console.WriteLine("  " + name);
            }

            // 3. Демонстрация Invoke
            // 3.1 Вызов метода с чтением аргументов из файла
            File.WriteAllLines("invoke_args.txt", new[] { "hello", "3" }); // для SecretMethod(string, int)

            object res1 = Reflector.Invoke(
                sampleTypeName,
                "SecretMethod",
                readArgumentsFromFile: true,
                argsFilePath: "invoke_args.txt");

            Console.WriteLine($"\nInvoke SecretMethod (из файла) -> {res1}");

            // 3.2 Вызов метода с автогенерацией аргументов
            object res2 = Reflector.Invoke(
                anotherTypeName,
                "Add",
                readArgumentsFromFile: false);

            Console.WriteLine($"Invoke AnotherClass.Add (генерация аргументов) -> {res2}");

            // 4. Демонстрация обобщённого метода Create<T>
            SampleClass created1 = Reflector.Create<SampleClass>(10, "created");
            AnotherClass created2 = Reflector.Create<AnotherClass>(3.14, true);

            Console.WriteLine($"\nCreate<SampleClass> -> {created1}");
            Console.WriteLine($"Create<AnotherClass> -> X={created2.X}, Flag={created2.Flag}");

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}
