$(document).ready(function () {

    $(".js-table-body").on("submit", "#EditRestForm", function (e) {
        e.preventDefault();

        var container = $("#EditRestForm");
        $.validator.unobtrusive.parse($('#EditRestForm'));

        if (container.valid()) {
            var target = container.attr("action");
            var form = container.serialize();

            $.post(target, form).done(function (data) {
                $(".js-rest-section").trigger("click");
            }).fail(function (data) {
                AlertModal.text = "Не получилось создать ресторан!";
                AlertModal.show();
            });
        }
    });
});
