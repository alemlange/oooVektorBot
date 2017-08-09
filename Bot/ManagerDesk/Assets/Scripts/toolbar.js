$(document).ready(function () {

    $(".header-top").on("click", ".toolbar-add", function (e) {
        e.preventDefault();
        var activeSection = $(".menu-section.active").data("type");

        var target = $(this).data("target");
        $.get(target, { activeSection: activeSection }).done(function (data) {
            $(".js-table-body").html(data);
        });
    });

});
