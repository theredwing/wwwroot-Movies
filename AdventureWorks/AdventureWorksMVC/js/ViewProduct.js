var HostPath = ""; $.support.cors = true;

    $(document).ready(function () {        
        $("#C1ExpanderRelated").wijexpander({ expanded: true });
        $("#C1ExpanderViewedProduct").wijexpander({ expanded: true });
        $("#C1ExpanderStats").wijexpander({ expanded: true });
        $("#C1MenuRelated").wijmenu({
            orientation: "vertical"
        });
        $("#C1MenuViewedProduct").wijmenu({
            orientation: "vertical"
        });
        $("#C1Dialog").wijdialog({
            autoOpen: false, 
            width: 605, 
            modal: true
        });        
        LoadProductRelatedData();
    });

function FillC1GVShopCart() {
    $.ajax({
        url: HostPath + "/ShoppingCart/GetOrderSummaryGridData",
        type: "POST",
        dataType: "json",
        data: {},
        success: function (data) {
            if (data.ResultType == "Fail") {
                alert(data.ErrMsg);
            }
            else if (data.ResultType == "Success") {
                $("#C1GVShopCart").wijgrid({
                    selectionMode: "none",
                    data: data.GridData,
                    columnsAutogenerationMode: "none",
                    columns: [
                        { dataKey: "Name", headerText: "Item", dataType: "string" },
                        { dataKey: "ListPrice", headerText: "Price", dataType: "number", dataFormatString: "c" },
                        { dataKey: "Cost", headerText: "Cost", dataType: "number", dataFormatString: "c" }
                    ]
                });
            }
        }
    });
}

function LoadProductRelatedData() {
    $.ajax({
        url: HostPath + "/Products/GetProductRelatedData",
        type: "POST",
        dataType: "json",
        data: { CatStr: GetQueryStringValue("Category"), SubCatStr: GetQueryStringValue("SubCategory"), ProductStr: GetQueryStringValue("Product") },
        success: function (data) {
            if (data.ResultType == "S/O") {
                window.location = "/Home/Default";
            }
            else if (data.ResultType == "Success") {
                for (i = 0; i < data.RelatedMenuListCount; i++) {
                    var RelatedMenuItem = data.RelatedMenuList[i];
                    $("#C1MenuRelated").wijmenu("add", RelatedMenuItem, i);
                }
                for (j = 0; j < data.ViewedProductMenuListCount; j++) {
                    var ViewedProductMenuItem = data.ViewedProductMenuList[j];
                    $("#C1MenuViewedProduct").wijmenu("add", ViewedProductMenuItem, j);
                }
            }
        }
    });
}

function closeDialog() {
    $("#C1Dialog").wijdialog("close");
}

function Checkout() {
    window.location = HostPath+ "/ShoppingCart/OpenCart";
}

function addToCart() {
    $.ajax({
        url: HostPath + "/Products/AddToCart",
        type: "POST",
        dataType: "json",
        data: { ProductID: $('#hdProdId').val() },
        success: function (data) {
            if (data.ResultType == "S/O") {
                window.location = "/Home/Default";
            }
            else if (data.ResultType == "Success") {
                if (data.ErrMsg == "N/A") {
                    UpdateCartItemsCount();
                    FillC1GVShopCart();
                    $("#C1Dialog").wijdialog("open");
                }
                else {
                    alert(data.ErrMsg);
                }
            }
        }
    });
}

