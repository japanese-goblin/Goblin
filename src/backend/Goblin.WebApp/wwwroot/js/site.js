function appendAttach(isAll) {
    let selector = isAll ? "attach-all" : "attach-one";
    let appendToSelector = isAll ? "attachments-all" : "attachments-one";
    let appendTo = document.getElementById(appendToSelector);

    let count = document.getElementsByClassName(selector).length + 1;
    if (count > 10) return;
    
    appendTo.innerHTML += `<div class="input-group mb-3">
<div class="input-group-prepend">
    <span class="input-group-text">${count}</span>
</div>
<input type="text" class="form-control ${selector}" name="attachments[]">
</div>`;
} 