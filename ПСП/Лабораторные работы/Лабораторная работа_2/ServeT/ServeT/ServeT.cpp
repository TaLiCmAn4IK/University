#include <iostream>
#include <string>
#include <cstring>
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

int main()
{
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);

    bool   wsStarted = false;
    SOCKET listenSocket = INVALID_SOCKET;

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
        listenSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
        if (listenSocket == INVALID_SOCKET) {
            throw SetErrorMsgText("socket failed", WSAGetLastError());
        }
        cout << "socket: OK" << endl;

        // --- bind ---
        // заполнение адреса
        sockaddr_in serverAddr;
        memset(&serverAddr, 0, sizeof(serverAddr));
        serverAddr.sin_family = AF_INET;
        serverAddr.sin_port = htons(2000);
        serverAddr.sin_addr.s_addr = INADDR_ANY; // принимать подключения на ВСЕХ сетевых интерфейсах

        if (bind(listenSocket, (sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR) {
            throw SetErrorMsgText("bind failed", WSAGetLastError());
        }
        cout << "bind: OK (порт 2000)" << endl;

        // --- listen ---
        if (listen(listenSocket, SOMAXCONN) == SOCKET_ERROR) {
            throw SetErrorMsgText("listen failed", WSAGetLastError());
        }
        cout << "listen: OK. Ожидание клиентов..." << endl;

        int clientCount = 0;
        bool serverRunning = true;

        while (serverRunning)
        {
            // --- accept ---
            sockaddr_in clientAddr;
            int         clientAddrLen = sizeof(clientAddr);
            memset(&clientAddr, 0, sizeof(clientAddr));

            SOCKET clientSocket = accept(listenSocket,
                (sockaddr*)&clientAddr,
                &clientAddrLen);
            if (clientSocket == INVALID_SOCKET) {
                cerr << "accept error: "
                    << SetErrorMsgText("accept", WSAGetLastError()) << endl;
                continue;
            }

            clientCount++;
            cout << endl;
            cout << "Клиент #" << clientCount << " подключён" << endl;
            cout << "  IP клиента:   " << inet_ntoa(clientAddr.sin_addr) << endl;
            cout << "  Порт клиента: " << ntohs(clientAddr.sin_port) << endl;

            clock_t startTime = clock();
            const int BUF_SIZE = 1024;
            char buffer[BUF_SIZE + 1];
            long totalReceived = 0;
            long totalSent = 0;
            int  msgCount = 0;
            bool clientFinished = false;

            // Задание 14: эхо-обмен, окончание по 0-длине 
            while (!clientFinished)
            {
                int received = recv(clientSocket, buffer, BUF_SIZE, 0);
                if (received == SOCKET_ERROR) {
                    cerr << "recv error: "
                        << SetErrorMsgText("recv", WSAGetLastError()) << endl;
                    break;
                }
                if (received == 0) {
                    cout << "Клиент завершил передачу (0 байт) — сигнал окончания." << endl;
                    clientFinished = true;
                    break;
                }

                buffer[received] = '\0';
                msgCount++;
                totalReceived += received;

                // Печатаем каждое 100-е сообщение, чтобы не засорять консоль
                if (msgCount % 100 == 0) {
                    cout << "Принято #" << msgCount
                        << " (" << received << " байт): " << buffer << endl;
                }

                // --- эхо обратно ---
                int sent = send(clientSocket, buffer, received, 0);
                if (sent == SOCKET_ERROR) {
                    cerr << "send error: "
                        << SetErrorMsgText("send", WSAGetLastError()) << endl;
                    break;
                }
                totalSent += sent;
            }

            clock_t endTime = clock();
            double seconds = double(endTime - startTime) / CLOCKS_PER_SEC;

            cout << endl << "===== ВЫВОД СЕРВЕРА =====" << endl;
            cout << "  Сообщений принято: " << msgCount << endl;
            cout << "  Байт принято:      " << totalReceived << endl;
            cout << "  Байт отправлено:   " << totalSent << endl;
            cout << "  Время сессии:      " << seconds << " сек." << endl;

            if (closesocket(clientSocket) == SOCKET_ERROR) {
                cerr << "closesocket(client) error: "
                    << SetErrorMsgText("closesocket", WSAGetLastError()) << endl;
            }

            // Задание 14: сервер завершается после сигнала окончания
            if (clientFinished) {
                cout << "Сервер завершает работу по сигналу клиента." << endl;
                serverRunning = false;
            }
            else {
                cout << "Ожидание следующего клиента..." << endl;
            }
        }
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

    if (listenSocket != INVALID_SOCKET) {
        closesocket(listenSocket);
        listenSocket = INVALID_SOCKET;
    }

    if (wsStarted) {
        if (WSACleanup() == SOCKET_ERROR) {
            cerr << "WSACleanup error: "
                << SetErrorMsgText("WSACleanup", WSAGetLastError()) << endl;
        }
    }

    cout << "Сервер завершил работу." << endl;
    return 0;
}