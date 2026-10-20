class Task {
    static idCounter = 1;
    
    constructor(title) {
        this.id = Task.idCounter++;
        this.title = title;
        this.completed = false;
    }
    
    setTitle(newTitle) {//измененияе названия 
        this.title = newTitle;
    }
    
    setCompleted(status) {//статус выполнения
        this.completed = status;
    }
    
    getInfo() {// вывод информации
        const status = this.completed ? 'выполнена' : 'не выполнена';
        return `[ID: ${this.id}] ${this.title} - ${status}`;
    }
}


class Todolist {
    static idCounter = 1;
    
    constructor(name) {
        this.id = Todolist.idCounter++;
        this.name = name;
        this.tasks = [];
    }
    
    setName(newName) {// меняет название
        this.name = newName;
    }
    
    addTask(task) {// добавляет задачу
        if (task instanceof Task) {
            this.tasks.push(task);
        }
    }
    
    filterTasks(completed) {// возвращает задачи по статусу выполнения
        return this.tasks.filter(task => task.completed === completed);
    }
    
    getAllTasks() {// возвращает все задачи списка
        return this.tasks;
    }
    
    showInfo() { // вывод информации о списке и его задачах
        console.log(`\n${this.name} (${this.tasks.length} задач)`);
        this.tasks.forEach(task => console.log(task.getInfo()));
    }
}


const workTodolist = new Todolist('Задание');
const task1 = new Task('Написать отчёт');
const task2 = new Task('Провести встречу');
const task3 = new Task('Отправить письмо клиенту');

workTodolist.addTask(task1);
workTodolist.addTask(task2);
workTodolist.addTask(task3);

task3.setCompleted(true);

workTodolist.showInfo();


const personalTodolist = new Todolist('Личное');
const task4 = new Task('Купить продукты');
const task5 = new Task('Записаться к врачу');
const task6 = new Task('Прочитать книгу');

personalTodolist.addTask(task4);
personalTodolist.addTask(task5);
personalTodolist.addTask(task6);

personalTodolist.showInfo();

console.log('\nВыполненные задачи (Задание):');
workTodolist.filterTasks(true).forEach(task => console.log(task.getInfo()));

console.log('\nНевыполненные задачи (Личное):');
personalTodolist.filterTasks(false).forEach(task => console.log(task.getInfo()));