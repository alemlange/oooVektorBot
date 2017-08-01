$(document).ready(function () {

    $(".navbar-left-btn").on("click", function (slideNavbar) {
        $(".navbar-left").toggle("left");
        $(".page-wrapper").css("padding-left", "220px");
        //$(".navbar-left-btn").on("click", function () {
        //    $(".navbar-left").slideDown();
        //    $(".page-wrapper").css("padding-left", "220px");
        //});
    });

    $(".navbar-left").on("click", function (activeItem) {
        $("a").addClass("active-item");
    });
});
