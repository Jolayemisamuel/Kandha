$('input[name="CategoryName"]').on('click', function () {
    var Id = $(this).attr('id');
    var FormData = {Id:Id}
    $.post('/HomeDelivery/GetAllItem', FormData, function (data) {
        $('#Items').html(data);
    });
   
});
$(document).on('click', 'input[name="itemName"]', function () {
    var el = $(this).attr('id');
    var Qty = $('#Qty').val();
    var radio = $('input[name="gradio"]:checked').val();
    var TokenNo = $('#RunningTable').val();
    var formData = { Id: el, Qty: Qty, Type: radio, TokenNo: TokenNo };
    //var Id = el + "_" + Qty + "_" + radio + "_" + TokenNo;
    $.post('/HomeDelivery/GetXmlData', formData, function(data) {
        BindData(data);
    });
    
});

$(document).on('click', '.deleterow', function (e) {
    if (!confirm("Do you want to delete")) {
        return false;
    }
    else {
        var name = $(this).attr('id');
        var TokenNo = $('#RunningTable').val();
        //var Id = name + "_" + TokenNo;
        var FormData = {Id:name,TokenNo:TokenNo}
        $.post('/HomeDelivery/DeleteNode' ,FormData, function (data) {
            BindData(data);
        });
    }
    e.preventDefault();
    
    
});
$(document).on('click', '#btnPrint', function () {
    var divContents = $("#KotDiv").html();
    var address = $("#outletaddress").val();
    var currentdate = $('#GetCurrentDateTime').val();
    var printWindow = window.open('', '', 'height=400,width=800');
    printWindow.document.write('<html><head><title>NIbs Print</title>');
    printWindow.document.write('</head><body >');
    printWindow.document.write('<b style="margin-left:20px;font-size:20px;">Nibs Cafe</b></br>');
    printWindow.document.write('<b>Address -' + address + '</b>');
    printWindow.document.write(divContents);
    printWindow.document.write('<b style="margin-left:18px;">' + currentdate + '</b>');
    printWindow.document.write('</body></html>');
    printWindow.document.close();
    printWindow.print();
    var TokenNo = $('#RunningTable').val();
    var FormData = {TokenNo:TokenNo};
    $.post('/HomeDelivery/ClearKot', FormData, function (data) {
        BindData(data);
        $('.modal').modal('hide'); 
    });
    
});

function BindData(data)
{
    var result = data.split("^");
    var html = result[0];
    var TotalVatAmount = result[2];
    var TOtalAmount = result[1];
    var servictax = $("#servictax").val();
    var sertax = (parseFloat(TOtalAmount) * parseFloat(servictax) / 100);
    $("#txtservice").val(sertax.toFixed(2));
    if (result[3] == "No Pending Kot Found") {
        $('#btnPrint').attr("disabled", true);
    }
    else {
        $('#btnPrint').attr("disabled", false);
    }
    $('#KotDiv').html(result[3]);
    $('#BindXmlData').html(html);
    $('#txtvatamt').val(Math.round(TotalVatAmount, 2).toFixed(2));
    $('#txtsubtotal').val(Math.round(TOtalAmount, 2).toFixed(2));
   // var Homeharge = $("#txtservice").val();
    var NetAmount = parseFloat(TotalVatAmount) + parseFloat(TOtalAmount) +parseFloat(sertax);
    $('#txtnettotal').val(NetAmount.toFixed(2));
    $('#txtdiscount').val(0);
    $('#txtdiscountprice').val(0);
}

$('#txtdiscount').on('change', function () {
    var discount = $(this).val();
    if (discount == "") {
        discount = "0";
    }
    Discount(discount);
    DiscountVat(discount);
    DiscountServiceTax(discount);
});
function Discount(data) {
    var TotalAmount = GetTotalAmount();
    var NetAmout = $('#txtnettotal').val();
    var ServiceTax = $('#txtservice').val();
    var VatAmount = $('#txtvatamt').val();
    var discountamount = (parseFloat(data / 100)) * TotalAmount;
    var discountVat = DiscountVat(data);
    var discountService = DiscountServiceTax(data);
    $('#txtdiscountprice').val((parseFloat(discountamount) + parseFloat(discountVat) + parseFloat(discountService)).toFixed(2));
    var Totalamt = parseFloat(TotalAmount) - parseFloat(discountamount);
    $('#txtsubtotal').val(Totalamt);
    var TotalNetAmount = parseFloat(Totalamt) + parseFloat(ServiceTax) + parseFloat(VatAmount) - parseFloat(discountVat) - parseFloat(discountService);
    $('#txtnettotal').val(TotalNetAmount.toFixed(2));

}
function DiscountVat(data) {
    var TotalVatAmount = "";
    $('#BindXmlData table tbody tr').each(function () {
        var eval = $(this).find('td:eq(6)').text();
        var vt = $(this).find('td:eq(7)').text();
        var vtamt = ((parseFloat(vt) / 100) * parseFloat(eval));
        if (TotalVatAmount != "") {
            TotalVatAmount = parseFloat(TotalVatAmount) + parseFloat(vtamt);
        }
        else {
            TotalVatAmount = parseFloat(vtamt);
        }
    });
    var vatamount = TotalVatAmount;
    var discountAmount = (parseFloat(data / 100)) * vatamount;
    var RemainingVatAmount = (parseFloat(vatamount) - parseFloat(discountAmount)).toFixed(2);
    $('#txtvatamt').val(RemainingVatAmount)
    return discountAmount;
}
function GetTotalAmount() {
    var TOtalAmount = "";
    $('#BindXmlData table tbody tr').each(function () {
        var eval = $(this).find('td:eq(6)').text();
        if (TOtalAmount != "") {
            TOtalAmount = parseFloat(TOtalAmount) + parseFloat(eval);
        }
        else {
            TOtalAmount = parseFloat(eval);
        }
    });
    return TOtalAmount;
}
function DiscountServiceTax(data) {


    var TOtalAmount = GetTotalAmount();
    var servictax = $("#servictax").val();
    var sertax = (parseFloat(TOtalAmount) * parseFloat(servictax) / 100);
    var amount = sertax;
    var discountAmount = (parseFloat(data / 100)) * amount;
    var RemainingVatAmount = (parseFloat(amount) - parseFloat(discountAmount)).toFixed(2);
    $('#txtservice').val(RemainingVatAmount)
    return discountAmount;
}
$("#txtdiscount").keypress(function (e) {
    //if the letter is not digit then display error and don't type anything
    if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
        //display error message
        $("#discounterrmsg").html("Digits Only").show().fadeOut("slow");
        return false;
    }
});
$('#CustomerName').on('change', function () {
    var name = $(this).val();
    $('#Customer').val(name);
});
$('#Address').on('change', function () {
    var name = $(this).val();
    $('#CustomerAddress').val(name);
    $('#TokenNo').val($('#RunningTable').val())
});

