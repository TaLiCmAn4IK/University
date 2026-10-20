import os
import subprocess
import sys
import time
import threading

# Автоматически настраиваем терминал Windows
os.system('chcp 65001 > nul')

try:
    import msvcrt
except ImportError:
    print("[Ошибка] Этот скрипт предназначен для работы в операционной системе Windows.")
    sys.exit(1)

def run_git(command_list):
    """Вызов команд Git через список аргументов"""
    result = subprocess.run(
        command_list, 
        stdout=subprocess.PIPE, 
        stderr=subprocess.PIPE, 
        text=True, 
        encoding='utf-8', 
        shell=False
    )
    return result

def get_folders():
    """Сканирует репозиторий, собирая папки предметов"""
    ignored = {'.git', '.vs', 'WPSDrive', 'edit-on-pc'}
    folders = []
    for item in os.listdir('.'):
        if os.path.isdir(item) and item not in ignored and not item.startswith('.'):
            folders.append(item)
    return sorted(folders)

def get_folder_status(folder):
    """Определяет Git статус папки"""
    status_res = run_git(['git', 'status', '--porcelain', folder])
    if status_res.stdout.strip():
        return "🔴 [ИЗМЕНЁН]"
    
    diff_res = run_git(['git', 'diff', '--quiet', 'HEAD', 'origin/main', '--', folder])
    if diff_res.returncode != 0:
        return "🟡 [ОБНОВИТЬ]"
    
    return "🟢 [СИНХРОН]"

def fix_large_files_in_gitignore(folder_name):
    """Автоматически находит файлы > 50МБ, добавляет в .gitignore и удаляет из индекса"""
    max_size = 50 * 1024 * 1024  # Снизили до 50МБ для надежности HTTP-загрузки
    detected_large_files = []
    
    for root, dirs, files in os.walk(folder_name):
        for file in files:
            file_path = os.path.join(root, file)
            try:
                if os.path.getsize(file_path) >= max_size:
                    git_path = file_path.replace('\\', '/')
                    detected_large_files.append(git_path)
            except OSError:
                continue
                
    if detected_large_files:
        print(f"⚠️ Обнаружено крупных файлов ({len(detected_large_files)} шт.) > 50 МБ!")
        existing_lines = []
        if os.path.exists('.gitignore'):
            with open('.gitignore', 'r', encoding='utf-8') as f:
                existing_lines = [line.strip() for line in f.readlines()]
                
        updated = False
        with open('.gitignore', 'a', encoding='utf-8') as f:
            for file_to_ignore in detected_large_files:
                if file_to_ignore not in existing_lines:
                    f.write(f"\n{file_to_ignore}")
                    print(f"  [Изолирован от отправки]: {file_to_ignore}")
                    updated = True
                    
        if updated:
            # Срочно выбиваем файлы из индекса Git, чтобы они не весили в коммите
            for file_to_ignore in detected_large_files:
                run_git(['git', 'rm', '--cached', '-r', '-f', file_to_ignore])

def spinner_animation(stop_event, message):
    """Плавная крутящаяся анимация загрузки"""
    spinners = ['|', '/', '-', '\\']
    idx = 0
    while not stop_event.is_set():
        sys.stdout.write(f"\r{message} {spinners[idx]}")
        sys.stdout.flush()
        idx = (idx + 1) % len(spinners)
        time.sleep(0.1)
    sys.stdout.write("\r" + " " * (len(message) + 4) + "\r")
    sys.stdout.flush()

def run_git_with_spinner(command_list, message):
    """Запускает Git-процесс параллельно с графическим спиннером"""
    stop_event = threading.Event()
    spinner_thread = threading.Thread(target=spinner_animation, args=(stop_event, message))
    spinner_thread.start()
    try:
        process = run_git(command_list)
    finally:
        stop_event.set()
        spinner_thread.join()
    return process

