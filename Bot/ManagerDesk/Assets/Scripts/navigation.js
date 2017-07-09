$(document).ready(function () {

    $(".js-table-section").on("click", function (e) {
        e.preventDefault();
        $(".fa.fa-envelope").removeClass("active");
        $(this).addClass("active");

        var target = $(this).data("target");
        $.get(target).done(function (data) {
            $("js - table - body").html(data);
        });
    });

    $(".navbar-left").on("click", ".js-menu-section",function (e) {
        e.preventDefault();
        $(".fa.fa-envelope").removeClass("active");
        $(this).addClass("active");

        var target = $(this).data("target");
        $.get(target).done(function (data) {
            $(".js-table-body").html(data);
        });
    });
});
