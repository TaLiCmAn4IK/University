using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

namespace Lab12_PVR
{
    // =========== ЗАДАНИЕ №1: Класс для логирования действий пользователя ===========
    // Назначение: Запись всех действий в текстовый файл _pvrlogfile.txt
    class PVRLog
    {
        static string logFile = "_pvrlogfile.txt";
        public static void Write(string action, string details)
        {
            try
            {
                string log = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {action} | {details}";
                File.AppendAllText(logFile, log + Environment.NewLine);
            }
            catch { }
        }
        public static void ShowLog()
        {
            if (File.Exists(logFile))
            {
                Console.WriteLine("\nСодержимое лога:");
                Console.WriteLine(new string('-', 50));
                Console.WriteLine(File.ReadAllText(logFile));
            }
        }

        // =========== ЗАДАНИЕ №6: Очистка лога (оставить записи за последний час) ===========
        public static void ClearOld()
        {
            try
            {
                string[] lines = File.ReadAllLines(logFile);
                List<string> newLines = new List<string>();
                DateTime oneHourAgo = DateTime.Now.AddHours(-1);

                foreach (string line in lines)
                {
                    try
                    {
                        string dateStr = line.Split('|')[0].Trim();
                        DateTime logDate = DateTime.Parse(dateStr); // парсим дату в каждой строке
                        if (logDate >= oneHourAgo) // оставляем только за последний час
                            newLines.Add(line);
                    }
                    catch { }
                }

                File.WriteAllLines(logFile, newLines);
                Write("LOG_CLEAN", "Оставлены записи за последний час");
                Console.WriteLine($"В логе осталось {newLines.Count} записей");
            }
            catch { }
        }
    }

