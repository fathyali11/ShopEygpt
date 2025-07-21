$(document).ready(function () {
    $(document).on('click', '.delete', function (e) {
        e.preventDefault();

        const itemId = $(this).data('item-id');
        Swal.fire({
            title: 'Delete',
            text: 'Are You Sure',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes',
            cancelButtonText: 'No'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/Cart/Delete?itemId=${itemId}`,
                    method: 'GET',
                    success: function (response) {
                        if (response.success) {
                            Swal.fire({
                                title: 'Deleted',
                                text: response.message,
                                icon: 'success',
                                timer: 1500,
                                showConfirmButton: false
                            }).then(() => {
                                location.reload();
                            });
                        }
                    },
                    error: function () {
                        Swal.fire('error','error', 'error');
                    }
                });
            }
        });
    });
});
