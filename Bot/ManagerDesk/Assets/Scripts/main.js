$(document).ready(function () {
    $(".navbar-left-btn").on("click", function () {
        $(".navbar-left").slideUp();
        $(".info-main, .footer-bottom, .copyright").css("margin-left", "0");
        $(".navbar-left-btn").on("click", function () {
            $(".navbar-left").slideDown();
            $(".info-main, .footer-bottom").css("margin-left", "220px");
        });
    });

    $(".info-item").fadeIn(1000);
});
