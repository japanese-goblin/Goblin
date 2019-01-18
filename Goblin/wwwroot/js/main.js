$(".send-message-all").click(function () {
    var res = confirm("Ты точно хочешь отправить сообщение ВСЕМ???");
    if (!res) return;
    sendMessage(true);
});

$(".send-message-one").click(function () {
    sendMessage(false);
});

$("#add-attach-all").click(() => appendAttach(true));
$("#add-attach-one").click(() => appendAttach(false));

function sendMessage(toAll) {
    const attachs = getAttachments(toAll);
    const msg = toAll ? $(".message-all") : $(".message-one");
    const text = msg.val().replace(/\n/g, "<br>");
    const url = toAll ? "AdminApi/SendToAll" : "AdminApi/SendToId";
    const button = toAll ? $(".send-message-all") : $(".send-message-one");

    let data = {
        "msg": text,
        "attach": attachs
    };
    if (!toAll) {
        data["id"] = $(".user-id").val();
    }

    $.ajax({
        type: "POST",
        url: url,
        async: true,
        data: data,
        success: () => resetButtonClass(button, "btn-warning", "btn-success"),
        error: () => {
            alert('error');
            resetButtonClass(button, "btn-warning", "btn-danger");
        }
    })
}

function getAttachments(fromAll) {
    let selector = fromAll ? "attach-all" : "attach-one";
    return $(selector).map(x => x.value).get().join(",")
}

function appendAttach(isall) {
    let selector = isAll ? "attach-all" : "attach-one";
    let appendTo = isAll ? ".attachments-all" : ".attachments-one";

    let count = $(`.${selector}`).length + 1;
    if (count > 10) return;

    $(appendTo).append(`
        <div class="input-group mb-3">
            <div class="input-group-prepend">
                <span class="input-group-text">${count}</span>
            </div>
            <input type="text" class="form-control ${selector}" id="attach">
        </div>`);
}

function resetButtonClass(button, start, end) {
    button.removeClass(start).addClass(end);
    setTimeout(() => button.removeClass(end).addClass(start), 5000);
}