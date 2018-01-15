$(document).ready(function () {

    $(".js-table-body").on("submit", "#EditDispForm", function (e) {
        e.preventDefault();

        var container = $("#EditDispForm");
        $.validator.unobtrusive.parse($('#EditDispForm'));

        if (container.valid()) {
            var target = container.attr("action");
            var form = container.serialize();

            $.post(target, form).done(function (data) {
                $(".js-disp-section").trigger("click");
            }).fail(function (data) {
                AlertModal.text = "Не получилось создать рассылку!";
                AlertModal.show();
            });
        }
    });
});
