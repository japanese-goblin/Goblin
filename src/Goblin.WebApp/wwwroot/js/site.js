$("#add-attach-all").click(() => appendAttach(true));
$("#add-attach-one").click(() => appendAttach(false));

function appendAttach(isAll) {
    let selector = isAll ? "attach-all" : "attach-one";
    let appendTo = isAll ? $(".attachments-all") : $(".attachments-one");

    let count = $(`.${selector}`).length + 1;
    if (count > 10) return;

    appendTo.append(`<div class="input-group mb-3">
<div class="input-group-prepend">
    <span class="input-group-text">${count}</span>
</div>
<input type="text" class="form-control ${selector}" name="attachments[]">
</div>`);
} 