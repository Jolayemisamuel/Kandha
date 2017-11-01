$(document).on('click', '._table_for_merged', function () {
    var $this = $(this);
    var $val = $this.val();
    $('#MergedTable').find('input[value="' + $val + '"]').show();
    $('#MergedTable').find('input[value="' + $val + '"]').addClass('merge')
});
$(document).on('click', '._table_merged', function () {
    var $this = $(this);
    var $val = $this.val();
    $('#MasterTable').find('input._table_master_merged').hide();
    $('#MasterTable').find('input[value="' + $val + '"]').show();
    $('#MasterTable').find('input[value="' + $val + '"]').addClass('master_merge');
});
$(document).on('click', '#btn-merge', function () {
    var mergetable = "";
    $('#MergedTable input[type="button"].merge').each(function () {
        var $this = $(this);
        var dd = $this.val();
        mergetable += dd + ',';
    });
    var str = itemval = mergetable.substring(0, mergetable.length - 1);
    var master = $('#MasterTable input[type="button"].master_merge').val();
    var append = '<input type="hidden" id="' + master + '" value="' + str + '" />';
    $('#_put_after_merged').append(append);
    var split = str.split(",");
    for (var i = 0; i < split.length; i++) {
        if (split[i] != master) {
            $('input[name="table"]#' + split[i]).attr("disabled", "disabled");
        }

    }
    $('input[name="table"]#' + master).addClass('current').addClass('table_merge');
    $('.modal').modal('hide');
});

$(document).on('click', '#_btn_merged', function (e) {
    var BillId = $('#BillId').val();
    if (parseInt(BillId)>0) {
        var str = 'Bill has printed !';
        GetToster(str, 'warning', 'Error Notification');
        return false;
    }
    margeTable();
    e.preventDefault();
    // alsomerged();
    //$('.modal').modal('show');
});
$(document).on('click', '#btnshiftclickpopup', function (e) {
    shiftTable();
    e.preventDefault();
    // alsomerged();
    //$('.modal').modal('show');
});
function margeTable() {
    var xmlhttp;
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {

            document.getElementById("_div_merged_table").innerHTML = xmlhttp.responseText;
            alsomerged();
            $('#MergerTable').modal('show');
        }
    }
    xmlhttp.open("GET", "/Nibs/_MargeTable", true);
    xmlhttp.send();
}
function shiftTable() {
    var xmlhttp;
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {

            document.getElementById("_div_shift_table").innerHTML = xmlhttp.responseText;
            $('#Shift').modal('show');
        }
    }
    xmlhttp.open("GET", "/Nibs/_shiftTable", true);
    xmlhttp.send();
}
function alsomerged() {
    var alsomerge = "";
    $('input[name=table]').each(function () {
        var $this = $(this);
        if ($this.is('[disabled=disabled]')) {
            alsomerge += $this.val() + ",";
        }
    });
    var str = alsomerge.substring(0, alsomerge.length - 1);
    for (var i = 0; i < str.length; i++) {
        $('input[type="button"]._table_for_merged').each(function () {
            var chk = $(this).val();
            if (chk == str[i]) {
                $('#TableForMerged input[value="' + str[i] + '"]').addClass('hello')
            }
        });
    }
}

function Removemerge(str) {
    var remove = $('#_put_after_merged input[type="hidden"]#' + str).val();
    if (typeof remove === "undefined" || remove != "") {

    }
    else {
        var split = remove.split(",");
        for (var i = 0; i < split.length; i++) {
            $('input[name="table"]#' + split[i]).removeAttr('disabled');
            $('input[name="table"]#' + split[i]).removeClass('table_merge');
        }
        $('#_put_after_merged input[type="hidden"]#' + str).remove();
    }

}
//========================================================= code for shift table===================

$(document).on('click', '._table_for_shift', function () {
    var $this = $(this);
    var $val = $this.val();
    $('#shifted').find('input._table_for_shifted').hide();
    $('#shifted').find('input[value="' + $val + '"]').removeClass('shift_to');
    $('#shifted').find('input[value="' + $val + '"]').show();
    $('#shifted').find('input[value="' + $val + '"]').addClass('shift_to');
});
$(document).on('click', '._table_to_shift', function () {
    var $this = $(this);
    var $val = $this.val();
    $('#ShiftMasterTable').find('input._table_to_shifted').hide();
    $('#ShiftMasterTable').find('input[value="' + $val + '"]').removeClass('shifted_to');
    $('#ShiftMasterTable').find('input[value="' + $val + '"]').show();
    $('#ShiftMasterTable').find('input[value="' + $val + '"]').addClass('shifted_to');
});
$(document).on('click', '#btn-shift', function () {

    var shiftFor = $('#shifted input.shift_to').val();
    var shiftTo = $('#ShiftMasterTable input.shifted_to').val();
    $('.modal').modal('hide');
    SaveshiftTable(shiftFor, shiftTo);
});