def custom_menu(items, statuses):
    """Интерактивное меню мульти-выбора"""
    index = 0
    selections = [False] * len(items)
    while True:
        os.system('cls')
        print("===================================================")
        print("         УЛЬТИМАТИВНЫЙ GIT-МЕНЕДЖЕР UNIVERSITY     ")
        print("  Навигация: СТРЕЛОЧКИ | Выбор/Снятие: ПРОБЕЛ       ")
        print("  [A] - Выбрать все изменённые | [Enter] - Выгрузить")
        print("  Легенда: 🟢 Синхрон | 🟡 Есть коммит | 🔴 Новые лабы")
        print("===================================================")
        
        for i in range(len(items)):
            marker = "[X]" if selections[i] else "[ ]"
            status = statuses[i]
            if i == index:
                print(f" -> {marker} {status} {items[i]}")
            else:
                print(f"    {marker} {status} {items[i]}")
        print("---------------------------------------------------")
        print(" Для выхода из менеджера нажмите ESC")
        
        key = msvcrt.getch()
        if key in (b'\xe0', b'\x00'):
            key = msvcrt.getch()
            if key == b'H':
                index = (index - 1 + len(items)) % len(items)
            elif key == b'P':
                index = (index + 1) % len(items)
        elif key == b' ':
            selections[index] = not selections[index]
        elif key in (b'a', b'A', b'\xd4', b'\xf4'):
            for i in range(len(items)):
                if "🔴" in statuses[i] or "🟡" in statuses[i]:
                    selections[i] = True
        elif key == b'\r':
            selected_folders = [items[i] for i in range(len(items)) if selections[i]]
            if selected_folders:
                return selected_folders
            else:
                print("\n⚠️ Вы не выбрали ни одной папки! Нажмите любую клавишу...")
                msvcrt.getch()
        elif key == b'\x1b':
            return None

def main():
    run_git(["git", "config", "--global", "core.quotepath", "false"])
    while True:
        os.system('cls')
        print("Синхронизирую карту данных с GitHub...")
        run_git(["git", "fetch", "origin", "main"])
        
        folders = get_folders()
        if not folders:
            print("\n[Ошибка] Папки предметов не обнаружены!")
            input("\nНажмите Enter для выхода...")
            break
            
        statuses = [get_folder_status(f) for f in folders]
        selected_folders = custom_menu(folders, statuses)
        
        if selected_folders is None:
            break
            
        os.system('cls')
        print(f" Начинаю пакетную выгрузку объектов ({len(selected_folders)} шт.)")
        print("===================================================")
        
        for folder in selected_folders:
            print(f"\n📂 Обработка папки: {folder}")
            print("---------------------------------------------------")
            
            print("[1/4] Поиск тяжелых файлов...")
            fix_large_files_in_gitignore(folder)
            
            print("[2/4] Индексация...")
            run_git(['git', 'add', folder])
            if os.path.exists('.gitignore'):
                run_git(['git', 'add', '.gitignore'])
            
            # Проверяем, есть ли что коммитить, чтобы не плодить пустые ошибки
            status_check = run_git(['git', 'status', '--porcelain'])
            if not status_check.stdout.strip():
                print("🎉 [ПРОПУСК] Нет изменений для фиксации.")
                continue

            print("[3/4] Сохранение изменений...")
            run_git(['git', 'commit', '-m', f'Авто-синхронизация папки: {folder}'])
            
            process = run_git_with_spinner(
                command_list=['git', 'push', 'origin', 'main'],
                message="[4/4] Отправка на GitHub"
            )
            
            if process.returncode == 0:
                print(f"🎉 [УСПЕХ] Папка '{folder}' успешно синхронизирована!")
            else:
                print(f"❌ [ОШИБКА] Сбой при синхронизации '{folder}'.")
                print(process.stderr.strip())
                print("\n💡 Пробуем сделать откат неудачного коммита, чтобы разблокировать очередь...")
                run_git(['git', 'reset', '--soft', 'HEAD~1'])
                
            print("---------------------------------------------------")
            
        print("\n===================================================")
        print(" Все доступные задачи обработаны!")
        print(" Нажмите любую клавишу для обновления списка статусов...")
        msvcrt.getch()

if __name__ == "__main__":
    main()
