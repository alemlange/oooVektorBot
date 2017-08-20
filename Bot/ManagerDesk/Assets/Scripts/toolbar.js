$(document).ready(function () {

    $(".header-top").on("click", ".toolbar-add", function (e) {
        e.preventDefault();
        var activeSection = $(".menu-section.active").data("type");

        var target = $(this).data("target");
        $.get(target, { activeSection: activeSection }).done(function (data) {
            $(".js-table-body").html(data);
        });
    });

    $(".header-top").on("click", ".toolbar-delete", function (e) {
        e.preventDefault();
        var chosenCard = $(".js-table-body").find(".chosen-card");

        if (chosenCard.length !== 0) {
            var itemId = chosenCard.data("itemid");
            var itemType = $(".menu-section.active").data("type");

            var target = $(this).data("target");
            $.post(target, { itemId: itemId, itemType: itemType }).done(function (data) {
                $(".menu-section.active").trigger("click");
            });
        }
        
    });
});
