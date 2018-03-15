(function($) {
  "use strict"; // Start of use strict

  // Closes the sidebar menu
  $("#menu-close").click(function(e) {
    e.preventDefault();
    $("#sidebar-wrapper").toggleClass("active");
  });

  // Opens the sidebar menu
  $("#menu-toggle").click(function(e) {
    e.preventDefault();
    $("#sidebar-wrapper").toggleClass("active");
  });

  // Smooth scrolling using jQuery easing
  $('a.js-scroll-trigger[href*="#"]:not([href="#"])').click(function() {
    if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') && location.hostname == this.hostname) {
      var target = $(this.hash);
      target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
      if (target.length) {
        $('html, body').animate({
          scrollTop: target.offset().top
        }, 1000, "easeInOutExpo");
        return false;
      }
    }
  });

  // Closes responsive menu when a scroll trigger link is clicked
  $('.js-scroll-trigger').click(function() {
    $("#sidebar-wrapper").removeClass("active");
  });

  $('.js-close-modal').on('click', function () {
      $("#MessageModal").hide();
  });

  $('.js-request').on('click', function (e) {
      e.preventDefault();
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

      if (email == null || email =="") {
          alert("Нужно заполнить поле контакт, чтобы мы могли с вами связаться!");
          container.find(".js-send-request").show();
          container.find(".js-spinner").hide();
      }
      else {
          $.post(target, { orgName: orgName, email: email, comment: comment, type: type }).done(function (data) {

              container.find(".request-success").show();
              container.find(".fields-request").hide();

              container.find(".js-send-request").hide();
              container.find(".js-spinner").hide();

              setTimeout(function () {
                  container.hide();
              }, 3000);
          });
      }
  });

  //#to-top button appears after scrolling
  var fixed = false;
  $(document).scroll(function() {
    if ($(this).scrollTop() > 250) {
      if (!fixed) {
        fixed = true;
        $('#to-top').show("slow", function() {
          $('#to-top').css({
            position: 'fixed',
            display: 'block'
          });
        });
      }
    } else {
      if (fixed) {
        fixed = false;
        $('#to-top').hide("slow", function() {
          $('#to-top').css({
            display: 'none'
          });
        });
      }
    }
  });

})(jQuery); // End of use strict

// Disable Google Maps scrolling
// See http://stackoverflow.com/a/25904582/1607849
// Disable scroll zooming and bind back the click event
var onMapMouseleaveHandler = function(event) {
  var that = $(this);
  that.on('click', onMapClickHandler);
  that.off('mouseleave', onMapMouseleaveHandler);
  that.find('iframe').css("pointer-events", "none");
}
var onMapClickHandler = function(event) {
  var that = $(this);
  // Disable the click handler until the user leaves the map area
  that.off('click', onMapClickHandler);
  // Enable scrolling zoom
  that.find('iframe').css("pointer-events", "auto");
  // Handle the mouse leave event
  that.on('mouseleave', onMapMouseleaveHandler);
}
// Enable map zooming with mouse scroll when the user clicks the map
$('.map').on('click', onMapClickHandler);
