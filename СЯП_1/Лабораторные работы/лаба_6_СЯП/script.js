// Задание 1 
const numbers = [10, 20, 30, 40, 50];
const [, y] = numbers;
console.log("Первый элемент массива numbers:", y);

// Задание 2
const user = { name: "Иван", age: 25 };
const admin = { admin: true, ...user };
console.log("Объект admin:", admin);

// Задание 3
const store = { user: { id: 1, profile: { name: "Вадим" } }, 
posts: [
    { id: 1, likesCount: 15 },
    { id: 2, likesCount: 25 },
    { id: 3, likesCount: 35 }
  ],
  dialogs: [
    { id: 1, name: "Пользователь 1" },
    { id: 2, name: "Пользователь 2" },
    { id: 3, name: "Пользователь 3" },
    { id: 4, name: "Пользователь 4" }
  ],
  messages: ["Привет", "Как дела?", "До встречи"]
};

let n = { id: 1, name: "Пользователь 1" }
let { id, name } = n;


const { user: { profile: { name: userName } }, posts, dialogs, messages } = store;


console.log("Имя пользователя из вложенного объекта:", userName);

console.log("Вывод likesCount из массива posts:");
posts.forEach(post => {
  console.log(`Пост ID ${post.id}: ${post.likesCount} лайков`);
});

const evenDialogs = dialogs.filter(dialog => dialog.id % 2 === 0);
console.log("Фильтрация массива dialogs — пользователи с четными id:");
console.log(evenDialogs);

const newMessages = messages.map(() => "Hello user");
console.log("Замена всех сообщений на Hello user:");
console.log(newMessages);

// Задание 4
const tasks = [
  { id: 1, title: "HTML&CSS", isDone: true },
  { id: 2, title: "JS", isDone: true },
  { id: 3, title: "ReactJS", isDone: false },
  { id: 4, title: "Rest API", isDone: false },
  { id: 5, title: "GraphQL", isDone: false }
];
const newTask = { id: 6, title: "TypeScript", isDone: false };
const updatedTasks = [...tasks, newTask];
console.log("Обновленный список задач:");
console.log(updatedTasks);

// Задание 5
function sumValues(a, b, c) {
  return a + b + c;
}

const arr = [1, 2, 3];
const result = sumValues(...arr);
console.log(`Сумма элементов массива [${arr}]: ${result}`);
