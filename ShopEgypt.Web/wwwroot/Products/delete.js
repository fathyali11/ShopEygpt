$(document).ready(function () {
    $(document).on("click", ".delete-btn", function (e) {
        e.preventDefault();
        let productId = $(this).data("product-id");
        const token = $('input[name="__RequestVerificationToken"]').val();

        console.log("Product ID:", productId);
        console.log("CSRF Token:", token);

        Swal.fire({
            title: "Delete",
            text: "This action cannot be undone!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Yes, delete it!"
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/Product/Delete`,
                    type: "POST",
                    headers: {
                        'RequestVerificationToken': token
                    },
                    data: {
                        id: productId
                    },
                    success: function (response) {
                        if (response.success === true) {
                            console.log("Product deleted successfully.");
                            Swal.fire("Deleted!", response.message, "success")
                                .then(() => {
                                    location.reload();
                                });
                        }
                        
                    },
                    error: function () {
                        Swal.fire("Error", "Something went wrong.", "error");
                    }
                });
            }
        });
    });
});
