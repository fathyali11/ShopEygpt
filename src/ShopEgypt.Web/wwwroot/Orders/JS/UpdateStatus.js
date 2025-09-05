$(document).ready(function () {

    $(document).on("click", ".update-order-status", function (e) {
        e.preventDefault();

        var orderId = $(this).data("order-id");
        var status = $(this).data("status");
        var token = $('input[name="__RequestVerificationToken"]').val();

        console.log("Order ID:", orderId);
        console.log("Status:", status);
        console.log("Token:", token);

        $.ajax({
            url: '/Orders/UpdateStatus',
            type: 'POST',
            headers: {
                'RequestVerificationToken': token
            },
            data: {
                orderId: orderId,
                status: status
            },
            success: function (response) {

                if (response.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message,
                        confirmButtonText: 'OK'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            location.reload();
                        }
                    });
                }
            },
            error: function () {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'An error occurred while updating the order status. Please try again.',
                    confirmButtonText: 'OK'
                });
            }
        });


    });

});