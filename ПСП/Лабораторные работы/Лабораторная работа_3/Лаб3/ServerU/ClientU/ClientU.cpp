#define _CRT_SECURE_NO_WARNINGS
#include "ErrorHandler.h"
#include <iostream>
#include <string>
#include <cstring>
#include <cstdio>
#include <ctime>

#define SERVER_PORT 2000
#define BUFFER_SIZE 1024

int main(int argc, char* argv[]) {
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);

    const char* serverIP = "10.53.251.21";
    if (argc >= 2) {
        serverIP = argv[1];
    }

    int totalMessages = 1000;
    std::cout << "Введите количество сообщений (max 1000): ";
    char line[64];
    if (std::cin.getline(line, sizeof(line)) && strlen(line) > 0) {
        int v = atoi(line);
        if (v > 0) totalMessages = v;
    }

    bool wsStarted = false;
    SOCKET clientSock = INVALID_SOCKET;

    try {
        InitWinsock();
        wsStarted = true;
        std::cout << "WSAStartup: OK" << std::endl;

        clientSock = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
        if (clientSock == INVALID_SOCKET) {
            ErrorHandler("socket failed");
        }
        std::cout << "socket: OK (UDP)" << std::endl;

        sockaddr_in serverAddr;
        memset(&serverAddr, 0, sizeof(serverAddr));
        serverAddr.sin_family = AF_INET;
        serverAddr.sin_port = htons(SERVER_PORT);
        if (inet_pton(AF_INET, serverIP, &serverAddr.sin_addr) != 1) {
            throw SetErrorMsgText("inet_pton failed", WSAGetLastError());
        }

        std::cout << "Отправка на " << serverIP << ":2000 (UDP)..." << std::endl;

        int serverAddrLen = sizeof(serverAddr);
        char buffer[BUFFER_SIZE + 1];

        clock_t startTime = clock();

        int counter = 1;
        snprintf(buffer, sizeof(buffer), "Hello from Client %d", counter);
        int len = (int)strlen(buffer) + 1;

        if (sendto(clientSock, buffer, len, 0,
            (sockaddr*)&serverAddr, serverAddrLen) == SOCKET_ERROR) {
            ErrorHandler("sendto failed");
        }
        std::cout << "[" << counter << "/" << totalMessages << "] "
            << "отправлено: \"" << buffer << "\"" << std::endl;

        for (int i = 1; i <= totalMessages; ++i) {
            int received = recvfrom(clientSock, buffer, BUFFER_SIZE, 0,
                (sockaddr*)&serverAddr, &serverAddrLen);

            if (received == SOCKET_ERROR) {
                int err = WSAGetLastError();
                if (err == WSAECONNRESET) {
                    std::cerr << "Сервер недоступен (порт закрыт). Попробуйте снова." << std::endl;
                    break;
                }
                ErrorHandler("recvfrom failed");
            }

            buffer[received] = '\0';

            int num = 0;
            if (sscanf(buffer, "Hello from Client %d", &num) == 1) {
                counter = num + 1;
            }
            else {
                counter++;
            }

            std::cout << "[" << i << "/" << totalMessages << "] "
                << "получено: \"" << buffer << "\", "
                << "отправляю: \"Hello from Client " << counter << "\"" << std::endl;

            snprintf(buffer, sizeof(buffer), "Hello from Client %d", counter);
            len = (int)strlen(buffer) + 1;

            if (sendto(clientSock, buffer, len, 0,
                (sockaddr*)&serverAddr, serverAddrLen) == SOCKET_ERROR) {
                ErrorHandler("sendto failed");
            }
        }

        clock_t endTime = clock();
        double seconds = double(endTime - startTime) / CLOCKS_PER_SEC;

        std::cout << "Вывод" << std::endl;
        std::cout << "  Сообщений отправлено: " << totalMessages << std::endl;
        std::cout << "  Время обмена:         " << seconds << " сек." << std::endl;
        if (seconds > 0) {
            std::cout << "  Сообщений в секунду:  "
                << (totalMessages / seconds) << std::endl;
            std::cout << "  Среднее время:        "
                << (seconds * 1000.0 / totalMessages) << " мс" << std::endl;
        }

        if (closesocket(clientSock) == SOCKET_ERROR) {
            std::cerr << "closesocket error: "
                << SetErrorMsgText("closesocket", WSAGetLastError()) << std::endl;
        }
        clientSock = INVALID_SOCKET;
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

    if (clientSock != INVALID_SOCKET) {
        closesocket(clientSock);
    }
    if (wsStarted) {
        WSACleanup();
    }

    std::cout << "Клиент завершил работу." << std::endl;
    return 0;
}