$('#btndispatch').click(function (e) {
    var tablelength = $('#BindXmlData table tbody tr').length;
    if (tablelength == "0") {
        alert("palese select any item before dispatch");
        return false;
    }
    var Customer = $('#CustomerAddress').val();
    var kotData = $('#KotDiv').html();
    if (kotData != "No Pending Kot Found")
    {
        alert("Please Clear Kot First");
        return false;
    }
    if (Customer == "") {
        $('#NameError').html("Please Enter Address").fadeIn();
        return false
    }
    else {
        $('#NameError').fadeOut();
    }
    if ($('#txtnettotal').val() == "") {
        alert("please select items for dispatch order")
        return false
    }
    var form = $('#form-dispatch');

    var url = form.attr("action");
    var formData = form.serialize();
    $.post(url, formData, function (data) {
        var printWindow = window.open('', '', 'height=400,width=800');
        printWindow.document.write('<html><head><title>Kot Print</title>');
        printWindow.document.write('</head><body >');
        printWindow.document.write(data);
        printWindow.document.write('</body></html>');
        printWindow.document.close();
        printWindow.print();
        $("#BindXmlData").html("");
        $('#txtvatamt').val("");
        $('#txtsubtotal').val("");
        $('#txtnettotal').val("");
        $('#txtdiscount').val("");
        $('#txtdiscountprice').val("")
        $('#Address').val("");
        var TokenNo = $("#RunningTable").val()
        $("#RunningTable").val(parseInt(TokenNo) + parseInt(1));
        $('.modal').modal('hide');
        $('#Net_amount').text("");
        $('#Cash_amt').val("");
        $('#Cash_Exchange').val("");
    });
    e.preventDefault();

});
$(document).on('click', '#canclegorder', function () {
    var TokenNo = $('#RunningTable').val();
    var formData = { TokenNo: TokenNo };
    $.ajax({
        url: "/HomeDelivery/CencelOrder",
        type: "POST",
        data: formData,
        success: function (data, textStatus, jqXHR) {
            $('#BindXmlData').html("");
            $('#txtvatamt').val("");
            $('#txtsubtotal').val("");
            $('#txtnettotal').val("");
            $('#txtdiscount').val("");
            $('#txtdiscountprice').val("");
        },
        error: function (jqXHR, textStatus, errorThrown) {

        }
    });
});

function ajaxindicatorstart(text) {
    if (jQuery('body').find('#resultLoading').attr('id') != 'resultLoading') {
        jQuery('body').append('<div id="resultLoading" style="display:none"><div><img src="/assets/admin/pages/img/ajax-loader.gif"><div>' + text + '</div></div><div class="bg"></div></div>');
    }

    jQuery('#resultLoading').css({
        'width': '100%',
        'height': '100%',
        'position': 'fixed',
        'z-index': '10000000',
        'top': '0',
        'left': '0',
        'right': '0',
        'bottom': '0',
        'margin': 'auto'
    });

    jQuery('#resultLoading .bg').css({
        'background': '#000000',
        'opacity': '0.7',
        'width': '100%',
        'height': '100%',
        'position': 'absolute',
        'top': '0'
    });

    jQuery('#resultLoading>div:first').css({
        'width': '250px',
        'height': '75px',
        'text-align': 'center',
        'position': 'fixed',
        'top': '0',
        'left': '0',
        'right': '0',
        'bottom': '0',
        'margin': 'auto',
        'font-size': '16px',
        'z-index': '10',
        'color': '#ffffff'

    });

    jQuery('#resultLoading .bg').height('100%');
    jQuery('#resultLoading').fadeIn(300);
    jQuery('body').css('cursor', 'wait');
}

function ajaxindicatorstop() {
    jQuery('#resultLoading .bg').height('100%');
    jQuery('#resultLoading').fadeOut(300);
    jQuery('body').css('cursor', 'default');
}
jQuery(document).ajaxStart(function () {
    //show ajax indicator
    ajaxindicatorstart('loading data.. please wait..');
}).ajaxStop(function () {
    //hide ajax indicator
    ajaxindicatorstop();
});