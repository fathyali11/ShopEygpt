$(document).ready(function () {
    $('.decrease-item').on('click', function (e) {
        e.preventDefault();
        const itemId = $(this).data('item-id');
        $.ajax({
            url: `/Cart/Decrease?cartItemId=${itemId}`,
            method: 'GET',
            success: function (response) {
                $(`a[data-item-id="${itemId}"]`).siblings('.item-count').text(response.count);
                if (response.count === 0) {
                    location.reload();
                }
            }
        });
    });
});