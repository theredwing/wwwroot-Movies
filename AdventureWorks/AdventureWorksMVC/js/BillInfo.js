$(document).ready(function () {
        $("#EmailCon").wijinputmask({
        });
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
        $("#StateCon").wijcombobox("option", "selectedValue", "PA");
        $("#StateCon").wijcombobox("option", "text","Pennsylvania");
        SetAddress();
    });

function SetAddress() {
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
                $("#PhoneCon").wijinputmask("option", "text", data.BillContact.Phone || '');
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
        }
    });

}