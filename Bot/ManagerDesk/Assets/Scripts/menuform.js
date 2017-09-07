$(document).ready(function () {

    $(".js-table-body").on("click", ".js-save-menu", function (e) {
        e.preventDefault();

        var container = $(this).parents(".js-menu-form");
        var menuId = container.data("itemid");
        var name = container.find(".js-name").val();

        var rest = "00000000-0000-0000-0000-000000000000";
        var chosenRestBtn = container.find(".js-chosen-rest");

        if (chosenRestBtn.lenth !== 0) {
            rest = chosenRestBtn.data("value");
        }

        var target = $(this).data("target");
        $.post(target, { menuId: menuId, name: name, rest: rest}).done(function (data) {
            $(".js-menu-section").trigger("click");
        });
    });

    $(".js-table-body").on("click", ".js-rest-choose", function (e) {
        e.preventDefault();

        var curRest = $(this);
        var restId = curRest.data("restid");
        var restName = curRest.html();

        var dropBtn = $(this).parents(".dropdown").find(".js-chosen-rest");

        dropBtn.html(restName + " <span class='caret'></span>");
        dropBtn.data("value", restId);
    });

});
