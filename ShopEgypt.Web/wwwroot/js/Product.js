var dtable;
$(document).ready(function () {
    loadData();
});

function loadData() {
    dtable = $("#MyTable").DataTable({
        "ajax": {
            "url": "/Admin/Product/GetData"
        },
        "columns": [
            { "data": "name" },
            { "data": "description" },
            { "data": "price" },
            { "data": "category.name" },
            {
                "data": "imageName",
                "render": function (data, type, row) {
                    return '<img src="/Images/Products/' + data + '" alt="' + row.name + '" style="max-width:150px;" />';
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return '<a href="/Admin/Product/Edit/' + data + '" class="btn btn-success">Edit</a> ' +
                        '<a href="javascript:void(0);" onclick="DeleteProduct(\'/Admin/Product/Delete/' + data + '\')" class="btn btn-danger">Delete</a>';
                }
            }
        ]
    });
}

function DeleteProduct(url) {
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
                        dtable.ajax.reload();
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
