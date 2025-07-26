$(document).ready(function () {
    let toggleBtn = document.querySelector('.toggle-btn');

    console.log(`btn was pressed`);
    toggleBtn.addEventListener('click', function () {
        const sidebar = document.getElementById('sidebar');
        const content = document.getElementById('main-content');
        sidebar.classList.toggle('collapsed');
        content.classList.toggle('expanded');
    });
});


