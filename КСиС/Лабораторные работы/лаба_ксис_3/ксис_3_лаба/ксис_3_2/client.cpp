#include <iostream>
#include <winsock2.h>
#include <string>
#include <ws2tcpip.h>
#include <vector>
#include <sstream>

#pragma comment(lib, "ws2_32.lib")

using namespace std;

string sendCommand(SOCKET socket, const string& command) {
    send(socket, command.c_str(), (int)command.length(), 0);

    const int BUFFER_SIZE = 1024;
    char buffer[BUFFER_SIZE];
    int bytes_received = recv(socket, buffer, BUFFER_SIZE - 1, 0);

    if (bytes_received > 0) {
        buffer[bytes_received] = '\0';
        return string(buffer);
    }

    return "Ошибка получения ответа от сервера";
}

int main() {
    SetConsoleOutputCP(1251);
    SetConsoleCP(1251);

    cout << "Клиентское приложение" << endl;

    WSADATA wsaData;
    int result = WSAStartup(MAKEWORD(2, 2), &wsaData);
    if (result != 0) {
        cout << "Ошибка инициализации Winsock: " << result << endl;
        return 1;
    }

    while (true) {
        cout << "\n=== МЕНЮ ===" << endl;
        cout << "1. Подключиться к серверу" << endl;
        cout << "2. Выйти" << endl;
        cout << "Выберите действие: ";

        int choice;
        cin >> choice;
        cin.ignore();

        if (choice == 2) {
            break;
        }

        if (choice != 1) {
            cout << "Неверный выбор!" << endl;
            continue;
        }

        SOCKET client_socket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
        if (client_socket == INVALID_SOCKET) {
            cout << "Ошибка создания сокета: " << WSAGetLastError() << endl;
            WSACleanup();
            return 1;
        }

        sockaddr_in server_addr;
        server_addr.sin_family = AF_INET;
        server_addr.sin_port = htons(1280);

        if (inet_pton(AF_INET, "127.0.0.1", &server_addr.sin_addr) <= 0) {
            cout << "Неверный адрес или адрес не поддерживается" << endl;
            closesocket(client_socket);
            WSACleanup();
            return 1;
        }

        cout << "Подключение к серверу..." << endl;

        if (connect(client_socket, (sockaddr*)&server_addr, sizeof(server_addr)) == SOCKET_ERROR) {
            cout << "Ошибка подключения: " << WSAGetLastError() << endl;
            closesocket(client_socket);
            cout << "Проверьте, запущен ли сервер!" << endl;
            continue;
        }

        cout << "Подключение к серверу установлено!" << endl;

        while (true) {
            cout << "\n=== ОПЕРАЦИИ С ТОВАРАМИ ===" << endl;
            cout << "1. Просмотреть все товары" << endl;
            cout << "2. Найти количество товара" << endl;
            cout << "3. Добавить товар" << endl;
            cout << "4. Редактировать товар" << endl;
            cout << "5. Удалить товар" << endl;
            cout << "6. Отключиться от сервера" << endl;
            cout << "Выберите операцию: ";

            int operation;
            cin >> operation;
            cin.ignore();

            string response;

            switch (operation) {
            case 1:
                response = sendCommand(client_socket, "VIEW:");
                cout << "\n" << response << endl;
                break;

            case 2: 
            {
                cout << "Введите наименование товара: ";
                string product_name;
                getline(cin, product_name);
                response = sendCommand(client_socket, "FIND:" + product_name);
                cout << "\nКоличество товара '" << product_name << "': " << response << " единиц" << endl;
            }
            break;

            case 3: 
            {
                string country, manufacturer, name;
                int quantity;

                cout << "Введите страну-изготовитель: ";
                getline(cin, country);
                cout << "Введите фирму-изготовитель: ";
                getline(cin, manufacturer);
                cout << "Введите наименование товара: ";
                getline(cin, name);
                cout << "Введите количество: ";
                cin >> quantity;
                cin.ignore();

                response = sendCommand(client_socket, "ADD:" + country + ":" + manufacturer + ":" + name + ":" + to_string(quantity));
                cout << "\n" << response << endl;
            }
            break;

            case 4: 
            {
                string old_name, new_country, new_manufacturer, new_name;
                int new_quantity;

                cout << "Введите текущее наименование товара: ";
                getline(cin, old_name);
                cout << "Введите новую страну-изготовитель: ";
                getline(cin, new_country);
                cout << "Введите новую фирму-изготовитель: ";
                getline(cin, new_manufacturer);
                cout << "Введите новое наименование товара: ";
                getline(cin, new_name);
                cout << "Введите новое количество: ";
                cin >> new_quantity;
                cin.ignore();

                response = sendCommand(client_socket, "EDIT:" + old_name + ":" + new_country + ":" + new_manufacturer + ":" + new_name + ":" + to_string(new_quantity));
                cout << "\n" << response << endl;
            }
            break;

            case 5:
            {
                string name;
                cout << "Введите наименование товара для удаления: ";
                getline(cin, name);
                response = sendCommand(client_socket, "DELETE:" + name);
                cout << "\n" << response << endl;
            }
            break;

            case 6: 
                response = sendCommand(client_socket, "EXIT");
                cout << "\n" << response << endl;
                closesocket(client_socket);
                goto disconnect;

            default:
                cout << "Неверная операция!" << endl;
                continue;
            }
        }

    disconnect:
        continue;
    }

    WSACleanup();
    cout << "Клиент завершил работу." << endl;
    return 0;
}