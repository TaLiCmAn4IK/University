#include <iostream>
#include <vector>

using namespace std;

int main() {
    setlocale(LC_ALL, "RUS");
    int n;
    cout << "¬ведите количество дней измерений: ";
    cin >> n;

    vector<int> Massive(n);
    cout << "¬ведите количество осадков дл€ каждого дн€:\n";
    for (int i = 0; i < n; i++) {
        cin >> Massive[i];
    }

    int maxWeek = 0, maxSum = 0;
    for (int i = 0; i <= n - 7; i++) {
        int sum = 0;
        for (int j = i; j < i + 7; j++) {
            sum += Massive[j];
        }
        if (sum > maxSum) {
            maxSum = sum;
            maxWeek = i / 7 + 1;
        }
    }

    cout << "Ќедел€ с наибольшим количеством осадков: " << maxWeek << endl;

    return 0;
}