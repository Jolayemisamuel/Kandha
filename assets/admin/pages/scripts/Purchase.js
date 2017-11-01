$(document).ready(function () {
   
    $('#table_ItemSave').hide();
    $('#btnAddMulti').click(function () {
        var itemId =parseInt($('#ItemId option:selected').val());
        var itemName = $('#ItemId option:selected').text();
        var Quantity = parseInt($('#Quantity1').val());
        var Amount = parseInt($('#txtitemamount').val());
        var Unit = $('#type1 option:selected').val();
        var items = "<tr><td>" + itemId + "</td><td>" + itemName + "</td><td>" + Quantity + "</td><td>" + Amount + "</td><td>" + Unit + "</td></tr>";
        
        var itemamout = parseInt($('#txttotalamount').val()) + Amount;
     
        $('#txttotalamount').val(itemamout);
       
        $('#table_ItemSave').append(items);
        $('#table_ItemSave').show();
       
      
    });
    $('#ditems').hide();
    
    $('#btnAdddetails').click(function () {
        var detailsname = $('#detailstext').val();
        var detailsamount = parseInt($('#detailamount').val());
        var cross = '&#215;';
        var ditems = "<tr><td>" + detailsname + "</td><td>" + detailsamount + "</td><td>"+cross +"</td></tr>";
        $('#ditems').append(ditems);
        $('#ditems').show();
        var amounts = parseInt($('#txtextra_amount').val());
        if (amounts != NaN && amounts != 0) {
            amounts += parseInt(detailsamount);
            $('#txtnetamount').val(parseInt($('#txttotalamount').val()) + amounts);
            //var totalamount = parseInt($('#txttotalamount').val());
            //var exteraamount;
            //exteraamount += parseInt(amounts);

        }
        else {
            amounts = detailsamount;
            $('#txtextra_amount').val(amounts);
            $('#txtnetamount').val(parseInt($('#txttotalamount').val()) + amounts);
        }
    });

    $('#rbtncahsarea').hide();
    $('#rbtncrditarea').hide();
    $('#rbtnchequearea').hide();
   

    $("#rbtcash").click(function () {
        if ($("#rbtcash").attr('checked')) {
            $("#rbtncahsarea").show();
            $("#txtcashdeposite").change(function () {
                var cashdeposite = parseInt($('#txtcashdeposite').val());
                var cashnetamount = parseInt($('#txtnetamount').val());
                //cashnetamount -= parseInt(cashdeposite);
                $('#txtcashremaing').val(cashnetamount - cashdeposite);
            });
            $('#rbtncrditarea').hide();
            $('#rbtnchequearea').hide();
        }
        else { $('#rbtncahsarea').hide(); }
    });
    $("#rbtcredit").click(function () {
        if ($("#rbtcredit").attr('checked')) {
            $("#rbtncrditarea").show();
            var creditamount = parseInt($('#txtnetamount').val());
            $('#txtcreditremaing').val(parseInt($('#txtnetamount').val()));
            $('#rbtncahsarea').hide();
            $('#rbtnchequearea').hide();
          
        }
        else { $('#rbtncrditarea').hide(); }
    });
    $("#rbtcheque").click(function () {
        if ($("#rbtcheque").attr('checked')) {
            $("#rbtnchequearea").show();
            $('#rbtncahsarea').hide();
            $('#rbtncrditarea').hide();
        }
        else { $('#rbtnchequearea').hide(); }
    });

    
});
