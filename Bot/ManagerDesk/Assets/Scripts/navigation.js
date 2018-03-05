$(document).ready(function () {

    $(".navbar-left").on("click", ".js-tables-section", function (e) {
        e.preventDefault();

        $(".menu-section.active").removeClass("active");
        $(this).addClass("active");
        $(".navbar-left").slideUp("slow");

        var target = $(this).data("target");
        $.get(target).done(function (data) {
            $(".active-rest").show();
            $(".active-rest-label").show();
            $(".js-table-body").html(data);
            //equalHeight($(".info-item"));
        }).fail(function (ex) {
            AlertModal.text = "Не получилось обновить столики, возможно отсутствует соединение с сетью!";
            AlertModal.show();
            });

        var selectTarget = $(".js-rest-select").data("target");
        $.get(selectTarget).done(function (data) {
            $(".js-rest-select").html(data);
        });
    });

    $(".navbar-left").on("click", ".js-rest-section", function (e) {
        e.preventDefault();

        $(".menu-section.active").removeClass("active");
        $(this).addClass("active");
        $(".navbar-left").slideUp("slow");

        var target = $(this).data("target");
        $.get(target).done(function (data) {
            $(".active-rest").hide();
            $(".active-rest-label").hide();
            $(".js-table-body").html(data);
            equalHeight($(".info-item"));
        });
    });

    $(".navbar-left").on("click", ".js-menu-section", function (e) {
        e.preventDefault();

        $(".menu-section.active").removeClass("active");
        $(this).addClass("active");
        $(".navbar-left").slideUp("slow");

        var target = $(this).data("target");
        $.get(target).done(function (data) {
            $(".active-rest").hide();
            $(".active-rest-label").hide();
            $(".js-table-body").html(data);
            equalHeight($(".info-item"));
        });
    });

    $(".navbar-left").on("click", ".js-dish-section", function (e) {
        e.preventDefault();

        $(".menu-section.active").removeClass("active");
        $(this).addClass("active");
        $(".navbar-left").slideUp("slow");

        var target = $(this).data("target");
        $.get(target).done(function (data) {
            $(".active-rest").hide();
            $(".active-rest-label").hide();
            $(".js-table-body").html(data);
            equalHeight($(".info-item"));
        });
    });

    $(".navbar-left").on("click", ".js-mod-section", function (e) {
        e.preventDefault();

        $(".menu-section.active").removeClass("active");
        $(this).addClass("active");
        $(".navbar-left").slideUp("slow");

        var target = $(this).data("target");
        $.get(target).done(function (data) {
            $(".active-rest").hide();
            $(".active-rest-label").hide();
            $(".js-table-body").html(data);
            equalHeight($(".info-item"));
        });
    });

    $(".navbar-left").on("click", ".js-disp-section", function (e) {
        e.preventDefault();

        $(".menu-section.active").removeClass("active");
        $(this).addClass("active");
        $(".navbar-left").slideUp("slow");

        var target = $(this).data("target");
        $.get(target).done(function (data) {
            $(".active-rest").hide();
            $(".active-rest-label").hide();
            $(".js-table-body").html(data);
            equalHeight($(".info-item"));
        });
    });

    $(".navbar-left").on("click", ".js-feed-section", function (e) {
        e.preventDefault();

        $(".menu-section.active").removeClass("active");
        $(this).addClass("active");
        $(".navbar-left").slideUp("slow");

        var target = $(this).data("target");
        $.get(target).done(function (data) {
            $(".active-rest").hide();
            $(".active-rest-label").hide();
            $(".js-table-body").html(data);
            equalHeight($(".info-item"));
        });
    });

    $(".navbar-left").on("click", ".js-book-section", function (e) {
        e.preventDefault();

        $(".menu-section.active").removeClass("active");
        $(this).addClass("active");
        $(".navbar-left").slideUp("slow");

        var target = $(this).data("target");
        $.get(target).done(function (data) {
            $(".active-rest").hide();
            $(".active-rest-label").hide();
            $(".js-table-body").html(data);
            equalHeight($(".info-item"));
        });
    });

    $(".navbar-left").on("click", ".js-cheque-section", function (e) {
        e.preventDefault();

        $(".menu-section.active").removeClass("active");
        $(this).addClass("active");
        $(".navbar-left").slideUp("slow");

        var target = $(this).data("target");
        $.get(target).done(function (data) {
            $(".active-rest").hide();
            $(".active-rest-label").hide();
            $(".js-table-body").html(data);
            equalHeight($(".info-item"));
        });
    });

    $(".navbar-left").on("click", ".js-add-plus", function (e) {
        e.preventDefault();

        $(".menu-section.active").removeClass("active");
        $(this).parent().find(".menu-section").addClass("active");
    });

    $(".navbar-top").on("click", ".js-config-section", function (e) {
        e.preventDefault();

        $(".menu-section.active").removeClass("active");
        $("li.active-item").removeClass("active-item");

        var target = $(this).data("target");
        $.get(target).done(function (data) {
            $(".active-rest").hide();
            $(".active-rest-label").hide();
            $(".js-table-body").html(data);
            //equalHeight($(".info-item"));
        });
    });

    $(".navbar-left").on("click", ".toolbar-add", function (e) {
        e.preventDefault();
        var activeSection = $(this).data("type");
        $(".navbar-left").slideUp("slow");

        var target = $(this).data("target");
        $.get(target, { activeSection: activeSection }).done(function (data) {
            $(".js-table-body").html(data);
        });
    });
});
