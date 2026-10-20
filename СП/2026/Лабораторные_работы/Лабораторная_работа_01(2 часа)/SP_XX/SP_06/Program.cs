using System;
using System.Runtime.InteropServices;

class SP_06
{
    delegate int MathFunc(int x, int y);

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("6 динамический вызов\n");

        IntPtr lib = NativeLibrary.Load("SP_XX.dll");
        Console.WriteLine($"Дескриптор библиотеки: 0x{lib:X}\n");

        var fnSum = Marshal.GetDelegateForFunctionPointer<MathFunc>(
                        NativeLibrary.GetExport(lib, "sum"));
        var fnSub = Marshal.GetDelegateForFunctionPointer<MathFunc>(
                        NativeLibrary.GetExport(lib, "sub"));
        var fnMul = Marshal.GetDelegateForFunctionPointer<MathFunc>(
                        NativeLibrary.GetExport(lib, "mul"));
        var fnDiv = Marshal.GetDelegateForFunctionPointer<MathFunc>(
                        NativeLibrary.GetExport(lib, "div"));

        int a = 10, b = 3;
        Console.WriteLine($"a = {a}, b = {b}\n");

        Console.WriteLine($"sum({a}, {b}) = {fnSum(a, b)}");
        Console.WriteLine($"sub({a}, {b}) = {fnSub(a, b)}");
        Console.WriteLine($"mul({a}, {b}) = {fnMul(a, b)}");
        Console.WriteLine($"div({a}, {b}) = {fnDiv(a, b)}");

        NativeLibrary.Free(lib);
        Console.WriteLine("\nБиблиотека выгружена");
    }
}