
$(".slider_container").slick({
    autoplay: true,
    autoplaySpeed: 1000,
    speed: 600,
    slidesToShow: 4,
    slidesToScroll: 1,
    pauseOnHover: false,
    draggable: false,
    prevArrow: '<button class="slick-prev"> </button>',
    nextArrow: '<button class="slick-next"></button>',
    responsive: [{
        breakpoint: 991,
        settings: {
            slidesToShow: 3,
            slidesToScroll: 1,
            adaptiveHeight: true,
        },
    },
    {
        breakpoint: 767,
        settings: {
            slidesToShow: 3,
            slidesToScroll: 1,
        },
    },
    {
        breakpoint: 576,
        settings: {
            slidesToShow: 2,
            slidesToScroll: 1,
        },
    },
    {
        breakpoint: 420,
        settings: {
            slidesToShow: 1,
            slidesToScroll: 1,
        },
    }
    ]
});

