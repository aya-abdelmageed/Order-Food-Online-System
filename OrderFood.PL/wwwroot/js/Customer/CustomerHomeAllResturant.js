
document.addEventListener("DOMContentLoaded", function () {
    // Preloader
    document.body.classList.add("preloader-site");

    window.addEventListener("load", function () {
        const preloader = document.querySelector(".preloader-wrapper");
        if (preloader) preloader.style.display = "none";
        document.body.classList.remove("preloader-site");
    });

    // Chocolat Lightbox
    if (typeof Chocolat !== "undefined") {
        Chocolat(document.querySelectorAll(".image-link"), {
            imageSize: "contain",
            loop: true,
        });
    }

    // Swipers
    if (typeof Swiper !== "undefined") {
        new Swiper(".main-swiper", {
            speed: 500,
            pagination: {
                el: ".swiper-pagination",
                clickable: true,
            },
        });

        new Swiper(".category-carousel", {
            slidesPerView: 6,
            spaceBetween: 30,
            speed: 500,
            navigation: {
                nextEl: ".category-carousel-next",
                prevEl: ".category-carousel-prev",
            },
            breakpoints: {
                0: { slidesPerView: 2 },
                768: { slidesPerView: 3 },
                991: { slidesPerView: 4 },
                1500: { slidesPerView: 6 },
            },
        });

        new Swiper(".brand-carousel", {
            slidesPerView: 4,
            spaceBetween: 30,
            speed: 500,
            navigation: {
                nextEl: ".brand-carousel-next",
                prevEl: ".brand-carousel-prev",
            },
            breakpoints: {
                0: { slidesPerView: 2 },
                768: { slidesPerView: 2 },
                991: { slidesPerView: 3 },
                1500: { slidesPerView: 4 },
            },
        });

        new Swiper(".products-carousel", {
            slidesPerView: 5,
            spaceBetween: 30,
            speed: 500,
            navigation: {
                nextEl: ".products-carousel-next",
                prevEl: ".products-carousel-prev",
            },
            breakpoints: {
                0: { slidesPerView: 1 },
                768: { slidesPerView: 3 },
                991: { slidesPerView: 4 },
                1500: { slidesPerView: 6 },
            },
        });
    }

    // Quantity increment/decrement
    document.querySelectorAll(".product-qty").forEach(function (el) {
        const input = el.querySelector("#quantity");
        const btnPlus = el.querySelector(".quantity-right-plus");
        const btnMinus = el.querySelector(".quantity-left-minus");

        btnPlus?.addEventListener("click", function (e) {
            e.preventDefault();
            const value = parseInt(input.value) || 0;
            input.value = value + 1;
        });

        btnMinus?.addEventListener("click", function (e) {
            e.preventDefault();
            const value = parseInt(input.value) || 0;
            if (value > 0) input.value = value - 1;
        });
    });

    // Jarallax
    if (typeof jarallax !== "undefined") {
        jarallax(document.querySelectorAll(".jarallax"));
        jarallax(document.querySelectorAll(".jarallax-keep-img"), {
            keepImg: true,
        });
    }
});

