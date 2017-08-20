$(document).ready(function () {

    $(".js-table-body").on("click", ".js-save", function (e) {
        e.preventDefault();

        var container = $(this).parents(".js-menu-form");
        var menuId = container.data("itemid");
        var name = container.find(".js-name").val();

        var target = $(this).data("target");
        $.post(target, { menuId: menuId, name: name }).done(function (data) {
            $(".js-menu-section").trigger("click");
        });
    });

});
