$(document).ready(function () {
    $(".navbar-top").on("change", ".js-rest-select", function (e) {
        var id = $(".js-rest-select option:selected").data("objid");

        var mngr = new CookieManager();
        mngr.setCookie("CurRest", id, { expires: 2147483647, path: "/" });
        $(".js-tables-section").trigger("click");
    });
});
