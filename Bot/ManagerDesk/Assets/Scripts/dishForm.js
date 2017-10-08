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
        if (price === "" || price === null)
            price = 0;
        var description = container.find(".js-description").val();

        var dish = { Category: category, Id: dishId, Name: name, Description: description, PictureUrl: pictureUrl, Price: price, SlashName: slashName };

        var target = $(this).data("target");
        $.post(target, { Dish: dish }).done(function (data) {
            $(".js-dish-section").trigger("click");
        });
    });

    $(".js-table-body").on("click", ".js-cat-choose", function (e) {
        e.preventDefault();

        var curCat = $(this);
        var catName = curCat.data("value");

        var catInput = curCat.parents(".js-dish-form").find(".js-category");
        catInput.val(catName);
    });

    $(".js-table-body").on("change", ".js-picture-url", function (e) {

        var value = $(this).val();
        if (value.includes("https://www.instagram.com/p/")) {
            var container = $(this).parents(".js-dish-form");

            var picture = container.find(".js-insta-picture");

            picture.attr("src", value + "/media");
        }
    });

});
