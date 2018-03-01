$(document).ready(function () {

    $(".navbar-left-btn").on("click", function () {
        if ($(this).hasClass("js-opened")) {
            $(this).removeClass("js-opened");
            $(".navbar-left").slideToggle("slow");/*, function () { $(".page-wrapper").css("padding-left", "0px"); });*/
        }
        else {
            $(this).addClass("js-opened");
            //$(".page-wrapper").css("padding-left", "220px");
            $(".navbar-left").slideToggle("slow");
        }
    });

    $(".menu li").on("click", function (activeItem) {
        $(".active-item").removeClass("active-item");
        $(this).addClass("active-item");
    });

    $(".js-table-body").on("click", ".menu-more-dishes", function (e) {

        $(".js-menu-card").removeClass("on-top");
        var menuCard = $(this).parents(".js-menu-card");
        menuCard.addClass("on-top");

        var menuid = menuCard.data("itemid");
        var target = menuCard.data("moretarget");
        $.get(target, { menuid: menuid }).done(function (data) {
            menuCard.find(".dish-dropdown").html(data);

            menuCard.find(".ul-dishes").each(function (ul) {
                var sortable = Sortable.create(this);
            })
        });
    });

    $(".js-table-body").on("click", ".dish-edit-mods", function (e) {

        $(".js-dish-card").removeClass("on-top");
        var dishCard = $(this).parents(".js-dish-card");
        dishCard.addClass("on-top");

        var dishid = dishCard.data("itemid");
        var target = dishCard.data("modstarget");
        $.get(target, { dishid: dishid }).done(function (data) {
            dishCard.find(".dish-dropdown").html(data);
        });
    });

    $(".js-table-body").on("click", ".js-mod-drop", function (e) {

        e.preventDefault();
        e.stopPropagation();
        var curMod = $(this);
        var active = curMod.hasClass("menu-selected");
        if (active) {
            curMod.find(".fa-check").hide();
            curMod.removeClass("menu-selected");
        }
        else {
            curMod.find(".fa-check").show();
            curMod.addClass("menu-selected");
        }

    });

    $(".js-table-body").on("click", ".js-renew-mods", function (e) {

        e.preventDefault();
        var dishCard = $(this).parents(".js-dish-card");
        var dishId = dishCard.data("itemid");
        var allActiveMods = [];
        dishCard.find(".js-mod-drop.menu-selected").each(function (i, e) {
            allActiveMods.push($(e).data("id"));
        });

        var target = $(this).data("target");
        $.post(target, { dishId: dishId, allActiveMods: allActiveMods }).done(function (data) {
            $(".js-dish-section").trigger("click");
        });
    });

    $(".js-table-body").on("click", ".menu-cat-list", function (e) {
        var menuCard = $(this).parents(".js-menu-card");
        var menuid = menuCard.data("itemid");
        var target = $(this).data("cattarget");

        $.get(target, { menuid: menuid }).done(function (data) {
            var catList = menuCard.find(".cat-list-dropdown");
            catList.html(data);
            
            var sortable = Sortable.create(catList[0]);
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

    $(".js-table-body").on("click", ".js-save-cat-list", function (e) {

        e.preventDefault();
        var menuCard = $(this).parents(".js-menu-card");
        var menuId = menuCard.data("itemid");
        var target = $(this).data("edittarget");

        var sortedCat = [];
        menuCard.find(".js-cat-value").each(function (i, e) {
            var cat = $(e).data("value");
            sortedCat.push(cat);  
        });

        $.post(target, { menuId: menuId, sortedCat: sortedCat }).done(function (data) {
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

        $(".js-table-body").find(".js-dish-form").replaceWith("");
        $(".js-dish-card.hidden").removeClass("hidden");

        var container = $(this).parents(".js-dish-card");
        var edittarget = container.data("edittarget");
        var dishId = container.data("itemid");

        $.get(edittarget, { dishId: dishId }).done(function (data) {
            container.addClass("hidden");
            container.parent().append(data);
        });
    });

    $(".js-table-body").on("click", ".js-edit-mod", function (e) {

        e.preventDefault();

        $(".js-table-body").find(".js-mod-form").replaceWith("");
        $(".js-mod-card.hidden").removeClass("hidden");

        var container = $(this).parents(".js-mod-card");
        var edittarget = container.data("edittarget");
        var modId = container.data("itemid");

        $.get(edittarget, { modId: modId }).done(function (data) {
            container.addClass("hidden");
            container.parent().append(data);
        });
    });

    $(".js-table-body").on("click", ".js-edit-menu", function (e) {

        e.preventDefault();

        $(".js-table-body").find(".js-menu-form").replaceWith("");
        $(".js-menu-card.hidden").removeClass("hidden");

        var container = $(this).parents(".js-menu-card");
        var edittarget = container.data("edittarget");
        var menuId = container.data("itemid");

        $.get(edittarget, { menuId: menuId }).done(function (data) {
            container.addClass("hidden");
            container.parent().append(data);
        });
    });

    $(".js-table-body").on("click", ".js-edit-rest", function (e) {

        e.preventDefault();

        $(".js-table-body").find(".js-rest-form").replaceWith("");
        $(".js-rest-card.hidden").removeClass("hidden");

        var container = $(this).parents(".js-rest-card");
        var edittarget = container.data("edittarget");
        var restId = container.data("itemid");

        $.get(edittarget, { restId: restId }).done(function (data) {
            container.addClass("hidden");
            container.parent().append(data);
        });
    });

    $(".js-table-body").on("click", ".toolbar-delete", function (e) {
        e.preventDefault();

        var chosenCard = $(this).parents(".info-item");

        var type = chosenCard.data("type");
        if (type == "Restaurant") {
            DeleteModal.setText("Удалить этот ресторан? При удалении ресторана произойдет удаление всех связанных с ним столиков!");
        }
        else {
            DeleteModal.setText("Удалить этот элемент?");
        }
        DeleteModal.show(chosenCard.data("itemid"), type, $(this).data("target"))
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

    $(".js-table-body").on("click", ".js-toolbar-close-booking", function (e) {
        e.preventDefault();
        var book = $(this).parents(".js-book-card");
        var bookId = book.data("itemid");

        var target = $(this).data("target");
        $.post(target, { bookId: bookId }).done(function (data) {
            $(".js-book-section").trigger("click");
        });

    });

    function UpdateTables() {

        var target = $(".js-tables-section").data("updatetarget");
        $.get(target).done(function (data) {

            if (data.tablesView != null) {
                $(".active-rest").show();
                $(".active-rest-label").show();
                $(".js-table-body").html(data.tablesView);
            }

            if (data.newTables) {
                notif("Table");
            }

            if (data.restOptionsView != null) {
                $(".js-rest-select").html(data.restOptionsView);
            }

        }).fail(function (ex) {
            AlertModal.text = "Не получилось обновить столики, возможно отсутствует соединение с сетью!";
            AlertModal.show();
        });
    }

    function UpdateBookings() {
        var target = $(".js-book-section").data("updatetarget");
        $.get(target).done(function (data) {

            if (data.newBookings) {
                notif("Book");
            }

        }).fail(function (ex) {
            AlertModal.text = "Не получилось обновить бронирования, возможно отсутствует соединение с сетью!";
            AlertModal.show();
        });
    }

    setInterval(function () {
        if ($(".js-tables-section.active").length !== 0)
            UpdateTables();

        UpdateBookings();
    }, 10000);


    $(".js-table-body").on("change", ".picture-input", function (e) {
        if (this.files && this.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('.settings-profile-img').attr('src', e.target.result);
            };

            reader.readAsDataURL(this.files[0]);
        }
    });

    $(".js-table-body").on("change", ".picture-input", function (e) {
        if (this.files && this.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('.settings-profile-img').attr('src', e.target.result);
            };

            reader.readAsDataURL(this.files[0]);
        }
    });

    $(".js-table-body").on("click", function (e) {
        if (e.target.className == "row") {
            var activeSection = $(".menu-section.active");

            if (activeSection.hasClass("js-dish-section")) {
                $(".js-table-body").find(".js-dish-form").replaceWith("");
                $(".js-dish-card.hidden").removeClass("hidden");
            }
            else if (activeSection.hasClass("js-menu-section")){
                $(".js-table-body").find(".js-menu-form").replaceWith("");
                $(".js-menu-card.hidden").removeClass("hidden");
            }
            else if (activeSection.hasClass("js-rest-section")){
                $(".js-table-body").find(".js-rest-form").replaceWith("");
                $(".js-rest-card.hidden").removeClass("hidden");
            }
            
        }
    });

    $(".js-table-body").on("click", ".js-start-bot", function (e) {
        e.preventDefault();
        var target = $(this).data("target");

        $.get(target).done(function (data) {
            if (data.okStatus != null) {
                alert(data.msg);
            }
        });
    });

});

function notif(type) {
    var audioFile = "";

    if (type == "Table")
        audioFile = "/Assets/AudioNotification/newtable.mp3"
    else if (type == "Book")
        audioFile = "/Assets/AudioNotification/newbooking.mp3"

    var audio = new Audio(audioFile);
    audio.play();

    if (Notification.permission !== "granted")
        Notification.requestPermission();
    else {
        var text = "";
        if (type == "Table")
            text = "Есть необработанные заказы."
        else if (type == "Book")
            text = "Есть необработанные бронирования."

        var notification = new Notification('ДайнерБот', {
            icon: '/Assets/Imgs/favicon.ico',
            body: text,
        });

        notification.onclick = function () {
            window.open(window.location.href);
        };

    }
}

function RequestNotif() {
    if (Notification.permission !== "granted")
        Notification.requestPermission();
}
