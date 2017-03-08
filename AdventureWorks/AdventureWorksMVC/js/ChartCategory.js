var HostPath = ""; $.support.cors = true;
$(document).ready(function () {
    LoadChart();
});
function LoadChart() {
    $("#PnlBarChart").show();
    var Category = $("#HFCategory").val();
    $("#BarChart").wijbarchart({
        width: 250,
        height: 110,
        horizontal: false,
        chartLabelFormatString: 'p0',
        chartLabelStyle: {
            fill: "#0099cc"
        },
        seriesStyles: [{ // default style 
            fill: "#0099cc", // bar fill color 
            stroke: "#0099cc"//bar stroke color
        }],
        axis: {
            x: {
                style: { fill: "#0099cc" },
                visible: false,
                textVisible: true,
                textStyle: { fill: "#0099cc", "font-family": "Segoe UI, Myriad, Myriad Pro, Calibri, Arial, Sans-Serif", "font-weight": "normal" },
                labels: {
                    style: { fill: "#0099cc" }
                },
                gridMajor: {
                    visible: false
                }
            },
            y: {
                visible: false,
                textVisible: false,
                compass: "west",
                min: 0,
                max: 1,
                gridMajor: {
                    visible: false,
                },
            }
        },
        hint: {
            enable: false
        },
        header: {
            visible: false
        },
        footer: {
            visible: false
        }
    });
    if (Category == "Mountain Bikes") {
        $("#BarChart").wijbarchart({
            seriesList: [{
                legendEntry: false,
                data:
                {
                    x: ['Speed', 'Rugged', 'Smooth', 'Shocks'],
                    y: [1, 0.9, 0.2, 0.4]
                }
            }]
        });
    }
    else if (Category == "Road Bikes") {
        $("#BarChart").wijbarchart({
            seriesList: [{
                legendEntry: false,
                data:
                {
                    x: ['Speed', 'Rugged', 'Smooth', 'Shocks'],
                    y: [0.2, 0.3, 0.8, 0.6]
                }
            }]
        });
    }
    else if (Category == "Touring Bikes") {
        $("#BarChart").wijbarchart({
            seriesList: [{
                legendEntry: false,
                data:
                {
                    x: ['Speed', 'Rugged', 'Smooth', 'Shocks'],
                    y: [0.1, 0.1, 0.7, 1]
                }
            }]
        });
    }
    else {
        $("#PnlBarChart").hide();
    }
}

