#include <winsock2.h>
#include <iostream>

#include <string>
#include <sstream>
#pragma comment(lib, "ws2_32.lib")

using namespace std;

string numberToWord(int num) {
    switch (num) {
    case 0: return "ноль";
    case 1: return "один";
    case 2: return "два";
    case 3: return "три";
    case 4: return "четыре";
    case 5: return "пять";
    case 6: return "шесть";
    case 7: return "семь";
    case 8: return "восемь";
    case 9: return "девять";
    case 10: return "десять";
    default: return "неизвестное число";
    }
}

int main() {
    setlocale(LC_ALL, "rus");
    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        cerr << "Ошибка инициализации WSAStartup.\n";
        return 1;
    }

    SOCKET serverSocket = socket(AF_INET, SOCK_STREAM, 0);
    if (serverSocket == INVALID_SOCKET) {
        cerr << "Ошибка создания сокета.\n";
        WSACleanup();
        return 1;
    }

    sockaddr_in serverAddr;
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = INADDR_ANY;  // Принимать соединения на любом IP
    serverAddr.sin_port = htons(12345);

    if (bind(serverSocket, (sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR) {
        cerr << "Ошибка привязки сокета к адресу сервера.\n";
        closesocket(serverSocket);
        WSACleanup();
        return 1;
    }

    if (listen(serverSocket, SOMAXCONN) == SOCKET_ERROR) {
        cerr << "Ошибка прослушивания порта на входящие соединения.\n";
        closesocket(serverSocket);
        WSACleanup();
        return 1;
    }

    cout << "Сервер запущен и прослушивает порт 12345...\n";

    while (true) {
        sockaddr_in clientAddr;
        int clientSize = sizeof(clientAddr);
        // Принимаем входящее соединение
        SOCKET clientSocket = accept(serverSocket, (sockaddr*)&clientAddr, &clientSize);
        if (clientSocket == INVALID_SOCKET) {
            cerr << "Ошибка принятия соединения.\n";
            continue;
        }

        cout << "Клиент подключен.\n";

        char buffer[1024];
        int bytesReceived;
        // Цикл приема данных от клиента
        while ((bytesReceived = recv(clientSocket, buffer, sizeof(buffer), 0)) > 0) {
            buffer[bytesReceived] = '\0';
            int number = atoi(buffer); // Преобразуем полученную строку в число
            string response = numberToWord(number);
            send(clientSocket, response.c_str(), response.size() + 1, 0);
        }

        closesocket(clientSocket);
        cout << "Клиент отключен.\n";
    }

    closesocket(serverSocket);
    WSACleanup();
    return 0;
}


