$(document).ready(function () {

    $(".info-item").fadeIn(1000);

    $(".navbar-left-btn").on("click", function (slideNavbar) {
        $(".navbar-left").toggle("left");
        $(".page-wrapper").css("padding-left", "0");
        $(".navbar-left-btn").on("click", function () {
            $(".navbar-left").slideDown();
            $(".page-wrapper").css("padding-left", "220px");
        });
    });

    $(".navbar-left").on("click", function (activeItem) {
        $("a").addClass("active-item");
    });
});
