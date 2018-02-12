$(document).ready(function () {

    $(".js-table-body").on("submit", "#EditModificatorForm", function (e) {
        e.preventDefault();

        var container = $("#EditModificatorForm");
        $.validator.unobtrusive.parse($('#EditModificatorForm'));

        if (container.valid()) {
            var target = container.attr("action");
            var form = container.serialize();

            $.post(target, form).done(function (data) {
                $(".js-mod-section").trigger("click");
            }).fail(function (data) {
                AlertModal.text = "Не получилось создать модификатор";
                AlertModal.show();
            });
        }
    });
});
