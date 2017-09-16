$(document).ready(function () {

    $(".navbar-left").on("click", ".menu-section", function (e) {
        e.preventDefault();

        $(".menu-section.active").removeClass("active");
        $(this).addClass("active");

        var target = $(this).data("target");
        $.get(target).done(function (data) {
            $(".js-table-body").html(data);
        });
    });
});
