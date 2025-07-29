$(document).ready(function () {
    let checkBtn = document.getElementById("hasSaleSwitch");
    let saleProperties = document.getElementById("sale-properties");

    if (checkBtn.checked) {
        saleProperties.classList.remove("hide");
    } else {
        saleProperties.classList.add("hide");
    }

    checkBtn.addEventListener("change", function () {
        saleProperties.classList.toggle("hide", !this.checked);
    });
});
