$(document).ready(function () {

    $(".js-table-body").on("click", ".js-save", function (e) {
        e.preventDefault();

        var container = $(this).parents(".js-dish-form");
        var dishId = container.data("itemid");
        var name = container.find(".js-name").val();
        var slashName = container.find(".js-slash-name").val();
        var price = container.find(".js-price").val();
        var description = container.find(".js-description").val();

        var target = $(this).data("target");
        $.post(target, { dishId: dishId, name: name, slashName: slashName, price: price, description: description }).done(function (data) {
            $(".js-dish-section").trigger("click");
        });
    });

});
