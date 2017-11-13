$(document).ready(function () {

    $(".js-table-body").on("submit", "#EditDishForm", function (e) {
        e.preventDefault();

        var container = $("#EditDishForm");
        $.validator.unobtrusive.parse($('#EditDishForm'));

        if (container.valid()) {
            var target = container.attr("action");
            var form = container.serialize();

            $.post(target, form).done(function (data) {
                $(".js-dish-section").trigger("click");
            }).fail(function (data) {
                AlertModal.text = "Не получилось создать блюдо";
                AlertModal.show();
            });
        }
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
        var container = $(this).parents(".js-dish-form");
        var picture = container.find(".js-insta-picture");

        if (value.includes("https://www.instagram.com/p/")) {
            picture.attr("src", value + "/media");
        }
        else {
            picture.attr("src", "/Assets/Imgs/insta_placeholder.png");
        }
    });
});
