#include <iostream>
#include <stack>
#include <string>

using namespace std;

bool checkBrackets(const string& s) {
    stack<char> stk;
    for (char ch : s) {
        if (ch == '(' || ch == '[' || ch == '{') {
            stk.push(ch);
        }
        else if (ch == ')' || ch == ']' || ch == '}') {
            if (stk.empty()) return false;
            char top = stk.top();
            stk.pop();
            if ((ch == ')' && top != '(') ||
                (ch == ']' && top != '[') ||
                (ch == '}' && top != '{')) {
                return false;
            }
        }
    }
    return stk.empty();
}

int main() {
    setlocale(LC_ALL, "Rus");
    cout << "¬ведите строку с символами, скобками и операци€ми:\n";
    string input;
    getline(cin, input);

    if (checkBrackets(input)) {
        cout << "скобки расставлены верно\n";
    }
    else {
        cout << "скобки расставлены не верно\n";
    }

    return 0;
}
