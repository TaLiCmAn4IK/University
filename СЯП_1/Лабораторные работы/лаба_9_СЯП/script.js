console.log('Задание - 1')
let Shape = {
	color: 'black',
	size: 'medium',
	getDescription() {
		return `Фигура: цвет - ${this.color}, размер - ${this.size}`
	},
}

let Circle = Object.create(Shape)
Circle.radius = 10
Circle.getArea = function () {
	return Math.PI * this.radius * this.radius
}

let GreenCircle = Object.create(Circle)
GreenCircle.color = 'green'
GreenCircle.radius = 5

let Triangle = Object.create(Shape)
Triangle.sides = 3
Triangle.hasLines = true
Triangle.lineCount = 3

let Square = Object.create(Shape)
Square.sides = 4

let SmallSquare = Object.create(Square)
SmallSquare.size = 'small'
SmallSquare.color = 'red'

console.log('Свойства зеленого круга:')
console.log('Цвет:', GreenCircle.color)
console.log('Радиус:', GreenCircle.radius)
console.log('Размер:', GreenCircle.size)
console.log(Object.getOwnPropertyNames(GreenCircle))

console.log('\nСвойства треугольника:')
console.log('Количество сторон:', Triangle.sides)
console.log('Есть линии:', Triangle.hasLines)
console.log('Количество линий:', Triangle.lineCount)
console.log(Object.getOwnPropertyNames(Triangle))

console.log('\nАнализ маленького квадрата:')
console.log(SmallSquare.hasOwnProperty('color'))
console.log('Цвет маленького квадрата:', SmallSquare.color)
console.log('Цвет в прототипе (Square):', Square.color)

console.log('Задание - 2')

class Human {
	#birthYear

	constructor(firstName, lastName, birthYear, address) {
		this.firstName = firstName
		this.lastName = lastName
		this.#birthYear = birthYear
		this.address = address
	}

	changeAddress(newAddress) {
		this.address = newAddress
		console.log(`Адрес изменен на: ${newAddress}`)
	}

	get age() {
		let currentYear = new Date().getFullYear() // Создает объект с текущей датой
		return currentYear - this.#birthYear
	}

	set age(newAge) {
		const currentYear = new Date().getFullYear()
		this.#birthYear = currentYear - newAge
		console.log(`Год рождения изменен на: ${this.#birthYear}`)
	}

	get birthYear() {
		return this.#birthYear
	}

	getFullName() {
		return `${this.lastName} ${this.firstName}`
	}

	displayInfo() {
		console.log(
			`Человек: ${this.getFullName()}, Возраст: ${this.age}, Адрес: ${this.address}`
		)
	}
}

class Student extends Human {
	constructor(
		firstName,
		lastName,
		birthYear,
		address,
		faculty,
		course,
		group,
		recordBookNumber
	) {
		super(firstName, lastName, birthYear, address)
		this.faculty = faculty
		this.course = course
		this.group = group
		this.recordBookNumber = recordBookNumber
	}

	changeCourse(newCourse) {
		this.course = newCourse
		console.log(`Курс изменен на: ${newCourse}`)
	}

	changeGroup(newGroup) {
		this.group = newGroup
		console.log(`Группа изменена на: ${newGroup}`)
	}

	analyzeRecordBook() {
		let recordStr = this.recordBookNumber.toString()

		if (recordStr.length !== 8) {
			console.log('Неверный формат номера зачетки')
			return null
		}

		return {
			facultyCode: parseInt(recordStr[0]),
			specialtyCode: parseInt(recordStr[1]),
			admissionYear: parseInt(recordStr.substring(2, 4)),
			budgetCode: parseInt(recordStr[4]),
			serialNumber: parseInt(recordStr.substring(5)),
		}
	}

