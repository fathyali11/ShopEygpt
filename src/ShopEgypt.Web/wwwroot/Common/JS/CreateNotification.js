$(document).ready(function () {
    const successMessage = $('#success-message-data').data('message');
    const errorMessage = $('#error-message-data').data('message');

    if (successMessage) {
        Swal.fire({
            icon: 'success',
            title: successMessage,
            toast: true,
            position: 'top-end', // ✅ أعلى يمين الشاشة
            showConfirmButton: false,
            timer: 2000,
            timerProgressBar: true
        });
    }

    if (errorMessage) {
        Swal.fire({
            icon: 'error',
            title: errorMessage,
            toast: true,
            position: 'top-end', // ✅ أعلى يمين الشاشة
            showConfirmButton: true
        });
    }
});
