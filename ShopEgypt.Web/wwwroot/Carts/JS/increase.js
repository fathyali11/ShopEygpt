$(document).ready(function () {
    $('.increase-item').on('click', function (e) {
        e.preventDefault();
        const itemId = $(this).data('item-id');
        $.ajax({
            url: `/Cart/Increase?cartItemId=${itemId}`,
            method: 'GET',
            success: function (response) {
                $(`a[data-item-id="${itemId}"]`).siblings('.item-count').text(response.count);
            }
        });
    });
});