// 1. Простой объект user
type User = {
    name: string;
    age: number;
};

// 2. Массив чисел
type Numbers = number[];

// 3. Объект с вложенным location
type UserLocation = {
    city: string;
    country: string;
};

type User1 = {
    name: string;
    age: number;
    location: UserLocation;
};

// 4. Объект с массивом навыков
type User2 = {
    name: string;
    age: number;
    skills: string[];
};

// 5. Массив объектов Array
type ArrayItem = {
    id: number;
    name: string;
    group: number;
};

type ArrayType = ArrayItem[];

// 6. Типы для user4
type Exams1 = {
    maths: boolean;
    programming: boolean;
};

type Studies4 = {
    university: string;
    speciality: string;
    year: number;
    exams: Exams1;
};

type User4 = {
    name: string;
    age: number;
    studies: Studies4;
};

// 7. Типы для user5
type Department = {
    faculty: string;
    group: number;
};

type Exam5 = {
    maths?: boolean;
    programming?: boolean;
    mark: number;
};

type Studies5 = {
    university: string;
    speciality: string;
    year: number;
    department: Department;
    exams: Exam5[];
};

type User5 = {
    name: string;
    age: number;
    studies: Studies5;
};

// 8. Типы для user6
type Professor = {
    name: string;
    degree: string;
};

type Exam6 = {
    maths?: boolean;
    programming?: boolean;
    mark: number;
    professor: Professor;
};

type Studies6 = {
    university: string;
    speciality: string;
    year: number;
    department: Department;
    exams: Exam6[];
};

type User6 = {
    name: string;
    age: number;
    studies: Studies6;
};

// 9. Типы для user7
type Article = {
    title: string;
    pagesNumber: number;
};

type ProfessorWithArticles = {
    name: string;
    degree: string;
    articles: Article[];
};

type Exam7 = {
    maths?: boolean;
    programming?: boolean;
    mark: number;
    professor: ProfessorWithArticles;
};

type Studies7 = {
    university: string;
    speciality: string;
    year: number;
    department: Department;
    exams: Exam7[];
};

// Пересечение (&) объединяет оба типа
type User77 = User7 & {
    // можно добавить еще поля
};

type User7 = {
    name: string;
    age: number;
    studies: Studies7;
};

//interface IPerson {
//    name: string;
//    age: number;
//}

//interface IUser7 extends IPerson {
//   studies: Studies7;  // добавляем новое поле
//}

// 10. Типы для store
type Post = {
    id: number;
    message: string;
    likesCount: number;
};

type ProfilePage = {
    posts: Post[];
    newPostText: string;
};

type Dialog = {
    id: number;
    name: string;
};

type Message = {
    id: number;
    message: string;
};

type DialogsPage = {
    dialogs: Dialog[];
    messages: Message[];
};

type State = {
    profilePage: ProfilePage;
    dialogsPage: DialogsPage;
    sidebar: any[];
};

type Store = {
    state: State;
};

// Исходные объекты из 8 лр

// 1
let user: User = {
    name: 'Masha',
    age: 21
};

// 2
let numbers: Numbers = [1, 2, 3];

// 3
let user1: User1 = {
    name: 'Masha',
    age: 23,
    location: {
        city: 'Minsk',
        country: 'Belarus'
    }
};

// 4
let user2: User2 = {
    name: 'Masha',
    age: 28,
    skills: ['HTML', 'CSS', 'JavaScript', 'React']
};

// 5
const array: ArrayType = [
    { id: 1, name: 'Vasya', group: 10 },
    { id: 2, name: 'Ivan', group: 11 },
    { id: 3, name: 'Masha', group: 12 },
    { id: 4, name: 'Petya', group: 10 },
    { id: 5, name: 'Kira', group: 11 }
];

// 6
let user4: User4 = {
    name: 'Masha',
    age: 19,
    studies: {
        university: 'BSTU',
        speciality: 'designer',
        year: 2020,
        exams: {
            maths: true,
            programming: false
        }
    }
};