    // =========== ЗАДАНИЕ №2: Класс для информации о дисках ===========
    // Назначение: Вывод информации о свободном месте, файловой системе и дисках
    class PVRDiskInfo
    {
        public static void ShowInfo()
        {
            try
            {
                Console.WriteLine("\n=== Информация о дисках ===");

                // a) Свободное место на диске
                DriveInfo cDrive = new DriveInfo("C");
                Console.WriteLine($"a) Свободное место на C: {cDrive.AvailableFreeSpace / 1024 / 1024 / 1024} ГБ");
                PVRLog.Write("DISK_SPACE", $"C: {cDrive.AvailableFreeSpace / 1024 / 1024 / 1024} ГБ свободно");

                // b) Файловая система
                Console.WriteLine($"b) Файловая система C: {cDrive.DriveFormat}");
                PVRLog.Write("DISK_FS", $"C: {cDrive.DriveFormat}");

                // c) Для каждого существующего диска - имя, объем, доступный объем, метка тома
                Console.WriteLine("c) Все диски:");
                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (DriveInfo d in drives)
                {
                    if (d.IsReady) //позволяет пропустить, например, пустой DVD‑привод
                    {
                        Console.WriteLine($"   {d.Name} - {d.VolumeLabel}");
                        Console.WriteLine($"     Объем: {d.TotalSize / 1024 / 1024 / 1024} ГБ");
                        Console.WriteLine($"     Свободно: {d.AvailableFreeSpace / 1024 / 1024 / 1024} ГБ");
                    }
                }
                PVRLog.Write("DISK_ALL", "Показаны все диски");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    // =========== ЗАДАНИЕ №3: Класс для информации о конкретном файле ===========
    // Принимает путь к файлу, оборачивает его в FileInfo и показывает полное имя, размер,
    // расширение, даты создания и последнего изменения.
    class PVRFileInfo
    {
        public static void ShowInfo(string filePath)
        {
            try
            {
                Console.WriteLine("\n=== Информация о файле ===");
                FileInfo file = new FileInfo(filePath);

                // a) Полный путь
                Console.WriteLine($"a) Полный путь: {file.FullName}");

                // b) Размер, расширение, имя
                Console.WriteLine($"b) Имя: {file.Name}");
                Console.WriteLine($"   Размер: {file.Length} байт");
                Console.WriteLine($"   Расширение: {file.Extension}");

                // c) Дата создания, изменения
                Console.WriteLine($"c) Дата создания: {file.CreationTime}");
                Console.WriteLine($"   Дата изменения: {file.LastWriteTime}");

                PVRLog.Write("FILE_INFO", $"{file.Name} ({file.Length} байт)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    // =========== ЗАДАНИЕ №4: Класс для информации о конкретной директории ===========
    // Назначение: Вывод информации о директории (файлы, поддиректории, родители)
    class PVRDirInfo
    {
        public static void ShowInfo(string dirPath)
        {
            try
            {
                Console.WriteLine("\n=== Информация о директории ===");
                DirectoryInfo dir = new DirectoryInfo(dirPath);

                // a) Количество файлов
                Console.WriteLine($"a) Количество файлов: {dir.GetFiles().Length}");

                // b) Время создания
                Console.WriteLine($"b) Время создания: {dir.CreationTime}");

                // c) Количество поддиректориев
                Console.WriteLine($"c) Количество поддиректорий: {dir.GetDirectories().Length}");

                // d) Список родительских директориев
                Console.WriteLine($"d) Родительские директории:");

                DirectoryInfo parent = dir.Parent;
                while (parent != null)
                {
                    Console.WriteLine($"   {parent.FullName}");
                    parent = parent.Parent;
                }

                PVRLog.Write("DIR_INFO", $"{dir.Name} (файлов: {dir.GetFiles().Length})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    // =========== ЗАДАНИЕ №5: Класс для управления файлами ===========
    // Назначение: Выполнение операций с файлами и директориями
    class PVRFileManager
    {
        // =========== ЗАДАНИЕ 5а: Работа с директорией и файлами ===========
        public static void TaskA(string drive)
        {
            try
            {
                Console.WriteLine("\n=== Задание 5а ===");

                // Создаем PVRInspect
                string inspectDir = Path.Combine(drive, "PVRInspect");
                if (Directory.Exists(inspectDir))
                    Directory.Delete(inspectDir, true);
                Directory.CreateDirectory(inspectDir);
                PVRLog.Write("DIR_CREATE", $"Создана {inspectDir}");

                // Создаем текстовый файл _pvrdirinfo.txt
                string filePath = Path.Combine(inspectDir, "pvrdirinfo.txt");
                File.WriteAllText(filePath, $"Информация\nСоздано: {DateTime.Now}");
                PVRLog.Write("FILE_CREATE", $"Создан {Path.GetFileName(filePath)}");

                // Создаем копию файла
                string copyPath = Path.Combine(inspectDir, "pvrdirinfo_copy.txt");
                File.Copy(filePath, copyPath);
                PVRLog.Write("FILE_COPY", $"Скопирован в {Path.GetFileName(copyPath)}");

                // Переименовываем копию
                string renamePath = Path.Combine(inspectDir, "pvrdirinfo_renamed.txt");
                File.Move(copyPath, renamePath);
                PVRLog.Write("FILE_RENAME", $"Переименован в {Path.GetFileName(renamePath)}");

                // Удаляем первоначальный файл
                File.Delete(filePath);
                PVRLog.Write("FILE_DELETE", $"Удален {Path.GetFileName(filePath)}");

                Console.WriteLine("Задание 5а выполнено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка 5а: {ex.Message}");
            }
        }

        // =========== ЗАДАНИЕ 5b: Копирование файлов по расширению ===========
        public static void TaskB(string sourceDir, string extension)
        {
            try
            {
                Console.WriteLine("\n=== Задание 5b ===");

                // Создаем PVRFiles
                string filesDir = Path.Combine(Path.GetPathRoot(sourceDir), "PVRFiles");
                if (Directory.Exists(filesDir))
                    Directory.Delete(filesDir, true);
                Directory.CreateDirectory(filesDir);
                PVRLog.Write("DIR_CREATE", $"Создана {filesDir}");

                // Копируем файлы с заданным расширением
                string[] files = Directory.GetFiles(sourceDir, "*" + extension);
                foreach (string file in files)
                {
                    string dest = Path.Combine(filesDir, Path.GetFileName(file));
                    File.Copy(file, dest);
                    PVRLog.Write("FILE_COPY", $"{Path.GetFileName(file)} -> {filesDir}");
                }

                // Перемещаем PVRFiles в PVRInspect
                string inspectDir = Path.Combine(Path.GetPathRoot(sourceDir), "PVRInspect");
                string movedDir = Path.Combine(inspectDir, "PVRFiles");
                Directory.Move(filesDir, movedDir);
                PVRLog.Write("DIR_MOVE", $"Перемещена в {movedDir}");

                Console.WriteLine($"Скопировано {files.Length} файлов с расширением {extension}");
                Console.WriteLine("Задание 5b выполнено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка 5b: {ex.Message}");
            }
        }

        // =========== ЗАДАНИЕ 5c: Архивирование и разархивирование ===========
        public static void TaskC(string drive)
        {
            try
            {
                Console.WriteLine("\n=== Задание 5c ===");

                string inspectDir = Path.Combine(drive, "PVRInspect");
                string filesDir = Path.Combine(inspectDir, "PVRFiles");

                // Создаем архив из файлов директория PVRFiles
                string zipPath = Path.Combine(inspectDir, "pvr_archive.zip");
                if (File.Exists(zipPath))
                    File.Delete(zipPath);

                ZipFile.CreateFromDirectory(filesDir, zipPath);
                PVRLog.Write("ZIP_CREATE", $"Создан архив {Path.GetFileName(zipPath)}");

                // Разархивируем его в другой директорий
                string extractDir = Path.Combine(inspectDir, "Extracted");
                if (Directory.Exists(extractDir))
                    Directory.Delete(extractDir, true);

                ZipFile.ExtractToDirectory(zipPath, extractDir);
                PVRLog.Write("ZIP_EXTRACT", $"Распакован в {Path.GetFileName(extractDir)}");

                Console.WriteLine("Задание 5c выполнено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка 5c: {ex.Message}");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Лабораторная работа №12: Потоки и файловая система ===");
            Console.WriteLine("Инициалы: PVR\n");

            PVRLog.Write("START", "Начало работы");

            try
            {
                // =========== ЗАДАНИЕ №2: Демонстрация работы класса PVRDiskInfo ===========
                PVRDiskInfo.ShowInfo();

                // =========== ЗАДАНИЕ №3: Демонстрация работы класса PVRFileInfo ===========
                string currentFile = System.Reflection.Assembly.GetExecutingAssembly().Location;
                PVRFileInfo.ShowInfo(currentFile);

                // =========== ЗАДАНИЕ №4: Демонстрация работы класса PVRDirInfo ===========
                string currentDir = Directory.GetCurrentDirectory();
                PVRDirInfo.ShowInfo(currentDir);

                // =========== ЗАДАНИЕ №5: Демонстрация работы класса PVRFileManager ===========
                string drive = Path.GetPathRoot(currentDir);

                // 5а: Создание директория, файла, копии, переименование, удаление
                PVRFileManager.TaskA(drive);

                // 5b: Копирование файлов с заданным расширением
                // Создаем тестовые файлы в текущей папке
                string testFile1 = Path.Combine(currentDir, "test1.txt");
                string testFile2 = Path.Combine(currentDir, "test2.txt");
                string testFile3 = Path.Combine(currentDir, "program.cs");

                File.WriteAllText(testFile1, "Тестовый текст 1");
                File.WriteAllText(testFile2, "Тестовый текст 2");
                File.WriteAllText(testFile3, "using System;");

                PVRFileManager.TaskB(currentDir, ".txt");

                // 5c: Архивирование и разархивирование
                PVRFileManager.TaskC(drive);

                // =========== ЗАДАНИЕ №6: Работа с лог-файлом ===========
                Console.WriteLine("\n=== Работа с логом ===");
                Console.WriteLine($"Записей в логе: {File.ReadAllLines("_pvrlogfile.txt").Length}");

                // Оставляем только записи за последний час
                Console.WriteLine("\nОчищаем лог (оставляем записи за последний час):");
                PVRLog.ClearOld();

                // Показываем лог
                PVRLog.ShowLog();

                // Удаляем тестовые файлы
                File.Delete(testFile1);
                File.Delete(testFile2);
                File.Delete(testFile3);

                PVRLog.Write("FINISH", "Работа завершена");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                PVRLog.Write("ERROR", ex.Message);
            }

            Console.WriteLine("\n\n=== Результаты ===");
            string inspectDir = Path.Combine(Path.GetPathRoot(Directory.GetCurrentDirectory()), "PVRInspect");
            if (Directory.Exists(inspectDir))
            {
                Console.WriteLine($"Создана папка: {inspectDir}");
                Console.WriteLine("Содержимое:");
                foreach (string item in Directory.GetFileSystemEntries(inspectDir))
                {
                    Console.WriteLine($"  {Path.GetFileName(item)}");
                }
            }

            Console.WriteLine($"\nСоздан лог-файл: _pvrlogfile.txt");
            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}