	displayStudentInfo() {
		super.displayInfo()
		console.log(
			`Факультет: ${this.faculty}, Курс: ${this.course}, Группа: ${this.group}`
		)

		let recordAnalysis = this.analyzeRecordBook()
		if (recordAnalysis) {
			console.log(`Номер зачетки: ${this.recordBookNumber}`)
			console.log(`  Код факультета: ${recordAnalysis.facultyCode}`)
			console.log(`  Код специальности: ${recordAnalysis.specialtyCode}`)
			console.log(`  Год поступления: 20${recordAnalysis.admissionYear}`)
			console.log(
				`  Форма обучения: ${recordAnalysis.budgetCode === 1 ? 'бюджет' : 'платное'}`
			)
		}
	}
}

class Faculty {
	constructor(facultyName, groupCount, studentCount, students = []) {
		this.facultyName = facultyName
		this.groupCount = groupCount
		this.studentCount = studentCount
		this.students = students
	}

	changeGroupCount(newCount) {
		this.groupCount = newCount
		console.log(`Количество групп изменено на: ${newCount}`)
	}

	changeStudentCount(newCount) {
		this.studentCount = newCount
		console.log(`Количество студентов изменено на: ${newCount}`)
	}

	addStudent(student) {
		this.students.push(student)
		this.studentCount++
		console.log(`Студент ${student.getFullName()} добавлен на факультет`)
	}

	getDev() {
		let deviCount = 0

		for (let student of this.students) {
			let recordAnalysis = student.analyzeRecordBook()
			if (recordAnalysis && recordAnalysis.specialtyCode === 3) {
				deviCount++
			}
		}

		console.log(`Количество студентов специальности ДЭВИ: ${deviCount}`)
		return deviCount
	}

	getGroup(groupName) {
		const groupStudents = this.students.filter(
			student => student.group === groupName
		)

		console.log(`\nСписок студентов группы ${groupName}:`)
		if (groupStudents.length === 0) {
			console.log('В этой группе нет студентов')
		} else {
			groupStudents.forEach((student, index) => {
				console.log(
					`${index + 1}. ${student.getFullName()} (зачетка: ${student.recordBookNumber})`
				)
			})
		}

		return groupStudents
	}

	displayFacultyInfo() {
		console.log(`\n=== Факультет: ${this.facultyName} ===`)
		console.log(`Количество групп: ${this.groupCount}`)
		console.log(`Количество студентов: ${this.studentCount}`)
	}
}

let human1 = new Human('Иван', 'Иванов', 1990, 'ул. Ленина, 10')
human1.displayInfo()
human1.age = 35
console.log(`Новый возраст: ${human1.age}`)

let student1 = new Student(
	'Анна',
	'Петрова',
	2001,
	'ул. Строителей, 5',
	'ФИТ',
	3,
	'ПОИТ-31',
	71201300
)

let student2 = new Student(
	'Сергей',
	'Сидоров',
	2000,
	'ул. Центральная, 15',
	'ФИТ',
	4,
	'ДЭВИ-41',
	73201245
)

let student3 = new Student(
	'Мария',
	'Козлова',
	2002,
	'ул. Школьная, 3',
	'ФИТ',
	2,
	'ПОИТ-31',
	71202278
)

student1.displayStudentInfo()
student2.displayStudentInfo()

let fitFaculty = new Faculty('Факультет информационных технологий', 12, 300)
fitFaculty.addStudent(student1)
fitFaculty.addStudent(student2)
fitFaculty.addStudent(student3)

fitFaculty.displayFacultyInfo()

fitFaculty.getDev()
fitFaculty.getGroup('ПОИТ-31')
fitFaculty.getGroup('ДЭВИ-41')

function Human_1(name) {
	this.name = name
}
Human_1.prototype.sayHi = function () {
	console.log('Hi, I am ' + this.name)
}
let h = new Human_1('Иван')

let arr = [1, 2, 3]
// arr.map берётся из Array.prototype
// arr.toString берётся из Array.prototype → Object.prototype
