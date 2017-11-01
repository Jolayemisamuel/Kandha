var mergearr = [];
var mergetable = [];
var tableforshift = [];
$(document).ready(function () {
    $('input[name="btn-Category"]').on('click', function (e) {
        var name = $(this).attr('id');
        var FormData = {Id:name};
        $.post("/Billing/GetAllItems" , FormData, function (data) {
            $('#Items').html(data)
            $('input[name="itemName"]').on('click', function (event) {
                var name = $(this).attr('id');
                var tbaleid = $("#RunningTable").val()
                if (tbaleid == "" || tbaleid == null)
                {
                    $('#ErrorTableNo').fadeIn('slow').css('opacity','1').fadeOut('slow');
                    return false;
                }
                var radio = $('input[name="gradio"]:checked').val();
                var Qty = $('#Qty').val();
                var Id = name + "_" + tbaleid + "_" + radio + "_" + Qty;
                var FormData = {Id:name,TableNo:tbaleid,Type:radio,Qty:Qty};
                var url = "/Billing/UpdateXML";
                $.post('/Billing/UpdateXML', FormData, function (data) {
                    $('#' + tbaleid).addClass("current");
                    FillData(data);
                });
                event.preventDefault();
            });
        });

        e.preventDefault();
    });
    //=====
    $(document).on('click', '.deleterow', function (even) {
        if (!confirm("Do you want to delete")) {
            return false;
        }
        else
        {
            var DeleteId = $(this).attr('id');
            var tableid = $("#RunningTable").val()
            var Id = DeleteId + "," + tableid;
            var FormData = { Id: DeleteId, TableNo: tableid };
            $.post("/Billing/DeleteNode", FormData, function (data) {
                FillData(data);
                var distxt = $('#txtdiscount').val();
                Discount(distxt);
            });
        }
        even.preventDefault();
    });
    //$('.deleterow').on('click', function (eevent) {
    //    ;
    //});
    //---- for create xml

    $('input[name="table"]').on('click', function () {
        var Id = $(this).attr('id');
        $("#RunningTable").val(Id);
        var name = $("#RunningTable").val();
        $('#TableNoDispatch').val(Id);
        var FormData = {Id:Id}
        $.post("/Billing/CreateXml", FormData, function (data) {
           
          //  $('input[name="table"]#' + name).addClass("current");
            FillData(data);
            $('input[name="MergeTable"]').each(function () {
                var id = $(this).val();
                if(id==name)
                {
                    $(this).addClass("table_dispaly_none");
                }
            });
            $('input[name="shiftTable"]').each(function () {
                var el = $(this).val();
                if(el==name)
                {
                    $(this).addClass("table_dispaly_none");
                }
            });
        });
       
    });
    $('#txtdiscount').on('change', function () {
        var discount = $(this).val();
        if(discount=="")
        {
            discount="0";
        }
        Discount(discount);
        DiscountVat(discount);
        DiscountServiceTax(discount);
    });
    $('#CustomerName').on('change', function () {
        var name = $(this).val();
        $('#Customer').val(name);
    });

    $("#txtdiscount").keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            $("#discounterrmsg").html("Digits Only").show().fadeOut("slow");
            return false;
        }
    });
});
$(document).on('click', '#btn_Only_Print', function () {
    var value = $(".table tbody").is(":empty");
    var tabledata = $("#RunningTable").val();
    $('#TableNoDispatch').val(tabledata);
    if (value==true || tabledata=="") {
        alert('First Select Table && Must have Items for Print');
    }
    else{
    var form = $('#form-dispatch');
    var runningtable = $("#RunningTable").val();
    var formData = form.serialize();
    $("#btndispatch").removeAttr('disabled');
    $.post("/Billing/PrintOrder", formData, function (data) {
        var printWindow = window.open('', '', 'height=400,width=800');
        printWindow.document.write('<html><head><title>Nibs Print</title>');
        printWindow.document.write('</head><body >');
        printWindow.document.write(data);
        printWindow.document.write('</body></html>');
        printWindow.document.close();
        printWindow.print();
    });
    }
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
    
    var TotalNetAmount = parseFloat(Totalamt) + parseFloat(ServiceTax) + parseFloat(VatAmount)-parseFloat(discountVat)-parseFloat(discountService);
    $('#txtnettotal').val(TotalNetAmount.toFixed(2))

}
function DiscountVat(data)
{
    var TotalVatAmount = "";
    $('#BindXmlData table tbody tr').each(function () {
        var eval = $(this).find('td:eq(4)').text();
        var vt = $(this).find('td:eq(5)').text();
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
function GetTotalAmount()
{
    var TOtalAmount = "";
    $('#BindXmlData table tbody tr').each(function () {
        var eval = $(this).find('td:eq(4)').text();
        if (TOtalAmount != "") {
            TOtalAmount = parseFloat(TOtalAmount) + parseFloat(eval);
        }
        else {
            TOtalAmount = parseFloat(eval);
        }
    });
    return TOtalAmount;
}
function DiscountServiceTax(data)
{
    
    
    var TOtalAmount = GetTotalAmount();
    var servictax = $("#servictax").val();
    var sertax = (parseFloat(TOtalAmount) * parseFloat(servictax) / 100);
    var amount = sertax;
    var discountAmount = (parseFloat(data / 100)) * amount;
    var RemainingVatAmount = (parseFloat(amount) - parseFloat(discountAmount)).toFixed(2);
    $('#txtservice').val(RemainingVatAmount)
    return discountAmount;
}
function FillData(data) {
    var result = data.split("^");
    var html = result[0];
    var TotalVatAmount = result[1];
    var TOtalAmount = result[2];
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
    var TVat = Math.round(TotalVatAmount, 2).toFixed(2);
    $('#txtvatamt').val(TVat);
    var Tamt = Math.round(TOtalAmount, 2).toFixed(2);
    $('#txtsubtotal').val(Tamt);
    var discount = $('#txtdiscount').val();
    var discountamount = (parseFloat(discount / 100)) * Tamt;
    $('#txtdiscountprice').val(discountamount);
    // var restrocharge = $("#txtservice").val();
    var subtotal = parseFloat(Tamt) - parseFloat(discountamount);
    $('#txtsubtotal').val(subtotal.toFixed(2));
    var NetAmount = parseFloat(TVat) + parseFloat(Tamt) + parseFloat(sertax)-parseFloat(discountamount);
    $('#txtnettotal').val(NetAmount.toFixed(2));
    $('#txtdiscount').val(0);
    $('#txtdiscountprice').val(0);
}
$('#btndispatch').click(function (e) {
    
    $('.modal').modal('hide');
    //var kotData = $('#KotDiv').html();
    //if (kotData != "No Pending Kot Found") {
    //    alert("Please Clear Kot First");
    //    return false;
    //}
    var ptype=$('#Payment_Type').val();
    if (ptype == "")
    {
        alert("Select Payment Type !");
        return false;
    }
    var Customer = $('#Customer').val();
    $('#NameError').fadeOut();
    
    var runningtable = $("#RunningTable").val();
        var form = $('#form-dispatch');

        var url = form.attr("action");
        var formData = form.serialize();
        if ($('#PrintOrNot').parent('span').hasClass("checked")) {
            
            $.post("/Billing/PrintOrder", formData, function (data) {
                var printWindow = window.open('', '', 'height=400,width=800');
                printWindow.document.write('<html><head><title>Nibs Print</title>');
                printWindow.document.write('</head><body >');
                printWindow.document.write(data);
                printWindow.document.write('</body></html>');
                printWindow.document.close();
                printWindow.print();
            });
        }
        $.post(url, formData, function (data) {
           
            var booked = $("#RunningTable").val();
            var idsa = $('#' + booked);
            $('input[name="table"]').each(function () {
                var el = $(this).attr('id');
                if(el==booked)
                {
                    $(this).removeClass("current");
                }
            });
           
            var mergeremove = $('#' + booked).val()
            $('input[name="merge'+booked+'"]').remove();
            var arraymerge = mergeremove.split(',');
            for (var i = 0; i <= arraymerge.length; i++) {
                if ($.inArray(arraymerge[i], arraymerge) < 0) {
                   
                }
                else
                {
                    $('input[name="table"]#' + arraymerge[i]).removeClass("disablemerge");
                    $('input[name="table"]#' + arraymerge[i]).attr('disabled', false);

                }
                $('input[name="MergeTable"]').each(function () {
                    var id = $(this).val();
                    if (id == arraymerge[i]) {
                        $(this).removeClass("table_dispaly_none");
                        $(this).removeClass("merge_table");
                    }
                });
            }
            var disdata = data.split("^^");
            var tbls = disdata[1];
            var finalitems = tbls.split(",");

            for (var i = 0; i < finalitems.length; i++) {
                $('input[name="itemName"]').each(function () {
                    var id = $(this).attr("id");
                    if (id == finalitems[i]) {
                        $(this).attr("disabled", true);
                    }
                });
                //$('#' + i).attr('disabled', true);
            }
           
            $.gritter.add({
                title: 'Order Dispached From TableNo' + runningtable,
                text: data
            });
            $("#BindXmlData").html("");
            $('#txtvatamt').val("");
            $('#txtsubtotal').val("");
            $('#txtnettotal').val("");
            $('#txtdiscount').val("");
            $('#txtdiscountprice').val("");
            $("#RunningTable").val("");
            //$("#txtservice").val("4.9440");
            $("#txtservice").val("5.6");
           
            $('#Cash_amt').val("");
            $('#Cash_Exchange').val("");
            $('#PrintOrNot').parent('span').removeClass("checked");
        });
        e.preventDefault();
})
$('#btn-merge').click(function (e) {
    var form = $('#form-merged');
    var url = form.attr("action");
    var formData = form.serialize();
    $.post(url, formData, function (data) {
        var merged = $('#MergedT').val();
        var master = $('#MasterT').val();
        $('body').prepend('<input type="hidden" id="' + master + '" value="' + merged + '" name="merge'+master+'" />');
        var array = merged.split(',');
        for(var i = 0; i < array.length; i++) {
            if(array[i]==master)
            {
                $('input[name="table"]').each(function () {
                    var id = $(this).val();
                    if (id == array[i]) {
                        $(this).addClass('current');
                    }
                });
                $('input[name="MergeTable"]').each(function () {
                    var el = $(this).val();
                    if(el==array[i])
                    {
                        $(this).removeClass("table_dispaly_none");
                    }
                });
                var button = '<input type="button" value="' + array[i] + '"  name="MergeTable" class="merged margin-bottom-5 margin-right-5" />';
                mergetable.pop(array[i]);
                mergearr.pop(button);
            }
            else
            {
                $('input[name="table"]').each(function () {
                    var id = $(this).val();
                    if(id==array[i])
                    {
                        $(this).addClass('disablemerge');
                        $(this).attr('disabled', true);
                    }
                });
                $('input[name="MergeTable"]').each(function () {
                    var el = $(this).val();
                    if (el == array[i]) {
                        $(this).removeClass("table_dispaly_none");
                    }
                });
                var button = '<input type="button" value="' + array[i] + '" name="MergeTable" class="merged margin-bottom-5 margin-right-5" />';
                mergetable.pop(array[i]);
                mergearr.pop(button);
            }
        }
        $('.modal').modal('hide');
        $('#MergedTable').html("");
        $('#MasterTable').html("");
        $('#MergedT').val("");
        $('#MasterT').val("");       
    });
    e.preventDefault();
});
$("#btnPrint").on("click", function () {
    var address = $("#outletaddress").val();
    var currentdate = $('#GetCurrentDateTime').val();
    var dNow = new Date();
    var utc = new Date(dNow.getTime() + dNow.getTimezoneOffset() * 60000)
    var utcdate = utc.getDate() + '/' + (utc.getMonth() + 1) + '/' + utc.getFullYear() + ' ' + utc.getHours() + ':' + utc.getMinutes();
    var divContents = $("#KotDiv").html();
     
    var printWindow = window.open('', '', 'height=400,width=800');
    printWindow.document.write('<html><head><title>NIbs Print</title>');
    printWindow.document.write('</head><body >');
    //printWindow.document.write('<div class="logo" style="border-bottom:1px dashed"><h2 style="margin-left:125px; padding-top:20px;">Nibs Cafe</h2><b style="margin-left:85px; font-weight:100">Near Friends Colony,Lalkothi</b><br /><b style="margin-left:85px; font-weight:100">Jaipur-302029</b><br /><b style="margin-left:85px; font-weight:100">PH:9680625173</b><br />');
    printWindow.document.write('<b style="margin-left:20px;font-size:20px;">Nibs Cafe</b></br>');
    printWindow.document.write('<b>Address -' + address + '</b>');
    printWindow.document.write(divContents);

    printWindow.document.write('<b style="margin-left:18px;">' + currentdate + '</b>');
    printWindow.document.write('</body></html>');
    printWindow.document.close();
    printWindow.print();
    var tbaleid = $("#RunningTable").val()
    var FormData = {Id:tbaleid}
    $("#KotDiv").html("");
    $('.modal').modal('hide')
    $.post("/Billing/ClearKot",FormData, function (data) {
        var result = data.split("^");
        var html = result[0];
        var TotalVatAmount = result[1];
        var TOtalAmount = result[2];
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
        $('#txtvatamt').val(Math.round(TotalVatAmount, 5));
        $('#txtsubtotal').val(Math.round(TOtalAmount, 2));
       // var NetAmount = parseFloat(TotalVatAmount) + parseFloat(TOtalAmount);
        var restrocharge = $("#txtservice").val();
        var NetAmount = parseFloat(TotalVatAmount) + parseFloat(TOtalAmount) +parseFloat(sertax);
        $('#txtnettotal').val(NetAmount.toFixed(2));
        $('#txtdiscount').val(0);
        $('#txtdiscountprice').val(0);
    });
});
$(document).ready(function () {
    var tableformerge = [];
   // var tableforshift = [];
    $('input[name="table"]').each(function () {
        if ($(this).hasClass('current'))
        {
            //var id = $(this).val();
            //var button = '<input type="button" value="' + id + '" name="ShiftTable" class="shift margin-bottom-5 margin-right-5" />';
            //tableforshift.push(button);
        }
        else
        {
            var id = $(this).val();
            var button = '<input type="button" value="' + id + '" name="MergeTable" class="merged margin-bottom-5 margin-right-5" />';
           // var buttonForShift = '<input type="button" value="' + id + '" name="shiftTable" class="merged margin-bottom-5 margin-right-5" />';
            tableformerge.push(button);
           // tableforshift.push(buttonForShift)

        }
    });
    $('#TableForMerged').html(tableformerge);
   // $('#shiftedTable').html(tableforshift);
    $('input[name="MergeTable"]').on('click', function () {
        var tblvalue = $(this).val();
        if ($.inArray(tblvalue, mergetable) < 0) {
            $(this).addClass('merge_table');
            var button = '<input type="button" value="' + $(this).val() + '"  name="MergedTable" class="merged margin-bottom-5 margin-right-5" />';
            mergearr.push(button);
            mergetable.push(tblvalue);
            $('#MergedTable').html(mergearr);
            $('#MergedT').val(mergetable)
        }
        else {
            var button = '<input type="button" value="' + $(this).val() + '"  name="MergedTable" class="merged margin-bottom-5 margin-right-5" />';
            $(this).removeClass('merge_table');
            mergearr.pop(button);
            mergetable.pop(tblvalue);
            $('#MergedTable').html(mergearr);
            $('#MergedT').val(mergetable)
            //alert(mergearr);
        }
        $('input[name="MergedTable"]').on('click', function () {
            var el = $(this).val();
            var button = '<input type="button" value="' + el + '" name="MasterTable" class="merged margin-bottom-5" />';
            $('#MasterTable').html(button);
            $('#MasterT').val(el);
        });
    });
});
$(document).on('click', '#canclegorder', function () {
    if (!confirm("Do you want to cancel")) {
        return false;
    }
    else
    {
        var Id = $("#RunningTable").val();
        var FormData = { Id: Id }
        $.post("/Billing/CancelOrder", FormData, function () {
            $('#' + Id).removeClass("current");
            $('input[name="MergeTable"]').each(function () {
                var el = $(this).val();
                if (el == Id) {
                    $(this).removeClass("table_dispaly_none");
                }
            });
            $('#BindXmlData').html("");
            $("#RunningTable").val("");
            $('#txtvatamt').val("");
            $('#txtsubtotal').val("");
            $('#txtnettotal').val("");
            $('#txtdiscount').val("");
            $('#txtdiscountprice').val("");
        });
    }
});

$('#btnshiftclickpopup').click(function () {
    var arrayofCurrentclass = [];
    var arrayofUnCurrentclass = [];
    $("input[name=table]").each(function () {
        if ($(this).hasClass('current')) {
            var tableshiftid = $(this).val();
            var shiftbutton = '<input type="button" value="' + tableshiftid + '"  name="TableToShift" class="merged margin-bottom-5 margin-right-5" />';
            arrayofCurrentclass.push(shiftbutton);
            $('#TableForShift').html(arrayofCurrentclass);
        }
        else if ($(this).not('current')) {
            var Freetableshift = $(this).val();
            var Freeshiftbutton = '<input type="button" value="' + Freetableshift + '"  name="FreeTableToShift" class="merged margin-bottom-5 margin-right-5" />';
            arrayofUnCurrentclass.push(Freeshiftbutton);
            $('#shiftedTable').html(arrayofUnCurrentclass);
        }
    });

    $("input[name=TableToShift]").click(function () {
        var wantshifttable = $(this).val();
        var wanttoshiftbutton = '<input type="button" value="' + wantshifttable + '"  name="SelectedTableToShift" class="merged margin-bottom-5 margin-right-5" />';
        $("#shifted").html(wanttoshiftbutton);
        $('#ShiftT').val(wantshifttable);
    });

    $("input[name=FreeTableToShift]").click(function () {
        var mastershifttable = $(this).val();
        var mastertablebutton = '<input type="button" value="' + mastershifttable + '"  name="Mastershifttable" class="merged margin-bottom-5 margin-right-5" />';
        $("#ShiftMasterTable").html(mastertablebutton);
        $('#ShiftMasterT').val(mastershifttable);
    });

});
$('#btn-shift').click(function (e) {

    var form = $('#form-shift');

    var url = form.attr("action");
    var formData = form.serialize();
    $.post(url, formData, function (data) {
        var merged = $('#ShiftT').val();

        var master = $('#ShiftMasterT').val();
        $('input[name="table"]').each(function () {
            var id = $(this).val();
            if(id==merged)
            {
                $(this).removeClass("current");
            }
            if(id==master)
            {
                $(this).addClass("current");

            }
        });
        $('#RunningTable').val(master);
        
        var result = data.split("^");
        var html = result[0];
        var TotalVatAmount = result[1];
        var TOtalAmount = result[2];
        var servictax = $("#servictax").val();
        var sertax = (parseFloat(TOtalAmount) * parseFloat(servictax) / 100);
        $("#txtservice").val(sertax.toFixed(2));
        $('#txtvatamt').val(Math.round(TotalVatAmount, 5));
        $('#txtsubtotal').val(Math.round(TOtalAmount, 2));
        var NetAmount = parseFloat(TotalVatAmount) + parseFloat(TOtalAmount)+parseFloat(sertax);
        $('#txtnettotal').val(NetAmount.toFixed(0));
        var shiftbutton = '<input type="button" value="' + master + '"  name="TableToShift" class="merged margin-bottom-5 margin-right-5" />';
        tableforshift.push(shiftbutton);
        $('#TableForShift').html(tableforshift);
        //$('input[name="shiftTable"]').each(function () {
        //   // if($(this).val==master)
        //});
        var buttonForShift = '<input type="button" value="' + merged + '" name="shiftTable" class="merged margin-bottom-5 margin-right-5" />';
        
        tableforshift.push(buttonForShift);
        $('#shiftedTable').html(tableforshift);
        $('.modal').modal('hide');
        $('#shifted').html("");
        $('#ShiftMasterTable').html("");
        $('#ShiftT').val("");
        $('#ShiftMasterT').val("");
       
    });
    e.preventDefault();
});
$(document).on('change', '#Payment_Type', function () {
    var eval = $(this).val();
    $('#Payment_Method').val(eval);
    if (eval == "Cheque")
    {
        $('#cheque_detail').fadeIn();
        $('#chequeno').on('change', function () {
            var chequeno = $(this).val();
            $('#Cheque_No').val(chequeno);
        });
        $('#chequedate').on('change', function () {
            var chequedate = $(this).val();
            $('#Cheque_Date').val(chequedate);
        });
    }
    else {
        $('#cheque_detail').fadeOut();
        $('#chequeno').val("");
        $('#chequedate').val("");
        $('#Cheque_No').val("");
        $('#Cheque_Date').val("");
    }
    
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
    jQuery('#resultLoading').fadeIn(100);
    jQuery('body').css('cursor', 'wait');
}

function ajaxindicatorstop() {
    jQuery('#resultLoading .bg').height('100%');
    jQuery('#resultLoading').fadeOut(100);
    jQuery('body').css('cursor', 'default');
}
jQuery(document).ajaxStart(function () {
    //show ajax indicator
    ajaxindicatorstart('loading data.. please wait..');
}).ajaxStop(function () {
    //hide ajax indicator
    ajaxindicatorstop();
});

//$(document).ready(function () {
//    $('input[name="table"]').each(function () {
//        var id = $(this).attr('id');
//        var Data = {Id:id};
//        $.get('/Billing/GetCurrent', Data, function (data) {
//            if (data == true) {
//                $('#' + id).addClass("current");
//                //$('input[name = "table"]#'+id).addClass('current');
//            }
//            else {

//            }
//        });
//    });
//});

