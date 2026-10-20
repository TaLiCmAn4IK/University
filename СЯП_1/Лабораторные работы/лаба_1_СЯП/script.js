"use strict";

// 1
let c = 5;
let name = "Name";
let x = 0;
let double = 0.23;
let result = 1/0;
let answer = true;
let no = null;

console.log("Задание 1:");
console.log(typeof c);
console.log(typeof name);
console.log(typeof x);
console.log(typeof double);
console.log(typeof result);
console.log(typeof answer);
console.log(typeof no);

// 2
let widthA = 45;
let heightA = 21;
let sideB = 5;

let countSquares = Math.floor((widthA * heightA) / (sideB * sideB));
console.log("\nЗадание 2:");
console.log("Количество квадратов:", countSquares);

// 3
let i = 2;
let a = ++i;
let b = i++;
console.log("\nЗадание 3:");
console.log("a =", a, "b =", b, "i =", i,);


// 4
function compare(x, y) {
  return x > y ? `${x} > ${y}` : `${x} <= ${y}`;
}

console.log("\nЗадание 4:");
console.log(compare("Котик", "котик"));
console.log(compare("Котик", "китик"));
console.log(compare("Кот", "Котик"));
console.log(compare("Привет", "Пока"));

console.log(compare(false, 0));
console.log(compare(54, true));
console.log(compare(123, false));
console.log(compare(true, "3"));

console.log(compare(73, "53"));
console.log(compare(3, "5мм"));
console.log(compare(8, "-2"));
console.log(compare(34, "34"));

console.log(compare(null, undefined));

// 5
console.log("\nЗадание 5:");
let Name = "Вадим";
let inputName = prompt("Введите имя:");

if (Name.toLowerCase().includes(inputName.toLowerCase())) {
  alert("Верно! Имя совпало.");
  console.log("Верно! Имя совпало.");
} else {
  alert("Данные не совпадают.");
  console.log("Данные не совпадают.");
}


// 6
console.log("\nЗадание 6:");
let rus = confirm("Сдан экзамен по русскому?");
let math = confirm("Сдан экзамен по математике?");
let eng = confirm("Сдан экзамен по английскому?");

if (rus && math && eng) {
  alert("Перевод на следующий курс!");
  console.log("Перевод на следующий курс!");
} else if (!rus && !math && !eng) {
  alert("Отчисление!");
  console.log("Отчисление!");
} else {
  alert("Пересдача.");
  console.log("Пересдача.");
}


// 7
console.log("\nЗадание 7:");
console.log(true + "5");    
console.log(0 + "5");        
console.log(5 + "мм");       
console.log(8 / Infinity);   
console.log(9 * "\n9");      
console.log(null - 1);       
console.log("5" - 2);        
console.log("5px" - 3);      
console.log(true - 3);       
console.log(7 || 0);  
console.log(2 && 7);    

// 8
console.log("\nЗадание 8:");
for (let n = 1; n <= 10; n++) {
  if (n % 2 === 0) {
    console.log(n + 2);
  } else {
    console.log(n + "мм");
  }
}


// 9
console.log("\nЗадание 9:");
let dayNum = +prompt("Введите номер дня недели (1-7):");

let daysArr = ["пн","вт","ср","чт","пт","сб","вс"];
console.log("Через массив:", daysArr[dayNum - 1]);

let daysObj = {
  1: "пн",
  2: "вт",
  3: "ср",
  4: "чт",
  5: "пт",
  6: "сб",
  7: "вс"
};
console.log("Через объект:", daysObj[dayNum]);

// 10
console.log("\nЗадание 10:");
function joinParams(a = "default", b = "middle", c) {
  return `${a} ${b} ${c}`;
}

let userParam = prompt("Введите третий параметр:");
console.log(joinParams("JS", "Lab", userParam));

// 11
console.log("\nЗадание 11:");

function params(a, b) {
  if (a === b) {
    return 4 * a;
  } else {
    return a * b;
  }
}

const paramsExp = function(a, b) {
  return a === b ? 4 * a : a * b;
};

const paramsArrow = (a, b) => (a === b ? 4 * a : a * b);

console.log(params(5, 5));      
console.log(paramsExp(5, 10));  
console.log(paramsArrow(7, 7)); 
