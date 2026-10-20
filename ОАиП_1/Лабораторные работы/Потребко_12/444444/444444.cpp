#include <iostream>
#include <vector>
#include <string>
#include <cmath> // Для математических функций (sin, cos, tan, exp, sqrt, log, ceil, asin, acos, atan)
#include <cstdlib> // Для std::stof и abs

const float M_PI = 3.14159265358979323846f; // Определение числа π

char ArithmeticSigns[]{ '+', '-', '*', '/', '^' };

bool thisIsArithmeticSign(char sign) {
    for (int i = 0; i < std::size(ArithmeticSigns); i++) {
        if (sign == ArithmeticSigns[i]) {
            return true;
        }
    }
    return false;
}

auto ParsingAnExample(const std::string& example) {
    std::vector<std::string> ParsingExample;
    std::string ValueBeforeSign;
    for (size_t i = 0; i < example.length(); i++) {
        if (thisIsArithmeticSign(example[i]) && (i == 0 || example[i - 1] != '(')) {
            ParsingExample.push_back(ValueBeforeSign);
            ParsingExample.push_back(std::string(1, example[i]));
            ValueBeforeSign.clear();
        }
        else {
            ValueBeforeSign += example[i];
            if (i == example.length() - 1) {
                ParsingExample.push_back(ValueBeforeSign);
            }
        }
    }
    return ParsingExample;
}

float StringToFloat(const std::string& str) {
    std::string newStr = str;

    if (newStr.find("sin") != std::string::npos) {
        newStr.erase(0, 4);
        newStr.erase(newStr.length() - 1);
        return sin(std::stof(newStr) * M_PI / 180.0f); // Преобразование градусов в радианы
    }
    else if (newStr.find("cos") != std::string::npos) {
        newStr.erase(0, 4);
        newStr.erase(newStr.length() - 1);
        return cos(std::stof(newStr) * M_PI / 180.0f);
    }
    else if (newStr.find("tan") != std::string::npos) {
        newStr.erase(0, 4);
        newStr.erase(newStr.length() - 1);
        return tan(std::stof(newStr) * M_PI / 180.0f);
    }
    else if (newStr.find("cot") != std::string::npos) {
        newStr.erase(0, 4);
        newStr.erase(newStr.length() - 1);
        return 1.0f / tan(std::stof(newStr) * M_PI / 180.0f);
    }
    if (newStr.find("asin") != std::string::npos) {
        newStr.erase(0, 5); // удаляем "asin("
        newStr.erase(newStr.length() - 1); // удаляем ")"
        float value = std::stof(newStr);
        if (value < -1 || value > 1) {
            throw std::invalid_argument("asin() принимает аргументы в диапазоне от -1 до 1");
        }
        return asin(value) * 180.0f / M_PI; // Результат в градусах
    } // Результат в градусах
    
    else if (newStr.find("acos") != std::string::npos) {
        newStr.erase(0, 5);
        newStr.erase(newStr.length() - 1);
        return acos(std::stof(newStr)) * 180.0f / M_PI;
    }
    else if (newStr.find("atan") != std::string::npos) {
        newStr.erase(0, 5);
        newStr.erase(newStr.length() - 1);
        return atan(std::stof(newStr)) * 180.0f / M_PI;
    }
    else if (newStr.find("acot") != std::string::npos) {
        newStr.erase(0, 5);
        newStr.erase(newStr.length() - 1);
        return (M_PI / 2.0f - atan(std::stof(newStr))) * 180.0f / M_PI; // Арккотангенс
    }
    else if (newStr.find("abs") != std::string::npos) {
        newStr.erase(0, 4);
        newStr.erase(newStr.length() - 1);
        return std::abs(std::stof(newStr));
    }
    else if (newStr.find("sqrt") != std::string::npos) {
        newStr.erase(0, 5);
        newStr.erase(newStr.length() - 1);
        return sqrt(std::stof(newStr));
    }
    else if (newStr.find("ceil") != std::string::npos) {
        newStr.erase(0, 5);
        newStr.erase(newStr.length() - 1);
        return ceil(std::stof(newStr));
    }
    else if (newStr.find("exp") != std::string::npos) {
        newStr.erase(0, 4);
        newStr.erase(newStr.length() - 1);
        return exp(std::stof(newStr));
    }
    else if (newStr.find("log") != std::string::npos) {
        newStr.erase(0, 4);
        newStr.erase(newStr.length() - 1);
        return log(std::stof(newStr));
    }
    else if (newStr.front() == '(' && newStr.back() == ')') {
        newStr.erase(0, 1);
        newStr.erase(newStr.length() - 1);
        return std::stof(newStr);
    }
    else {
        return std::stof(newStr);
    }
}

std::string CalculateWithAction(float x, float y, const std::string& action) {
    float answer = 0;
    if (action == "+") {
        answer = x + y;
    }
    else if (action == "-") {
        answer = x - y;
    }
    else if (action == "^") {
        answer = pow(x, y);
    }
    else if (action == "*") {
        answer = x * y;
    }
    else if (action == "/") {
        answer = x / y;
    }

    std::string Str = std::to_string(answer);
    if (answer < 0) {
        Str.insert(0, "(");
        Str.append(")");
    }
    return Str;
}

int main() {
    setlocale(LC_CTYPE, "Russian");
    std::string input;

    while (true) {
        std::cout << "Введите математическое выражение (или введите 'exit' для выхода): ";
        std::getline(std::cin, input);

        if (input == "exit") {
            break; // Выход из программы
        }

        auto parsedExample = ParsingAnExample(input);

        // Проверка, что хотя бы одно значение и одна операция были распознаны
        if (parsedExample.size() < 3) {
            // Проверка на одиночную функцию
            if (parsedExample.size() == 1) {
                float result = StringToFloat(parsedExample[0]);
                std::cout << "Результат: " << result << std::endl;
                continue; // Переход к следующему вводу
            }
            else {
                std::cout << "Ошибка: Недостаточно данных для вычисления." << std::endl;
                continue; // Переход к следующему вводу
            }
        }

        float x = StringToFloat(parsedExample[0]);
        float y = StringToFloat(parsedExample[2]);
        std::string action = parsedExample[1];

        std::string result = CalculateWithAction(x, y, action);
        std::cout << "Результат: " << result << std::endl;
    }

    return 0;
}