var dtable;
$(document).ready(function () {
    loadData();
});

function loadData() {
    dtable = $("#MyTable").DataTable({
        "ajax": {
            "url": "/Admin/Users/GetData"
        },
        "columns": [
            { "data": "name" },
            { "data": "role" },
            {
                "data": "id",
                "render": function (data, type, row) {
                    var lockoutEnd = new Date(row.LockoutEnd);
                    var now = new Date();

                    if (row.LockoutEnd == null || lockoutEnd < now) {
                        return '<a class="btn btn-success" href="/Admin/Users/LockOrOpen?userId=' + data + '"><i class="bi bi-unlock-fill"></i></a>';
                    } else {
                        return '<a class="btn btn-danger" href="/Admin/Users/LockOrOpen?userId=' + data + '"><i class="bi bi-lock-fill"></i></a>';
                    }
                }
            }
        ]
    });
}
