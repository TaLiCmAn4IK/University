console.log("\n1. Объединение массивов:");
const arr1 = [1, 2];
const arr2 = [3, 4];
const merged = [arr1, arr2].reduce((acc, arr) => acc.concat(arr), []);
console.log("Массив 1:", arr1);
console.log("Массив 2:", arr2);
console.log("Объединенный:", merged);

console.log("\n2. Сумма вложенного массива:");
const nestedArray = [1, [2, 3], 4];
function sumArray(arr) {
    return arr.reduce((sum, item) => sum + (Array.isArray(item) ? sumArray(item) : item), 0);
}
console.log("Массив:", nestedArray);
console.log("Сумма:", sumArray(nestedArray));

console.log("\n3. Группировка студентов:");
const students = [
    { name: "Иван", age: 18, groupId: 1 },
    { name: "Мария", age: 18, groupId: 1 },
    { name: "Петр", age: 19, groupId: 2 }
];

const grouped = students.reduce((groups, student) => {
    if (student.age > 17) {
        if (!groups[student.groupId]) groups[student.groupId] = [];
        groups[student.groupId].push(student.name);
    }
    return groups;
}, {});

console.log("Студенты:", students);
console.log("Сгруппировано:", grouped);

console.log("\n4. Разность ASCII кодов:");
function asciiDiff(str) {
    const total1 = str.split('').map(char => char.charCodeAt(0)).join('');
    const total2 = total1.replace(/7/g, '1');
    return Number(total1) - Number(total2);
}
const testString = "ABC";
console.log("Строка:", testString);
console.log("Разность:", asciiDiff(testString));

console.log("\n5. Объединение объектов:");
function combineObjects(...objects) {
    return Object.assign({}, ...objects);
}

const obj1 = { a: 1, b: 2 };
const obj2 = { c: 3 };
const combined = combineObjects(obj1, obj2);
console.log("Объект 1:", obj1);
console.log("Объект 2:", obj2);
console.log("Объединенный:", combined);

console.log("\n6. Башня-пирамида:");
function buildTower(n) {
    const tower = [];
    for (let i = 1; i <= n; i++) {
        const stars = '*'.repeat(2 * i - 1);
        const spaces = ' '.repeat(n - i);
        tower.push(spaces + stars + spaces);
    }
    return tower;
}

const tower3 = buildTower(3);
console.log("Башня (3 этажа):");
tower3.forEach(floor => console.log(floor));