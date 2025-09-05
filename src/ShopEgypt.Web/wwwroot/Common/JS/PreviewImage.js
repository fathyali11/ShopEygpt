$(document).ready(function () {
    let imageFileInput = document.getElementById('imageFile');
    let previewImage = document.getElementById('previewImage');

    imageFileInput.addEventListener('change', function (event) {
        let file = event.target.files[0];
        if (file) {
            previewImage.src = URL.createObjectURL(file);
            previewImage.style.display = 'block';
        } else {
            previewImage.style.display = 'none';
        }
    });

});