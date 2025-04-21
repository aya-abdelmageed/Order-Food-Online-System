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



  // document ready
  $(document).ready(function() {
    
    initPreloader();
    initSwiper();
    initProductQty();


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

let deleteMode = false;
function toggleDeleteIcons() {
    deleteMode = !deleteMode;
    document.querySelectorAll('.delete-category-btn').forEach(btn => {
        btn.classList.toggle('d-none', !deleteMode);

    });
}


function decrease(button) {
    const input = button.parentElement.querySelector("input");
    let value = parseInt(input.value) || 0;
    if (value > 1) {
        input.value = value - 1;
    }  
}

function initializeMap(restaurantId, lat, lng, restaurantName) {
    var map = L.map('map-' + restaurantId, {
        attributionControl: false
    }).setView([lat, lng], 13);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '' 
    }).addTo(map);

    L.marker([lat, lng])
        .addTo(map)
        .bindPopup(restaurantName)
        .openPopup();

    map.on('click', function (e) {
        var clickedLat = e.latlng.lat;
        var clickedLng = e.latlng.lng;
        console.log("Latitude: " + clickedLat + ", Longitude: " + clickedLng);

        L.popup()
            .setLatLng(e.latlng)
            .setContent("You clicked at latitude: " + clickedLat + ", longitude: " + clickedLng)
            .openOn(map);
    });
}


function InitializeFromMap() {
    var map = L.map('map', {
        attributionControl: false // Disable the attribution control completely
    }).setView([30.0444, 31.2357], 13);

    // Add OpenStreetMap tile layer without attribution
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: ''  // Remove any attribution (including logo)
    }).addTo(map);

    // Add marker at the default location
    var marker = L.marker([30.0444, 31.2357]).addTo(map);

    // Handle map click event
    map.on('click', function (e) {
        var lat = e.latlng.lat;
        var lng = e.latlng.lng;

        // Update the marker position and the Lat/Long hidden fields
        marker.setLatLng([lat, lng]);
        document.getElementById("Lat").value = lat;
        document.getElementById("Long").value = lng;

        marker.bindPopup("Selected Location: <br/> Lat: " + lat + "<br/> Long: " + lng).openPopup();
    });
}








