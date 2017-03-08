var HostPath = ""; $.support.cors = true;

$(document).ready(function () {
    //To Initiate Wijmo Controls
    $("#C1ComboShippingCompany").wijcombobox();
    $("#C1ComboShippingCompany").wijcombobox("option", "selectedIndex", -1);
    $("#C1ComboShippingCompany").wijcombobox("option", "selectedIndex", 0);
    FillC1GVShopCart();
    SetShopCartTotalData();
    $('#C1ComboShippingCompany').change(function () {
        SetShopCartTotalData();
    });
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
                    editingMode: "cell",
                    beforeCellEdit: onBeforeCellEdit,
                    beforeCellUpdate: onBeforeCellUpdate,
                    data: data.GridData,
                    columnsAutogenerationMode: "none",
                    columns: [
                        { dataKey: "ShoppingCartItemID", headerText: "Cart Item ID", dataType: "number", visible: false, readOnly: true },
                        { dataKey: "Name", headerText: "Item", dataType: "string", readOnly: true },
                        { dataKey: "ListPrice", headerText: "Price", dataType: "number", dataFormatString: "c", readOnly: true },
                        { dataKey: "Quantity", headerText: "Quantity", dataType: "number", dataFormatString: "n0" },
                        { dataKey: "Cost", headerText: "Cost", dataType: "number", dataFormatString: "c", readOnly: true },
                        {
                            headerText: "Remove",
                            buttonType: "button",
                            dataType: "number",
                            command: {
                                text: "Remove",
                                click: C1GVShopCart_Remove_Click
                            }
                        }
                    ]
                });
                var IsInWindow = $("#HFIsInWindow").val();
                if (IsInWindow == 1) {
                    $("#PnlTotal").hide();
                    $("#C1GVShopCart").wijgrid({
                        width: 550
                    });
                    $("#C1GVShopCart").wijgrid("columns")[3].option("visible", false);
                    $("#C1GVShopCart").wijgrid("columns")[5].option("visible", false);
                }
            }
        }
    });
}

//To update qty of current item of ShoppingCart
function C1GVShopCart_UpdateItemQuantity(quantity, cartItemId) {
    $.ajax({
        type: "POST",
        url: HostPath + "/ShoppingCart/UpdateItemQuantity",
        data: { cartItemId: cartItemId, quantity: quantity },
        success: function (data) {
            if (data.success = "true") {
                FillC1GVShopCart();
                SetShopCartTotalData();
            }
            else {
                alert(data.error);
            }
        },
        error: function (data) {
            alert("Task failed due to an unknown error.");
        }
    });
}

//create custom editors for Quantity column
function onBeforeCellEdit(e, args) {
    switch (args.cell.column().dataKey) {
        case "Quantity":
            createNumberInput(args, 0);
            args.handled = true;
            break;
    }
}

function createNumberInput(args, decimalPlaces) {
    $("<input/>")
    .width("60px")
    .appendTo(args.cell.container().empty())
    .wijinputnumber({
        minValue: 1,
        maxValue:1000,
        decimalPlaces: decimalPlaces,
        showSpinner: true,
        value: args.cell.value(),
        valueChanged: function (e, data) {
        }
    })
    .focus();
}

//update Quantity of selected Cart Item
function onBeforeCellUpdate(e, args) {
    switch (args.cell.column().dataKey) {
        case "Quantity":
            var editor = args.cell.container().find("input");
            var qty = editor.wijinputnumber("getValue");
            if (qty == null || qty == 0) {
                FillC1GVShopCart();
                break;
            }
            if (qty != args.cell.value()) {
                var ShoppingCartItemID = args.cell.row().data.ShoppingCartItemID;
                C1GVShopCart_UpdateItemQuantity(qty, ShoppingCartItemID);
            }
            break;
    }
}

//To Remove current item from ShoppingCart
function C1GVShopCart_Remove_Click(e, args) {
    var ShoppingCartItemID = args.row.data.ShoppingCartItemID;
    $.ajax({
        type: "POST",
        url: HostPath + "/ShoppingCart/RemoveItem",
        data: { cartItemId: ShoppingCartItemID },
        success: function (data) {
            if (data.success = "true") {
                UpdateCartItemsCount();
                FillC1GVShopCart();
                SetShopCartTotalData();
            }
            else {
                alert(data.error);
            }
        },
        error: function (data) {
            alert("Task failed due to an unknown error.");
        }
    });
}

function SetShopCartTotalData() {
    if ($('#C1ComboShippingCompany').wijcombobox('option', 'selectedIndex') > -1) {
        $.ajax({
            url: HostPath + "/ShoppingCart/GetOrderSummaryTotalData",
            type: "POST",
            dataType: "json",
            data: { ShippingCharges: $('#C1ComboShippingCompany').wijcombobox('option', 'selectedValue') },
            success: function (data) {
                if (data.ResultType == "Fail") {
                    alert(data.ErrMsg);
                }
                else if (data.ResultType == "Success") {
                    $('#lblSubTotalAmount').text(data.SubTotalAmount);
                    $('#lblShippingAmount').text(data.ShippingAmount);
                    $('#lblTotalAmount').text(data.TotalAmount);
                }
            }
        });
    }
}