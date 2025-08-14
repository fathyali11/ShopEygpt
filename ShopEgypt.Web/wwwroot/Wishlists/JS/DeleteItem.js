$(document).ready(function () {
    $(document).on('click', '.delete', function (e) {
        e.preventDefault();

        const productId = $(this).data('product-id');
        const wishlistId = $(this).data('wishlist-id');
        const token = $('input[name="__RequestVerificationToken"]').val();

        console.log(`Product ID: ${productId}, Wishlist ID: ${wishlistId}`);
        console.log(`CSRF Token: ${token}`);

        Swal.fire({
            title: 'Delete',
            text: 'Are You Sure',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes',
            cancelButtonText: 'No'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/Wishlists/Delete`,
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': token
                    },
                    data: {
                        WishlistId: wishlistId,
                        ProductId: productId
                    },
                    success: function (response) {
                        if (response.success) {
                            $('#items-count').text("€ " + response.wishlistItemsCount);
                            Swal.fire({
                                title: 'Deleted',
                                text: response.message,
                                icon: 'success',
                                timer: 1500,
                                showConfirmButton: false
                            }).then(() => {
                                location.reload();
                            });

                        }
                    },
                    error: function () {
                        Swal.fire('error', 'error', 'error');
                    }
                });
            }
        });
    });
});
