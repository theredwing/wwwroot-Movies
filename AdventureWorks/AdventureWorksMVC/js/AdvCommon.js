var HostPath = ""; $.support.cors = true;

    function SetMsgColor(Id, color) {
        $("#" + id).css("color", color);
    }

function SetFocus(id) {
    document.getElementById(id).focus();
}

//To Check Login Status
function CheckLoginStatus(successcallback) {
    $.ajax({
        url: HostPath + "/Home/CheckLoginStatus",
        type: "POST",
        dataType: "json",
        data: {},
        success: function (data) {
            successcallback(data);
        },
        error: function (data) {
            alert("An unknown error occcured.");
        }
    });
}



//To Update Shopping Cart Items Count
function UpdateCartItemsCount() {
    $.ajax({
        url: HostPath + "/ShoppingCart/GetCartItemsCount",
        type: "POST",
        dataType: "json",
        data: {},
        success: function (data) {
            if (data.ResultType == "Success" && data.ErrMsg == "N/A") {
                $('#LblCartItemsCount').text(data.CartItemsCount);
            }
            else {
                $('#LblCartItemsCount').text(0);
            }
        },
        error: function (data) {
            $('#LblCartItemsCount').text(0);
        }
    });
}

function IsValidEmail(email) {
    //$.trim(str)
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test($.trim(email));
}

function ConvertJsonDateString(jsonDate) {
    var shortDate = null;
    if (jsonDate) {
        var regex = /-?\d+/;
        var matches = regex.exec(jsonDate);
        var dt = new Date(parseInt(matches[0]));
        var month = dt.getMonth() + 1;
        var monthString = month > 9 ? month : '0' + month;
        var day = dt.getDate();
        var dayString = day > 9 ? day : '0' + day;
        var year = dt.getFullYear();
        shortDate = monthString + '-' + dayString + '-' + year;
    }
    return shortDate;
}

function GetQueryStringValue(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results == null ? null : decodeURIComponent(results[1].replace(/\+/g, " "));
}

