var HostPath = ""; $.support.cors = true;

function ContinueShopping() {
    window.location = HostPath + "/Home/Default";    
}

function Checkout() {
    $.ajax({
        url: HostPath + "/ShoppingCart/CheckOutClick",
        type: "POST",
        dataType: "json",
        data: { ShippingCharges: $('#C1ComboShippingCompany').wijcombobox('option', 'selectedValue') },
        success: function (data) {
            if (data.ResultType == "Success" && data.ErrMsg == "N/A") {
                window.location = HostPath + data.RedirectURL;
            }
            else {
                alert(data.ErrMsg);
            }
        },
        error: function (data) {
            alert("An unknown error occcured.");
        }
    });    
}