#include <iostream>
#include <string>
#include <cstring>
#include <cstdio>
#include <ctime>

#include <winsock2.h>
#include <ws2tcpip.h>

#pragma comment(lib, "WS2_32.lib")
#pragma warning(disable:4996)

using namespace std;

string GetErrorMsgText(int code)
{
    string msgText;
    switch (code)
    {
    case WSAEINTR:              msgText = "Работа функции прервана"; break;
    case WSAEACCES:             msgText = "Разрешение отвергнуто"; break;
    case WSAEFAULT:             msgText = "Ошибочный адрес"; break;
    case WSAEINVAL:             msgText = "Ошибка в аргументе"; break;
    case WSAEMFILE:             msgText = "Открыто слишком много файлов"; break;
    case WSAEWOULDBLOCK:        msgText = "Ресурс временно недоступен"; break;
    case WSAEINPROGRESS:        msgText = "Операция в процессе"; break;
    case WSAEALREADY:           msgText = "Операция уже выполняется"; break;
    case WSAENOTSOCK:           msgText = "Сокет задан неправильно"; break;
    case WSAEDESTADDRREQ:       msgText = "Требуется адрес расположения"; break;
    case WSAEMSGSIZE:           msgText = "Сообщение слишком длинное"; break;
    case WSAEPROTOTYPE:         msgText = "Неправильный тип протокола для сокета"; break;
    case WSAENOPROTOOPT:        msgText = "Ошибка в опции протокола"; break;
    case WSAEPROTONOSUPPORT:    msgText = "Протокол не поддерживается"; break;
    case WSAESOCKTNOSUPPORT:    msgText = "Тип сокета не поддерживается"; break;
    case WSAEOPNOTSUPP:         msgText = "Операция не поддерживается"; break;
    case WSAEPFNOSUPPORT:       msgText = "Тип протоколов не поддерживается"; break;
    case WSAEAFNOSUPPORT:       msgText = "Тип адресов не поддерживается протоколом"; break;
    case WSAEADDRINUSE:         msgText = "Адрес уже используется"; break;
    case WSAEADDRNOTAVAIL:      msgText = "Запрошенный адрес не может быть использован"; break;
    case WSAENETDOWN:           msgText = "Сеть отключена"; break;
    case WSAENETUNREACH:        msgText = "Сеть не достижима"; break;
    case WSAENETRESET:          msgText = "Сеть разорвала соединение"; break;
    case WSAECONNABORTED:       msgText = "Программный отказ связи"; break;
    case WSAECONNRESET:         msgText = "Связь не восстановлена"; break;
    case WSAENOBUFS:            msgText = "Не хватает памяти для буферов"; break;
    case WSAEISCONN:            msgText = "Сокет уже подключен"; break;
    case WSAENOTCONN:           msgText = "Сокет не подключен"; break;
    case WSAESHUTDOWN:          msgText = "Нельзя выполнить send: сокет завершил работу"; break;
    case WSAETIMEDOUT:          msgText = "Закончился отведенный интервал времени"; break;
    case WSAECONNREFUSED:       msgText = "Соединение отклонено"; break;
    case WSAEHOSTDOWN:          msgText = "Хост в неработоспособном состоянии"; break;
    case WSAEHOSTUNREACH:       msgText = "Нет маршрута для хоста"; break;
    case WSAEPROCLIM:           msgText = "Слишком много процессов"; break;
    case WSASYSNOTREADY:        msgText = "Сеть не доступна"; break;
    case WSAVERNOTSUPPORTED:    msgText = "Данная версия недоступна"; break;
    case WSANOTINITIALISED:     msgText = "Не выполнена инициализация WS2_32.dll"; break;
    case WSAEDISCON:            msgText = "Выполняется отключение"; break;
    case WSATYPE_NOT_FOUND:     msgText = "Класс не найден"; break;
    case WSAHOST_NOT_FOUND:     msgText = "Хост не найден"; break;
    case WSATRY_AGAIN:          msgText = "Неавторизованный хост не найден"; break;
    case WSANO_RECOVERY:        msgText = "Неопределенная ошибка"; break;
    case WSANO_DATA:            msgText = "Нет записи запрошенного типа"; break;
    case WSASYSCALLFAILURE:     msgText = "Аварийное завершение системного вызова"; break;
    default:                    msgText = "Unknown error"; break;
    }
    return msgText;
}

string SetErrorMsgText(const string& msgText, int code)
{
    return msgText + " : " + GetErrorMsgText(code) + " (" + to_string(code) + ")";
}

