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
let user7Copy = {
    ...user7,
    studies: {
        ...user7.studies,
        department: { ...user7.studies.department },
        exams: [
            {
                ...user7.studies.exams[0],
                professor: {
                    ...user7.studies.exams[0].professor,
                    articles: [
                        { ...user7.studies.exams[0].professor.articles[0] },
                        { ...user7.studies.exams[0].professor.articles[1] },
                        { ...user7.studies.exams[0].professor.articles[2] }
                    ]
                }
            },
            {
                ...user7.studies.exams[1],
                professor: {
                    ...user7.studies.exams[1].professor,
                    articles: [
                        { ...user7.studies.exams[1].professor.articles[0] },
                        { ...user7.studies.exams[1].professor.articles[1] },
                        { ...user7.studies.exams[1].professor.articles[2] }
                    ]
                }
            }
        ]
    }
};

user7Copy.studies.exams[1].professor.degree = 'CSS';
console.log(user7Copy)