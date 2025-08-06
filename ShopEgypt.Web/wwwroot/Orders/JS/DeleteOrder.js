$(document).ready(function () {

    $(document).on("click", "#delete-order-btn", function (e) {
        e.preventDefault();

        let orderId = $(this).data("order-id");
        const token = $('input[name="__RequestVerificationToken"]').val();

        console.log(`order id =${orderId}`);
        console.log("CSRF Token:", token);


        Swal.fire({
            title: "Delete",
            text: "Do You Want To Delete And Cancel This Order",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Yes, delete and cancel it!"
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/Orders/Delete`,
                    type: "POST",
                    headers: {
                        'RequestVerificationToken': token
                    },
                    data: {
                        id: orderId
                    },
                    success: function (response) {
                        if (response.success === true) {
                            console.log("deleted successfully.");
                            Swal.fire("Deleted!", response.message, "success")
                                .then(() => {
                                    location.reload();
                                });
                        } else {
                            console.log("not deleted");
                            Swal.fire("Not Deleted", response.message);
                        }

                    },
                    error: function () {
                        Swal.fire("Error", "Something went wrong.", "error");
                    }
                });
            }
        });

    });
})