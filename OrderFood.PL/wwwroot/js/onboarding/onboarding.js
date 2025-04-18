"use strict";

// Spinner
var spinner = function () {
    setTimeout(function () {
        var spinnerEl = document.getElementById('spinner');
        if (spinnerEl) {
            spinnerEl.classList.remove('show');
        }
    }, 1);
};
spinner();

// Initiate the wowjs
new WOW().init();

// Sticky Navbar

// Dropdown on mouse hover
const dropdown = document.querySelectorAll(".dropdown");
const showClass = "show";

window.addEventListener("load", function () {
    if (window.matchMedia("(min-width: 992px)").matches) {
        dropdown.forEach(function (dropdownEl) {
            dropdownEl.addEventListener("mouseenter", function () {
                dropdownEl.classList.add(showClass);
                dropdownEl.querySelector(".dropdown-toggle").setAttribute("aria-expanded", "true");
                dropdownEl.querySelector(".dropdown-menu").classList.add(showClass);
            });
            dropdownEl.addEventListener("mouseleave", function () {
                dropdownEl.classList.remove(showClass);
                dropdownEl.querySelector(".dropdown-toggle").setAttribute("aria-expanded", "false");
                dropdownEl.querySelector(".dropdown-menu").classList.remove(showClass);
            });
        });
    }
});

// Back to top button
window.addEventListener('scroll', function () {
    var backToTop = document.querySelector('.back-to-top');
    if (window.scrollY > 300) {
        backToTop.style.display = 'block';
    } else {
        backToTop.style.display = 'none';
    }
});

document.querySelector('.back-to-top').addEventListener('click', function () {
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
});

// Facts counter
//const counters = document.querySelectorAll('[data-toggle="counter-up"]');
//counters.forEach(function (counter) {
//    let count = 0;
//    const target = parseInt(counter.getAttribute('data-target'));
//    const interval = setInterval(function () {
//        count += 1;
//        counter.textContent = count;
//        if (count >= target) {
//            clearInterval(interval);
//        }
//    }, 10);
//});

// Testimonials carousel (If you're using a carousel library, you'd need the relevant JS implementation, here assuming it's initialized via plain JS)
const testimonialCarousel = document.querySelector('.testimonial-carousel');
if (testimonialCarousel) {
    new OwlCarousel(testimonialCarousel, {
        autoplay: true,
        smartSpeed: 1000,
        center: true,
        margin: 24,
        dots: true,
        loop: true,
        nav: false,
        responsive: {
            0: {
                items: 1
            },
            500: {
                items: 2
            },
            700: {
                items: 3
            }
        }
    });
}

const myCarouselEl = document.querySelector('#myCarousel');
const carousel = new bootstrap.Carousel(myCarouselEl, {
    interval: 2000,
    ride: 'carousel'
});