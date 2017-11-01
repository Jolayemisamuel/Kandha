

function GetToster(str, shortCutFunction, title) {
    var toastCount = 0;
    var $toastlast;
    var shortCutFunction = shortCutFunction;
    var msg = str;
    var title = title;

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
$(document).on('change', '#VendorId', function () {
    var $val = $(this).val();
    if ($val != "") {
        window.location = "/Vendor/CreateBilling?VendorId=" + $val;
    }
})

function checkQty(str) {
    var $form = $('#_ajax_form');
    $form.find('input[type="hidden"][name="ItemId"]').val(str);
    var Qty = $('input[name="Qty"]').val();
    var value = Qty.replace(/^\s\s*/, '').replace(/\s\s*$/, '');
    var intRegex = /^\d+$/;
    if (!intRegex.test(value)) {
        var string = 'please enter correct quantity';
        GetToster(string, 'error', 'Error Notification');
        return false;
    }
    if (!$.isNumeric(Qty) || Qty < parseInt(1)) {
        var string = 'please enter correct quantity';
        GetToster(string, 'error', 'Error Notification');
        return false;
    }
    else {
        return true;
    }
    // var status = checkoutstockItem(str, Qty);
}

function checkoutstockItem(Id, Qty) {
    var status;
    var Data = { ItemId: Id, Qty: Qty, TableNo: RunningTable }
    $.get("/Nibs/checkOutStock", Data, function (result) {

        if (result == "True") {
            // ShowBillingItemsOnItem(Id, RunningTable, Qty, Type);
            return true;
        }
        else {
            var str = 'This item is out of stock !!!!';
            GetToster(str, 'error', 'Error Notification');
            return false;
        }
    });
}

$(document).on('change', '#PaymentMethod', function () {
    var $val = $(this).val();
    if ($val != "") {

        if ($val == "Cheque") {
            $('#_div_check').show();
            $('#_div_due').hide();
            $('.date-picker').datepicker({
                rtl: Metronic.isRTL(),
                orientation: "left",
                autoclose: true
            });
            $('#DepositAmount').val('');
            $('#remainingAmount').val('');
        }
        else if ($val == "Due") {
            $('#_div_check').hide();
            $('#_div_due').show();
            $('#ChequeDate').val('');
            $('#ChequeNo').val('');
        }
            //else if ($val=="Cash") {
            //    $('input[name="NetAmount"][type="hidden"]').val();

            //}
        else {
            $('#_div_check').hide();
            $('#_div_due').hide();
            $('#DepositAmount').val('');
            $('#remainingAmount').val('');
            $('#ChequeDate').val('');
            $('#ChequeNo').val('');
        }
    }
});
$(document).on('keyup', '#DepositAmount', function () {
    var $val = $(this).val();
    var $net = $('input[name="NetAmount"][type="hidden"]').val();
    if (parseFloat($val) > parseFloat($net)) {
        var str = "please enter equal to NetAmount";
        GetToster(str, 'warning', 'Warning Notification');
        var $remaining = parseFloat($net) - parseFloat($val);
        $('input[name="remainingAmount"]').val($remaining);

    }
    else {
        var $remaining = parseFloat($net) - parseFloat($val);
        $('input[name="remainingAmount"]').val($remaining);
    }
});
function AddHeight() {
    $(".DivItemsDisplay").animate({ "scrollTop": $('.DivItemsDisplay')[0].scrollHeight }, "slow");

}

$(document).on('click', '._clear_kot', function (e) {
    //  var RunningTable = $('input[name="TableNo"]').val();
    $('.modal').modal('hide');
    printKot();
    ClearKot();
    e.preventDefault();
});
function printKot() {
    var contents = $("#KotDiv").html();
    var frame1 = $('<iframe />');
    frame1[0].name = "frame1";
    frame1.css({ "position": "absolute", "top": "-1000000px" });
    $("body").append(frame1);
    var frameDoc = frame1[0].contentWindow ? frame1[0].contentWindow : frame1[0].contentDocument.document ? frame1[0].contentDocument.document : frame1[0].contentDocument;
    frameDoc.document.open();
    //Create a new HTML document.
    frameDoc.document.write('<html><head><title>Nibs Cafe Kot Print</title>');
    frameDoc.document.write('</head><body>');
    //Append the external CSS file.
    //frameDoc.document.write('<link href="~/assets/global/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" type="text/css" />');
    //frameDoc.document.write('<link href="~/assets/admin/layout/css/print.css" rel="stylesheet" type="text/css" />');
    //Append the DIV contents.
    frameDoc.document.write(contents);
    frameDoc.document.write('</body></html>');
    frameDoc.document.close();
    setTimeout(function () {
        window.frames["frame1"].focus();
        window.frames["frame1"].print();
        frame1.remove();
    }, 500);
}
function ClearKot() {
    var val = $('#VendorId').val();
    if (val != "") {
        $.get("/Vendor/ClearKot?VendorId=" + val, function (data) {
            if (data == "True") {
                $('#KotDiv').html('');
            }
        });
    }

}
$(document).on('click', '#print', function () {
    var val = $('#VendorId').val();
    if (val != "") {
        centeredPopup("/Vendor/_printBill?VendorId=" + val, 'myWindow', '600', '400', 'yes');

    }
});

var popupWindow = null;
function centeredPopup(url, winName, w, h, scroll) {
    LeftPosition = (screen.width) ? (screen.width - w) / 2 : 0;
    TopPosition = (screen.height) ? (screen.height - h) / 2 : 0;
    settings =
    'height=' + h + ',width=' + w + ',top=' + TopPosition + ',left=' + LeftPosition + ',scrollbars=' + scroll + ',resizable'
    popupWindow = window.open(url, winName, settings);
    popupWindow.onbeforeunload = function () {
        var val = $('#VendorId').val();
        if (val != "") {
            ShowBillingItemsOnPrint(val)
        }
        return null;
    }

}
function ShowBillingItemsOnPrint(str) {
    var xmlhttp;
    if (str == "") {
        document.getElementById("Billing_table").innerHTML = "";
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

            document.getElementById("Billing_table").innerHTML = xmlhttp.responseText;

            AddHeight();
            $('.date-picker').datepicker({
                rtl: Metronic.isRTL(),
                orientation: "left",
                autoclose: true
            });
            var print = $('input[name="IsPrinted"][type="hidden"]').val();
            if (print == "True") {
                DisableAddItem();
            }
        }
    }

    xmlhttp.open("GET", "/Vendor/_getBillItemOnPrint?VendorID=" + str, true);
    xmlhttp.send();


}
function DisableAddItem() {


    //var RunningTable = $('input[name="TableNo"]').val();
    //$('#' + RunningTable).removeClass('current').addClass('printed');

    $("#VendorId").prop("disabled", true);
    $('#_dispatch').removeClass('disabled');
}

function checkBillPrint() {
    var print = $('input[name="IsPrinted"][type="hidden"]').val();
    if (print != "True") {
        return true;
    }
    else {
        var str = "Bill has been Printed !!!";
        GetToster(str, 'warning', 'Warning Notification');
        return false;
    }
}