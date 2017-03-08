var HostPath = ""; $.support.cors = true;

$(document).ready(function () {
    LoadListCategoriesData();
});

function LoadListCategoriesData() {
    $.ajax({
        url: HostPath + "/Products/GetListCategoriesCarouselData",
        type: "POST",
        dataType: "json",
        data: { CatStr: GetQueryStringValue("Category") },
        success: function (data) {
            if (data.ResultType == "Success" && data.ErrMsg=="N/A") {
                for (i = 0; i < data.ProductSubCategoryIDList.length; i++) {
                    SetChart("C1BarChart_" + data.ProductSubCategoryIDList[i], data.ProductSubCategoryNameList[i]);
                    FillListCategoriesProductsCarousel("C1Carousel_" + data.ProductSubCategoryIDList[i], data.CarouselDataList[i]);
                }

                $(".SubCategory li a").wijtooltip({
                    showcallout: true, calloutFilled: false,
                    position: { my: 'center bottom', at: 'center top' },
                    ajaxCallback: function () {
                        var ele = this;
                        $.ajax({
                            url: HostPath + "/Products/GetToolTipData",
                            type: "POST",
                            dataType: "json",
                            data: { ProductId: ele.attr('id') },
                            success: function (data) {
                                if (data.ResultType == "Success" && data.ErrMsg == "N/A") {
                                    ele.wijtooltip("option", "content", data.ToolTipData);
                                }
                            },
                            error: function (data) {
                            }
                        });
                    }
                });

            }
            else {
                alert(data.ErrMsg);
            }
        }
    });   
}

function FillListCategoriesProductsCarousel(carouselId, carouselData) {
    var carousel = $('#' + carouselId);
    carousel.wijcarousel({
        display: 1,
        loop: false,
        buttonPosition: "outside"
    });
    for (j = 0; j < carouselData.length; j++) {
        var Item = carouselData[j];
        if (Item != null) {
            carousel.wijcarousel("add", Item, j);
        }
    }
}

function SetChart(ChartID, Category) {
    $('#' + ChartID).wijbarchart({
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
        $('#' + ChartID).wijbarchart({
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
        $('#' + ChartID).wijbarchart({
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
        $('#' + ChartID).wijbarchart({
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
}

