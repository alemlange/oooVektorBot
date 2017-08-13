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

    $(".js-table-body").on("click", ".js-dish-drop", function (e) {

        e.preventDefault();
        e.stopPropagation();
        var curDish = $(this);
        var active = $(this).hasClass("menu-selected");
        if (active)
            curDish.removeClass("menu-selected");
        else
            curDish.addClass("menu-selected");
    });

    $(".js-table-body").on("click", ".js-renew-menu", function (e) {

        e.preventDefault();
        var menuHeader = $(this).parents(".menu-header");
        var menuId = menuHeader.data("menuid");
        var allActiveDishes = [];
        menuHeader.find(".js-dish-drop.menu-selected").each(function (i, e) {
            allActiveDishes.push($(e).data("id"));
        });

        var target = $(this).data("target");
        $.post(target, { menuId: menuId, allActiveDishes: allActiveDishes }).done(function (data) {
            $(".js-menu-section").trigger("click");
        });
    });

    $(".js-table-body").on("dblclick", ".js-dish-card", function (e) {

        e.preventDefault();
        var container = $(this);
        var edittarget = container.data("target");
        var dishId = container.data("itemid");

        $.get(edittarget, { dishId: dishId }).done(function (data) {
            container.parent().replaceWith(data);
        });
    });
});
