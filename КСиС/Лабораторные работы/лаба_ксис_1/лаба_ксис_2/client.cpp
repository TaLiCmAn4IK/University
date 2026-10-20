#include <winsock2.h>
#include <iostream>
#include <string>
#include <ws2tcpip.h>
#pragma comment(lib, "ws2_32.lib")

using namespace std;

int main() {
    setlocale(LC_ALL, "rus");
    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        cerr << "Ошибка инициализации WSAStartup.\n";
        return 1;
    }

    SOCKET clientSocket = socket(AF_INET, SOCK_STREAM, 0);
    if (clientSocket == INVALID_SOCKET) {
        cerr << "Ошибка создания сокета.\n";
        WSACleanup();
        return 1;
    }

    sockaddr_in serverAddr;
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(12345);
    inet_pton(AF_INET, "127.0.0.1", &serverAddr.sin_addr);

    if (connect(clientSocket, (sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR) {
        cerr << "Ошибка соединения с сервером.";
        closesocket(clientSocket);
        WSACleanup();
        return 1;
    }

    cout << "Подключено к серверу. Вводите числа от 0 до 10 (или 'exit' для выхода):\n";

    string input;
    while (true) {
        cout << "> ";
        getline(cin, input);
        if (input == "exit") break;
        // Проверяем корректность ввода
        try {
            int num = stoi(input);
            if (num < 0 || num > 10) {
                cout << "Пожалуйста, введите число от 0 до 10.\n";
                continue;
            }
        }
        catch (...) {
            cout << "Неверный ввод. Пожалуйста, введите число.\n";
            continue;
        }
        // Отправляем число на сервер
        send(clientSocket, input.c_str(), input.size() + 1, 0);

        char buffer[1024];
        // Получаем ответ от сервера
        int bytesReceived = recv(clientSocket, buffer, sizeof(buffer), 0);
        if (bytesReceived > 0) {
            buffer[bytesReceived] = '\0';
            cout << "Ответ сервера: " << buffer << endl;
        }
    }

    closesocket(clientSocket);
    WSACleanup();
    return 0;
}