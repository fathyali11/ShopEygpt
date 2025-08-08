$(document).ready(function () {
    updateWishlistCount();

    $(document).on("click", ".toggle-wishlist", function (e) {
        e.preventDefault();
        const icon = $(this).find(".wishlist-icon");

        const productId = $(this).data("product-id");
        const productName = $(this).data("product-name");
        const imageName = $(this).data("image-name");
        const price = $(this).data("price");
        const isInWishlist = $(this).data("is-in-wishlist");
        const token = $('input[name="__RequestVerificationToken"]').val();

        console.log(`product id = ${productId}`);
        console.log(`product name = ${productName}`);
        console.log(`is in wishlist = ${isInWishlist}`);
        console.log(`icon = ${icon}`);

        $.ajax({
            url: `/Wishlists/Toggle`,
            method: 'POST',
            headers: {
                'RequestVerificationToken': token
            },
            data: {
                productId: productId,
                productName: productName,
                imageName: imageName,
                price: price,
                IsInWishlist:isInWishlist
            },
            success: function (response) {
                if (response.success) {

                    if (response.isInWishlist) {
                        icon
                            .removeClass('bi-heart')
                            .addClass('bi-heart-fill text-danger');
                    } else {
                        icon
                            .removeClass('bi-heart-fill text-danger')
                            .addClass('bi-heart');
                    }
                    updateWishlistCount();
                }
                
            },
            error: function (xhr) {
                const returnUrl = window.location.pathname + window.location.search;
                if (xhr.status === 401 || xhr.status === 403) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Authentication Required',
                        text: 'Please log in to add items to your wishlist.'
                    }).then(() => {
                        window.location.href = `/Auths/Login?returnUrl=${returnUrl}`;
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Oops...',
                        text: 'Something went wrong!'
                    });
                }
            }
        });
    });
});

function updateWishlistCount() {
    $.ajax({
        url: "/Wishlists/Count",
        type: "GET",
        success: function (data) {
            $("#wishlist-count-id").text("(" + data.count + ")");
        },
        error: function (xhr, status, error) {
            console.error("Error fetching wishlist count:", error);
        }
    });
}