// Example starter JavaScript for disabling form submissions if there are invalid fields
(() => {
    'use strict'

    // Fetch all the forms we want to apply custom Bootstrap validation styles to
    const forms = document.querySelectorAll('.needs-validation')

    // Loop over them and prevent submission
    Array.from(forms).forEach(form => {
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault()
                event.stopPropagation()
            }

            form.classList.add('was-validated')
        }, false)
    })
})()

document.addEventListener('DOMContentLoaded', function () {
    const paymentMethods = document.getElementsByName('paymentMethod');
    const cardSection = document.getElementById('card-info');

    function toggleCardInfo() {
        const selected = [...paymentMethods].find(radio => radio.checked)?.id;
        if (selected === 'paypal') {
            cardSection.style.display = 'block';
        } else {
            cardSection.style.display = 'none';
        }
    }

    paymentMethods.forEach(method => {
        method.addEventListener('change', toggleCardInfo);
    });

    // Run on load in case one is already selected
    toggleCardInfo();
});
