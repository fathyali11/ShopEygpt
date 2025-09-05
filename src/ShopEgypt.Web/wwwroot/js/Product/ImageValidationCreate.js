
$(document).ready(function () {
    $('#productForm').submit(function (e) {
        var fileInput = $('#imageFile')[0];
        var filePath = fileInput.value;
        var allowedExtensions = /(\.jpg|\.jpeg|\.png|\.gif)$/i;
        var maxSize = 2 * 1024 * 1024; // 2MB in bytes

        // Validate file extension
        if (!allowedExtensions.exec(filePath)) {
            e.preventDefault();
            fileInput.nextElementSibling.textContent = 'Please upload file having extensions .jpeg/.jpg/.png/.gif only.';
            fileInput.value = '';
            return false;
        } else {
            fileInput.nextElementSibling.textContent = ''; // Clear any previous error message
        }

        // Validate file size
        if (fileInput.files && fileInput.files[0]) {
            if (fileInput.files[0].size > maxSize) {
                e.preventDefault();
                fileInput.nextElementSibling.textContent = 'File size must be less than 2MB.';
                fileInput.value = '';
                return false;
            } else {
                fileInput.nextElementSibling.textContent = ''; // Clear any previous error message
            }
        }
    });
});
