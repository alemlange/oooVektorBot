$(document).ready(function () {

    $(".js-table-body").on("submit", "#EditMenuForm", function (e) {
        e.preventDefault();

        var form = $("#EditMenuForm");
        $.validator.unobtrusive.parse($('#EditMenuForm'));

        if (form.valid()) {
            var target = form.attr("action");

            var container = form.parent();
            var menuId = container.data("itemid");
            var name = container.find(".js-name").val();

            var defaultMenu = false;
            if (container.find(".js-defaultMenu").is(":checked")) {
                var defaultMenu = true;
            }

            var rest = "00000000-0000-0000-0000-000000000000";
            var chosenRestBtn = container.find(".js-chosen-rest");

            if (chosenRestBtn.lenth !== 0) {
                rest = chosenRestBtn.data("value");
            }

            $.post(target, { menuId: menuId, name: name, defaultMenu: defaultMenu, rest: rest }).done(function (data) {
                $(".js-menu-section").trigger("click");
            }).fail(function (data) {
                AlertModal.text = "Не получилось создать меню!";
                AlertModal.show();
            });
        }
    });

    $(".js-table-body").on("click", ".js-rest-choose", function (e) {
        e.preventDefault();

        var curRest = $(this);
        var restId = curRest.data("restid");
        var restName = curRest.html();

        var dropBtn = $(this).parents(".dropdown").find(".js-chosen-rest");

        dropBtn.val(restName);
        dropBtn.attr("data-value", restId);
    });

});
