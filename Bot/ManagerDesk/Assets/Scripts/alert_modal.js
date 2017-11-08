var AlertModal = {
    id:"#alertmodal",
    text: "Раз раз раз",
    title: "Сообщение",
    show: function () {
        if (!$(this.id).is(':visible')) {
            $(this.id).find(".js-alert-text").html(this.text);
            $(this.id).find(".modal-title").html(this.title);

            $(this.id).modal('show');
        }
    }
}

