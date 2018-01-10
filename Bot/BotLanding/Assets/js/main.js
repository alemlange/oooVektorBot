jQuery(document).ready(function() {

	$('.colors li a').click(function(e){
		e.preventDefault();
		$(this).parent().parent().find('a').removeClass('active');
		$(this).addClass('active');
	})


	$('a[href*=#]').bind("click", function(e){
		var anchor = $(this);
		$('html, body').stop().animate({
			scrollTop: $(anchor.attr('href')).offset().top - 50
		}, 1500);
		e.preventDefault();
	});

    $(window).scroll(function() {
      if ($(this).scrollTop() > 100) {
        $('.menu-top').addClass('menu-shrink');
      } else {
        $('.menu-top').removeClass('menu-shrink');
      }
    });

	$(document).on('click','.navbar-collapse.in',function(e) {
		if( $(e.target).is('a') && $(e.target).attr('class') != 'dropdown-toggle' ) {
			$(this).collapse('hide');
		}
	});
    
    $('.js-close-modal').on('click', function () {
        $("#MessageModal").hide();
    });

    $('.js-try').on('click', function () {
        $("#MessageModal").show();

        var container = $("#MessageModal");
        container.data("btntp", $(this).data("btntp"));
        container.find(".js-send-request").show();
        container.find(".js-spinner").hide();
        container.find(".request-success").hide();
        container.find(".fields-request").show();
    });

    $('.js-send-request').on('click', function () {

        var container = $("#MessageModal");
        container.find(".js-send-request").hide();
        container.find(".js-spinner").show();
        var type = container.data("btntp");

        var orgName = container.find(".js-org-name").val();
        var email = container.find(".js-email").val();
        var comment = container.find(".js-comment").val();
        var target = $(this).data("target");

        $.post(target, { orgName: orgName, email: email, comment: comment,type: type }).done(function (data) {

            container.find(".request-success").show();
            container.find(".fields-request").hide();
            
            container.find(".js-send-request").hide();
            container.find(".js-spinner").hide();

            setTimeout(function () {
                container.hide();
            }, 3000);
        });
    });

    $('.slick-screens').slick({
        dots: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        autoplay: true,
        arrows: false,
        autoplaySpeed: 3000
    });

    $('.slick-screens-admin').slick({
        dots: true,
        slidesToShow: 2,
        slidesToScroll: 1,
        autoplay: true,
        arrows: false,
        autoplaySpeed: 3000
    });
});

