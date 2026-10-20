"use strict";
let user = {
    name: 'Masha',
    age: 21
};
let numbers = [1, 2, 3];
let user1 = {
    name: 'Masha',
    age: 23,
    location: {
        city: 'Minsk',
        country: 'Belarus'
    }
};
let user2 = {
    name: 'Masha',
    age: 28,
    skills: ['HTML', 'CSS', 'JavaScript', 'React']
};
const array = [
    { id: 1, name: 'Vasya', group: 10 },
    { id: 2, name: 'Ivan', group: 11 },
    { id: 3, name: 'Masha', group: 12 },
    { id: 4, name: 'Petya', group: 10 },
    { id: 5, name: 'Kira', group: 11 }
];
let user4 = {
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
let user5 = {
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
let user6 = {
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
let user7 = {
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
let store = {
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
let userCopy = Object.assign({}, user);
let numbersCopy = [...numbers];
let user1Copy = Object.assign(Object.assign({}, user1), { location: Object.assign({}, user1.location) });
let user2Copy = Object.assign(Object.assign({}, user2), { skills: [...user2.skills] });
let arrayCopy = array.map(item => (Object.assign({}, item)));
let user4Copy = Object.assign(Object.assign({}, user4), { studies: Object.assign(Object.assign({}, user4.studies), { exams: Object.assign({}, user4.studies.exams) }) });
let user5Copy = Object.assign(Object.assign({}, user5), { studies: Object.assign(Object.assign({}, user5.studies), { department: Object.assign({}, user5.studies.department), exams: user5.studies.exams.map(exam => (Object.assign({}, exam))) }) });
let user6Copy = Object.assign(Object.assign({}, user6), { studies: Object.assign(Object.assign({}, user6.studies), { department: Object.assign({}, user6.studies.department), exams: user6.studies.exams.map(exam => (Object.assign(Object.assign({}, exam), { professor: Object.assign({}, exam.professor) }))) }) });
let user7Copy = Object.assign(Object.assign({}, user7), { studies: Object.assign(Object.assign({}, user7.studies), { department: Object.assign({}, user7.studies.department), exams: user7.studies.exams.map(exam => (Object.assign(Object.assign({}, exam), { professor: Object.assign(Object.assign({}, exam.professor), { articles: exam.professor.articles.map(article => (Object.assign({}, article))) }) }))) }) });
let storeCopy = Object.assign(Object.assign({}, store), { state: Object.assign(Object.assign({}, store.state), { profilePage: Object.assign(Object.assign({}, store.state.profilePage), { posts: store.state.profilePage.posts.map(post => (Object.assign({}, post))) }), dialogsPage: Object.assign(Object.assign({}, store.state.dialogsPage), { dialogs: store.state.dialogsPage.dialogs.map(dialog => (Object.assign({}, dialog))), messages: store.state.dialogsPage.messages.map(message => (Object.assign({}, message))) }), sidebar: [...store.state.sidebar] }) });
user5Copy.studies.department.group = 12;
user5Copy.studies.exams[1].mark = 10;
user6Copy.studies.exams[1].professor.name = 'Сергей Сергеев';
user7Copy.studies.exams[1].professor.articles[1].pagesNumber = 3;
store.state.profilePage.posts[0].message = 'Hello';
store.state.profilePage.posts[1].message = 'Hello';
store.state.dialogsPage.messages[0].message = 'Hello';
store.state.dialogsPage.messages[1].message = 'Hello';
store.state.dialogsPage.messages[2].message = 'Hello';
console.log('='.repeat(60));
console.log('ЛАБОРАТОРНАЯ РАБОТА №1. TypeScript');
console.log('='.repeat(60));
console.log('\n✅ ЗАДАНИЕ 2: user5Copy изменён');
console.log('   group:', user5Copy.studies.department.group);
console.log('   programming mark:', user5Copy.studies.exams[1].mark);
console.log('\n✅ ЗАДАНИЕ 3: user6Copy изменён');
console.log('   professor name:', user6Copy.studies.exams[1].professor.name);
console.log('\n✅ ЗАДАНИЕ 4: user7Copy изменён');
console.log('   pagesNumber for "About CSS":', user7Copy.studies.exams[1].professor.articles[1].pagesNumber);
console.log('\n✅ ЗАДАНИЕ 5: store изменён');
console.log('   posts messages:', store.state.profilePage.posts.map(p => p.message));
console.log('   dialogs messages:', store.state.dialogsPage.messages.map(m => m.message));
console.log('\n🔍 ПРОВЕРКА: Оригиналы не изменились');
console.log('   user5 original group:', user5.studies.department.group);
console.log('   user6 original professor:', user6.studies.exams[1].professor.name);
console.log('   user7 original pages:', user7.studies.exams[1].professor.articles[1].pagesNumber);
