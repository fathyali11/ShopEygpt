var dtble;
$(document).ready(function () {
    loadData();
});

function loadData() {
    dtble = $("#MyTable").DataTable({
        "ajax": {
            "url": "/Admin/Category/GetData"
        },
        "columns": [
            { "data": "name" },
            { "data": "createdDate" },
            {
                "data": "id",
                "render": function (data) {
                    return '<a href="/Admin/Category/Edit/' + data + '" class="btn btn-success">Edit</a> ' +
                        '<a href="javascript:void(0);" onclick="DeleteCategory(\'/Admin/Category/Delete/' + data + '\')" class="btn btn-danger">Delete</a>';
                }
            }
        ]
    });
}


function DeleteCategory(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                method: "DELETE",
                success: function (data) {
                    if (data.success) {
                        dtble.ajax.reload();
                        toastr.success(data.message);
                    } else {
                        toastr.error(data.message);
                    }
                }
            }).done(function () {
                Swal.fire({
                    title: "Deleted!",
                    text: "Your file has been deleted.",
                    icon: "success"
                });
            });
        }
    });
}
