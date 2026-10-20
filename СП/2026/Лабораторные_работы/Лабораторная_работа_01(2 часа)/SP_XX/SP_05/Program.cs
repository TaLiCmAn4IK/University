using System;
using System.Runtime.InteropServices; // Обязательно для DllImport

class Program
{
    [DllImport("SP_XX.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sum")]
    public static extern int sum(int x, int y);

    [DllImport("SP_XX.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sub")]
    public static extern int sub(int x, int y);

    [DllImport("SP_XX.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mul")]
    public static extern int mul(int x, int y);

    [DllImport("SP_XX.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "div")]
    public static extern int div(int x, int y);

    static void Main(string[] args)
    {
        Console.WriteLine("=== Демонстрация работы SP_05 (C# Статический вызов) ===\n");

        int a = 12;
        int b = 4;

        try
        {
            Console.WriteLine($"C# sum({a}, {b}) = {sum(a, b)}");
            Console.WriteLine($"C# sub({a}, {b}) = {sub(a, b)}");
            Console.WriteLine($"C# mul({a}, {b}) = {mul(a, b)}");
            Console.WriteLine($"C# div({a}, {b}) = {div(a, b)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при вызове DLL: {ex.Message}");
        }

        Console.WriteLine("\nДля продолжения нажмите любую клавишу...");
        Console.ReadKey();
    }
}
