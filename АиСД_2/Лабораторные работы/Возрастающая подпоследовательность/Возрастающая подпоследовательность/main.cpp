#include <iostream>
#include <vector>
#include <algorithm>
using namespace std;

int main() {
    setlocale(LC_ALL, "RUS");
    int n;

    cout << "Введите размер последовательности: "; cin >> n;
    vector<int> nums(n);

    for (int i = 0; i < n; i++) {
        cout << "Введите " << i + 1 << " элемент последовательности: "; cin >> nums[i];
    }

    cout << "\nВведенная последовательность: ";
    for (int i = 0; i < n; i++) {
        cout << nums[i] << " ";
    }
    cout << endl;

    // Максимальная возрастающая подпоследовательность
    vector<int> tail;
    vector<int> prev(n, -1);

    for (int i = 0; i < n; i++) {
        int left = 0, right = tail.size();
        while (left < right) {
            int mid = left + (right - left) / 2;
            if (nums[tail[mid]] < nums[i]) {
                left = mid + 1;
            }
            else {
                right = mid;
            }
        }

        if (left == tail.size()) {
            tail.push_back(i);
        }
        else {
            tail[left] = i;
        }

        if (left > 0) {
            prev[i] = tail[left - 1];
        }
    }

    vector<int> lis;
    int idx = tail.back();
    while (idx != -1) {
        lis.push_back(nums[idx]);
        idx = prev[idx];
    }
    reverse(lis.begin(), lis.end());

    cout << "\nМаксимальная возрастающая подпоследовательность: ";
    for (size_t i = 0; i < lis.size(); i++) {
        cout << lis[i] << " ";
    }
    cout << endl;

    cout << "Длина подпоследовательности: " << tail.size() << endl;

    // Максимальная убывающая подпоследовательность
    vector<int> buff(n, 1);
    vector<int> path(n, -1);

    for (int j = 1; j < n; j++) {
        for (int k = 0; k < j; k++) { // j
            if (nums[j] < nums[k]) { // условия убывания 
                if (buff[j] <= buff[k]) { // можем ли увеличить длину убыв. послед
                    buff[j] = buff[k] + 1;
                    path[j] = k;
                }
            }
        }
    }
    // Находим максимальную длину и конечный индекс самой длинной подпоследовательности
    int max_len = 0, end_index = 0;
    for (int i = 0; i < n; i++) {
        if (buff[i] > max_len) {
            max_len = buff[i];
            end_index = i;
        }
    }
    // Восстанавливаем подпоследовательность в обратном порядке
    vector<int> decreasing_seq(max_len);
    int index = max_len - 1;
    for (int i = end_index; i >= 0; i = path[i]) {
        decreasing_seq[index--] = nums[i];
        if (path[i] == -1) break;
    }

    cout << "\nМаксимальная убывающая подпоследовательность: ";
    for (int i = 0; i < max_len; i++) {
        cout << decreasing_seq[i] << " ";
    }
    cout << endl;

    cout << "Длина подпоследовательности: " << max_len << endl;

    return 0;
}