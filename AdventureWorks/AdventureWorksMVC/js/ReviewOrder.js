var HostPath = ""; $.support.cors = true;

$(document).ready(function () {
    //To Initiate Wijmo Controls
    FillC1GVOrder();
});

function FillC1GVOrder() {
    $.ajax({
        url: HostPath + "/ShoppingCart/GetReviewOrderGridData",
        type: "POST",
        dataType: "json",
        data: {},
        success: function (data) {
            if (data.ResultType == "Fail") {
                alert(data.ErrMsg);
            }
            else if (data.ResultType == "Success") {
                $("#gvOrderDetails").wijgrid({
                    selectionMode: "none",
                    data: data.GridData,
                    columnsAutogenerationMode: "none",
                    columns: [
                        { dataKey: "Name", headerText: "Item", dataType: "string" },
                        { dataKey: "UnitPrice", headerText: "Price", dataType: "number", dataFormatString: "c" },
                        { dataKey: "OrderQty", headerText: "Quantity", dataType: "number", dataFormatString: "n0" },
                        { dataKey: "LineTotal", headerText: "Cost", dataType: "number", dataFormatString: "c" },
                    ]
                });
            }
        }
    });
}

function btnBack_Click() {
    window.location = HostPath + "/ShoppingCart/Shipping";
}

function btnNext_Click() {
    window.location = HostPath + "/ShoppingCart/OrderComplete";
}