function SaveshiftTable(shiftFor, shiftTo) {
    var xmlhttp;
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
            if (xmlhttp.responseText == "True") {
                var str = "Table No: " + shiftFor + " shifted to Table No :" + shiftTo + " successfully...";
                var notification = "Success Notification"
                GetTosterFull(str, "success", notification, shiftFor, shiftTo);
                $('input[name="table"]#' + shiftTo).addClass("current");
                $('input[name="table"]#' + shiftFor).removeClass("current");
            }
            else {
                var str = "Table No: " + shiftFor + " not shifted to Table No :" + shiftTo;
                var notification = "Error Notification"
                GetTosterFulld(str, "error", notification, shiftFor, shiftTo);
            }
            //document.getElementById("_div_shift_table").innerHTML = xmlhttp.responseText;

        }
    }
    xmlhttp.open("GET", "/Nibs/ShiftTable?Shiftfrom=" + shiftFor + "&ShiftTo=" + shiftTo, true);
    xmlhttp.send();
}
function GetTosterFull(str, shortcutfunction, notification) {
    var toastCount = 0;
    var $toastlast;
    var shortCutFunction = shortcutfunction;
    var msg = str;
    var title = notification;
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
$(document).on('click', '._return_item', function (e) {
    var BillId = $('#BillId').val();
    if (parseInt(BillId) > 0) {
        var str = 'Bill has printed !';
        GetToster(str, 'warning', 'Warning Notification');
        return false;
    }
    
    e.preventDefault();
    var kot = $('#KotDiv table tbody').html();
    var str = "";
    if (kot.trim() == "") {
        str = 'Item has ready ! Do you want to return this item ?';
        //GetToster(str, 'warning', 'Warning Notification');
    }
    else{
        str = "Do you want to return this item ? ";
    }
    var Id = $(this).attr('Id');
    if (!confirm(str)) {
        return false;
    }
    else {
        
    }
    var RunningTable = $('input[name="TableNo"]').val();
    ReturnItem(Id, RunningTable);
});
function ReturnItem(ItemId, TableNo) {
    var xmlhttp;
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
            var str = "Item Successfuly Return from Table No :" + TableNo;
            var notification = "Success Notification"
            GetTosterFull(str, "success", notification);
            document.getElementById("Div_Billing_items").innerHTML = xmlhttp.responseText;
        }
    }
    xmlhttp.open("GET", "/Nibs/_Return?TableNo=" + TableNo + "&ItemId=" + ItemId, true);
    xmlhttp.send();
}

$(document).on('change', 'input[name="OrderType"]:radio', function (e) {
    var RunningTable = $('input[name="TableNo"]').val();
    var $this = $(this).attr('data-id');
    var BillId = $('#BillId').val();
    if (parseInt(BillId) == 0) {
        if (typeof RunningTable === "undefined") {
            var str = 'Please Select Table No';
            GetToster(str, 'error', 'Error Notification');
            return false;
        }
        if (!$(this).parent('span').parent('div.radio').hasClass('disabled')) {
            if ($this == "T") {
                $('#_Customer_charges').show();
                $('#_customer_address').hide();
                $('#CustomerAddress').val('');
                $('#optionsRadios4').removeAttr("checked");
            }
            if ($this == "H") {
                $('#_customer_address').show();
                $('#_Customer_charges').show();
                $('#optionsRadios4').removeAttr("checked");
            }
            if ($this == "R") {
                $('#_Customer_charges').hide();
                $('#_customer_address').hide();
                $('#PackingCharges').val(0);
                $('#CustomerAddress').val('');
            }
    }
    
    }
    else {
        var str = 'Bill has printed !';
        GetToster(str, 'warning', 'Warning Notification');
        e.preventDefault();
        return false;
    }

});
//$(document).on('change', 'select[name="ItemAutocomplete"]', function () {

//    //var txt = $('select[name="ItemAutocomplete"]:selected').text();
//    var Id = $(this).val();
//    var RunningTable = $('input[name="TableNo"]').val();
//    if (typeof RunningTable === "undefined") {
//        var str = 'Please Select Table No';
//        GetToster(str, 'error', 'Error Notification');
//        return false;
//    }
//    var Qty ='1';
//    //var value = Qty.replace(/^\s\s*/, '').replace(/\s\s*$/, '');
//    //var intRegex = /^\d+$/;
//    //if (!intRegex.test(value)) {
//    //    var str = 'please enter correct quantity';
//    //    GetToster(str, 'error', 'Error Notification');
//    //    return false;
//    //}
//    //if (!$.isNumeric(Qty) || Qty < parseInt(1)) {
//    //    var str = 'please enter correct quantity';
//    //    GetToster(str, 'error', 'Error Notification');
//    //    return false;
//    //}
//    var Type = 'Full';
//   // ShowBillingItemsOnItem(Id, RunningTable, Qty, Type);
//});

$(document).on('click', '#_Add_btn_autocomplete', function () {
    var BillId = $('#BillId').val();
    if (parseInt(BillId) > 0) {
        var str = 'Bill has printed !';
        GetToster(str, 'warning', 'Warning Notification');
        return false;
    }
    var RunningTable = $('input[name="TableNo"]').val();
    if (typeof RunningTable === "undefined") {
        var str = 'Please Select Table No';
        GetToster(str, 'error', 'Error Notification');
        return false;
    }
    var Id = $('#ItemAutocomplete option:selected').val();
    if (typeof Id === "undefined" || Id == "") {
        var str = 'Please Select Item Name';
        GetToster(str, 'error', 'Error Notification');
        return false;
    }
    var qty = $('#Qty_autocomplete').val();
    var value = qty.replace(/^\s\s*/, '').replace(/\s\s*$/, '');
    var intRegex = /^\d+$/;
    if (!intRegex.test(value)) {
        var str = 'please enter correct quantity';
        GetToster(str, 'error', 'Error Notification');
        return false;
    }
    if (!$.isNumeric(qty) || qty < parseInt(1)) {
        var str = 'please enter correct quantity';
        GetToster(str, 'error', 'Error Notification');
        return false;
    }
    var Type = 'Full';
    ShowBillingItemsOnItem(Id, RunningTable, qty, Type);
    //$('#s2id_ItemAutocomplete').addClass('select2-dropdown-open');
    //$('#ItemAutocomplete').focus();
    $("#ItemAutocomplete").select2("open");
    $('#s2id_autogen1_search').focus();
    $('#Qty_autocomplete').val('1');
});
// from home page


