$(document).ready(function () {
    $(document).on("click", ".toggle", function (e) {
        e.preventDefault();

        const button = $(this);
        const id = $(this).data("id");
        const token = $('input[name="__RequestVerificationToken"]').val();

        console.log(`user id = ${id}\ntoken = ${token}`);

        $.ajax({
            url: `/Users/Toggle`,
            method: 'POST',
            headers: {
                'RequestVerificationToken': token
            },
            data: {
                id:id
            },
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Toggled',
                        text: response.message,
                        timer: 1500,
                        showConfirmButton: false
                    }).then(() => {
                        const icon = button.find('i');

                        if (icon.hasClass('bi-lock-fill')) {
                            icon.removeClass('bi-lock-fill').addClass('bi-unlock-fill');
                        } else {
                            icon.removeClass('bi-unlock-fill').addClass('bi-lock-fill');
                        }
                    });
                } else {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Notice',
                        text: response.message || 'Unexpected response.'
                    });
                }
            }
        });

    })

})