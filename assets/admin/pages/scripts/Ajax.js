$(document).ready(function () {

    $('input[name="btn-Category"]').on('click', function (e) {
        var name = $(this).attr('id');
        //var formData = id;
        $.post("/Billing/GetAllItems/" + name, function (data) {
            $('#Items').html(data)
            $('input[name="itemName"]').on('click', function () {
                var name = $(this).attr('id');
                var tbaleid = $("#RunningTable").val()
                var radio = $('input[name="gradio"]:checked').val();
                var Qty = $('#Qty').val();
                var ids = name + "," + tbaleid + "," + radio + "," + Qty;
                $.post("/Billing/UpdateXML/" + ids, function (data) {
                    var result = data.split("^");
                    var html = result[0];
                    var TotalVatAmount = result[1];
                    var TOtalAmount = result[2];
                    $('#BindXmlData').html(html);
                    $('#txtvatamt').val(Math.round(TotalVatAmount, 5));
                    $('#txtsubtotal').val(Math.round(TOtalAmount, 2));
                    var NetAmount = parseFloat(TotalVatAmount) + parseFloat(TOtalAmount);
                    $('#txtnettotal').val(NetAmount);
                    // $("#BindXmlData").html(data);
                });
            });
        });

        e.preventDefault();
    });
    //=====
    $(document).on('click', '.deleterow', function () {
        var DeleteId = $(this).attr('id');
        var tableid = $("#RunningTable").val()
        var Id = DeleteId + "," + tableid;
        $.post("/Billing/DeleteNode/" + Id, function (data) {
            var result = data.split("^");
            var html = result[0];
            var TotalVatAmount = result[1];
            var TOtalAmount = result[2];
            $('#BindXmlData').html(html);
            $('#txtvatamt').val(Math.round(TotalVatAmount, 5));
            $('#txtsubtotal').val(Math.round(TOtalAmount, 2));
            var NetAmount = parseFloat(TotalVatAmount) + parseFloat(TOtalAmount);
            $('#txtnettotal').val(NetAmount);
            
        });
        eevent.preventDefault();
    });
    //$('.deleterow').on('click', function (eevent) {
    //    ;
    //});
    //---- for create xml

    $('input[name="table"]').on('click', function (e) {
        var name = $(this).attr('id');
        $("#RunningTable").val(name);
        var tableno = $("#RunningTable").val();
        $('#TableNoDispatch').val(name);
        $.post("/Billing/CreateXml/" + tableno, function (data) {
            $('input[name="table"]#' + name).addClass("current");
            var result = data.split("^");
            var html = result[0];
            var TotalVatAmount = result[1];
            var TOtalAmount = result[2];
            $('#BindXmlData').html(html);
            $('#txtvatamt').val(Math.round(TotalVatAmount, 5));
            $('#txtsubtotal').val(Math.round(TOtalAmount,2));
            var NetAmount = parseFloat(TotalVatAmount) + parseFloat(TOtalAmount);
            $('#txtnettotal').val(NetAmount);
        });
        e.preventDefault();
    });
    //--- for update xml
    //$(document).on('click', 'input[name="itemName"]', function () {
    //    alert('dfs');
    //});
    $('#txtdiscount').on('change', function () {
        var discount = $(this).val();
        if (discount == "")
        {
            $('#txtdiscountprice').val("");
        }
        else
        {
            var Totalamount = $('#txtsubtotal').val();
            var discountamount = ((parseFloat(discount) / 100) * Totalamount);
            var amount = parseFloat(Totalamount) - parseFloat(discountamount);
            $('#txtdiscountprice').val(Math.round(discountamount,2));
            $('#txtsubtotal').val(Math.round(amount, 2));
            var Vatamount = $('#txtvatamt').val();
            var NetAmount = parseFloat(Vatamount) + parseFloat(amount);
            $('#txtnettotal').val(NetAmount);

        }
        

    });
    $('#CustomerName').on('change', function () {
        var name = $(this).val();
        $('#Customer').val(name);
    });
});

$('#btndispatch').click(function (e) {

    var form = $('#form-dispatch');

    var url = form.attr("action");
    var formData = form.serialize();
    $.post(url, formData, function (data) {
        alert(data);

    });
    e.preventDefault();
})
//$(document).ajaxStart(function () {
//    $('@(item.UserId + "btn-grid")').attr("disabled", true);
//    $('#loading-1-@item.UserId').show();
//}).ajaxStop(function () {
//    $('@(item.UserId + "btn-grid")').attr("disabled", false);
//    $('#loading-1-@item.UserId').hide();
//});