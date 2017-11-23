jQuery(document).ready(function() {

	//*****  STYLE SWITCHER  *****//

	$(".style1" ).click(function(){
		$("#colors" ).attr("href", "assets/css/style1.css" );
		return false;
	});

	$(".style2" ).click(function(){
		$("#colors" ).attr("href", "assets/css/style2.css" );
		return false;
	});

	$(".style3" ).click(function(){
		$("#colors" ).attr("href", "assets/css/style3.css" );
		return false;
	});

	$(".style4" ).click(function(){
		$("#colors" ).attr("href", "assets/css/style4.css" );
		return false;
	});

	$(".style5" ).click(function(){
		$("#colors" ).attr("href", "assets/css/style5.css" );
		return false;
	});

	$(".style6" ).click(function(){
		$("#colors" ).attr("href", "assets/css/style6.css" );
		return false;
	});

	$(".style7" ).click(function(){
		$("#colors" ).attr("href", "assets/css/style7.css" );
		return false;
	});

	$(".style8" ).click(function(){
		$("#colors" ).attr("href", "assets/css/style8.css" );
		return false;
	});

	$(".style9" ).click(function(){
		$("#colors" ).attr("href", "assets/css/style9.css" );
		return false;
	});


	// Style Switcher	
	$('#style-switcher').animate({
		left: '-150px'
	});

	$('#style-switcher h2 a').click(function(e){
		e.preventDefault();
		var div = $('#style-switcher');
		console.log(div.css('left'));
		if (div.css('left') === '-150px') {
			$('#style-switcher').animate({
				left: '0px'
			}); 
		} else {
			$('#style-switcher').animate({
				left: '-150px'
			});
		}
	})

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
        slidesToShow: 4,
        slidesToScroll: 1,
        autoplay: false,
        autoplaySpeed: 2000,
    });

    $('.slick-screens-admin').slick({
        dots: true,
        slidesToShow: 2,
        slidesToScroll: 1,
        autoplay: false,
        autoplaySpeed: 2000,
    });
});

