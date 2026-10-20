#include <winsock2.h>
#include <iostream>
#include <string>
#include <cctype>
#pragma comment(lib, "ws2_32.lib")

using namespace std;

int main() {
    setlocale(LC_ALL, "rus");

    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        cerr << "Ошибка инициализации WSAStartup.\n";
        return 1;
    }

    SOCKET serverSocket = socket(AF_INET, SOCK_DGRAM, 0);
    if (serverSocket == INVALID_SOCKET) {
        cerr << "Ошибка создания сокета.\n";
        WSACleanup();
        return 1;
    }

    sockaddr_in serverAddr;
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = INADDR_ANY;
    serverAddr.sin_port = htons(12345);

    if (bind(serverSocket, (sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR) {
        cerr << "Ошибка привязки сокета.\n";
        closesocket(serverSocket);
        WSACleanup();
        return 1;
    }

    cout << "UDP сервер запущен и прослушивает порт 12345...\n";

    char buffer[1024];
    sockaddr_in clientAddr;
    int clientAddrLen = sizeof(clientAddr);

    while (true) {
        int bytesReceived = recvfrom(serverSocket, buffer, sizeof(buffer), 0,
            (sockaddr*)&clientAddr, &clientAddrLen);

        if (bytesReceived == SOCKET_ERROR) {
            cerr << "Ошибка приема данных.\n";
            continue;
        }

        buffer[bytesReceived] = '\0';
        string receivedStr(buffer);

        cout << "Получена строка от клиента: " << receivedStr << endl;
        cout << "Длина строки: " << receivedStr.length() << endl;

        string resultStr;
        int removedCount = 0;

        if (receivedStr.length() > 15) {
            for (char c : receivedStr) {
                if (!(c >= 'a' && c <= 'z')) {
                    resultStr += c;
                }
                else {
                    removedCount++;
                }
            }
        }
        else {
            resultStr = receivedStr;
        }

        string response = resultStr + "|Удалено символов: " + to_string(removedCount);

        sendto(serverSocket, response.c_str(), response.length(), 0,
            (sockaddr*)&clientAddr, clientAddrLen);

        cout << "Отправлен ответ клиенту: " << response << endl;
    }

    closesocket(serverSocket);
    WSACleanup();
    return 0;
}