// 7
let user5: User5 = {
    name: 'Masha',
    age: 22,
    studies: {
        university: 'BSTU',
        speciality: 'designer',
        year: 2020,
        department: {
            faculty: 'FIT',
            group: 10
        },
        exams: [
            { maths: true, mark: 8 },
            { programming: true, mark: 4 }
        ]
    }
};

// 8
let user6: User6 = {
    name: 'Masha',
    age: 21,
    studies: {
        university: 'BSTU',
        speciality: 'designer',
        year: 2020,
        department: {
            faculty: 'FIT',
            group: 10
        },
        exams: [
            {
                maths: true,
                mark: 8,
                professor: {
                    name: 'Ivan Ivanov',
                    degree: 'PhD'
                }
            },
            {
                programming: true,
                mark: 10,
                professor: {
                    name: 'Petr Petrov',
                    degree: 'PhD'
                }
            }
        ]
    }
};

// 9
let user7: User7 = {
    name: 'Masha',
    age: 20,
    studies: {
        university: 'BSTU',
        speciality: 'designer',
        year: 2020,
        department: {
            faculty: 'FIT',
            group: 10
        },
        exams: [
            {
                maths: true,
                mark: 8,
                professor: {
                    name: 'Ivan Petrov',
                    degree: 'PhD',
                    articles: [
                        { title: "About HTML", pagesNumber: 3 },
                        { title: "About CSS", pagesNumber: 5 },
                        { title: "About JavaScript", pagesNumber: 1 }
                    ]
                }
            },
            {
                programming: true,
                mark: 10,
                professor: {
                    name: 'Petr Ivanov',
                    degree: 'PhD',
                    articles: [
                        { title: "About HTML", pagesNumber: 3 },
                        { title: "About CSS", pagesNumber: 5 },
                        { title: "About JavaScript", pagesNumber: 1 }
                    ]
                }
            }
        ]
    }
};

let user77: User77 = {
    name: 'Masha',
    age: 20,
    studies: {
        university: 'BSTU',
        speciality: 'designer',
        year: 2020,
        department: {
            faculty: 'FIT',
            group: 10
        },
        exams: [
            {
                maths: true,
                mark: 8,
                professor: {
                    name: 'Ivan Petrov',
                    degree: 'PhD',
                    articles: [
                        { title: "About HTML", pagesNumber: 3 },
                        { title: "About CSS", pagesNumber: 5 },
                        { title: "About JavaScript", pagesNumber: 1 }
                    ]
                }
            },
            {
                programming: true,
                mark: 10,
                professor: {
                    name: 'Petr Ivanov',
                    degree: 'PhD',
                    articles: [
                        { title: "About HTML", pagesNumber: 3 },
                        { title: "About CSS", pagesNumber: 5 },
                        { title: "About JavaScript", pagesNumber: 1 }
                    ]
                }
            }
        ]
    }
};

// 10
let store: Store = {
    state: {
        profilePage: {
            posts: [
                { id: 1, message: 'Hi', likesCount: 12 },
                { id: 2, message: 'By', likesCount: 1 }
            ],
            newPostText: 'About me'
        },
        dialogsPage: {
            dialogs: [
                { id: 1, name: 'Valera' },
                { id: 2, name: 'Andrey' },
                { id: 3, name: 'Sasha' },
                { id: 4, name: 'Viktor' }
            ],
            messages: [
                { id: 1, message: 'hi' },
                { id: 2, message: 'hi hi' },
                { id: 3, message: 'hi hi hi' }
            ]
        },
        sidebar: []
    }
};

// Глубокое копирование

// 1
let userCopy: User = { ...user };

// 2
let numbersCopy: Numbers = [...numbers];

// 3
let user1Copy: User1 = {
    ...user1,
    location: { ...user1.location }
};

