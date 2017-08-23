$(document).ready(function () {

    $(".navbar-left-btn").on("click", function () {
        if ($(this).hasClass("js-opened")) {
            $(this).removeClass("js-opened");
            $(".navbar-left").toggle("left");
            $(".page-wrapper").css("padding-left", "0px");
        }
        else {
            $(this).addClass("js-opened");
            $(".navbar-left").slideDown();
            
            $(".page-wrapper").css("padding-left", "220px");
        }
    });

    $(".menu li").on("click", function (activeItem) {
        $(".active-item").removeClass("active-item");
        $(this).addClass("active-item");
    });

    $(".js-table-body").on("click", ".info-item", function (e) {
        //e.stopPropagation();
        if ($(this).hasClass("chosen-card"))
            $(this).removeClass("chosen-card");
        else {
            $(".info-item").removeClass("chosen-card");
            $(this).addClass("chosen-card");
        }
    });

    $(".js-table-body").on("click", ".more-menu-btn", function (e) {

        var menuHeader = $(this).parents(".js-menu-card");
        var menuid = menuHeader.data("itemid");
        var target = menuHeader.data("moretarget");
        $.get(target, { menuid: menuid }).done(function (data) {
            menuHeader.find(".dropdown-menu").html(data);
        });
    });


    $(".js-table-body").on("click", ".js-renew-menu", function (e) {

        e.preventDefault();
        var menuHeader = $(this).parents(".js-menu-card");
        var menuId = menuHeader.data("itemid");
        var allActiveDishes = [];
        menuHeader.find(".js-dish-drop.menu-selected").each(function (i, e) {
            allActiveDishes.push($(e).data("id"));
        });

        var target = $(this).data("target");
        $.post(target, { menuId: menuId, allActiveDishes: allActiveDishes }).done(function (data) {
            $(".js-menu-section").trigger("click");
        });
    });

    $(".js-table-body").on("click", ".js-dish-drop", function (e) {

        e.preventDefault();
        e.stopPropagation();
        var curDish = $(this);
        var active = $(this).hasClass("menu-selected");
        if (active) {
            curDish.find(".fa-check").hide();
            curDish.removeClass("menu-selected");
        }
        else {
            curDish.find(".fa-check").show();
            curDish.addClass("menu-selected");
        }
            
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

    $(".js-table-body").on("click", ".js-eddit-card", function (e) {

        e.preventDefault();
        $(this).parent().trigger("dblclick");
    });
});
