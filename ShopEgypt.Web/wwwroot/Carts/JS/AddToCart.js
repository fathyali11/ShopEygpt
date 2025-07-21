$(document).ready(function () {

    $(document).on("click", ".add-to-cart", function (e) {
        e.preventDefault();

        
        const productId = $(this).data("product-id");
        const productName = $(this).data("product-name");
        const imageName = $(this).data("image-name");
        const price = $(this).data("price");
        const count = $(this).data("count");

        // طباعة البيانات للتحقق
        console.log('Product ID:', productId);
        console.log('Product Name:', productName);
        console.log('Image Name:', imageName);
        console.log('Price:', price);
        console.log('Count:', count);
        //console.log('Token exists:', $('input[name="__RequestVerificationToken"]').length);
        //console.log('Token value:', $('input[name="__RequestVerificationToken"]').val());
        /*console.log($('input[name "__RequestVerificationToken"]').length)*/
        $.ajax({
            url: `/Cart/Add`,
            method: 'POST',
            data: {
                productId: productId,
                productName: productName,
                imageName: imageName,
                price: price,
                count: count
                /*__RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()*/
            },
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Added to Cart',
                        text: response.message,
                        timer: 1500,
                        showConfirmButton: false
                    });
                }
            },
            error: function () {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong!',
                });
            }
        });
    });
});