function showSubItem(str) {
    var xmlhttp;
    if (str == "") {
        document.getElementById("Items_Div").innerHTML = "";
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
            document.getElementById("Items_Div").innerHTML = xmlhttp.responseText;
        }
    }
    xmlhttp.open("GET", "/Nibs/_GetAllItemPartial?Id=" + str, true);
    xmlhttp.send();
}
$(document).on('click', '._sub_item', function (e) {
    var id = $(this).attr('Id');
    var url = $(this).attr('href');
    var BillId = $('#BillId').val();
    if (parseInt(BillId) > 0) {
        var str = 'Bill has printed !';
        GetToster(str, 'warning', 'Warning Notification');
        return false;
    }
    showSubItem(id);
    e.preventDefault();
});
//==========================
function ShowBillingItems(str) {
   
    var xmlhttp;
    if (str == "") {
        document.getElementById("Div_Billing_items").innerHTML = "";
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

            document.getElementById("Div_Billing_items").innerHTML = xmlhttp.responseText;
            var ordertype = $('#Get_OrderType').val();
            if (ordertype!="") {
                if (ordertype == "H") {
                    $('#uniform-optionsRadios4').children('span').removeClass('checked');
                    $('#uniform-optionsRadios4').children('span').children('input[type=radio]#optionsRadios4').removeAttr('checked');

                    $('#uniform-optionsRadios5').children('span').removeClass('checked');
                    $('#uniform-optionsRadios5').children('span').children('input[type=radio]#optionsRadios4').removeAttr('checked');
                    $('#uniform-optionsRadios6').children('span').addClass('checked');
                    $('#uniform-optionsRadios6').children('span').children('input[type=radio]#optionsRadios4').attr('checked');
                   // $('#optionsRadios6').prop('checked', true);
                    var address = $('#Get_CustomerAddress').val();
                    $('#_customer_address').show();
                    $('#CustomerAddress').val(address);
                    var packing = $('#Get_packingCharges').val();
                    $('#_Customer_charges').show();
                    $('#PackingCharges').val(packing)
                }
                else if (ordertype == "T") {
                    $('#uniform-optionsRadios4').children('span').removeClass('checked');
                    $('#uniform-optionsRadios4').children('span').children('input[type=radio]#optionsRadios4').removeAttr('checked');
                    $('#uniform-optionsRadios6').children('span').removeClass('checked');
                    $('#uniform-optionsRadios6').children('span').children('input[type=radio]#optionsRadios4').removeAttr('checked');
                    $('#uniform-optionsRadios5').children('span').addClass('checked');
                    $('#uniform-optionsRadios5').children('span').children('input[type=radio]#optionsRadios4').attr('checked');
                    $('#_Customer_charges').show();
                }
                else {
                    $('optionsRadios4').prop('checked', true);
                    $('#_Customer_charges').hide();
                    $('#_customer_address').hide();
                }
            }
            else {
                $('#uniform-optionsRadios5').children('span').removeClass('checked');
                $('#uniform-optionsRadios6').children('span').children('input[type=radio]#optionsRadios4').removeAttr('checked');
                $('#uniform-optionsRadios6').children('span').removeClass('checked');
                $('#uniform-optionsRadios6').children('span').children('input[type=radio]#optionsRadios4').removeAttr('checked');
                $('#uniform-optionsRadios4').children('span').addClass('checked');
                $('#uniform-optionsRadios4').children('span').children('input[type=radio]#optionsRadios4').attr('checked');
                $('#_Customer_charges').hide();
                $('#_customer_address').hide();
                $('optionsRadios4').prop('checked', true);

            }
            
            
            $('[data-toggle="tooltip"]').tooltip();
            $('.date-picker').datepicker({
                rtl: Metronic.isRTL(),
                orientation: "left",
                autoclose: true
            });
            AddHeight();
            DisableAddItem();
        }
    }
    xmlhttp.open("GET", "/Nibs/_CreatePartial?Id=" + str, true);
    xmlhttp.send();
}
function DisableAddItem() {
    var BillId = $('#BillId').val();
    if (parseInt(BillId) > 0) {
        var RunningTable = $('input[name="TableNo"]').val();
        $('#' + RunningTable).removeClass('current').addClass('printed');
        
        $("#optionsRadios4").prop("disabled", true);
        $("#optionsRadios4").attr("disabled", true);
        $("#optionsRadios5").prop("disabled", true);
        $("#optionsRadios6").prop("disabled", true);
        $('#uniform-optionsRadios4').addClass("disabled");
        $("#optionsRadios5").parents("span").parent('div.radio').addClass("disabled");
        $("#optionsRadios6").parents("span").parent('div.radio').addClass("disabled");
        $('#ItemAutocomplete').prop("disabled", true);
        $('#open_food_a').attr("disabled", true);
        $('#Discount').prop("disabled", true);
        var charges = $('#PackingCharges').val();
        if (parseInt(charges)>=0) {
            $('#PackingCharges').attr('disabled',true);
        }
        var address = $('#CustomerAddress').val();
        if (address.trim()!="") {
            $('#CustomerAddress').attr('disabled', true);
        }
    }
    else {
        $("#optionsRadios4").removeAttr("disabled");
        $("#optionsRadios5").removeAttr("disabled");
        $("#optionsRadios6").removeAttr("disabled");
        $('#uniform-optionsRadios4').removeClass("disabled")
        $("#optionsRadios4").parents("span").parent('div.radio').removeClass("disabled");
        $("#optionsRadios5").parents("span").parent('div.radio').removeClass("disabled");
        $("#optionsRadios6").parents("span").parent('div.radio').removeClass("disabled");
        $('#ItemAutocomplete').prop("disabled", false);
        $('#open_food_a').prop("disabled", false);
        $('#Discount').prop("disabled", false);
        $('#_customer_address').hide();
        $('#_Customer_charges').hide();
        $('#uniform-optionsRadios5').children('span').removeClass('checked');
        $('#uniform-optionsRadios6').children('span').children('input[type=radio]#optionsRadios4').removeAttr('checked');
        $('#uniform-optionsRadios6').children('span').removeClass('checked');
        $('#uniform-optionsRadios6').children('span').children('input[type=radio]#optionsRadios4').removeAttr('checked');
        $('#uniform-optionsRadios4').children('span').addClass('checked');
        $('#uniform-optionsRadios4').children('span').children('input[type=radio]#optionsRadios4').attr('checked');
        

    }
}
function AddHeight() {
    $(".DivItemsDisplay").animate({ "scrollTop": $('.DivItemsDisplay')[0].scrollHeight }, "slow");

}
$(document).on('click', 'input[name="table"]', function () {
    var Id = $(this).val();
    if (!$(this).hasClass("current")) {
        $('input[name="table"]').removeClass('current-active')
        $(this).addClass('current-active');
    }
    ShowBillingItems(Id);
});
//=============================== Show Billing Items
function ShowBillingItemsOnItem(str, RunningTable, Qty, Type) {
    var xmlhttp;
    if (str == "") {
        document.getElementById("Div_Billing_items").innerHTML = "";
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

            document.getElementById("Div_Billing_items").innerHTML = xmlhttp.responseText;
            $('[data-toggle="tooltip"]').tooltip();
            AddHeight();
            DisableAddItem();
           // updateOrderTypeData();
            $('.date-picker').datepicker({
                rtl: Metronic.isRTL(),
                orientation: "left",
                autoclose: true
            });
        }
    }
    var data = new FormData();
    data.append("ItemId", str);
    data.append("Qty", Qty);
    data.append("RunningTable", RunningTable);
    data.append("Type", Type);
    xmlhttp.open("POST", "/Nibs/UpdateBillingXml?ItemId=" + str + "&Qty=" + Qty + "&RunningTable=" + RunningTable + "&Type=" + Type, true);
    xmlhttp.send();
    if ($('#' + RunningTable).hasClass("current")) {

    }
    else {
        $('#' + RunningTable).addClass("current");
    }

}
function updateOrderTypeData()
{
    var $this = $('input:radio[name="OrderType"]:checked').attr('data-id');
    
    if ($this == "H") {

        $('#_customer_address').show();
        $('#_Customer_charges').show()
    }
    else if ($this=="T") {
        $('#_Customer_charges').show()
    }
    else {

        $('#_Customer_charges').hide();
        $('#_customer_address').hide();
    }
 
}
$(document).on('click', '.ajax_btn', function () {
    var Id = $(this).attr('Id');
    var BillId = $('#BillId').val();
    if (parseInt(BillId) > 0) {
        var str = 'Bill has printed !';
        GetToster(str, 'warning', 'Warning Notification');
        return false;
    }
    var RunningTable = $('input[name="TableNo"]').val();
    if (typeof RunningTable === "undefined") {
        var str = 'Please Select Table No';
        GetToster(str, 'error', 'Error Notification');
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
    if (!$.isNumeric(Qty) || Qty < parseInt(1)) {
        var str = 'please enter correct quantity';
        GetToster(str, 'error', 'Error Notification');
        return false;
    }
    var Type = $('input:radio[name=Type]:checked').val();
    var status = checkoutstockItem(Id,RunningTable,Qty,Type);
    

    //
    //ShowBillingItemsOnItem(Id, RunningTable, Qty, Type);
});
 //check outstockItem 
function checkoutstockItem(Id, RunningTable, Qty, Type) {
    var status;
    var Data = { ItemId: Id, Qty: Qty,TableNo:RunningTable }
    $.get("/Nibs/checkOutStock", Data, function (result) {

        if (result == "True") {
            ShowBillingItemsOnItem(Id, RunningTable, Qty, Type);
        }
        else {
                var str = 'This item is out of stock !!!!';
                GetToster(str, 'error', 'Error Notification');
                return false;
        }
    });
}
   
//    //var xmlhttp;
//    //if (Id == "") {
//    //    document.getElementById("Div_Billing_items").innerHTML = "";
//    //    return;
//    //}
//    //if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
//    //    xmlhttp = new XMLHttpRequest();
//    //}
//    //else {// code for IE6, IE5
//    //    xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
//    //}
//    //xmlhttp.onreadystatechange = function () {
//    //    if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
//    //        return xmlhttp.responseText;
//    //    }
//    //}
//    //xmlhttp.open("GET", "/Nibs/checkOutStock?ItemId=" + Id, true);
//    //xmlhttp.send();
//}
//==============================
$(document).on('click', '._delete_item', function (e) {
    e.preventDefault();
    var BillId = $('#BillId').val();
    
   // alert(kot);
    if (parseInt(BillId) > 0) {
        var str = 'Bill has printed !';
        GetToster(str, 'warning', 'Warning Notification');
        return false;
    }
    var Id = $(this).attr('Id');
    var name = $(this).attr("data-itemname");
    var kot = $('#KotDiv table tbody').html();
    var str = "";
    if (kot.trim() == "") {
        str = 'Item has ready ! Do you want to return this item ?';
        //GetToster(str, 'warning', 'Warning Notification');
    }
    else {
        str = "Do you want to return this item ? ";
    }
    if (!confirm(str)) {
        
        return false;
    }
    
    var RunningTable = $('input[name="TableNo"]').val();
    deleteItem(Id, RunningTable, name);
});
function deleteItem(str, RunningTable, name) {
    var xmlhttp;
    if (str == "") {
        document.getElementById("Div_Billing_items").innerHTML = "";
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
            var str = "Item Successfuly delete from Table No :" + RunningTable;
            var notification = "Success Notification"
            GetTosterFull(str, "success", notification);
            document.getElementById("Div_Billing_items").innerHTML = xmlhttp.responseText;
            AddHeight();
        }
    }
    xmlhttp.open("GET", "/Nibs/DeleteItem?Id=" + str + "&TableNo=" + RunningTable + "&ItemName=" + name, true);
    xmlhttp.send();
}

