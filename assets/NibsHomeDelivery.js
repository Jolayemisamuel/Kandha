$(document).ready(function () {
    var tokenNo = $('#_TokenNo').val();
    ShowTakeBillingItemsOnPageLoad(tokenNo);
});

//======================= get take away billing  Items on page load==================
function ShowTakeBillingItemsOnPageLoad(TokenNo) {
    var xmlhttp;
    if (TokenNo == "") {
        document.getElementById("_show_take_billingItems").innerHTML = "";
        return;
    }
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
            document.getElementById("_show_take_billingItems").innerHTML = xmlhttp.responseText;
            $('.date-picker').datepicker({
                rtl: Metronic.isRTL(),
                orientation: "left",
                autoclose: true
            });
            $('[data-toggle="tooltip"]').tooltip();
            AddHeight();
        }
    }
    var data = new FormData();
    data.append("TokenNo", TokenNo);
    xmlhttp.open("POST", "/HomeDelivery/_getTakeAwayBillingItemOnPageLoad", true);
    xmlhttp.send(data);


}

function showSubItem(str) {
    var xmlhttp;
    if (str == "") {
        document.getElementById("_show_subItems").innerHTML = "";
        return;
    }
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
            document.getElementById("_show_subItems").innerHTML = xmlhttp.responseText;
        }
    }
    xmlhttp.open("GET", "/HomeDelivery/_GetAllSubItemPartial?Id=" + str, true);
    xmlhttp.send();
}
$(document).on('click', '._sub_item', function (e) {
    var id = $(this).attr('Id');
    var url = $(this).attr('href');
    showSubItem(id);
    e.preventDefault();
});
function AddHeight() {
    $(".DivItemsDisplay").animate({ "scrollTop": $('.DivItemsDisplay')[0].scrollHeight }, "slow");

}
//======================= get take away billing Items==================
function ShowTakeBillingItemsOnItem(str, TokenNo, Qty, Type) {
    var xmlhttp;
    if (str == "") {
        document.getElementById("_show_take_billingItems").innerHTML = "";
        return;
    }
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
            document.getElementById("_show_take_billingItems").innerHTML = xmlhttp.responseText;
            $('.date-picker').datepicker({
                rtl: Metronic.isRTL(),
                orientation: "left",
                autoclose: true
            });
            $('[data-toggle="tooltip"]').tooltip();
            AddHeight();
        }
    }
    var data = new FormData();
    data.append("Id", str);
    data.append("Qty", Qty);
    data.append("TokenNo", TokenNo);
    data.append("Type", Type);
    xmlhttp.open("POST", "/HomeDelivery/_GetTakeAwayBillingItems", true);
    xmlhttp.send(data);


}
$(document).on('click', '.ajax_btn', function () {
    var Id = $(this).attr('Id');
    var RunningTable = $('#_TokenNo').val();
    if (typeof RunningTable === "undefined") {
        var str = 'Please Select Table No';
        //GetToster(str, 'error', 'Error Notification');
        return false;
    }
    var Qty = $('input[name="Qty"]').val();
    var value = Qty.replace(/^\s\s*/, '').replace(/\s\s*$/, '');
    var intRegex = /^\d+$/;
    if (!intRegex.test(value)) {
        var str = 'please enter correct quantity';
        GetToster(str, 'error', 'Error Notification');
        return false;
    }
    if (!$.isNumeric(Qty) || Qty <= parseInt(0)) {
        var str = 'please enter correct quantity';
        GetToster(str, 'error', 'Error Notification');
        return false;
    }
    var Type = $('input:radio[name=Type]:checked').val();
    ShowTakeBillingItemsOnItem(Id, RunningTable, Qty, Type);
});
///=====================================delete items=============================
function DeleteItem(str) {
    var xmlhttp;
    if (str == "") {
        document.getElementById("_show_take_billingItems").innerHTML = "";
        return;
    }
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
            document.getElementById("_show_take_billingItems").innerHTML = xmlhttp.responseText;
            GetToster('Item Deleted successfully', 'success', 'Success Notification');
            AddHeight();
        }
    }
    xmlhttp.open("GET", "/HomeDelivery/_DeleteItem?Id=" + str, true);
    xmlhttp.send();
}
$(document).on('click', '._delete_item', function (e) {
   var id = $(this).attr('Id');
    fnOpenNormalDialog(id);
    e.preventDefault();
});
//======================= return Item===================
function ReturnItem(str) {
    var xmlhttp;
    if (str == "") {
        document.getElementById("_show_take_billingItems").innerHTML = "";
        return;
    }
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
            document.getElementById("_show_take_billingItems").innerHTML = xmlhttp.responseText;
            GetToster('Item returned successfully', 'success', 'Success Notification');
            AddHeight();
        }
    }
    xmlhttp.open("GET", "/HomeDelivery/_ReturnItem?Id=" + str, true);
    xmlhttp.send();
}
$(document).on('click', '._return_item', function (e) {
    var id = $(this).attr('Id');
    if (!confirm("Do you want to return this item ? ")) {
        return false;
    }
    ReturnItem(id);
    e.preventDefault();
});
function ClearKot() {
    var xmlhttp;
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
            document.getElementById("_show_take_billingItems").innerHTML = xmlhttp.responseText;
            AddHeight();
        }
    }
    xmlhttp.open("GET", "/HomeDelivery/_clearKOT", true);
    xmlhttp.send();
}
$(document).on('click', '._clear_kot', function (e) {
    ClearKot();
    e.preventDefault();
    $('.modal').modal('hide');
});
//========================= script for cancel order==========================
function CancelOrder() {
    var xmlhttp;
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
            document.getElementById("_show_take_billingItems").innerHTML = xmlhttp.responseText;
            GetToster('Order cancelled successfully', 'success', 'Success Notification');
            AddHeight();
        }
    }
    xmlhttp.open("GET", "/HomeDelivery/CancelOrder", true);
    xmlhttp.send();
}
$(document).on('click', '.canclegorder_Click', function (e) {
    if (!confirm("Do you want to cancel Order ? ")) {
        return false;
    }
    CancelOrder();
    e.preventDefault();

});
//====================script for Dispatch Order=====================
$(document).on('click', '#_Bill_dispatch', function (e) {
    DispatchOrder();
    $('.modal').modal('hide');
    e.preventDefault();
});
function DispatchOrder() {
    var xmlhttp;
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {

            document.getElementById("_show_take_billingItems").innerHTML = xmlhttp.responseText;
            GetToster('Order dispatched successfully Token No ' + parseInt($('input[name="TableNo"]').val() - 1), 'success', 'Success Notification');
            AddHeight();
        }
    }
    var CustomerName = $('input[name="CustomerName"]').val();
    var TableNo = $('input[name="TableNo"]').val();
    var PaymentType = $('select[name="PaymentType"]').val();
    var CheckDate = $('input[name="CheckDate"]').val();
    var ChequeNo = $('input[name="ChequeNo"]').val();
    var Discount = $('input[name="Discount"]').val();
    var VatAmount = $('input[name="VatAmount"]').val();
    var ServiceCharge = $('input[name="ServiceCharge"]').val();
    var DiscountAmount = $('input[name="DiscountAmount"]').val();
    var TotalAmount = $('input[name="TotalAmount"]').val();
    var NetAmount = $('input[name="NetAmount"]').val();
    var form = $('#_ajax_Bill_form');
    var url = form.attr('action');
    var data = new FormData();

    data.append("CustomerName", CustomerName);
    data.append("TableNo", TableNo);
    data.append("PaymentType", PaymentType);
    data.append("CheckDate", CheckDate);
    data.append("ChequeNo", ChequeNo);
    data.append("Discount", Discount);
    data.append("VatAmount", VatAmount);
    data.append("ServiceCharge", ServiceCharge);
    data.append("DiscountAmount", DiscountAmount);
    data.append("TotalAmount", TotalAmount);
    data.append("NetAmount", NetAmount);
    xmlhttp.open("POST", url, true);
    xmlhttp.send(data);
    $('#' + TableNo).removeClass("current");

}
//=========================== script for print Data=========================

