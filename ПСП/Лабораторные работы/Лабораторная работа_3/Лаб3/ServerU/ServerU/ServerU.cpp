#include "ErrorHandler.h"
#include <iostream>
#include <cstring>
#include <ctime>

#define SERVER_PORT 2000
#define BUFFER_SIZE 1024

int main() {
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);

    bool wsStarted = false;
    SOCKET serverSocket = INVALID_SOCKET;

    try {
        InitWinsock();
        wsStarted = true;
        std::cout << "WSAStartup: OK" << std::endl;

        serverSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
        if (serverSocket == INVALID_SOCKET) {
            ErrorHandler("socket failed");
        }
        std::cout << "socket: OK (UDP)" << std::endl;

        sockaddr_in serverAddr;
        memset(&serverAddr, 0, sizeof(serverAddr));
        serverAddr.sin_family = AF_INET;
        serverAddr.sin_port = htons(SERVER_PORT);
        serverAddr.sin_addr.s_addr = INADDR_ANY;

        if (bind(serverSocket, (sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR) {
            ErrorHandler("bind failed");
        }
        std::cout << "bind: OK (порт 2000)" << std::endl;

   
        int bufSize = 24;
        setsockopt(serverSocket, SOL_SOCKET, SO_RCVBUF,
            (char*)&bufSize, sizeof(bufSize));

        std::cout << "Ожидание клиентов..." << std::endl;

        clock_t startTime = clock();
        long totalReceived = 0;
        long totalSent = 0;
        int msgCount = 0;

        sockaddr_in clientAddr;
        int clientAddrLen = sizeof(clientAddr);
        char buffer[BUFFER_SIZE + 1];

        while (true) {
            clientAddrLen = sizeof(clientAddr);

            Sleep(1000);

            int received = recvfrom(serverSocket, buffer, BUFFER_SIZE, 0,
                (sockaddr*)&clientAddr, &clientAddrLen);

            if (received == SOCKET_ERROR) {
                int err = WSAGetLastError();
                if (err == WSAECONNRESET) {
                    std::cout << "Клиент закрыл сокет, продолжаю ждать..." << std::endl;
                    continue;
                }
                ErrorHandler("recvfrom failed");
            }

            buffer[received] = '\0';
            msgCount++;
            totalReceived += received;

            char clientIP[INET_ADDRSTRLEN];
            inet_ntop(AF_INET, &clientAddr.sin_addr, clientIP, INET_ADDRSTRLEN);

            std::cout << "Принято #" << msgCount
                << " от " << clientIP << ":" << ntohs(clientAddr.sin_port)
                << " (" << received << " байт): " << buffer << std::endl;

            int sent = sendto(serverSocket, buffer, received, 0,
                (sockaddr*)&clientAddr, clientAddrLen);
            if (sent == SOCKET_ERROR) {
                ErrorHandler("sendto failed");
            }
            totalSent += sent;
        }

        clock_t endTime = clock();
        double seconds = double(endTime - startTime) / CLOCKS_PER_SEC;

        std::cout << "Вывод сервер" << std::endl;
        std::cout << "  Сообщений принято: " << msgCount << std::endl;
        std::cout << "  Байт принято:      " << totalReceived << std::endl;
        std::cout << "  Байт отправлено:   " << totalSent << std::endl;
        std::cout << "  Время сессии:      " << seconds << " сек." << std::endl;

        if (closesocket(serverSocket) == SOCKET_ERROR) {
            std::cerr << "closesocket error: "
                << SetErrorMsgText("closesocket", WSAGetLastError()) << std::endl;
        }
        serverSocket = INVALID_SOCKET;
    }
    catch (const std::string& errMsg) {
        std::cerr << "Исключение: " << errMsg << std::endl;
    }
    catch (const std::exception& e) {
        std::cerr << "Стандартное исключение: " << e.what() << std::endl;
    }
    catch (...) {
        std::cerr << "Неизвестное исключение." << std::endl;
    }

    if (serverSocket != INVALID_SOCKET) {
        closesocket(serverSocket);
    }
    if (wsStarted) {
        WSACleanup();
    }

    std::cout << "Сервер завершил работу." << std::endl;
    return 0;
}