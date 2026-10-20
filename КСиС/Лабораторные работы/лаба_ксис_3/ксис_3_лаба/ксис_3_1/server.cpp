#include <iostream>
#include <winsock2.h>
#include <vector>
#include <string>
#include <algorithm>
#include <mutex>

#pragma comment(lib, "ws2_32.lib")

using namespace std;

mutex products_mutex;

struct Product {
    string country;
    string manufacturer;
    string name;
    int quantity;
};

vector<Product> products = {
    {"Россия", "ООО Ромашка", "Ноутбук", 15},
    {"Германия", "BMW AG", "Автомобиль", 3},
    {"Китай", "Huawei", "Смартфон", 25},
    {"США", "Apple", "Планшет", 8},
    {"Япония", "Sony", "Телевизор", 12},
    {"Россия", "ООО Берёзка", "Холодильник", 7},
    {"Китай", "Xiaomi", "Наушники", 30}
};

string viewAllProducts() {
    lock_guard<mutex> lock(products_mutex);
    string result = "Список всех товаров:\n";
    for (size_t i = 0; i < products.size(); i++) {
        result += to_string(i + 1) + ". " + products[i].name +
            " (Страна: " + products[i].country +
            ", Производитель: " + products[i].manufacturer +
            ", Количество: " + to_string(products[i].quantity) + ")\n";
    }
    return result;
}

int findProductByName(const string& name) {
    lock_guard<mutex> lock(products_mutex);
    for (size_t i = 0; i < products.size(); i++) {
        if (products[i].name == name) {
            return i; 
        }
    }
    return -1; 
}

string addProduct(const string& country, const string& manufacturer,
    const string& name, int quantity) {
    lock_guard<mutex> lock(products_mutex);

    for (const auto& product : products) {
        if (product.name == name) {
            return "Ошибка: товар с таким названием уже существует!";
        }
    }


    products.push_back({ country, manufacturer, name, quantity });
    return "Товар '" + name + "' успешно добавлен!";
}

string editProduct(const string& oldName, const string& newCountry,
    const string& newManufacturer, const string& newName,
    int newQuantity) {
    lock_guard<mutex> lock(products_mutex);

    int index = -1;
    for (size_t i = 0; i < products.size(); i++) {
        if (products[i].name == oldName) {
            index = i;
            break;
        }
    }

    if (index == -1) {
        return "Ошибка: товар '" + oldName + "' не найден!";
    }

    if (oldName != newName) {
        for (const auto& product : products) {
            if (product.name == newName && product.name != oldName) {
                return "Ошибка: товар с названием '" + newName + "' уже существует!";
            }
        }
    }

    products[index].country = newCountry;
    products[index].manufacturer = newManufacturer;
    products[index].name = newName;
    products[index].quantity = newQuantity;

    return "Товар '" + oldName + "' успешно обновлен на '" + newName + "'!";
}


string deleteProduct(const string& name) {
    lock_guard<mutex> lock(products_mutex);

    auto it = remove_if(products.begin(), products.end(),
        [&](const Product& p) { return p.name == name; });

    if (it != products.end()) {
        products.erase(it, products.end());
        return "Товар '" + name + "' успешно удален!";
    }

    return "Ошибка: товар '" + name + "' не найден!";
}

string findProductQuantity(const string& name) {
    lock_guard<mutex> lock(products_mutex);

    for (const auto& product : products) {
        if (product.name == name) {
            return to_string(product.quantity);
        }
    }

    return "0"; 
}

