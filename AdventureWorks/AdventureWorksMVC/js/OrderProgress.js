$(document).ready(function () {
    //To Initiate Wijmo Controls
    var BarValue = parseInt($('#HFOrderStep').val());

    $("#C1ProgressBarShoppingProgress").wijprogressbar({
        labelAlign: 'center',
        labelFormatString: 'Step {0} of {5}',
        value: BarValue,
        minValue: 0,
        maxValue: 4
    });
});
