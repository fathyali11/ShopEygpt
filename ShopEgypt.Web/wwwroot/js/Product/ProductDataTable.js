// dataTableSetup.js
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
                        '<a href="javascript:void(0);" onclick="DeleteItem(\'/Admin/Product/Delete/' + data + '\')" class="btn btn-danger">Delete</a>';
                }
            }
        ]
    });
}
