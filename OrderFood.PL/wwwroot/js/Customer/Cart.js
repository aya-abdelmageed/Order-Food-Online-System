function updateQuantity(productId, change) {
    const input = event.target.parentElement.querySelector('.quantity-input');
    let value = parseInt(input.value) + change;
    if (value >= 1) {
        input.value = value;
    }
}