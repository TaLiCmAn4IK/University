// Задача 1: Объект person
console.log('=== Задача 1 ===');
const person = {
    name: 'Иван',
    age: 25,
    greet() {
        console.log(`Привет, меня зовут ${this.name}`);
    },
    ageAfterYears(years) {
        return this.age + years;
    }
};
person.greet();
console.log(`Возраст через 5 лет: ${person.ageAfterYears(5)}`);

// Задача 2: Объект car
console.log('\n=== Задача 2 ===');
const car = {
    model: 'Tesla Model 3',
    year: 2022,
    getInfo() {
        console.log(`Модель: ${this.model}, Год выпуска: ${this.year}`);
    }
};
car.getInfo();

// Задача 3: Функция-конструктор Book
console.log('\n=== Задача 3 ===');
function Book(title, author) {
    this.title = title;
    this.author = author;
    this.getTitle = function() {
        return this.title;
    };
    this.getAuthor = function() {
        return this.author;
    };
}
const book = new Book('1984', 'Джордж Оруэлл');
const book_1 = new Book('1984', 'Джордж Оруэлл');
console.log(`Название: ${book_1.getTitle()}`);
console.log(`Название: ${book.getTitle()}`);
console.log(`Автор: ${book.getAuthor()}`);

// Задача 4: Объект team
console.log('\n=== Задача 4 ===');
const team = {
    players: ['Алексей', 'Мария', 'Игорь'],
    showPlayers() {
        this.players.forEach(function(player) {
            console.log(`Игрок: ${player}`);
        }, this);
    }
};
team.showPlayers();

// Задача 5: Модуль counter
console.log('\n=== Задача 5 ===');
const counter = (function() {
    let count = 0;
    return {
        increment() {
            return ++count;
        },
        decrement() {
            return --count;
        },
        getCount() {
            return count;
        }
    };
})();
console.log(counter.increment());
console.log(counter.increment());
console.log(counter.decrement());
console.log(counter.getCount());

// Задача 6: Объект item с изменением дескрипторов
console.log('\n=== Задача 6 ===');
let item = {};
Object.defineProperty(item, 'price', {
    value: 100,
    writable: true,
    configurable: true,
    enumerable: true
});
item.price = 200;
console.log(`Цена после изменения: ${item.price}`);
Object.defineProperty(item, 'price', {
    writable: false,
    configurable: false
});
console.log(`Цена после блокировки: ${item.price}`);

// Задача 7: Объект circle с геттером и сеттером
console.log('\n=== Задача 7 ===');
const circle = {
    _radius: 5,
    get area() {
        return Math.PI * this._radius ** 2;
    },
    get radius() {
        return this._radius;
    },
    set radius(value) {
        if (value > 0) {
            this._radius = value;
        } else {
            console.log('Радиус должен быть положительным');
        }
    }
};
console.log(`Площадь круга (r=5): ${circle.area.toFixed(2)}`);
circle.radius = 10;
console.log(`Площадь круга (r=10): ${circle.area.toFixed(2)}`);

// Задача 8: Объект car с неизменяемыми свойствами
console.log('\n=== Задача 8 ===');
let car2 = {};
Object.defineProperties(car2, {
    make: {
        value: 'Passat',
        writable: true,
        configurable: true,
        enumerable: true
    },
    model: {
        value: 'CC',
        writable: true,
        configurable: true,
        enumerable: true
    },
    year: {
        value: 2020,
        writable: true,
        configurable: true,
        enumerable: true
    }
});
Object.keys(car2).forEach(key => {
    Object.defineProperty(car2, key, {
        writable: false,
        configurable: false
    });
});
console.log(`Марка: ${car2.make}, Модель: ${car2.model}, Год: ${car2.year}`);

// Задача 9: Массив с геттером sum
console.log('\n=== Задача 9 ===');
let arr = [10, 20, 30];
Object.defineProperty(arr, 'sum', {
    get() {
        return this.reduce((acc, val) => acc + val, 0);
    },
    enumerable: false,
    configurable: false
});
console.log(`Сумма элементов массива: ${arr.sum}`);

// Задача 10: Объект rectangle
console.log('\n=== Задача 10 ===');
const rectangle = {
    _width: 10,
    _height: 5,
    get area() {
        return this._width * this._height;
    },
    get width() {
        return this._width;
    },
    set width(value) {
        if (value > 0) this._width = value;
    },
    get height() {
        return this._height;
    },
    set height(value) {
        if (value > 0) this._height = value;
    }
};
console.log(`Площадь прямоугольника (10x5): ${rectangle.area}`);
rectangle.width = 15;
rectangle.height = 10;
console.log(`Площадь прямоугольника (15x10): ${rectangle.area}`);

// Задача 11: Объект user с геттером и сеттером полного имени
console.log('\n=== Задача 11 ===');
const user = {
    firstName: 'Иван',
    lastName: 'Иванов',
    get fullName() {
        return `${this.firstName} ${this.lastName}`;
    },
    set fullName(value) {
        const parts = value.split(' ');
        if (parts.length === 2) {
            this.firstName = parts[0];
            this.lastName = parts[1];
        } else {
            console.log('Введите имя и фамилию через пробел');
        }
    }
};
console.log(`Полное имя: ${user.fullName}`);
user.fullName = 'Петр Петров';
console.log(`Новое имя: ${user.firstName}`);
console.log(`Новая фамилия: ${user.lastName}`);
console.log(`Полное имя после изменения: ${user.fullName}`);


