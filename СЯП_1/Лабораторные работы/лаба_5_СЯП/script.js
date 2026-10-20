// Задание 1
function makeCounter() {
    let count = 0;
    return function() {
        return count++; //!
    };
}
// лекс окружение это из чего состоит
let counter1 = makeCounter(); //! в counter1 будет хранится в прошлой функции return
let counter2 = makeCounter();

console.log(counter1());
console.log(counter1());
console.log(counter2());
console.log(counter2());

let counter = makeCounter();
console.log(counter());
console.log(counter());
console.log(counter());

// Задание 2
function volume(l) { //! карирование это
    return function(w) {
        return function(h) {
            return l * w * h;
        };
    };
}

const volumeArrow = l => w => h => l * w * h;

function createVolumeWithFixedLength(l) {
    return function(w, h) {
        return l * w * h;
    };
}

console.log(volume(2)(3)(4));
console.log(volumeArrow(2)(3)(4));

const volumeWithLength5 = volume(5); //!
console.log(volumeWithLength5(2)(3));

const fixedLengthVolume = createVolumeWithFixedLength(10);
console.log(fixedLengthVolume(2, 3));
console.log(fixedLengthVolume(4, 5));

// Задание 3
function* createMovementGenerator() {
    let x = 0, y = 0;
    let stepCount = 0;
    
    while (true) {
        const directions = ['left', 'right', 'up', 'down', 'stop'];
        const direction = directions[Math.floor(Math.random() * directions.length)];
        
        if (direction === 'stop') {
            return { x, y, totalSteps: stepCount };
        }
        
        for (let step = 1; step <= 10; step++) {
            switch (direction) {
                case 'left': x--; break;
                case 'right': x++; break;
                case 'up': y++; break;
                case 'down': y--; break;
            }
            stepCount++;
            yield { x, y, step: stepCount, direction };
        }
    }
}

const movement = createMovementGenerator();
for (let i = 0; i < 3; i++) {
    const result = movement.next();
    if (result.done) break;
    console.log(result.value);
}

function* manualMovementGenerator() {
    let x = 0, y = 0;
    let step = 0;
    
    while (true) {
        const command = yield { x, y, step };
        step++;
        
        switch (command) {
            case 'left': x--; break;
            case 'right': x++; break;
            case 'up': y++; break;
            case 'down': y--; break;
            case 'reset': x = 0; y = 0; step = 0; break;
        }
    }
}

const manualMove = manualMovementGenerator();
manualMove.next();
const commands = ['right', 'right', 'up', 'up', 'left', 'down'];
commands.forEach(cmd => {
    const result = manualMove.next(cmd);
    console.log(result.value);
});

// Задание 4
var globalVar = 'Я глобальная переменная var';
let globalLet = 'Я глобальная переменная let';
const globalConst = 'Я глобальная константа const';

function globalFunction() {
    return 'Я глобальная функция';
}

const globalObj = typeof window !== 'undefined' ? window : global;

console.log(globalObj.globalVar);
console.log(globalObj.globalLet);
console.log(globalObj.globalConst);
console.log(typeof globalObj.globalFunction);

globalObj.customGlobalVar = 'Создана через globalObj';
console.log(globalObj.customGlobalVar);

console.log(globalVar);
globalObj.globalVar = 'Переопределена через globalObj';
console.log(globalVar);

const allGlobalProps = Object.getOwnPropertyNames(globalObj);
console.log(allGlobalProps.length);
console.log(allGlobalProps.slice(0, 10));

globalObj.appCounter = 0;
function incrementGlobalCounter() {
    globalObj.appCounter++;
    return globalObj.appCounter;
}

console.log(globalObj.appCounter);
console.log(incrementGlobalCounter());
console.log(incrementGlobalCounter());
console.log(appCounter);