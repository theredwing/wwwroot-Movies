var HostPath = ""; $.support.cors = true;

$(document).ready(function () {
    $("#CardNumber").wijinputtext({
        format: '9',
        maxLength: 16
    });
    $("#Expiration").wijinputdate({
        dateFormat: 'MM/yy',
        showSpinner: true
    });
    $("#Code").wijinputnumber({
        decimalPlaces: 0
    });
    $("#EmailCon").attr("readonly", true);
});

function btnNext_Click() {
    var ExpirationDate = new Date($("#Expiration").wijinputdate("option", "date"));
    var Email = $("#EmailCon").wijinputmask("option", "text"), FirstName = $("#FirstNameCon").wijinputtext("option", "text"), LastName = $("#LastNameCon").wijinputtext("option", "text"), Phone = $("#PhoneCon").wijinputmask("option", "text"), AddressLine1 = $("#AddressLine1Con").wijinputtext("option", "text"), AddressLine2 = $("#AddressLine2Con").wijinputtext("option", "text"), City = $("#CityCon").wijinputtext("option", "text"), State = $("#StateCon").wijcombobox("option", "selectedValue"), Zip = $("#ZipCon").wijinputtext("option", "text"), ExpirationYear = ExpirationDate.getYear(), ExpirationMonth = ExpirationDate.getMonth() + 1, CardNumber = $("#CardNumber").wijinputtext("option", "text"), PaymentType = $("input:radio[name='PaymentType']:checked").val(), Code = $("#Code").wijinputnumber("option", "value");
    var ErrMsg = "";
    $("#ErrCardNumber").text("");
    $("#LblBillInfoErrMessage").text("");
    if (Email==null) {
        ErrMsg = ErrMsg + "\nEmail can't be null";
    }
    else if (!IsValidEmail(Email)) {
        ErrMsg = ErrMsg + "\nInvalid Email";
    }
    if (FirstName == null) {
        ErrMsg = ErrMsg + "\nFirst Name can't be null";
    }
    if (LastName == null) {
        ErrMsg = ErrMsg + "\nLast Name can't be null";
    }
    if (AddressLine1 == null) {
        ErrMsg = ErrMsg + "\nAddress Line 1 can't be null";
    }
    if (City == null) {
        ErrMsg = ErrMsg + "\nCity can't be null";
    }
    if (State == null) {
        ErrMsg = ErrMsg + "\nState can't be null";
    }
    if (Zip == null) {
        ErrMsg = ErrMsg + "\nPostal Code can't be null";
    }
    if (ErrMsg==null) {
        ErrMsg = "Errors:\n" + ErrMsg;
        $("#LblBillInfoErrMessage").text(ErrMsg);
        return;
    }
    if (CardNumber == null) {
        $("#ErrCardNumber").text("Card Number can't be empty");
        return;
    }
    $.ajax({
        url: HostPath + "/ShoppingCart/CheckOut_btnNextClick",
        type: "POST",
        dataType: "json",
        data: { Email: Email, FirstName: FirstName, LastName: LastName, Phone: Phone, AddressLine1: AddressLine1, AddressLine2: AddressLine2, City: City, State: State, Zip: Zip, ExpirationYear: ExpirationYear, ExpirationMonth: ExpirationMonth, CardNumber: CardNumber, PaymentType: PaymentType, Code: Code },
        success: function (data) {
            if (data.ResultType == "Success" && data.ErrMsg == "N/A") {
                window.location = HostPath + data.RedirectURL;
            }
            else {
                alert(data.ErrMsg);
            }
        },
        error: function (data) {
        }
    });
}

function btnBack_Click() {
    window.location = HostPath + "/ShoppingCart/OpenCart";
}
