$(document).ready(function () {
    $(".navbar-top").on("change", ".js-rest-select", function (e) {
        var id = $(".js-rest-select option:selected").data("objid");

        var mngr = new CookieManager();
        mngr.setCookie("CurRest", id, { expires: 2147483647, path: "/" });
        $(".js-tables-section").trigger("click");

        $(".js-rest-select").addClass("js-changed");
    });

    //$(".navbar-top").on("click", ".js-rest-select", function (e) {
    //    var select = $(this);
    //    if (!select.hasClass("js-changed")) {

    //        var target = select.data("target");
    //        $.get(target).done(function (data) {
    //            select.html(data);
    //        });
    //    }
    //    else {
    //        select.removeClass("js-changed");
    //    }
    //});

});
