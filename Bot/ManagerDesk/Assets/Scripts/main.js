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

    $(".js-table-body").on("click", ".more-menu-btn", function (e) {

        var menuHeader = $(this).parents(".menu-header");
        var menuid = menuHeader.data("menuid");
        var target = menuHeader.data("moretarget");
        $.get(target, { menuid: menuid }).done(function (data) {
            menuHeader.find(".dropdown-menu").html(data);
        });
    });
});
