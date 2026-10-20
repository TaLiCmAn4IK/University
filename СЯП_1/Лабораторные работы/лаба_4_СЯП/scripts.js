// 1. Управление товарами с помощью Set
const productsSet = new Set();

function addProduct(product) {
    productsSet.add(product);
}

function removeProduct(product) {
    productsSet.delete(product);
}

function hasProduct(product) {
    return productsSet.has(product);
}

function getProductsCount() {
    return productsSet.size;
}

addProduct("Яблоки");
addProduct("Бананы");
addProduct("Апельсины");
console.log("Товары:", productsSet);
console.log("Есть ли яблоки?", hasProduct("Яблоки"));
console.log("Количество товаров:", getProductsCount());
removeProduct("Бананы");
console.log("После удаления бананов:", Array.from(productsSet));

// 2. Список студентов с помощью Set
const studentsSet = new Set();

function createStudent(recordBookId, group, fullName) {
    return { recordBookId: recordBookId, group: group, fullName: fullName };
}

function addStudent(student) {
    studentsSet.add(student);
}

function removeStudentByRecordId(recordBookId) {
    for (let student of studentsSet) {
        if (student.recordBookId === recordBookId) {
            studentsSet.delete(student);
            break;
        }
    }
}

function filterStudentsByGroup(group) {
    return Array.from(studentsSet).filter(function(student) {
        return student.group === group;
    });
}

function sortStudentsByRecordId() {
    return Array.from(studentsSet).sort(function(a, b) {
        return a.recordBookId - b.recordBookId;
    });
}

addStudent(createStudent(101, "Группа А", "Иванов Иван"));
addStudent(createStudent(103, "Группа Б", "Петров Петр"));
addStudent(createStudent(102, "Группа А", "Сидорова Мария"));

console.log("Все студенты:", Array.from(studentsSet));
console.log("Студенты группы А:", filterStudentsByGroup("Группа А"));
console.log("Отсортированные по номеру:", sortStudentsByRecordId());
removeStudentByRecordId(102);
console.log("После удаления студента 102:", Array.from(studentsSet));

// 3. Хранилище товаров с помощью Map
const productsMap = new Map();

function addProductToMap(id, name, quantity, price) {
    productsMap.set(id, { name: name, quantity: quantity, price: price });
}

function removeProductById(id) {
    productsMap.delete(id);
}
+
function removeProductsByName(name) {
    for (let [id, product] of productsMap) {
        if (product.name === name) {
            productsMap.delete(id);
        }
    }
}

function updateProductQuantity(id, newQuantity) {
    const product = productsMap.get(id);
    if (product) {
        product.quantity = newQuantity;
    }
}

function updateProductPrice(id, newPrice) {
    const product = productsMap.get(id);
    if (product) {
        product.price = newPrice;
    }
}

function getProductPositionsCount() {
    return productsMap.size;
}

function getTotalValue() {
    let total = 0;
    for (let product of productsMap.values()) {
        total += product.quantity * product.price;
    }
    return total;
}

addProductToMap(1, "Яблоки", 10, 50);
addProductToMap(2, "Бананы", 5, 30);
addProductToMap(3, "Яблоки", 8, 45);

console.log("Все товары:", Object.fromEntries(productsMap));
console.log("Количество позиций:", getProductPositionsCount());
console.log("Общая стоимость:", getTotalValue());

updateProductQuantity(1, 15);
updateProductPrice(2, 35);
console.log("После изменений:", Object.fromEntries(productsMap));
console.log("Новая общая стоимость:", getTotalValue());

removeProductsByName("Яблоки");
console.log("После удаления яблок:", Object.fromEntries(productsMap));

// 4. Кеширование с помощью WeakMap
const cache = new WeakMap();

function cachedFunction(fn) {
    return function(input) {
        if (!cache.has(input)) {
            console.log("Вычисление результата...");
            const result = fn(input);
            cache.set(input, result);
            return result;
        } else {
            console.log("Результат взят из кеша");
            return cache.get(input);
        }
    };
}

function expensiveCalculation(n) {
    let result = 0;
    for (let i = 0; i < n; i++) {
        result += i;
    }
    return result;
}
const cachedCalculation = cachedFunction(expensiveCalculation);

const input1 = { value: 1000 };
const input2 = { value: 2000 };

console.log("Первый вызов:", cachedCalculation(input1));
console.log("Второй вызов:", cachedCalculation(input1));
console.log("Новый ввод:", cachedCalculation(input2));
console.log("Повтор нового ввода:", cachedCalculation(input2));