//================== clear Kot
$(document).on('click', '._clear_kot', function (e) {
    var RunningTable = $('input[name="TableNo"]').val();
    $('.modal').modal('hide');
    printKot();
    ClearKot(RunningTable);
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
function ClearKot(str) {
    var xmlhttp;
    if (str == "") {
        document.getElementById("Div_Billing_items").innerHTML = "";
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

            document.getElementById("Div_Billing_items").innerHTML = xmlhttp.responseText;
            AddHeight();
        }
    }
    xmlhttp.open("GET", "/Nibs/ClearKotItem?Id=" + str, true);
    xmlhttp.send();
}
//=============== discount===============================
$(document).on('change', 'input[name="Discount"]', function () {
    var discount = $(this).val();
    var BillId = $('#BillId').val();
    if (parseInt(BillId) > 0) {
        var str = 'Bill has printed !';
        GetToster(str, 'warning', 'Warning Notification');
        $('input[name="Discount"]').val('');
        return false;
    }
    if (discount == "") {
        discount = "0";
        $(this).val("0");
    }
    if (discount < parseInt(0) || discount > parseInt(100) || !$.isNumeric(discount)) {
        var str = 'Please enter proper discount';
        GetToster(str, 'error', 'Error Notification');
        $(this).val('');
        return false;
    }
    CalculateDiscount(discount,false);
    

});
function CalculateDiscount(discount,str)
{
    var TotalAmount = $('input[name="GetTotalAmount"]').val();
    var TotalVatAmount = $('input[name="GetTotalVatAmount"]').val();
    var TotalServiceChargeAmount = $('input[name="GetTotalServiceAmount"]').val();
    var TotalServiceTaxAmount = $('input[name="GetTotalServiceTaxAmount"]').val();
    var discountTotal = DiscountOnTotalAmount(discount, TotalAmount);
    var discountVat = DiscountOnVat(discount, TotalVatAmount);
    var discountServiceCharge = DiscountOnServiceAmount(discount, TotalServiceChargeAmount);
    var discountServicetax = DiscountOnServicesTax(discount, TotalServiceTaxAmount);
    var WithoutDiscountNetAmount = parseFloat(TotalAmount) + parseFloat(TotalVatAmount) + parseFloat(TotalServiceChargeAmount) + parseFloat(TotalServiceTaxAmount)
    //var NetAmount = (DiscountOnTotalAmount(discount, TotalAmount) + DiscountOnVat(discount, TotalVatAmount) + DiscountOnServiceAmount(discount, TotalServiceChargeAmount) + DiscountOnServicesTax(discount, TotalServiceTaxAmount)).toFixed(2);
    //var DiscountAmount = (( DiscountOnServiceAmount(discount, TotalServiceChargeAmount)) + (TotalVatAmount - DiscountOnVat(discount, TotalVatAmount)) + (TotalAmount - DiscountOnTotalAmount(discount, TotalAmount)) + (DiscountOnServicesTax(discount, TotalServiceTaxAmount))).toFixed(2)
    var NetAmount = parseFloat(discountTotal) + parseFloat(discountVat) + parseFloat(discountServiceCharge) + parseFloat(discountServicetax);
    var AmountDiscount = parseFloat(WithoutDiscountNetAmount) - parseFloat(NetAmount);
    $('input[name="DiscountAmount"]').val(AmountDiscount.toFixed(3));
    if (!str) {
        var PackingCharges = $('#PackingCharges').val();
        if (PackingCharges != 0) {
            NetAmount = parseFloat(NetAmount) + parseFloat(PackingCharges);
        }
    }
    
    //$('input[name="NetAmount"]').val(Math.round(NetAmount, 2));
    $('input[name="NetAmount"]').val(NetAmount.toFixed(3));
  
    return NetAmount;
}
function DiscountOnServicesTax(data, serviceTaxAmount) {
    var DiscountAmount = (parseFloat(data) * parseFloat(serviceTaxAmount)) / 100;
    var RemaningAmount = serviceTaxAmount - DiscountAmount;
    $('input[name="ServiceTax"]').val(RemaningAmount.toFixed(3));
    return RemaningAmount;
}
function DiscountOnTotalAmount(data, totalAmount) {
    var DiscountAmount = (parseFloat(data) * parseFloat(totalAmount)) / 100;
    var RemaningAmount = totalAmount - DiscountAmount;
    $('input[name="TotalAmount"]').val(RemaningAmount.toFixed(3));
    return RemaningAmount;
}
function DiscountOnVat(data, TotalVatAmount) {
    var DiscountAmount = (parseFloat(data) * parseFloat(TotalVatAmount)) / 100;
    var RemaningAmount = TotalVatAmount - DiscountAmount;
    $('input[name="VatAmount"]').val(RemaningAmount.toFixed(3));
    return RemaningAmount;
}
function DiscountOnServiceAmount(data, TotalServiceAmount) {
    var DiscountAmount = (parseFloat(data) * parseFloat(TotalServiceAmount)) / 100;
    var RemaningAmount = TotalServiceAmount - DiscountAmount;
    $('input[name="ServicesCharge"]').val(RemaningAmount.toFixed(3));
    return RemaningAmount;
}
$(document).on('keyup', '#Cash_amt', function () {
    var amount = $(this).val();
    var NetAmount = $('#Net_amount').text();
    var Remaning = (parseFloat(amount) - parseFloat(NetAmount)).toFixed(2);
    $('#Cash_Exchange').val(Remaning);


});
$(document).on('click', '#btn_Call_Dispatch', function (e) {
    var pay = $('#PaymentType').val();
    var OrderType = $('input:radio[name="OrderType"]:checked').attr('data-id');
    var Discount = $('#Discount').val();
    if (pay == "") {
        e.preventDefault();
        $('.modal').modal('hide');
        var str = 'Please Select Payment Type';
        GetToster(str, 'error', 'Error Notification');
        return false;
    }

    else if (OrderType == "T") {
        var PackingCharges = $('#PackingCharges').val();
        var value = PackingCharges.replace(/^\s\s*/, '').replace(/\s\s*$/, '');
        var intRegex = /^\d+$/;
        if (!intRegex.test(value)) {
            var str = 'please enter correct charges';
            GetToster(str, 'error', 'Error Notification');
            return false;
        }
        
        if (!$.isNumeric(PackingCharges) || PackingCharges < parseInt(0)) {
            var str = 'please enter correct charges';
            GetToster(str, 'error', 'Error Notification');
            return false;
        }
       
        if (parseFloat(Discount)>0) {
            var CustomerName = $('#CustomerName').val();
            if (CustomerName == "") {
                var str = 'please enter Customer Name';
                GetToster(str, 'error', 'Error Notification');
                return false
            }
            else {
                $('#d').modal('show');
            }
        }
        else {
            $('#d').modal('show');
        }
        
    }
    else if (OrderType == "H") {
        var CustomerAddress = $('#CustomerAddress').val();
        var ContactNo = $('#ContactNo').val();
        var mob = /^[1-9]{1}[0-9]{9}$/;
        if (CustomerAddress == "") {
            var str = 'please enter Address';
            GetToster(str, 'error', 'Error Notification');
            return false
        }
        
        if (ContactNo == "") {
            var str = 'please enter Contact No';
            GetToster(str, 'error', 'Error Notification');
            return false
        }
        if (!mob.test(ContactNo)) {
            var str = 'please enter correct Contact No';
            GetToster(str, 'error', 'Error Notification');
            $('#ContactNo').focus();
            return false
        }
        if (parseFloat(Discount) > 0) {
            var CustomerName = $('#CustomerName').val();
            if (CustomerName == "") {
                var str = 'please enter Customer Name';
                GetToster(str, 'error', 'Error Notification');
                return false
            }
            else {
                $('#d').modal('show');
            }
        }
        else {
            $('#d').modal('show');
        }
       
    }
    else if (parseFloat(Discount)>0) {
        var CustomerName = $('#CustomerName').val();
        if (CustomerName == "") {
            var str = 'please enter Customer Name';
            GetToster(str, 'error', 'Error Notification');
            return false
        }
        else {
            $('#d').modal('show');
        }
    }
    else {
        $('#d').modal('show');
    }
   
   
    var NetAmount = $('input[name="NetAmount"]').val();
    $('#Net_amount').text(NetAmount);

});
//====================== dispatch order========================
$(document).on('click', '#_Bill_dispatch', function (e) {
    $('.modal').modal('hide');
    Removemerge($('input[name="TableNo"]').val());
   
    if ($('#PrintOrNot').is(':checked')) {
        printduringDispatch();
        setTimeout(function () {
            DispatchOrder();
        }, 3000);
    }
    else {
        DispatchOrder();
    }
   
   

    e.preventDefault();
});
function printduringDispatch() {
    var RunningTable = $('input[name="TableNo"]').val();
    var CustomerName = $('input[name="CustomerName"]').val();
    var TableNo = $('input[name="TableNo"]').val();
    var Discount = $('input[name="Discount"]').val();
    var VatAmount = $('input[name="VatAmount"]').val();
    var ServiceCharge = $('input[name="ServicesCharge"]').val();
    var ServiceTax = $('input[name="ServiceTax"]').val();
    var DiscountAmount = $('input[name="DiscountAmount"]').val();
    var TotalAmount = $('input[name="TotalAmount"]').val();
    var NetAmount = $('input[name="NetAmount"]').val();
    var CustomerAddress = $('#CustomerAddress').val();
    var PackingCharges = $('#PackingCharges').val();
    var BillId = $('#BillId').val();

    centeredPopup("/Nibs/PrintOrder?TableNo=" + RunningTable + "&Discount=" + Discount + "&VatAmount=" + VatAmount + "&ServicesCharge=" + ServiceCharge + "&DiscountAmount=" + DiscountAmount + "&TotalAmount=" + TotalAmount + "&NetAmount=" + NetAmount + "&CustomerName=" + CustomerName + "&ServiceTax=" + ServiceTax + "&BillId=" + BillId + "&CustomerAddress=" + CustomerAddress + "&PackingCharges=" + PackingCharges, 'myWindow', '600', '400', 'yes');
    return false;

    

}
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

            document.getElementById("Div_Billing_items").innerHTML = xmlhttp.responseText;
            
            GetToster('Order dispatched successfully from Table No' + $('input[name="TableNo"]').val(), 'success', 'Success Notification');
            DisableAddItem();
            //$('input[name="OrderType"]:radio').val('R').change();
        }
    }
    var CustomerName = $('input[name="CustomerName"]').val();
    var TableNo = $('input[name="TableNo"]').val();
    var PaymentType = $('select[name="PaymentType"]').val();
    var CheckDate = $('input[name="CheckDate"]').val();
    var ChequeNo = $('input[name="ChequeNo"]').val();
    var Discount = $('input[name="Discount"]').val();
    var VatAmount = $('input[name="VatAmount"]').val();
    var ServiceCharge = $('input[name="ServicesCharge"]').val();
    var ServiceTax = $('input[name="ServiceTax"]').val();
    var DiscountAmount = $('input[name="DiscountAmount"]').val();
    var TotalAmount = $('input[name="TotalAmount"]').val();
    var NetAmount = $('input[name="NetAmount"]').val();
    var form = $('#_ajax_Bill_form');
    var OrderType = $('input:radio[name="OrderType"]:checked').attr('data-id');
    var BillId = $('#BillId').val();
    var PackingCharges = $('#PackingCharges').val();
    var ContactNo = $('#ContactNo').val();
    var CustomerAddress = $('#CustomerAddress').val();
   
    var url = form.attr('action');
    var data = new FormData();

    data.append("CustomerName", CustomerName);
    data.append("TableNo", TableNo);
    data.append("PaymentType", PaymentType);
    data.append("CheckDate", CheckDate);
    data.append("ChequeNo", ChequeNo);
    data.append("Discount", Discount);
    data.append("VatAmount", VatAmount);
    data.append("ServicesCharge", ServiceCharge);
    data.append("DiscountAmount", DiscountAmount);
    data.append("TotalAmount", TotalAmount);
    data.append("NetAmount", NetAmount);
    data.append("OrderType", OrderType);
    data.append("PackingCharges", PackingCharges);
    data.append("ContactNo", ContactNo);
    data.append("CustomerAddress", CustomerAddress);
    data.append("ServiceTax", ServiceTax);
    data.append("BillId", BillId);
    xmlhttp.open("POST", url, true);
    xmlhttp.send(data);
    $('#' + TableNo).removeClass("current");
    $('#' + TableNo).removeClass("printed");

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

