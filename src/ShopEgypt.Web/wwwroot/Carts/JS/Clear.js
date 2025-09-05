$(document).ready(function () {
    $(document).on('click', '.clear', function (e) {
        e.preventDefault();

        const cartId = $(this).data('cart-id');
        const token = $('input[name="__RequestVerificationToken"]').val();

        console.log(`Cart ID: ${cartId}`);
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
                    url: `/Cart/Clear?cartId=${cartId}`,
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': token
                    },
                    data: {
                        cartId: cartId
                    },
                    success: function (response) {
                        if (response.success) {
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
