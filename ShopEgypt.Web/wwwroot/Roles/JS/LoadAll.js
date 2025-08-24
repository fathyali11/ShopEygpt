$(document).ready(function () {
    var $select = $("#role-select-list-item-container");
    var currentRole = $select.data("current-role");

    $select.load("/Roles/LoadRolesSelectList", function () {
        if (currentRole) {
            $select.val(currentRole);
        }
    });
});
