$(document).ready(function () {
    $(document).on("click", ".add-to-cart", function (e) {
        e.preventDefault();

        const productId = $(this).data("product-id");
        const productName = $(this).data("product-name");
        const imageName = $(this).data("image-name");
        const price = $(this).data("price");
        const count = $(this).data("count");
        const token = $('input[name="__RequestVerificationToken"]').val();

        //console.log('Product ID:', productId);
        //console.log('Product Name:', productName);
        //console.log('Image Name:', imageName);
        //console.log('Price:', price);
        //console.log('Count:', count);
        //// Log the CSRF token for debugging purposes
        //if (token === undefined || token === null || token.trim() === '') {
        //    console.error('CSRF token is missing or invalid.');
        //    Swal.fire({
        //        icon: 'error',
        //        title: 'Error',
        //        text: 'CSRF token is missing or invalid. Please refresh the page.'
        //    });
        //    return;
        //} else {
        //    console.log('CSRF Token:', token);
        //}

        $.ajax({
            url: `/Cart/Add`,
            method: 'POST',
            headers: {
                'RequestVerificationToken': token
            },
            data: {
                productId: productId,
                productName: productName,
                imageName: imageName,
                price: price,
                count: count
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
                } else {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Notice',
                        text: response.message || 'Unexpected response.'
                    });
                }
            },
            error: function (xhr) {
                const returnUrl = window.location.pathname + window.location.search;
                console.log('Redirecting to login with return URL:', returnUrl);
                if (xhr.status === 401 || xhr.status === 403) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Authentication Required',
                        text: 'Please log in to add items to your cart.'
                    }).then(() => {
                        window.location.href = `/Auths/Login?returnUrl=${returnUrl}`;
                    });
                    
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Oops...',
                        text: 'Something went wrong!'
                    });
                }
            }
        });
    });
});