DWORD WINAPI ClientHandler(LPVOID param) { 
    SOCKET client_socket = *(SOCKET*)param;
    delete (SOCKET*)param;

    const int BUFFER_SIZE = 1024;
    char buffer[BUFFER_SIZE];

    while (true) {
        
        int bytes_received = recv(client_socket, buffer, BUFFER_SIZE - 1, 0);
        if (bytes_received <= 0) {
            break;
        }

        buffer[bytes_received] = '\0';
        string command(buffer);

        string response;

        if (command.substr(0, 5) == "VIEW:") {
            response = viewAllProducts();
        }
        else if (command.substr(0, 5) == "FIND:") {
            string product_name = command.substr(5);
            response = findProductQuantity(product_name);
        }
        else if (command.substr(0, 4) == "ADD:") {
            size_t pos1 = command.find(':', 4);
            size_t pos2 = command.find(':', pos1 + 1);
            size_t pos3 = command.find(':', pos2 + 1);

            if (pos1 != string::npos && pos2 != string::npos && pos3 != string::npos) {
                string country = command.substr(4, pos1 - 4);
                string manufacturer = command.substr(pos1 + 1, pos2 - pos1 - 1);
                string name = command.substr(pos2 + 1, pos3 - pos2 - 1);
                int quantity = stoi(command.substr(pos3 + 1));

                response = addProduct(country, manufacturer, name, quantity);
            }
            else {
                response = "Ошибка: неверный формат команды ADD";
            }
        }
        else if (command.substr(0, 5) == "EDIT:") {
            size_t pos1 = command.find(':', 5);
            size_t pos2 = command.find(':', pos1 + 1);
            size_t pos3 = command.find(':', pos2 + 1);
            size_t pos4 = command.find(':', pos3 + 1);

            if (pos1 != string::npos && pos2 != string::npos &&
                pos3 != string::npos && pos4 != string::npos) {
                string old_name = command.substr(5, pos1 - 5);
                string new_country = command.substr(pos1 + 1, pos2 - pos1 - 1);
                string new_manufacturer = command.substr(pos2 + 1, pos3 - pos2 - 1);
                string new_name = command.substr(pos3 + 1, pos4 - pos3 - 1);
                int new_quantity = stoi(command.substr(pos4 + 1));

                response = editProduct(old_name, new_country, new_manufacturer, new_name, new_quantity);
            }
            else {
                response = "Ошибка: неверный формат команды EDIT";
            }
        }
        else if (command.substr(0, 7) == "DELETE:") {
            string product_name = command.substr(7);
            response = deleteProduct(product_name);
        }
        else if (command == "EXIT") {
            response = "Сеанс завершен";
            send(client_socket, response.c_str(), (int)response.length(), 0);
            break;
        }
        else {
            response = "Неизвестная команда. Доступные команды: VIEW, FIND:название, ADD, EDIT, DELETE, EXIT";
        }
        send(client_socket, response.c_str(), (int)response.length(), 0);
    }

    closesocket(client_socket);
    return 0;
}

int main() {
    SetConsoleOutputCP(1251);
    SetConsoleCP(1251);

    cout << "Инициализация сервера..." << endl;

    WSADATA wsaData;
    int result = WSAStartup(MAKEWORD(2, 2), &wsaData);
    if (result != 0) {
        cout << "Ошибка инициализации Winsock: " << result << endl;
        return 1;
    }

    SOCKET server_socket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (server_socket == INVALID_SOCKET) {
        cout << "Ошибка создания сокета: " << WSAGetLastError() << endl;
        WSACleanup();
        return 1;
    }

    sockaddr_in server_addr;
    server_addr.sin_family = AF_INET;
    server_addr.sin_addr.s_addr = INADDR_ANY;
    server_addr.sin_port = htons(1280);

    if (bind(server_socket, (sockaddr*)&server_addr, sizeof(server_addr)) == SOCKET_ERROR) {
        cout << "Ошибка привязки сокета: " << WSAGetLastError() << endl;
        closesocket(server_socket);
        WSACleanup();
        return 1;
    }

    if (listen(server_socket, SOMAXCONN) == SOCKET_ERROR) {
        cout << "Ошибка перевода сокета в режим прослушивания: " << WSAGetLastError() << endl;
        closesocket(server_socket);
        WSACleanup();
        return 1;
    }

    cout << "Сервер запущен на порту 1280" << endl;
    cout << "Доступные товары:" << endl;
    for (const auto& product : products) {
        cout << "  - " << product.name << " (количество: " << product.quantity << ")" << endl;
    }
    cout << "Ожидание подключений..." << endl << endl;

    int client_count = 0;

    while (true) {
        sockaddr_in client_addr;
        int client_addr_size = sizeof(client_addr);

        SOCKET client_socket = accept(server_socket, (sockaddr*)&client_addr, &client_addr_size);
        if (client_socket == INVALID_SOCKET) {
            cout << "Ошибка принятия подключения: " << WSAGetLastError() << endl;
            continue;
        }

        client_count++;
        cout << "Клиент " << client_count << " подключен!" << endl;
        SOCKET* client_socket_ptr = new SOCKET;
        *client_socket_ptr = client_socket;

        DWORD thread_id;
        HANDLE thread_handle = CreateThread(NULL, 0, ClientHandler, client_socket_ptr, 0, &thread_id);
        if (thread_handle == NULL) {
            cout << "Ошибка создания потока" << endl;
            closesocket(client_socket);
            delete client_socket_ptr;
        }
        else {
            CloseHandle(thread_handle);
        }
    }

    closesocket(server_socket);
    WSACleanup();
    return 0;
}