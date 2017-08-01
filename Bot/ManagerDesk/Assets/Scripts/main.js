$(document).ready(function () {

    $(".navbar-left-btn").on("click", function (slideNavbar) {
        $(".navbar-left").toggle("left");
        $(".page-wrapper").css("padding-left", "220px");
        //$(".navbar-left-btn").on("click", function () {
        //    $(".navbar-left").slideDown();
        //    $(".page-wrapper").css("padding-left", "220px");
        //});
    });

    $(".menu li").on("click", function (activeItem) {
        $(".active-item").removeClass("active-item")
        $(this).addClass("active-item");
    });
});
