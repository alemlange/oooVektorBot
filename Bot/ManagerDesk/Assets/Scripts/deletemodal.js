$(document).ready(function () {

    $("#deletemodal").on("click", ".js-ok", function (e) {
        e.preventDefault();

        var chosenCard = $("#deletemodal");
        var itemId = chosenCard.data("itemid");
        var itemType = chosenCard.data("type");
        var target = chosenCard.data("target");

        $.post(target, { itemId: itemId, itemType: itemType }).done(function (data) {
            $(".menu-section.active").trigger("click");
        });

    });
});
