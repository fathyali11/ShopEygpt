$(document).ready(function () {
    $(document).on('click', '.delete', function (e) {
        e.preventDefault();

        const itemId = $(this).data('item-id');
        const cartId = $(this).data('cart-id');
        const token = $('input[name="__RequestVerificationToken"]').val();

        console.log(`Item ID: ${itemId}, Cart ID: ${cartId}`);
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
                    url: `/Cart/Delete?itemId=${itemId}&cartId=${cartId}`,
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': token
                    },
                    data: {
                        cartItemId: itemId,
                        cartId: cartId
                    },
                    success: function (response) {
                        if (response.success) {
                            $('#total-price').text("€ " + response.totalPrice);
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
                        Swal.fire('error','error', 'error');
                    }
                });
            }
        });
    });
});
