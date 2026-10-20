class Task {
	constructor(id, name, state) {
		this.id = id
		this.name = name
		this.state = state
	}
}

class ToDoList {
	constructor() {
		this.tasks = []
	}

	addTask(task) {
		this.tasks.push(task)
	}

	filterTasks(statement) {
		if (statement === 'all') return this.tasks
		return this.tasks.filter(task => task.state === statement)
	}
}

const toDoList = new ToDoList()
const taskListElement = document.getElementById('taskList')

function generateTasks(tasks) {
	taskListElement.innerHTML = ''

	tasks.forEach((task, index) => {
		const taskElement = document.createElement('li')
		taskElement.className = 'task'
		taskElement.innerHTML = `
            <div>
                <input type="checkbox" ${task.state ? 'checked' : ''} onchange="changeTaskState(${task.id})">
                ${task.name}
            </div>
            <div>
                <button class="edit" onclick="editTask(${task.id})">Редактировать</button>
                <button class="delete" onclick="deleteTask(${task.id})">Удалить</button>
            </div>
        `
		taskListElement.appendChild(taskElement)
	})
}

function addTask() {
	const taskName = document.getElementById('taskName').value
	if (!taskName) return alert('Введите название задачи')

	const newTask = new Task(Date.now(), taskName, false)
	toDoList.addTask(newTask)
	generateTasks(toDoList.filterTasks('all'))
	document.getElementById('taskName').value = ''
}

function editTask(taskId) {
	const task = toDoList.tasks.find(task => task.id === taskId)
	if (task) {
		const newName = prompt('Введите новое название задачи', task.name)
		if (newName) {
			task.name = newName
			generateTasks(toDoList.filterTasks('all'))
		}
	}
}

function deleteTask(taskId) {
	toDoList.tasks = toDoList.tasks.filter(task => task.id !== taskId)
	generateTasks(toDoList.filterTasks('all'))
}

function filterTasks(statement) {
	const filteredTasks = toDoList.filterTasks(statement)
	generateTasks(filteredTasks)
}

function changeTaskState(taskId) {
	const task = toDoList.tasks.find(task => task.id === taskId)
	if (task) {
		task.state = !task.state
		generateTasks(toDoList.filterTasks('all'))
	}
}

generateTasks(toDoList.filterTasks('all'))
