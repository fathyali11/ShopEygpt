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
                "render": function (data, row) {
                    var lockoutEnd = row.LockoutEnd ? new Date(row.LockoutEnd) : null;
                    var now = new Date();

                    if (lockoutEnd || lockoutEnd < now) {
                        return '<a class="btn btn-success" href="/Admin/Users/LockOrOpen/' + data + '"><i class="bi bi-unlock-fill"></i></a>';
                    } else {
                        return '<a class="btn btn-danger" href="/Admin/Users/LockOrOpen/' + data + '"><i class="bi bi-lock-fill"></i></a>';
                    }
                }
            }
            

        ]
    });
}