$(document).on('click', '#_bill_print', function (e) {
    var RunningTable = $('input[name="TableNo"]').val();
    var address = $('#CustomerAddress').val();
    if (typeof RunningTable === "undefined" || RunningTable == "") {
        var str = 'Please Select Table No';
        GetToster(str, 'error', 'Error Notification');
        return false;
    }
    if (address=="") {
        e.preventDefault();
        $('.modal').modal('hide');
        var str = 'Please Enter Customer Address';
        GetToster(str, 'error', 'Error Notification');
        return false;
    }
    var CustomerName = $('input[name="CustomerName"]').val();
    var TableNo = $('input[name="TableNo"]').val();
    var Discount = $('input[name="Discount"]').val();
    var VatAmount = $('input[name="VatAmount"]').val();
    var ServiceCharge = $('input[name="ServiceCharge"]').val();
    var DiscountAmount = $('input[name="DiscountAmount"]').val();
    var TotalAmount = $('input[name="TotalAmount"]').val();
    var NetAmount = $('input[name="NetAmount"]').val();
    var CustomerAddress = $('#CustomerAddress').val();
    centeredPopup("/HomeDelivery/_printData?TableNo=" + RunningTable + "&Discount=" + Discount + "&VatAmount=" + VatAmount + "&ServiceCharge=" + ServiceCharge + "&DiscountAmount=" + DiscountAmount + "&TotalAmount=" + TotalAmount + "&NetAmount=" + NetAmount + "&CustomerName=" + CustomerName + "&CustomerAddress="+CustomerAddress, 'myWindow', '600', '400', 'yes'); return false
    e.preventDefault();
});
var popupWindow = null;
function centeredPopup(url, winName, w, h, scroll) {
    LeftPosition = (screen.width) ? (screen.width - w) / 2 : 0;
    TopPosition = (screen.height) ? (screen.height - h) / 2 : 0;
    settings =
    'height=' + h + ',width=' + w + ',top=' + TopPosition + ',left=' + LeftPosition + ',scrollbars=' + scroll + ',resizable'
    popupWindow = window.open(url, winName, settings)
}
function GetToster(str, shortCutFunction, title) {
    var toastCount = 0;
    var $toastlast;
    var shortCutFunction = shortCutFunction;
    var msg = str;
    var title = title;
    //var $showDuration = $('#showDuration');
    //var $hideDuration = $('#hideDuration');
    //var $timeOut = $('#timeOut');
    //var $extendedTimeOut = $('#extendedTimeOut');
    //var $showEasing = $('#showEasing');
    //var $hideEasing = $('#hideEasing');
    //var $showMethod = $('#showMethod');
    //var $hideMethod = $('#hideMethod');
    var toastIndex = toastCount++;

    toastr.options = {
        closeButton: $('#closeButton').prop('checked'),
        debug: $('#debugInfo').prop('checked'),
        positionClass: $('#positionGroup input:checked').val() || 'toast-top-right',
        onclick: null
    };

    if ($('#addBehaviorOnToastClick').prop('checked')) {
        toastr.options.onclick = function () {
            alert('You can perform some custom action after a toast goes away');
        };
    }


    toastr.options.showDuration = 1000;

    toastr.options.hideDuration = 1000;

    toastr.options.timeOut = 5000;

    toastr.options.extendedTimeOut = 1000;

    toastr.options.showEasing = 'swing';

    toastr.options.hideEasing = 'linear';

    toastr.options.showMethod = 'fadeIn';

    toastr.options.hideMethod = 'fadeOut';
    if (!msg) {
        msg = getMessage();
    }

    $("#toastrOptions").text("Command: toastr[" + shortCutFunction + "](\"" + msg + (title ? "\", \"" + title : '') + "\")\n\ntoastr.options = " + JSON.stringify(toastr.options, null, 2));

    var $toast = toastr[shortCutFunction](msg, title); // Wire up an event handler to a button in the toast, if it exists
    $toastlast = $toast;
    if ($toast.find('#okBtn').length) {
        $toast.delegate('#okBtn', 'click', function () {
            alert('you clicked me. i was toast #' + toastIndex + '. goodbye!');
            $toast.remove();
        });
    }
    if ($toast.find('#surpriseBtn').length) {
        $toast.delegate('#surpriseBtn', 'click', function () {
            alert('Surprise! you clicked me. i was toast #' + toastIndex + '. You could perform an action here.');
        });
    }

    $('#clearlasttoast').click(function () {
        toastr.clear($toastlast);
    });
}
$(document).on('change', 'input[name="Discount"]', function () {
    var discount = $(this).val();
    if (discount < parseInt(0) || discount > parseInt(100) || !$.isNumeric(discount)) {
        var str = 'Please enter proper discount';
        GetToster(str, 'error', 'Error Notification');
        $(this).val('');
        return false;
    }
    if (discount == "") {
        discount = "0";
    }
    var TotalAmount = $('input[name="GetTotalAmount"]').val();
    var TotalVatAmount = $('input[name="GetTotalVatAmount"]').val();
    var TotalServiceAmount = $('input[name="GetTotalServiceAmount"]').val();
    DiscountOnTotalAmount(discount, TotalAmount);
    DiscountOnVat(discount, TotalVatAmount);
    DiscountOnServiceAmount(discount, TotalServiceAmount);
    var NetAmount = (DiscountOnTotalAmount(discount, TotalAmount) + DiscountOnVat(discount, TotalVatAmount) + DiscountOnServiceAmount(discount, TotalServiceAmount)).toFixed(2);
    var DiscountAmount = ((TotalServiceAmount - DiscountOnServiceAmount(discount, TotalServiceAmount)) + (TotalVatAmount - DiscountOnVat(discount, TotalVatAmount)) + (TotalAmount - DiscountOnTotalAmount(discount, TotalAmount))).toFixed(2)
    $('input[name="NetAmount"]').val(NetAmount);
    $('input[name="DiscountAmount"]').val(DiscountAmount);
});
function DiscountOnTotalAmount(data, totalAmount) {
    var DiscountAmount = (parseFloat(data) * parseFloat(totalAmount)) / 100;
    var RemaningAmount = totalAmount - DiscountAmount;
    $('input[name="TotalAmount"]').val(RemaningAmount.toFixed(2));
    return RemaningAmount;
}
function DiscountOnVat(data, TotalVatAmount) {
    var DiscountAmount = (parseFloat(data) * parseFloat(TotalVatAmount)) / 100;
    var RemaningAmount = TotalVatAmount - DiscountAmount;
    $('input[name="VatAmount"]').val(RemaningAmount.toFixed(2));
    return RemaningAmount;
}
function DiscountOnServiceAmount(data, TotalServiceAmount) {
    var DiscountAmount = (parseFloat(data) * parseFloat(TotalServiceAmount)) / 100;
    var RemaningAmount = TotalServiceAmount - DiscountAmount;
    $('input[name="ServiceCharge"]').val(RemaningAmount.toFixed(2));
    return RemaningAmount;
}
$(document).on('change', '#Cash_amt', function () {
    var amount = $(this).val();
    var NetAmount = $('#Net_amount').text();
    var Remaning = (parseFloat(amount) - parseFloat(NetAmount)).toFixed(2);
    $('#Cash_Exchange').val(Remaning);


});

