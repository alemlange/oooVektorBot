$(document).ready(function () {
    $(".navbar-top").on("change", ".js-rest-select", function (e) {
        var id = $(".js-rest-select option:selected").data("objid");

        var mngr = new CookieManager();
        mngr.setCookie("CurRest", id, { expires: 2147483647, path: "/" });
        $(".js-tables-section").trigger("click");

        $(".js-rest-select").addClass("js-changed");
    });

    setInterval(function () {
        var target = $(".bot-status").data("target");

        $.get(target).done(function (data) {

            if (data.okStatus != null) {
                if (!data.okStatus) {
                    if ($(".status-popover").find(".popover-content").html() != data.msg) {
                        $(".bot-status").show();

                        $(".bot-status").popover({
                            content: data.msg,
                            title: "Статус бота",
                            placement: "bottom",
                            trigger: "click",
                            template: '<div class="popover status-popover"><div class="arrow"></div><div class="popover-inner"><h3 class="popover-title"></h3><div class="popover-content"><p></p></div></div></div>'
                        });
                        $(".bot-status").popover("show");
                    }
                }
                else {
                    $(".bot-status").hide();
                    $(".bot-status").popover("hide");
                }
            }
        });
    }, 60000);

});
