$(document).ready(function () {
    $('.increase-item').on('click', function (e) {
        e.preventDefault();

        const itemId = $(this).data('item-id');
        const cartId = $(this).data('cart-id');

        console.log(`Increasing item with ID: ${itemId} in cart with ID: ${cartId}`);

        $.ajax({
            url: `/Cart/Increase?cartItemId=${itemId}&cartId=${cartId}`,
            method: 'GET',
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
