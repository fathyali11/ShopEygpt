$(document).ready(function () {
    $('.decrease-item').on('click', function (e) {
        e.preventDefault();
        const itemId = $(this).data('item-id');
        const cartId = $(this).data('cart-id');

        console.log(`Increasing item with ID: ${itemId} in cart with ID: ${cartId}`);
        $.ajax({
            url: `/Cart/Decrease?cartItemId=${itemId}&cartId=${cartId}`,
            method: 'GET',
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