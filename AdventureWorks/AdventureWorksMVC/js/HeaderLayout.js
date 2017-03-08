var HostPath = ""; $.support.cors = true;

$(document).ready(function () {
    $("#MainMenu").wijmenu();
    $("#SubMenu").wijmenu();
    LoadMenus();
});

//function ShoppingCartItemsCount() {
//    $("#LblShoppingCartItemsCount").text("N/A");
//    $.ajax({
//        url: HostPath + "/SiteLayout/ShoppingCartItemsCount",
//        type: "POST",
//        dataType: "json",
//        data: { },
//        success: function (data) {
//            if (data.ResultType == "S/O") {
//                window.location = "/Home/Default";
//            }
//            else if (data.ResultType == "Success") {
//                $("#LblShoppingCartItemsCount").text(data.CartItemsCount);
//            }
//        }
//    });
//}

function LoadMenus() {
    $.ajax({
        url: HostPath + "/Products/GetMenus",
        type: "POST",
        dataType: "json",
        data: { CurrentCategoryName: GetQueryStringValue("Category"), CurrentSubCategory: GetQueryStringValue("SubCategory") },
        success: function (data) {
            if (data.ResultType == "S/O") {
                window.location = "/Home/Default";
            }
            else if (data.ResultType == "Success") {
                for (i = 0; i < data.MenuListItemsCount; i++) {
                    var MenuItem = data.MenuList[i];
                    $("#MainMenu").wijmenu("add", MenuItem, i);
                }
                for (i = 0; i < data.SubMenuListItemsCount; i++) {
                    var SubMenuItem = data.SubMenuList[i];
                    $("#SubMenu").wijmenu("add", SubMenuItem, i);
                }
            }
        }
    });

    
}