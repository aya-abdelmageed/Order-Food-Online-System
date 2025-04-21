(function($) {

  "use strict";

  var initPreloader = function() {
    $(document).ready(function($) {
    var Body = $('body');
        Body.addClass('preloader-site');
    });
    $(window).load(function() {
        $('.preloader-wrapper').fadeOut();
        $('body').removeClass('preloader-site');
    });
    }

  // init Chocolat light box
	var initChocolat = function() {
		Chocolat(document.querySelectorAll('.image-link'), {
		  imageSize: 'contain',
		  loop: true,
		})
	}

  var initSwiper = function() {

    var swiper = new Swiper(".main-swiper", {
      speed: 500,
      pagination: {
        el: ".swiper-pagination",
        clickable: true,
      },
    });

    var category_swiper = new Swiper(".category-carousel", {
      slidesPerView: 3,
      spaceBetween: 30,
      speed: 500,
      navigation: {
        nextEl: ".category-carousel-next",
        prevEl: ".category-carousel-prev",
      },
      breakpoints: {
        0: {
          slidesPerView: 1,
        },
        768: {
          slidesPerView: 2,
        },
        992: {
          slidesPerView: 3,
        },
      }
    });

    var brand_swiper = new Swiper(".brand-carousel", {
      slidesPerView: 4,
      spaceBetween: 30,
      speed: 500,
      navigation: {
        nextEl: ".brand-carousel-next",
        prevEl: ".brand-carousel-prev",
      },
      breakpoints: {
        0: {
          slidesPerView: 2,
        },
        768: {
          slidesPerView: 2,
        },
        991: {
          slidesPerView: 3,
        },
        1500: {
          slidesPerView: 4,
        },
      }
    });

    var products_swiper = new Swiper(".products-carousel", {
      slidesPerView: 5,
      spaceBetween: 30,
      speed: 500,
      navigation: {
        nextEl: ".products-carousel-next",
        prevEl: ".products-carousel-prev",
      },
      breakpoints: {
        0: {
          slidesPerView: 1,
        },
        768: {
          slidesPerView: 3,
        },
        991: {
          slidesPerView: 4,
        },
        1500: {
          slidesPerView: 6,
        },
      }
    });
  }

  var initProductQty = function(){

    $('.product-qty').each(function(){

      var $el_product = $(this);
      var quantity = 0;

      $el_product.find('.quantity-right-plus').click(function(e){
          e.preventDefault();
          var quantity = parseInt($el_product.find('#quantity').val());
          $el_product.find('#quantity').val(quantity + 1);
      });

      $el_product.find('.quantity-left-minus').click(function(e){
          e.preventDefault();
          var quantity = parseInt($el_product.find('#quantity').val());
          if(quantity>0){
            $el_product.find('#quantity').val(quantity - 1);
          }
      });

    });

  }

  // init jarallax parallax
  var initJarallax = function() {
    jarallax(document.querySelectorAll(".jarallax"));

    jarallax(document.querySelectorAll(".jarallax-keep-img"), {
      keepImg: true,
    });
  }

  // document ready
  $(document).ready(function() {
    
    initPreloader();
    initSwiper();
    initProductQty();
    initJarallax();
    initChocolat();

  }); // End of a document

  //---------------------------------------------------
   
       

 

 
})(jQuery);

function myFunction(icon) {
    icon.classList.toggle("bi-heart");
    icon.classList.toggle("bi-heart-fill");
}

function increase(button) {
    const input = button.parentElement.querySelector("input");
    let value = parseInt(input.value) || 0;
    input.value = value + 1;
}

function toggleDeleteIcons() {
    document.querySelector('.category-carousel .swiper-wrapper')
        .classList.toggle('show-delete-icons');
}

function decrease(button) {
    const input = button.parentElement.querySelector("input");
    let value = parseInt(input.value) || 0;
    if (value > 1) {
        input.value = value - 1;
    }
   
}

function choose(element) {
    var categoryId = $(element).data('category-id');
    var restaurantId = @Model.Id;

    $('.category-item').removeClass('active-category');
    $(element).addClass('active-category');

    fetch('@Url.Action("GetCategoryMeals", "Restaurants")' + `?restaurantId=${restaurantId}&categoryId=${categoryId}`)
        .then(response => {
            if (!response.ok) throw new Error("Failed to load meals");
            return response.text();
        })
        .then(html => {
            document.getElementById("mealsContent").innerHTML = html;
        })
        .catch(error => {
            document.getElementById("mealsContent").innerHTML =
                `<div class="text-danger text-center">${error.message}</div>`;
        });
}

function toggleDeleteIcons() {
    const icons = document.querySelectorAll('.delete-category-btn');
    icons.forEach(icon => icon.classList.toggle('d-none'));
}

function deleteCategory(button) {
    if (!confirm('Are you sure you want to delete this category?')) return;

    const categoryId = button.getAttribute('data-category-id');
    const token = document.querySelector('#csrfTokenForm input[name="__RequestVerificationToken"]').value;

    fetch('@Url.Action("DeleteCategory", "Restaurants")', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: `id=${categoryId}&__RequestVerificationToken=${encodeURIComponent(token)}`
    })
        .then(response => {
            if (response.ok) {
                // Remove the category from DOM
                button.closest('.swiper-slide').remove();
            } else {
                throw new Error("Failed to delete category");
            }
        })
        .catch(error => {
            alert(error.message);
        });
}