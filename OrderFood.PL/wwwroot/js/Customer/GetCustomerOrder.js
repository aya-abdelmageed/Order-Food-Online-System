function ViewOrderTracking(orderId) {


    const url = `/Customer/Orders/OrderTracking/${orderId}`;
    fetch(url)
        .then(response => {
            if (!response.ok) throw new Error("Failed to fetch order Tracking");
            return response.text();
        })
        .then(html => {
            //window.open(url, "_blank")
            window.location.href = url;
        })
        .catch(error => console.error("Error loading order details:", error));
}