$(document).ready(function () {

    $(document).on("click", "#cancel-order-btn", function (e) {

        e.preventDefault();

        let orderId = $(this).data("order-id");
        const token = $('input[name="__RequestVerificationToken"]').val();

        console.log(`order id =${orderId}`);
        console.log("CSRF Token:", token);


        Swal.fire({
            title: "Cancel",
            text: "Do You Want To Cancel This Order",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Yes, cancel it!"
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/Orders/Cancel`,
                    type: "POST",
                    headers: {
                        'RequestVerificationToken': token
                    },
                    data: {
                        id: orderId
                    },
                    success: function (response) {
                        if (response.success === true) {
                            console.log("canceld successfully.");
                            Swal.fire("Cancelled!", response.message, "success")
                                .then(() => {
                                    location.reload();
                                });
                        } else {
                            console.log("not canceld");
                            Swal.fire("Not Cancelled", response.message);
                        }

                    },
                    error: function () {
                        Swal.fire("Error", "Something went wrong.", "error");
                    }
                });
            }
        });
    })
});