$(document).on('keydown', '#CustomerName', function (e) {
    //if (e.which < 97 /* a */ || e.which > 122 /* z */) {
    //    var str = 'Please enter characters only';
    //    GetToster(str, 'error', 'Error Notification');
    //    e.preventDefault();
    //}
    if (e.shiftKey) {
        //e.preventDefault();
        //var str = 'Please enter characters only';
        //GetToster(str, 'error', 'Error Notification');
    } else {
        var key = e.keyCode;
        if (!((key == 8) || (key == 32) || (key == 46) || (key >= 35 && key <= 40) || (key >= 65 && key <= 90))) {
            e.preventDefault();
            var str = 'Please enter characters only';
            GetToster(str, 'error', 'Error Notification');
        }
    }

});
$(document).on('click', '#btn_Call_Dispatch', function (e) {
    var pay = $('#PaymentType').val();
    var address = $('#CustomerAddress').val();
   
    if (pay == "") {
        e.preventDefault();
        $('.modal').modal('hide');
        var str = 'Please Select Payment Type';
        GetToster(str, 'error', 'Error Notification');
        return false;
    }
    else if (address=="") {
        e.preventDefault();
        $('.modal').modal('hide');
        var str = 'Please Enter Customer Address';
        GetToster(str, 'error', 'Error Notification');
        return false;
    }
    else {
        $('#d').modal('show');
    }
    var NetAmount = $('input[name="NetAmount"]').val();
    $('#Net_amount').text(NetAmount);

});
$(document).on('change', '#PaymentType', function () {
    var eval = $(this).val();
    if (eval == "Cheque") {
        $('#cheque_detail').fadeIn();
    }
    else {
        $('#cheque_detail').fadeOut();
    }

});

function fnOpenNormalDialog(str) {
    $("#dialog-confirm").html("are you sure want to delete this item ?");

    // Define the Dialog and its properties.
    $("#dialog-confirm").dialog({
        resizable: false,
        modal: true,
        title: "Delete",
        height: 250,
        width: 400,
        buttons: {
            "Yes": function () {
                $(this).dialog('close');
                callback(true,str);
            },
            "No": function () {
                $(this).dialog('close');
                callback(false,str);
            }
        }
    });
}

$('#btnOpenDialog').click(function () {
    fnOpenNormalDialog();
});

function callback(value,id) {
    if (value) {
        DeleteItem(id);
    } else {
        return false;
    }
}