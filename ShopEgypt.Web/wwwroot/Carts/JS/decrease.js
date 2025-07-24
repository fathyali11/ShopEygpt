$(document).ready(function () {
    $('.decrease-item').on('click', function (e) {
        e.preventDefault();
        const itemId = $(this).data('item-id');
        const cartId = $(this).data('cart-id');
        const token = $('input[name="__RequestVerificationToken"]').val();

        console.log(`Increasing item with ID: ${itemId} in cart with ID: ${cartId}`);
        console.log(`Using token: ${token}`);
        $.ajax({
            url: `/Cart/Decrease`,
            method: 'POST',
            headers: {
                'RequestVerificationToken': token
            },
            data: {
                cartItemId: itemId,
                cartId: cartId
            },
            success: function (response) {
                $(`a[data-item-id="${itemId}"]`).siblings('.item-count').text(response.count);
                $('#total-price').text("€ " + response.totalPrice);
                if (response.count === 0) {
                    location.reload();
                }
            }
        });
    });
});