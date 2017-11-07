$(document).ready(function () {

    $(".navbar-left").on("click", ".js-tables-section", function (e) {
        e.preventDefault();

        $(".menu-section.active").removeClass("active");
        $(this).addClass("active");

        var target = $(this).data("target");
        $.get(target).done(function (data) {
            $(".active-rest").show();
            $(".js-table-body").html(data);
            equalHeight($(".info-item"));
        });
    });

    $(".navbar-left").on("click", ".js-rest-section", function (e) {
        e.preventDefault();

        $(".menu-section.active").removeClass("active");
        $(this).addClass("active");

        var target = $(this).data("target");
        $.get(target).done(function (data) {
            $(".active-rest").hide();
            $(".js-table-body").html(data);
            equalHeight($(".info-item"));
        });
    });

    $(".navbar-left").on("click", ".js-menu-section", function (e) {
        e.preventDefault();

        $(".menu-section.active").removeClass("active");
        $(this).addClass("active");

        var target = $(this).data("target");
        $.get(target).done(function (data) {
            $(".active-rest").hide();
            $(".js-table-body").html(data);
            equalHeight($(".info-item"));
        });
    });

    $(".navbar-left").on("click", ".js-dish-section", function (e) {
        e.preventDefault();

        $(".menu-section.active").removeClass("active");
        $(this).addClass("active");

        var target = $(this).data("target");
        $.get(target).done(function (data) {
            $(".active-rest").hide();
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
            $(".js-table-body").html(data);
            //equalHeight($(".info-item"));
        });
    });

    $(".navbar-left").on("click", ".toolbar-add", function (e) {
        e.preventDefault();
        var activeSection = $(this).data("type");

        var target = $(this).data("target");
        $.get(target, { activeSection: activeSection }).done(function (data) {
            $(".js-table-body").html(data);
        });
    });
});
