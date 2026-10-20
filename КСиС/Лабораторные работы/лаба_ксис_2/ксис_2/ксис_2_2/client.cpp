#include <winsock2.h>
#include <iostream>
#include <string>
#include <ws2tcpip.h>
#pragma comment(lib, "ws2_32.lib")

using namespace std;

int main() {
    setlocale(LC_ALL, "rus");

    // Инициализация Winsock
    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        cerr << "Ошибка инициализации WSAStartup.\n";
        return 1;
    }

    // Создание UDP сокета
    SOCKET clientSocket = socket(AF_INET, SOCK_DGRAM, 0);
    if (clientSocket == INVALID_SOCKET) {
        cerr << "Ошибка создания сокета.\n";
        WSACleanup();
        return 1;
    }

    // Настройка адреса сервера
    sockaddr_in serverAddr;
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(12345);
    inet_pton(AF_INET, "127.0.0.1", &serverAddr.sin_addr);

    cout << "UDP клиент запущен. Подключение к серверу 127.0.0.1:12345\n";
    cout << "Вводите строки (или 'exit' для выхода):\n";

    string input;
    while (true) {
        cout << "> ";
        getline(cin, input);

        if (input == "exit") break;

        // Отправка данных серверу
        sendto(clientSocket, input.c_str(), input.length(), 0,
            (sockaddr*)&serverAddr, sizeof(serverAddr));

        char buffer[1024];
        sockaddr_in fromAddr;
        int fromLen = sizeof(fromAddr);

        // Получение ответа от сервера
        int bytesReceived = recvfrom(clientSocket, buffer, sizeof(buffer), 0,
            (sockaddr*)&fromAddr, &fromLen);

        if (bytesReceived > 0) {
            buffer[bytesReceived] = '\0';
            cout << "Ответ сервера: " << buffer << endl;
        }
    }

    closesocket(clientSocket);
    WSACleanup();
    return 0;
}