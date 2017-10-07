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
        var active = curDish.hasClass("menu-selected");
        if (active) {
            curDish.find(".fa-check").hide();
            curDish.removeClass("menu-selected");
        }
        else {
            curDish.find(".fa-check").show();
            curDish.addClass("menu-selected");
        }
            
    });

    $(".js-table-body").on("click", ".js-action", function (e) {

        e.preventDefault();
        e.stopPropagation();
        var action = $(this);
        var active = action.hasClass("menu-selected");
        if (active) {
            action.find(".fa-check").hide();
            action.removeClass("menu-selected");
        }
        else {
            action.find(".fa-check").show();
            action.addClass("menu-selected");
        }

    });

    $(".js-table-body").on("click", ".js-edit-table-actions", function (e) {

        e.preventDefault();
        var table = $(this).parents(".js-table-card");
        var tableId = table.data("itemid");

        var orderProc = table.find(".js-order-proc").hasClass("menu-selected");
        var helpNeeded = table.find(".js-help-needed").hasClass("menu-selected");
        var checkPlease = table.find(".js-check-please").hasClass("menu-selected");

        var target = $(this).data("target");
        $.post(target, { tableId: tableId, orderProc: orderProc, helpNeeded: helpNeeded, checkPlease: checkPlease }).done(function (data) {
            $(".js-tables-section").trigger("click");
        });
    });

    $(".js-table-body").on("click", ".js-set-table-action", function (e) {

        e.preventDefault();

        var inactive = $(this).hasClass("un-checked");
        if (inactive) {
            $(this).removeClass("un-checked");
        }
        else {
            $(this).addClass("un-checked");
        }

        var table = $(this).parents(".js-table-card");
        var tableId = table.data("itemid");

        var value = !$(this).hasClass("un-checked");

        var target = $(this).data("target");
        $.post(target, { tableId: tableId, value: value }).done(function (data) {
            $(".js-tables-section").trigger("click");
        });
    });

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

    $(".js-table-body").on("click", ".js-toolbar-close-table", function (e) {
        e.preventDefault();
        var table = $(this).parents(".js-table-card");
        var tableId = table.data("itemid");

        var target = $(this).data("target");
        $.post(target, { tableId: tableId }).done(function (data) {
            $(".js-tables-section").trigger("click");
        });
        
    });

    setInterval(function () {
        if ($(".js-tables-section.active").length !== 0)
            $(".js-tables-section").trigger("click");
    }, 10000);


    $(".js-table-body").on("change", ".picture-input", function (e) {
        if (this.files && this.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('.settings-profile-img').attr('src', e.target.result);
            }

            reader.readAsDataURL(this.files[0]);
        }
    });

});
