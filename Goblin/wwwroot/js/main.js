$(".send-message-all").click(function () {
    var msg = $(".message-all").val().replace(/\n/g, "<br>");
    $.ajax({
        type: "POST",
        url: "AdminApi/SendToAll",
        async: true,
        data: {
            "msg": msg,
            "attach": $(".attach-all").map(function() {return this.value;}).get().join(",") // TODO:
        },
        success: function() {
            $(".send-message-all").removeClass("btn-primary").addClass("btn-success")
        },
        error: function() {
            alert('error');
            $(".send-message-all").removeClass("btn-primary").addClass("btn-danger")
        }
    })
});

$(".send-message-one").click(function () {
    var msg = $(".message-one").val().replace(/\n/g, "<br>");
    $.ajax({
        type: "POST",
        url: "AdminApi/SendToId",
        async: true,
        data: {
            "msg": msg,
            "id": $(".user-id").val(),
            "attach": $(".attach-one").map(function() {return this.value;}).get().join(",") // TODO:
        },
        success: function() {
            $(".send-message-one").removeClass("btn-primary").addClass("btn-success")
        },
        error: function() {
            alert('error');
            $(".send-message-one").removeClass("btn-primary").addClass("btn-danger")
        }
    })
});

//TODO вот ето быдлокод конечно да
$("#add-attach-all").click(function () {
    var count = $(".attach-all").length + 1;
    if (count <= 10) {
        $(".attachments-all").append(`
        <div class="input-group mb-3">
            <div class="input-group-prepend">
                <span class="input-group-text">${count}</span>
            </div>
            <input type="text" class="form-control attach-all" id="attach">
        </div>
    `);
    }
});

$("#add-attach-one").click(function () {
    var count = $(".attach-one").length + 1;
    if (count <= 10) {
        $(".attachments-one").append(`
        <div class="input-group mb-3">
            <div class="input-group-prepend">
                <span class="input-group-text">${count}</span>
            </div>
            <input type="text" class="form-control attach-one" id="attach">
        </div>
    `);
    }
});