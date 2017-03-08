var HostPath = ""; $.support.cors = true;

var ProductWeightsList, ProductSizesList;    
$(document).ready(function () {
    if ($('#HFColorDivVisibility').val() == 'visible') {
        $('#PnlColor').css("height", 'auto');
    }
    else {
        $('#PnlColor').height('0');
    }
    if ($('#HFWeightDivVisibility').val() == 'visible') {
        $('#PnlWeight').css("height", 'auto');
    }
    else {
        $('#PnlWeight').height('0');
    }
    GetProductWeightSizeLists();

    $("#C1CarouselListProducts").wijcarousel({
        display: 1,
        loop: false,
        height: '555',
        buttonPosition: "outside"
    });
    $("#C1ComboColorPick").wijcombobox();        
    $("#C1SliderSize").wijslider({
        orientation: "horizontal",
        min: $('#HFProductSizeMin').val(),
        max: $('#HFProductSizeMax').val(),
        step: 1, value: 0,
        values: null,
        change: function (e) {
            var NewSizeIndex = $("#C1SliderSize").wijslider("option", "value");
            var NewSize = "All";
            if (NewSizeIndex > 0) {
                NewSize = ProductSizesList[NewSizeIndex - 1];
            }
            $("#LblSize").text("Size: " + NewSize);
            FillProductsList();
        }
    });

    $("#C1ComboWeightPick").wijcombobox();


    $('#C1ComboColorPick').change(function () {
        FillProductsList();
    });
    $('#C1ComboWeightPick').change(function () {
        FillProductsList();
    });
    $("#PnlEmptyList").hide();

    FillProductsList();
});

function GetProductWeightSizeLists() {
    ProductWeightsList = ProductSizesList = null;
    $.ajax({
        url: HostPath + "/Products/GetProductWeightSizeLists",
        type: "POST",
        dataType: "json",
        data: { ProductCategoryId: $('#HFProductCategoryId').val() },
        success: function (data) {
            if (data.ResultType == "Success" && data.ErrMsg == "N/A") {
                ProductWeightsList = data.WeightsList;
                ProductSizesList = data.SizesList;
            }
        }
    });
}

function FillProductsList() {
    var carousel = $("#C1CarouselListProducts");
    var ul = carousel.find("ul").empty();
    var ColorValue = "", SizeValue = "", WeightValue = "";

    if (carousel.data("wijmo-wijcarousel")) {
        carousel.wijcarousel("destroy");
    }
    if ($("#PnlColor").css("visibility") == "visible" && $("#C1ComboColorPick").wijcombobox("option", "selectedValue") != "All") {
        ColorValue = $("#C1ComboColorPick").wijcombobox("option", "selectedValue");
    }
    if ($("#PnlSize").css("visibility") == "visible" && $("#C1SliderSize").wijslider("option", "value") > 0) {
        SizeValue = ProductSizesList[$("#C1SliderSize").wijslider("option", "value") - 1];
    }
    if ($("#PnlWeight").css("visibility") == "visible" && $("#C1ComboWeightPick").wijcombobox("option", "selectedValue") != "All") {
        WeightValue = $("#C1ComboWeightPick").wijcombobox("option", "selectedValue");
    }

    $.ajax({
        url: HostPath + "/Products/GetListProductsCarouselData",
        type: "POST",
        dataType: "json",
        data: { CatStr: GetQueryStringValue("Category"), SubCatStr: GetQueryStringValue("SubCategory"), Color: ColorValue, Size: SizeValue, Weight: WeightValue },
        success: function (data) {
            if (data.ResultType == "Success") {
                if (data.PageCount > 0) {
                    $("#PnlEmptyList").hide();
                    carousel.wijcarousel({
                        display: 1,
                        loop: false,
                        height: '555',
                        buttonPosition: "outside"
                    });
                    for (i = 0; i < data.PageCount; i++) {
                        var Item = data.CarouselData[i];
                        carousel.wijcarousel("add", Item, i);
                    }

                    $("#C1CarouselListProducts .ListProductsImage").wijtooltip({
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
                    $("#PnlEmptyList").show();
                }
            }
        }
    });
}