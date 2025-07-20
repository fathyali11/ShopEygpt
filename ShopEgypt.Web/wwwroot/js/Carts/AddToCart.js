$(document).ready(function () {

    $(document).on("click", ".add-to-cart", function (e) {
        e.preventDefault();

        
        const productId = $(this).data("product-id");
        console.log(`Adding product with ID: ${productId} to cart`);
        $.ajax({
            url: `/Cart/Add?productId=${productId}`,
            method: 'GET',
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Added to Cart',
                        text: response.message,
                        timer: 1500,
                        showConfirmButton: false
                    });
                }
            },
            error: function () {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong!',
                });
            }
        });
    });
});
