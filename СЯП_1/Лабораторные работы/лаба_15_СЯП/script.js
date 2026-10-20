const form = document.getElementById("validationForm");

form.addEventListener("submit", function (event) {
    event.preventDefault(); 

    let isValid = true;

    const lastName = document.getElementById("lastName");
    const lastNameError = document.getElementById("lastNameError");
    if (!lastName.value.trim() || !/^[a-zA-Zа-яА-ЯёЁ]{1,20}$/.test(lastName.value.trim())) {
        lastNameError.textContent = "Только буквы, до 20 символов!";
        lastNameError.style.display = "block";
        isValid = false;
    } else {
        lastNameError.style.display = "none";
    }

    const firstName = document.getElementById("firstName");
    const firstNameError = document.getElementById("firstNameError");
    if (!firstName.value.trim() || !/^[a-zA-Zа-яА-ЯёЁ]{1,20}$/.test(firstName.value.trim())) {
        firstNameError.textContent = "Только буквы, до 20 символов!";
        firstNameError.style.display = "block";
        isValid = false;
    } else {
        firstNameError.style.display = "none";
    }

    const email = document.getElementById("email");
    const emailError = document.getElementById("emailError");
    if (!email.value.trim() || !/^[^\s@]+@[a-zA-Z]{2,5}\.[a-zA-Z]{2,3}$/.test(email.value.trim())) {
        emailError.textContent = "Недопустимый формат email! Пример: user@mail.ru";
        emailError.style.display = "block";
        isValid = false;
    } else {
        emailError.style.display = "none";
    }

    const phone = document.getElementById("phone");
    const phoneError = document.getElementById("phoneError");
    if (!phone.value.trim() || !/^\(0\d{2}\)\d{3}-\d{2}-\d{2}$/.test(phone.value.trim())) {
        phoneError.textContent = "Формат: (0xx)xxx-xx-xx";
        phoneError.style.display = "block";
        isValid = false;
    } else {
        phoneError.style.display = "none";
    }

    const city = document.getElementById("city");
    const cityError = document.getElementById("cityError");
    if (!city.value) {
        cityError.textContent = "Выберите город!";
        cityError.style.display = "block";
        isValid = false;
    } else {
        cityError.style.display = "none";
        if (city.value !== "Минск") {
            if (!confirm("Вы выбрали не Минск. Уверены в своем выборе?")) {
                isValid = false;
                city.focus();
            }
        }
    }

    const studyInBGTU = document.getElementById("studyInBGTU");
    if (!studyInBGTU.checked) {
        if (!confirm("Вы указали, что не учитесь в БГТУ. Уверены?")) {
            isValid = false;
        }
    }

    const course = document.querySelector('input[name="course"]:checked');
    const courseError = document.getElementById("courseError");
    if (!course) {
        courseError.textContent = "Выберите курс!";
        courseError.style.display = "block";
        isValid = false;
    } else {
        courseError.style.display = "none";
        if (course.value !== "3") {
            if (!confirm("Вы не выбрали 3 курс. Уверены в своем выборе?")) {
                isValid = false;
            }
        }
    }
    const about = document.getElementById("about");
    const aboutError = document.getElementById("aboutError");
    if (!about.value.trim()) {
        aboutError.textContent = "Заполните это поле!";
        aboutError.style.display = "block";
        isValid = false;
    } else if (about.value.length > 250) {
        aboutError.textContent = "Максимум 250 символов!";
        aboutError.style.display = "block";
        isValid = false;
    } else {
        aboutError.style.display = "none";
    }

    if (isValid) {
        alert("Форма успешно проверена!");
        
        console.log("Данные формы:");
        console.log("Фамилия:", lastName.value);
        console.log("Имя:", firstName.value);
        console.log("Email:", email.value);
        console.log("Телефон:", phone.value);
        console.log("Город:", city.value);
        console.log("БГТУ:", studyInBGTU.checked ? "Да" : "Нет");
        console.log("Курс:", course ? course.value : "Не выбран");
        console.log("О себе:", about.value);
    }
});

document.getElementById('phone').addEventListener('input', function(e) {
    let value = e.target.value.replace(/\D/g, '');
    
    if (value.length > 0) {
        if (!value.startsWith('0')) {
            value = '0' + value;
        }
        
        if (value.length > 3) {
            value = '(' + value.substring(0, 3) + ')' + value.substring(3);
        }
        
        if (value.length > 8) {
            value = value.substring(0, 8) + '-' + value.substring(8);
        }
        
        if (value.length > 11) {
            value = value.substring(0, 11) + '-' + value.substring(11);
        }
        
        if (value.length > 14) {
            value = value.substring(0, 14);
        }
    }
    
    e.target.value = value;
});