$(document).on('click', '#open_food_a', function (e) {
    var RunningTable = $('.run_table').val();
    var BillId = $('#BillId').val();
    if (parseInt(BillId) > 0) {
        var str = 'Bill has printed !';
        GetToster(str, 'warning', 'Warning Notification');
        $('input[name="Discount"]').val('');
        return false;
    }
    if (typeof RunningTable === "undefined" || RunningTable == "") {
        e.preventDefault();
        //$('.modal').modal('hide');
        var str = 'Please Select Table No';
        GetToster(str, 'error', 'Error Notification');
        return false;
    }
    else {
        $('#OpenFood').modal("show");
    }

});

//==================== function for cancel order=================
$(document).on('click', '#canclegorder', function (e) {
    var RunningTable = $('input[name="TableNo"]').val();
    if (!confirm("Do you want to cancel Order From Table No:" + RunningTable)) {
        return false;
    }
    CancelOrder(RunningTable);
    if ($('#' + RunningTable).hasClass("current")) {
        $('#' + RunningTable).removeClass('current');
    }

    e.preventDefault();
});
function CancelOrder(str) {
    var xmlhttp;
    if (str == "") {
        document.getElementById("Div_Billing_items").innerHTML = "";
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

            document.getElementById("Div_Billing_items").innerHTML = xmlhttp.responseText;

        }
    }
    xmlhttp.open("POST", "/Nibs/CancelOrder?TableNo=" + str, true);
    xmlhttp.send();
}

