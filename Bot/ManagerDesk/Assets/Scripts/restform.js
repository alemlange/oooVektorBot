$(document).ready(function () {

    $(".js-table-body").on("click", ".js-save-rest", function (e) {
        e.preventDefault();

        var container = $(this).parents(".js-rest-form");
        var id = container.data("itemid");
        var name = container.find(".js-name").val();
        var address = container.find(".js-address").val();
        var tblCount = container.find(".js-table-count").val();
        var description = container.find(".js-description").val();

        var rest = { Id: id, Name: name, Address: address, TableCount: tblCount, Description: description };

        var target = $(this).data("target");
        $.post(target, { Rest: rest }).done(function (data) {
            $(".js-rest-section").trigger("click");
        });
    });
});
