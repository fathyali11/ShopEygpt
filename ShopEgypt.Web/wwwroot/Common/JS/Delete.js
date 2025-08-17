$(document).ready(function () {
    $(document).on("click", ".delete-btn", function (e) {
        e.preventDefault();
        let itemId = $(this).data("id");
        let controllerName = $(this).data("controller-name");

        const token = $('input[name="__RequestVerificationToken"]').val();

        console.log("Item ID:", itemId);
        console.log("Controller Name :", controllerName);

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
                    url: `/${controllerName}/Delete`,
                    type: "POST",
                    headers: {
                        'RequestVerificationToken': token
                    },
                    data: {
                        id: itemId
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
});
