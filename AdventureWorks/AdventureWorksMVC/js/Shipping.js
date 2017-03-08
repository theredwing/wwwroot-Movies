var HostPath = ""; $.support.cors = true;

$(document).ready(function () {
    var MinDate = new Date();
    MinDate.setDate(MinDate.getDate() - 1);
    var MaxDate = new Date();
    MaxDate.setDate(MaxDate.getDate() + 14);
    $("#ShipDate").wijcalendar({
        allowQuickPick: false,
        minDate: MinDate,
        maxDate: MaxDate
    });

    $("#EmailCon").wijinputmask({});
    $("#FirstNameCon").wijinputtext();
    $("#LastNameCon").wijinputtext();
    $("#AddressLine1Con").wijinputtext();
    $("#AddressLine2Con").wijinputtext();
    $("#CityCon").wijinputtext();
    $("#StateCon").wijcombobox();
    $("#ZipCon").wijinputtext();
    $("#PhoneCon").wijinputmask({
        mask: '(999)000-0000'
    });


    MinDate = new Date();
    $("#ShipDate").wijcalendar("selectDate", MinDate);

    $("#StateCon").wijcombobox("option", "selectedValue", "PA");
    $("#StateCon").wijcombobox("option", "text", "Pennsylvania");
    SetSameShippingAddress();
    $("#EmailCon").attr('readonly', true);
    $("#FirstNameCon").attr('readonly', true);
    $("#LastNameCon").attr('readonly', true);
    $("#PhoneCon").attr('readonly', true);
    $("#AddressLine1Con").attr('readonly', true);
    $("#AddressLine2Con").attr('readonly', true);
    $("#CityCon").attr('readonly', true);
    $("#StateCon").attr('readonly', true);
    $("#StateCon").attr('readonly', true);
    $("#ZipCon").attr('readonly', true);

    $('#SameAddress').change(function () {
        if ($('#SameAddress').is(":checked")) {
            SetSameShippingAddress();
            $("#EmailCon").attr('readonly', true);
            $("#FirstNameCon").attr('readonly', true);
            $("#LastNameCon").attr('readonly', true);
            $("#PhoneCon").attr('readonly', true);
            $("#AddressLine1Con").attr('readonly', true);
            $("#AddressLine2Con").attr('readonly', true);
            $("#CityCon").attr('readonly', true);
            $("#StateCon").attr('readonly', true);
            $("#StateCon").attr('readonly', true);
            $("#ZipCon").attr('readonly', true);
        }
        else {
            $("#EmailCon").removeAttr('readonly');
            $("#FirstNameCon").removeAttr('readonly');
            $("#LastNameCon").removeAttr('readonly');
            $("#PhoneCon").removeAttr('readonly');
            $("#AddressLine1Con").removeAttr('readonly');
            $("#AddressLine2Con").removeAttr('readonly');
            $("#CityCon").removeAttr('readonly');
            $("#StateCon").removeAttr('readonly');
            $("#StateCon").removeAttr('readonly');
            $("#ZipCon").removeAttr('readonly');
        }
    });

});


function SetSameShippingAddress() {
    $.ajax({
        url: HostPath + "/ShoppingCart/GetBillAddress",
        type: "POST",
        dataType: "json",
        data: {},
        success: function (data) {
            if (data.ResultType == "Success" && data.ErrMsg == "N/A") {
                //contact details
                $("#EmailCon").wijinputmask("option", "text", data.BillContact.EmailAddress);
                $("#FirstNameCon").wijinputtext("option", "text", data.BillContact.FirstName);
                $("#LastNameCon").wijinputtext("option", "text", data.BillContact.LastName)
                $("#PhoneCon").wijinputmask("option", "text", data.BillContact.Phone);
                //Address Details
                if (data.Address == "Yes") {
                    $("#AddressLine1Con").wijinputtext("option", "text", data.AddressLine1);
                    $("#AddressLine2Con").wijinputtext("option", "text", data.AddressLine2);
                    $("#CityCon").wijinputtext("option", "text", data.City);
                    $("#StateCon").wijcombobox("option", "selectedValue", data.StateCode);
                    $("#StateCon").wijcombobox("option", "text", data.StateName);
                    $("#ZipCon").wijinputtext("option", "text", data.Zip);
                }

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

function btnNext_Click() {
    var ShipDate = $("#ShipDate").wijcalendar("getSelectedDate");
    var Email = $("#EmailCon").wijinputmask("option", "text"), FirstName = $("#FirstNameCon").wijinputtext("option", "text"), LastName = $("#LastNameCon").wijinputtext("option", "text"), Phone = $("#PhoneCon").wijinputmask("option", "text"), AddressLine1 = $("#AddressLine1Con").wijinputtext("option", "text"), AddressLine2 = $("#AddressLine2Con").wijinputtext("option", "text"), City = $("#CityCon").wijinputtext("option", "text"), State = $("#StateCon").wijcombobox("option", "selectedValue"), Zip = $("#ZipCon").wijinputtext("option", "text");
    var ErrMsg = "";
    $("#LblBillInfoErrMessage").text("");
    if (Email==null) {
        ErrMsg = ErrMsg + "\nEmail can't be null";
    }
    else if (!IsValidEmail(Email)) {
        ErrMsg = ErrMsg + "\nInvalid Email";
    }
    if (FirstName==null) {
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
    if (ErrMsg.length > 0) {
        ErrMsg = "Errors:\n" + ErrMsg;
        $("#LblBillInfoErrMessage").text(ErrMsg);
        return;
    }
    var SameAddress = "0";
    if ($('#SameAddress').is(":checked")) {
        SameAddress = "1";
    }
    var ShipDateStr = (ShipDate.getMonth() + 1) + "/" + ShipDate.getDate() + "/" + ShipDate.getFullYear();
    $.ajax({
        url: HostPath + "/ShoppingCart/Shipping_btnNextClick",
        type: "POST",
        dataType: "json",
        data: { Email: Email, FirstName: FirstName, LastName: LastName, Phone: Phone, AddressLine1: AddressLine1, AddressLine2: AddressLine2, City: City, State: State, Zip: Zip, ShipDate: ShipDateStr, SameAddress: SameAddress },
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

function btnBack_Click() {
    window.location = HostPath + "/ShoppingCart/CheckOut";
}
