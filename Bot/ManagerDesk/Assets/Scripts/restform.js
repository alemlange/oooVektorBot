$(document).ready(function () {

    $(".js-table-body").on("submit", "#EditRestForm", function (e) {
        e.preventDefault();

        var container = $("#EditRestForm");
        $.validator.unobtrusive.parse($('#EditRestForm'))
        //var target = container.attr("action");
        //var form = container.serialize();

        if (container.valid()) {
            alert(1);
        }
        else {
            alert(2);
        }
        return;
        $.post(target, container.serialize()).done(function (data) {
            $(".js-rest-section").trigger("click");
        });

        //var id = container.data("itemid");
        //var name = container.find(".js-name").val();
        //var address = container.find(".js-address").val();
        //var code = container.find(".js-code").val();
        //var tblCount = container.find(".js-table-count").val();
        //if (tblCount === "" || tblCount === null)
        //    tblCount = 0;
        //var description = container.find(".js-description").val();

        //var rest = { Id: id, Name: name, Address: address, Code: code, TableCount: tblCount, Description: description };

        //var target = $(this).data("target");
        //$.post(target, { Rest: rest }).done(function (data) {
        //    $(".js-rest-section").trigger("click");
        //});
    });
});
