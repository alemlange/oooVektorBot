$(document).ready(function () {

    $(".navbar-left-btn").on("click", function () {
        if ($(this).hasClass("js-opened")) {
            $(this).removeClass("js-opened");

            $(".navbar-left").slideToggle("slow", function () { $(".page-wrapper").css("padding-left", "0px"); });
            
        }
        else {
            $(this).addClass("js-opened");
            $(".page-wrapper").css("padding-left", "220px");
            $(".navbar-left").slideToggle("slow");
        }
    });

    $(".menu li").on("click", function (activeItem) {
        $(".active-item").removeClass("active-item");
        $(this).addClass("active-item");
    });

    //$(".js-table-body").on("click", ".info-item", function (e) {
    //    //e.stopPropagation();
    //    if ($(this).hasClass("chosen-card"))
    //        $(this).removeClass("chosen-card");
    //    else {
    //        $(".info-item").removeClass("chosen-card");
    //        $(this).addClass("chosen-card");
    //    }
    //});

    $(".js-table-body").on("click", ".menu-more-dishes", function (e) {

        var menuHeader = $(this).parents(".js-menu-card");
        var menuid = menuHeader.data("itemid");
        var target = menuHeader.data("moretarget");
        $.get(target, { menuid: menuid }).done(function (data) {
            menuHeader.find(".dropdown-menu").html(data);
        });
    });

    $(".js-table-body").on("click", ".table-actions", function (e) {

        var table = $(this).parents(".js-table-card");
        var tableid = table.data("itemid");
        var target = table.data("actionstarget");
        $.get(target, { tableid: tableid }).done(function (data) {
            table.find(".dropdown-menu").html(data);
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

    //$(".js-table-body").on("dblclick", ".js-dish-card", function (e) {

    //    e.preventDefault();
    //    var container = $(this);
    //    var edittarget = container.data("target");
    //    var dishId = container.data("itemid");

    //    $.get(edittarget, { dishId: dishId }).done(function (data) {
    //        container.parent().replaceWith(data);
    //    });
    //});

    $(".js-table-body").on("click", ".js-edit-dish", function (e) {

        e.preventDefault();
        var container = $(this).parents(".js-dish-card");
        var edittarget = container.data("edittarget");
        var dishId = container.data("itemid");

        $.get(edittarget, { dishId: dishId }).done(function (data) {
            container.parent().replaceWith(data);
        });
    });

    $(".js-table-body").on("click", ".js-edit-menu", function (e) {

        e.preventDefault();
        var container = $(this).parents(".js-menu-card");
        var edittarget = container.data("edittarget");
        var menuId = container.data("itemid");

        $.get(edittarget, { menuId: menuId }).done(function (data) {
            container.parent().replaceWith(data);
        });
    });

    $(".js-table-body").on("click", ".js-edit-rest", function (e) {

        e.preventDefault();
        var container = $(this).parents(".js-rest-card");
        var edittarget = container.data("edittarget");
        var restId = container.data("itemid");

        $.get(edittarget, { restId: restId }).done(function (data) {
            container.parent().replaceWith(data);
        });
    });

    $(".js-table-body").on("click", ".toolbar-delete", function (e) {
        e.preventDefault();
        var chosenCard = $(this).parents(".info-item");

        if (chosenCard.length !== 0) {
            var itemId = chosenCard.data("itemid");
            var itemType = chosenCard.data("type");

            var target = $(this).data("target");
            $.post(target, { itemId: itemId, itemType: itemType }).done(function (data) {
                $(".menu-section.active").trigger("click");
            });
        }

    });
});
