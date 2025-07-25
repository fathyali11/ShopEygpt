﻿// imagePreview.js
$(document).ready(function () {
    $('#imageFile').on('change', function () {
        var file = this.files[0];
        var reader = new FileReader();

        if (file && file.type.startsWith('image/')) {
            reader.onload = function (e) {
                $('#previewImage').attr('src', e.target.result).show();
            };
            reader.readAsDataURL(file);
        } else {
            $('#previewImage').hide();
        }
    });
});
