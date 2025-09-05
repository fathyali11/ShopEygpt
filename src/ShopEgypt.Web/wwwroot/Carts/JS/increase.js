$(document).ready(function () {
    $('.increase-item').on('click', function (e) {
        e.preventDefault();

        const itemId = $(this).data('item-id');
        const cartId = $(this).data('cart-id');
        const token = $('input[name="__RequestVerificationToken"]').val();

        console.log(`Increasing item with ID: ${itemId} in cart with ID: ${cartId}`);
        console.log(`Using token: ${token}`);

        $.ajax({
            url: `/Cart/Increase`,
            method: 'POST',
            headers: {
                'RequestVerificationToken': token
            },
            data: {
                cartItemId: itemId,
                cartId: cartId
            },
            success: function (response) {
                // Update item count
                $(`a[data-item-id="${itemId}"]`).siblings('.item-count').text(response.count);

                // Update total price globally (from summary section)
                $('#total-price').text("€ " + response.totalPrice);
            },
            error: function () {
                console.error("Error increasing item");
            }
        });
    });
});