//=====================script for print Data==========================
$(document).on('click', '#_bill_print', function (e) {
    var RunningTable = $('input[name="TableNo"]').val();
    var OrderType = $('input:radio[name="OrderType"]:checked').attr('data-id');
    if (OrderType=="") {
        var str = 'Please Select OrderType';
        GetToster(str, 'error', 'Error Notification');
        return false;
    }
    var $order = $('input:radio[name="OrderType"]:checked').attr('data-id');
    if ($order!=""&&$order=="H") {
        var $address = $('#CustomerAddress').val();
        var $charges = $('#PackingCharges').val();
        var $contactNo = $('#ContactNo').val();
        if ($address=="") {
            var str = 'Please Enter Customer Address';
            GetToster(str, 'error', 'Error Notification');
            return false;
        }
        if ($contactNo=="") {
            var str = 'Please Enter Customer Contact No';
            GetToster(str, 'error', 'Error Notification');
            return false;
        }
        //if ($charges == 0) {
        //    var str = 'Please Enter Delivery Charges';
        //    GetToster(str, 'error', 'Error Notification');
        //    return false;
        //}

    }
    if (typeof RunningTable === "undefined" || RunningTable == "") {
        var str = 'Please Select Table No';
        GetToster(str, 'error', 'Error Notification');
        return false;
    }
    var Discount = $('input[name="Discount"]').val();
    if (parseFloat(Discount)>0) {
        var cNmae = $('input[name="CustomerName"]').val();
        if (cNmae=="") {
            var str = 'Please Enter Customer Name';
            GetToster(str, 'error', 'Error Notification');
            return false;
        }
    }
    var CustomerName = $('input[name="CustomerName"]').val();
    var TableNo = $('input[name="TableNo"]').val();
   
    var VatAmount = $('input[name="VatAmount"]').val();
    var ServiceCharge = $('input[name="ServicesCharge"]').val();
    var ServiceTax = $('input[name="ServiceTax"]').val();
    var DiscountAmount = $('input[name="DiscountAmount"]').val();
    var TotalAmount = $('input[name="TotalAmount"]').val();
    var NetAmount = $('input[name="NetAmount"]').val();
    var NetAmountWithoutDiscount = $('#Label_netAmount_text').text();
    var ContactNo = $('#ContactNo').val();
    var BillId = $('#BillId').val();
    var CustomerAddress = $('#CustomerAddress').val();
    var PackingCharges = $('#PackingCharges').val();
    centeredPopup("/Nibs/PrintOrder?TableNo=" + RunningTable + "&Discount=" + Discount + "&VatAmount=" + VatAmount + "&ServicesCharge=" + ServiceCharge + "&DiscountAmount=" + DiscountAmount + "&TotalAmount=" + TotalAmount + "&NetAmount=" + NetAmount + "&CustomerName=" + CustomerName + "&ServiceTax=" + ServiceTax + "&BillId=" + BillId + "&OrderType=" + OrderType + "&NetAmountWithoutDiscount=" + NetAmountWithoutDiscount + "&CustomerAddress=" + CustomerAddress + "&PackingCharges=" + PackingCharges + "&ContactNo=" + ContactNo, 'myWindow', '600', '400', 'yes');
    
   // return false

    e.preventDefault();
   
  
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

var popupWindow = null;
function centeredPopup(url, winName, w, h, scroll) {
    LeftPosition = (screen.width) ? (screen.width - w) / 2 : 0;
    TopPosition = (screen.height) ? (screen.height - h) / 2 : 0;
    settings =
    'height=' + h + ',width=' + w + ',top=' + TopPosition + ',left=' + LeftPosition + ',scrollbars=' + scroll + ',resizable'
    popupWindow = window.open(url, winName, settings);
    popupWindow.onbeforeunload = function () {
        var RunningTable = $('input[name="TableNo"]').val();
        ShowBillingItems(RunningTable);
        return null;
    }
   
}


//$('#Discount').keydown(function (event) {
//    if (event.keyCode == 13) {
//        event.preventDefault();
//    }
//});
$('input[type="text"]').live('keypress', function (e) {
    if (e.which == 13) return false;
    if (e.which == 13) e.preventDefault();
});
$(function () {
    $('[data-toggle="tooltip"]').tooltip()
    $('#_btn_merged').tooltip();
    $('#btnshiftclickpopup').tooltip();
})

//$(document).on('keydown', '#CustomerName', function (e) {
//    //if (e.which < 97 /* a */ || e.which > 122 /* z */) {
//    //    var str = 'Please enter characters only';
//    //    GetToster(str, 'error', 'Error Notification');
//    //    e.preventDefault();
//    //}
//    if (e.shiftKey) {
//        //e.preventDefault();
//        //var str = 'Please enter characters only';
//        //GetToster(str, 'error', 'Error Notification');
//    } else {
//        var key = e.keyCode;
//        if (!((key == 8) || (key == 32) || (key == 46) || (key >= 35 && key <= 40) || (key >= 65 && key <= 90))) {
//            e.preventDefault();
//            var str = 'Please enter characters only';
//            GetToster(str, 'error', 'Error Notification');
//        }
//    }

//});
$(document).on('change', '#PackingCharges', function () {
    var net = $('#Label_netAmount').val();
    var value = "";
    var $this = $(this).val();
    if ($this=="") {
        $this = 0;
        $('#PackingCharges').val('0');
    }
    else {
        value = $this.replace(/^\s\s*/, '').replace(/\s\s*$/, '');
    }
  
    var intRegex = /^\d+$/;
    if (!intRegex.test(value)) {
        $('#PackingCharges').val('0');
        var str = 'please enter correct charges';
        GetToster(str, 'error', 'Error Notification');
        $('#Label_netAmount_text').text(net);
        $('#NetAmount').val(net);
        return false;
    }

    if (!$.isNumeric($this) || $this < parseInt(0)) {
        $('#PackingCharges').val('0');
        var str = 'please enter correct charges';
        GetToster(str, 'error', 'Error Notification');
        $('#Label_netAmount_text').text(net);
        $('#NetAmount').val(net);
        return false;
    }
    else {
        var discount = $('#Discount').val();
        if (discount == "") {
            discount = "0";
           
        }
     var netAmount=   CalculateDiscount(discount, true);

     var addedNet = parseFloat(netAmount) + parseFloat($this);
     $('#Label_netAmount_text').text(parseFloat(net) + parseFloat($this));
        $('#NetAmount').val(addedNet.toFixed(3));
    }
});