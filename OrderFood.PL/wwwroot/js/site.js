const btn_menu = document.querySelector(".btn-menu");
const side_bar = document.querySelector(".sidebar");

btn_menu.addEventListener("click", function () {
    side_bar.classList.toggle("expand");
    changebtn();
});

function changebtn() {
    if (side_bar.classList.contains("expand")) {
        btn_menu.classList.replace("bx-menu", "bx-menu-alt-right");
    } else {
        btn_menu.classList.replace("bx-menu-alt-right", "bx-menu");
    }
}

const btn_theme = document.querySelector(".theme-btn");
const theme_ball = document.querySelector(".theme-ball");

const localData = localStorage.getItem("theme");

if (localData == null) {
    localStorage.setItem("theme", "light");
}

if (localData == "dark") {
    document.body.classList.add("dark-mode");
    theme_ball.classList.add("dark");
} else if (localData == "light") {
    document.body.classList.remove("dark-mode");
    theme_ball.classList.remove("dark");
}

btn_theme.addEventListener("click", function () {
    document.body.classList.toggle("dark-mode");
    theme_ball.classList.toggle("dark");
    if (document.body.classList.contains("dark-mode")) {
        localStorage.setItem("theme", "dark");
    } else {
        localStorage.setItem("theme", "light");
    }
});



function changeQty(button, delta) {
    const input = button.parentElement.querySelector('.qty-input');
    let currentVal = parseInt(input.value) || 1;
    let newVal = currentVal + delta;
    if (newVal < 1) newVal = 1;
    input.value = newVal;
    console.log(input.value);
}

const removeButtons = document.querySelectorAll('.remove-item');

removeButtons.forEach(button => {
    button.addEventListener('click', function (e) {
        const item = e.target.closest('.wishlist-item');
        item.remove();
    });
});
