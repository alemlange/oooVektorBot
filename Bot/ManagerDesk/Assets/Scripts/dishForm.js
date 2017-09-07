$(document).ready(function () {

    $(".js-table-body").on("click", ".js-save-dish", function (e) {
        e.preventDefault();

        var container = $(this).parents(".js-dish-form");
        var dishId = container.data("itemid");
        var name = container.find(".js-name").val();
        var category = container.find(".js-category").val();
        var slashName = container.find(".js-slash-name").val();
        var pictureUrl = container.find(".js-picture-url").val();
        var price = container.find(".js-price").val();
        var description = container.find(".js-description").val();

        var dish = { Category: category, Id: dishId, Name: name, Description: description, PictureUrl: pictureUrl, Price: price, SlashName: slashName };

        var target = $(this).data("target");
        $.post(target, { Dish: dish }).done(function (data) {
            $(".js-dish-section").trigger("click");
        });
    });

});
