#include <iostream>
#include <iomanip>
#include <cstdint>

using namespace std;

using Hour = uint8_t;  
using Minute = uint8_t;  
using Second = uint8_t;  

struct TimeFormat {
    Hour   hours;
    Minute minutes;
    Second seconds;

    void normalize() {
        minutes += seconds / 60;
        seconds %= 60;
        hours += minutes / 60;
        minutes %= 60;
        hours %= 24;
    }
};

ostream& operator<<(ostream& os, const TimeFormat& t) {
    os << setw(2) << setfill('0') << int(t.hours) << ':'
        << setw(2) << setfill('0') << int(t.minutes) << ':'
        << setw(2) << setfill('0') << int(t.seconds);
    return os;
}

TimeFormat operator+(TimeFormat a, const TimeFormat& b) {
    a.seconds += b.seconds;
    a.minutes += b.minutes;
    a.hours += b.hours;
    a.normalize();  
    return a;
}

TimeFormat operator-(const TimeFormat& a, const TimeFormat& b) {
    int totalA = a.hours * 3600 + a.minutes * 60 + a.seconds;
    int totalB = b.hours * 3600 + b.minutes * 60 + b.seconds;
    int diff = totalA - totalB;
    if (diff < 0) diff += 24 * 3600;  
    TimeFormat result;
    result.hours = diff / 3600;
    result.minutes = (diff % 3600) / 60;
    result.seconds = diff % 60;
    return result;
}

int main() {
    TimeFormat t1{ 1, 45, 30 };   
    TimeFormat t2{ 2, 20, 50 };   

    cout << "t1 = " << t1 << endl;
    cout << "t2 = " << t2 << endl;


    TimeFormat sum = t1 + t2;
    cout << "t1 + t2 = " << sum << endl;  


    TimeFormat diff = t2 - t1;
    cout << "t2 - t1 = " << diff << endl; 

    return 0;
}
