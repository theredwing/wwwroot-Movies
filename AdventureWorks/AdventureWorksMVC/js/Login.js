var HostPath = ""; $.support.cors = true;

$(document).ready(function () {    
        $("#txtUserEmail").wijinputtext();
        $("#txtPassword").wijinputtext();
        $("#txtConfirmPassword").wijinputtext();
        $("#txtFirstName").wijinputtext();
        $("#txtLastName").wijinputtext();
        $("#UserName").wijinputtext();
        $("#Password").wijinputtext();
    });

function CallSignUp() {
    $("#lblErrorMessage").text("");
    var UserName = $("#txtUserEmail").wijinputtext("option", "text");
    var Password = $("#txtPassword").wijinputtext("option", "text");
    var CPassword = $("#txtConfirmPassword").wijinputtext("option", "text");
    var FirstName = $("#txtFirstName").wijinputtext("option", "text");
    var LastName = $("#txtLastName").wijinputtext("option", "text");
    if (UserName == null) {
        $("#lblErrorMessage").text("Enter Email/User Name.");
        SetFocus(txtUserEmail);
        return;
    }
    if (!IsValidEmail(UserName)) {
        $("#lblErrorMessage").text("Invalid Email");
        return;
    }
    if (Password == null) {
        $("#lblErrorMessage").text("Enter login password.");
        return;
    }
    if (CPassword == null) {
        $("#lblErrorMessage").text("Re-enter login password.");
        return;
    }
    if (Password!= CPassword) {
        $("#lblErrorMessage").text("Password and confirm password must be same.");
        return;
    }
    if (FirstName == null) {
        $("#lblErrorMessage").text("Enter First Name.");
        return;
    }
    if (LastName == null) {
        $("#lblErrorMessage").text("Enter Last Name.");
        return;
    }
    var curUrl = window.location;
    $.ajax({
        url: HostPath + "/Home/SignUp",
        type: "POST",
        dataType: "json",
        data: { UserEmail: UserName, Password: Password, FirstName: FirstName, LastName: LastName },
        success: function (data) {
            if (data.ResultType == "Success" && data.ErrMsg == "N/A") {
                window.location = $("#HFReturnURL").val();
            }
            else {
                $("#lblErrorMessage").text(data.ErrMsg);
            }
        },
        error: function (data) {
            window.location = curUrl + "?RetURL=" + $("#HFReturnURL").val();
        }


    });
}
function CallLogin() {
    $("#FailureText").text("");
    var UserName = $("#UserName").wijinputtext("option", "text");
    var Password = $("#Password").wijinputtext("option", "text");
    if (UserName==null) {
        $("#FailureText").text("Enter Email/User Name.");
        return;
    }
    if (Password == null) {
        $("#FailureText").text("Enter login password.");
        return;
    }
    var RememberMe = "false";
    if ($("#RememberMe").is(":checked")) {
        RememberMe = "true";
    }
    var curUrl = window.location;
    $.ajax({
        url: HostPath + "/Home/UserLogin",
        type: "POST",
        dataType: "json",
        data: { UserName: UserName, Password: Password, RememberMe: RememberMe },
        success: function (data) {
            if (data.ResultType == "Success" && data.ErrMsg == "N/A") {
                window.location = $("#HFReturnURL").val();
            }
            else {
                $("#FailureText").text(data.ErrMsg);
            }
        },
        error: function (data) {
            window.location = curUrl + "?RetURL=" + $("#HFReturnURL").val();
        }
    });


}