// 4
let user2Copy: User2 = {
    ...user2,
    skills: [...user2.skills]
};

// 5
let arrayCopy: ArrayType = array.map(item => ({ ...item }));

// 6
let user4Copy: User4 = {
    ...user4,
    studies: {
        ...user4.studies,
        exams: { ...user4.studies.exams }
    }
};

// 7
let user5Copy: User5 = {
    ...user5,
    studies: {
        ...user5.studies,
        department: { ...user5.studies.department },
        exams: user5.studies.exams.map(exam => ({ ...exam }))
    }
};

// 8
let user6Copy: User6 = {
    ...user6,
    studies: {
        ...user6.studies,
        department: { ...user6.studies.department },
        exams: user6.studies.exams.map(exam => ({
            ...exam,
            professor: { ...exam.professor }
        }))
    }
};


// 9
let user7Copy: User7 = {
    ...user7,
    studies: {
        ...user7.studies,
        department: { ...user7.studies.department },
        exams: user7.studies.exams.map(exam => ({
            ...exam,
            professor: {
                ...exam.professor,
                articles: exam.professor.articles.map(article => ({ ...article }))
            }
        }))
    }
};

// 10
let storeCopy: Store = {
    ...store,
    state: {
        ...store.state,
        profilePage: {
            ...store.state.profilePage,
            posts: store.state.profilePage.posts.map(post => ({ ...post }))
        },
        dialogsPage: {
            ...store.state.dialogsPage,
            dialogs: store.state.dialogsPage.dialogs.map(dialog => ({ ...dialog })),
            messages: store.state.dialogsPage.messages.map(message => ({ ...message }))
        },
        sidebar: [...store.state.sidebar]
    }
};

// Задания 2-5

// Задание 2: изменить group на 12 и оценку по программированию на 10
user5Copy.studies.department.group = 12;
user5Copy.studies.exams[1].mark = 10;

// Задание 3: изменить имя преподавателя
user6Copy.studies.exams[1].professor.name = 'Сергей Сергеев';

// Задание 4: изменить количество страниц для статьи "About CSS"
user7Copy.studies.exams[1].professor.articles[1].pagesNumber = 3;

// Задание 5: заменить все сообщения на "Hello"
store.state.profilePage.posts[0].message = 'Hello';
store.state.profilePage.posts[1].message = 'Hello';
store.state.dialogsPage.messages[0].message = 'Hello';
store.state.dialogsPage.messages[1].message = 'Hello';
store.state.dialogsPage.messages[2].message = 'Hello';

// Вывод в консоль

console.log('='.repeat(60));
console.log('ЛАБОРАТОРНАЯ РАБОТА №1. TypeScript');
console.log('='.repeat(60));

console.log('\n ЗАДАНИЕ 2: user5Copy изменён');
console.log('group:', user5Copy.studies.department.group); // 12
console.log('programming mark:', user5Copy.studies.exams[1].mark); // 10

console.log('\n ЗАДАНИЕ 3: user6Copy изменён');
console.log('professor name:', user6Copy.studies.exams[1].professor.name); // Сергей Сергеев

console.log('\n ЗАДАНИЕ 4: user7Copy изменён');
console.log('pagesNumber for "About CSS":', user7Copy.studies.exams[1].professor.articles[1].pagesNumber); // 3

console.log('\n ЗАДАНИЕ 5: store изменён');
console.log('posts messages:', store.state.profilePage.posts.map(p => p.message)); // ["Hello", "Hello"]
console.log('dialogs messages:', store.state.dialogsPage.messages.map(m => m.message)); // ["Hello", "Hello", "Hello"]

console.log('\n ПРОВЕРКА: Оригиналы не изменились');
console.log('user5 original group:', user5.studies.department.group); // 10
console.log('user6 original professor:', user6.studies.exams[1].professor.name); // Petr Petrov
console.log('user7 original pages:', user7.studies.exams[1].professor.articles[1].pagesNumber); // 5