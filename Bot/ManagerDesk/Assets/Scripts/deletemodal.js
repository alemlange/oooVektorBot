var DeleteModal = {
    id: "#deletemodal",
    text: "Раз раз раз",
    title: "Сообщение",
    itemid: "",
    type: "",
    target:"",
    show: function (_itemid, _type, _target) {
        if (!$(this.id).is(':visible')) {
            this.itemid = _itemid;
            this.type = _type;
            this.target = _target;
            $(this.id).modal('show');
        }
    },
    setText: function (newText) {
        $("#deletemodal").find(".modal-text").html(newText);
    },
    delete: function () {
        $.post(this.target, { itemId: this.itemid, itemType: this.type }).done(function (data) {
            $("#deletemodal").modal('hide');
            $(".menu-section.active").trigger("click");
        });
    }
}

$(document).ready(function () {
    $("#deletemodal").on("click", ".js-ok", function (e) {
        e.preventDefault();

        DeleteModal.delete();
    });
});
