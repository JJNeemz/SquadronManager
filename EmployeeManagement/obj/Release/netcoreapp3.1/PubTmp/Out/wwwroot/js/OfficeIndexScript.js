function checkManning(officeId) {
    var minimumManning = document.getElementById(`minimumManningValue_${officeId}`).innerHTML;
    var minimumManningInt = parseInt(minimumManning);

    var currentManning = document.getElementById(`currentManningValue_${officeId}`).innerHTML;
    var currentManningInt = parseInt(currentManning);

    var currentManningElement = document.getElementById(`currentManningElement_${officeId}`);

    console.log(currentManningInt);
    console.log(minimumManningInt);

    if (currentManningInt < minimumManningInt) {
        currentManningElement.classList.add("text-danger");
    }
}