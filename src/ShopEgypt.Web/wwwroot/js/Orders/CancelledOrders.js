var dtable;

$(document).ready(function () {
    loadData();
});

function loadData() {
    dtable = $("#MyTable").DataTable({
        "ajax": {
            "url": "/Admin/Order/GetCancelledOrders"
        },
        "columns": [
            { "data": "id" },
            { "data": "name" },
            { "data": "email" },
            { "data": "phoneNumber" },
            {
                "data": "id",
                "render": function (data) {
                    return '<a href="/Admin/Order/Details?OrderId=' + data + '" class="btn btn-success">Details</a>';
                }
            }
        ]
    });
}
