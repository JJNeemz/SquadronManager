function confirmDelete(uniqueId, isDeleteClicked) {
    var deleteSpan = `deleteSpan_${uniqueId}`;
    var confirmDeleteDiv = `confirmDeleteDiv_${uniqueId}`;

    if (isDeleteClicked) {
        $(`#${deleteSpan}`).hide();
        $(`#${confirmDeleteDiv}`).show();
    } else {
        $(`#${deleteSpan}`).show();
        $(`#${confirmDeleteDiv}`).hide();
    }
}