var dtable;
$(document).ready(function () {
    loadData();
});

function loadData() {
    dtable = $("#MyTable").DataTable({
        "ajax": {
            "url": "/Admin/Category/GetData"
        },
        "columns": [
            { "data": "name" },
            {
                "data": "createdDate",
                "render": function (data) {
                    var date = new Date(data);
                    return date.toLocaleDateString('en-US');// Example: Aug 9, 2024 3:45 PM
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return '<a href="/Admin/Category/Edit/' + data + '" class="btn btn-success">Edit</a> ' +
                        '<a href="javascript:void(0);" onclick="DeleteItem(\'/Admin/Category/Delete/' + data + '\')" class="btn btn-danger">Delete</a>';
                }
            }
        ]
    });
}
