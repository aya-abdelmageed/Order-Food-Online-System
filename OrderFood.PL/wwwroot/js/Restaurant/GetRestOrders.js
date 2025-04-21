function loadOrderDetails(orderId) {


    const url = `/Resturant/Orders/Details/${orderId}`;
    fetch(url)
        .then(response => {
            if (!response.ok) throw new Error("Failed to fetch order details");
            return response.text();
        })
        .then(html => {
            // Inject partial view HTML into container
            document.getElementById("orderDetailsContainer").innerHTML = html;
            // Initialize collapse on the returned element
            const collapseEl = document.getElementById(`collapse${orderId}`);
            if (collapseEl) {
                const bsCollapse = new bootstrap.Collapse(collapseEl, { toggle: true });
            }
        })
        .catch(error => console.error("Error loading order details:", error));
}


function disableButtonsOnly() {
    document.getElementById("rejectBtn").style.visibility = "hidden";
    document.getElementById("acceptBtn").style.visibility = "hidden";
}

function showDoneButton() {
    document.getElementById("doneButtonContainer").style.visibility = "visible";
}

function rejectOrder(orderId) {
    if (confirm("Are you sure you want to reject this order?")) {
        disableButtonsOnly();

        fetch(`/Resturant/Orders/RejectOrder/${orderId}`, {
            method: 'POST'
        })
            .then(response => {
                if (response.ok) {
                    alert("Order rejected successfully!");
                    location.reload();

                } else {
                    alert("Failed to reject order.");
                }
            });
    }
}


function acceptOrder(orderId) {
    disableButtonsOnly();

    fetch(`/Resturant/Orders/AcceptOrder/${orderId}`, {
        method: 'POST'
    })
        .then(response => {
            if (response.ok) {
                alert("Order accepted successfully!");
                showDoneButton();
                location.reload();

            } else {
                alert("Failed to accept order.");
            }
        });
}

function OrderDone(orderId) {
    document.getElementById("doneButtonContainer").style.visibility = "hidden";

    fetch(`/Resturant/Orders/DoneOrder/${orderId}`, {
        method: 'POST'
    })
        .then(response => {
            if (response.ok) {
                alert("Order Done successfully and Waiting for shipping!");
                location.reload();
            } else {
                alert("Failed to mark order as done.");
            }
        });
}
