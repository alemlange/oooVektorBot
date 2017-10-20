$(document).ready(function () {

    $(".navbar-left").on("click", ".menu-section", function (e) {
        e.preventDefault();

        $(".menu-section.active").removeClass("active");
        $(this).addClass("active");

        var target = $(this).data("target");
        $.get(target).done(function (data) {
            $(".js-table-body").html(data);
            equalHeight($(".info-item"));
        });
    });

    $(".navbar-top").on("click", ".js-config-section", function (e) {
        e.preventDefault();

        $(".menu-section.active").removeClass("active");

        var target = $(this).data("target");
        $.get(target).done(function (data) {
            $(".js-table-body").html(data);
            equalHeight($(".info-item"));
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
