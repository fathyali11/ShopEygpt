$(document).ready(function () {
    let toggleBtn = document.querySelector('.toggle-btn');

    console.log(`btn was pressed`);
    toggleBtn.addEventListener('click', function () {
        const sidebar = document.getElementById('sidebar');
        const contents = document.querySelectorAll('.main-content');
        sidebar.classList.toggle('collapsed');
        contents.forEach(content => {
            content.classList.toggle('expanded');
        });
    });
});


