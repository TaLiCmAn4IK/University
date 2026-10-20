// 1. Функция basicOperation
function basicOperation(operation, value1, value2) {
  if (operation === '+') return value1 + value2;
  else if (operation === '-') return value1 - value2;
  else if (operation === '*') return value1 * value2;
  else if (operation === '/') return value1 / value2;
}
console.log(basicOperation('+', 5, 10));
console.log(basicOperation('-', 5, 10));
console.log(basicOperation('*', 5, 10));
console.log(basicOperation('/', 5, 10));

// 2. Сумма кубов чисел до n
function sumOfCubes(n) {
  let sum = 0;
  for (let i = 1; i <= n; i++) {
    sum += i ** 3;
  }
  return sum;
}
console.log(sumOfCubes(3)); // 36 (1^3+2^3+3^3)

// 3. Среднее арифметическое массива
function average(numbers) {
    let sum = 0;
    for (let i = 0; i < numbers.length; i++) {
        sum = sum + numbers[i];
    }
    return sum / numbers.length;
}
console.log(average([1, 2, 3, 4])); // 2.5

// 4. Переворот строки, оставить только буквы
function reverseLettersOnly(str) {
  let result = '';
  for (let i = str.length - 1; i >= 0; i--) {
    let char = str[i];
    if ((char >= 'a' && char <= 'z') || (char >= 'A' && char <= 'Z')) {
      result += char;
    }
  }
  return result; 
}
console.log(reverseLettersOnly("JavaScript"));
console.log(reverseLettersOnly("JavaScr53? ipt")); 

// 5. Повторение строки n раз
repeatString();
function repeatString(n, s) {
  return s.repeat(n);
  console.log(repeatString(3, "Hi"));
}

// 6. Разница массивов строк
function arrayDiff(arr1, arr2) {
  return arr1.filter(item => !arr2.includes(item));
}
console.log(arrayDiff(["a", "b", "c"], ["b"])); 

// Function Expression - функция присвоенная переменной
let sayHello = function() {
    console.log("Привет!");
};

sayHello(); // Вызов функции