int main(int argc, char* argv[])
{
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);

    // Задание 12: IP сервера можно передать аргументом 
    const char* serverIP = "10.200.135.121";   // по умолчанию — локальная петля
    if (argc >= 2) {
        serverIP = argv[1];
    }

    // Задание 13/14: количество сообщений вводится с консоли
    int totalMessages = 1000;
    cout << "Введите количество сообщений (Enter = 1000): ";
    char line[64];
    if (cin.getline(line, sizeof(line)) && strlen(line) > 0) {
        int v = atoi(line);
        if (v > 0) totalMessages = v;
    }

    bool   wsStarted = false;
    SOCKET clientSock = INVALID_SOCKET;

    try
    {
        // --- WSAStartup ---
        WSADATA wsaData;
        int wsaInitResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
        if (wsaInitResult != 0) {
            throw SetErrorMsgText("WSAStartup failed", wsaInitResult);
        }
        wsStarted = true;
        cout << "WSAStartup: OK" << endl;

        // --- socket ---
        clientSock = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
        if (clientSock == INVALID_SOCKET) {
            throw SetErrorMsgText("socket failed", WSAGetLastError());
        }
        cout << "socket: OK" << endl;

        // --- connect --- 
        sockaddr_in serverAddr; // структура адреса для IPv4
        memset(&serverAddr, 0, sizeof(serverAddr));
        serverAddr.sin_family = AF_INET;
        serverAddr.sin_port = htons(2000);
        serverAddr.sin_addr.s_addr = inet_addr(serverIP);

        cout << "Подключение к " << serverIP << ":2000..." << endl;
        if (connect(clientSock, (sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR) {
            throw SetErrorMsgText("connect failed", WSAGetLastError());
        }
        cout << "connect: OK" << endl;

        const int BUF_SIZE = 1024;
        char buffer[BUF_SIZE + 1];

        clock_t startTime = clock();

        // Задание 14: ровно totalMessages обменов, без +1
        int counter = 1;
        int successfulExchanges = 0;

        for (int i = 1; i <= totalMessages; ++i)
        {
            // --- отправка ---
            snprintf(buffer, sizeof(buffer), "Hello from Client %d", counter);
            int len = (int)strlen(buffer) + 1;

            if (send(clientSock, buffer, len, 0) == SOCKET_ERROR) { // send — отправка данных по TCP-соединению
                throw SetErrorMsgText("send failed", WSAGetLastError());
            }

            // --- приём эха ---
            int received = recv(clientSock, buffer, BUF_SIZE, 0);
            if (received == SOCKET_ERROR) {
                throw SetErrorMsgText("recv failed", WSAGetLastError());
            }
            if (received == 0) {
                cout << "Сервер закрыл соединение на сообщении #" << i << endl;
                break;
            }
            buffer[received] = '\0';

            // --- парсим счётчик из эха ---
            int num = 0;
            if (sscanf(buffer, "Hello from Client %d", &num) == 1) {
                counter = num + 1;
            }
            else {
                counter++;
            }

            successfulExchanges++;

            // Печатаем не каждое сообщение, а каждое 100-е
            if (i % 100 == 0 || i == totalMessages) {
                cout << "[" << i << "/" << totalMessages << "] "
                    << "получено: \"" << buffer << "\", "
                    << "отправлено: \"Hello from Client " << counter << "\"" << endl;
            }
        }

        // Сигнал окончания — сообщение нулевой длины
        send(clientSock, "", 0, 0);
        shutdown(clientSock, SD_SEND); // корректно закрываем отправляющую сторону TCP

        clock_t endTime = clock();
        double seconds = double(endTime - startTime) / CLOCKS_PER_SEC;

        // Итоговая статистика 
        cout << endl << "===== ВЫВОД КЛИЕНТА =====" << endl;
        cout << "  Обменов выполнено:    " << successfulExchanges << endl;
        cout << "  Время обмена:         " << seconds << " сек." << endl;
        if (seconds > 0) {
            cout << "  Сообщений в секунду:  "
                << (successfulExchanges / seconds) << endl;
            cout << "  Среднее время:        "
                << (seconds * 1000.0 / successfulExchanges) << " мс" << endl;
        }

        if (closesocket(clientSock) == SOCKET_ERROR) {
            cerr << "closesocket error: "
                << SetErrorMsgText("closesocket", WSAGetLastError()) << endl;
        }
        clientSock = INVALID_SOCKET;
    }
    catch (const string& errMsg)
    {
        cerr << "Исключение: " << errMsg << endl;
    }
    catch (const exception& e)
    {
        cerr << "Стандартное исключение: " << e.what() << endl;
    }
    catch (...)
    {
        cerr << "Неизвестное исключение." << endl;
    }

    if (clientSock != INVALID_SOCKET) {
        closesocket(clientSock);
        clientSock = INVALID_SOCKET;
    }

    if (wsStarted) {
        if (WSACleanup() == SOCKET_ERROR) {
            cerr << "WSACleanup error: "
                << SetErrorMsgText("WSACleanup", WSAGetLastError()) << endl;
        }
    }

    cout << "Клиент завершил работу." << endl;
    return 0;
}