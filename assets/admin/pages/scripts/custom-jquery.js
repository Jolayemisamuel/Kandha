/// <reference path="../../../global/plugins/jquery-1.11.0.min.js" />

//---------------------Validation----------------------------//

$(document).ready(function () {
    $(".number").keypress(function (e) {
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            $(".errmsg").html("Digits Only").show().fadeOut(2000);
            return false;
        }
    });
    $(".alphaonly").keypress(function (event) {
        var inputValue = event.which;
        //if digits or not a space then don't let keypress work.
        if ((inputValue > 47 && inputValue < 58) && (inputValue != 32)) {
            event.preventDefault();
        }
    });

    $('.Decinumber').keypress(function (event) {
        if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }

        var text = $(this).val();

        if ((text.indexOf('.') != -1) && (text.substring(text.indexOf('.')).length > 2)) {
            event.preventDefault();
        }
    });
});



//----------------------------------------------------------------//
//$(document).ready(function () {
//    $("input[name=optionsRadios]:radio").on('change', function () {
//        var name = $(this).val();
//        $.getJSON('/Category/changeItem/' + name, function (data) {
//            var CatName = '<option value="">Select Category</option>';
//            var CatID = $("#CategoryEditId").val();
//            var catValue = CatName.Value;

//            $.each(data, function (i, Categoryname) {
//                if (CatID != null && CatID != "") {
//                    if (parseInt(CatID) == parseInt(catValue)) {
//                        CatName += "<option Selected value='" + Categoryname.Value + "'>" + Categoryname.Text + "</option>";
//                    }
//                    else {
//                        CatName += "<option value='" + Categoryname.Value + "'>" + Categoryname.Text + "</option>";
//                    }
//                }
//                else {
//                    CatName += "<option value='" + Categoryname.Value + "'>" + Categoryname.Text + "</option>";
//                }
//            });
//            $("#CategoryId").html(CatName);
//        });

//    });
//    //for edit radio button checked
//    var radioSelected = $("#RadioEdit").val();

//    $.getJSON('/Category/changeItem/' + radioSelected, function (data) {
//        var CatName = '<option>Select Category</option>';
//        var CatID = $("#CategoryEditId").val();


//        $.each(data, function (i, Categoryname) {
//            var catValue = Categoryname.Value;
//            if (CatID != null && CatID != "") {
//                if (parseInt(CatID) == parseInt(catValue)) {
//                    CatName += "<option Selected value='" + Categoryname.Value + "'>" + Categoryname.Text + "</option>";
//                }
//                else {
//                    CatName += "<option value='" + Categoryname.Value + "'>" + Categoryname.Text + "</option>";
//                }
//            }
//            else {
//                CatName += "<option value='" + Categoryname.Value + "'>" + Categoryname.Text + "</option>";
//            }
//            // CatName += '<option value=' + Categoryname.Value + '>' + Categoryname.Text + '</option>'
//        });
//        $("#CategoryId").html(CatName);
//        var id = $('input[type="radio"][value="' + radioSelected + '"]').attr("id");
//        $("#" + id).parent('span').addClass("checked");
//        //$('#optionsRadios4').prop('checked', this.checked);
//        // $("#optionsRadios4").attr('checked', true).button('refresh');
//    });
//    // for stock transfer category//
//});

// menu assign jquery



    $('#btnassign').on('click', function (e) {

        var IsValid = true;
        var optionid = $('#hidcategoryId').val();
        e.preventDefault();
        var itemval = optionid + '-';
        var chekval = "";
        $('#tblmenuitems tbody tr').each(function () {

            if ($(this).find('td:eq(2) input').val() != "") {

                var ItemId = $(this).find('td:eq(0) input').val();

                var idsa = $("#" + ItemId)
                // alert(ItemId)
                if ($("#" + ItemId).prop('checked') == true) {

                    itemval += parseInt(ItemId) + "^";
                    var FullPrice = $(this).find('td:eq(1) input').val();
                    itemval = itemval + FullPrice + "^";

                    var HalfPrice = $(this).find('td:eq(2) input').val();
                    itemval = itemval + HalfPrice + "^";
                    var basePriceID = $(this).find('td:eq(3) input').val();
                    itemval = itemval + basePriceID + ";";
                }

            }
            else {
                $(this).find('td:eq(1) input').addClass("errorSubmit");
                IsValid = false;
            }

        });

        if (IsValid == true) {
            itemval = itemval.substring(0, itemval.length - 1);
            var items = { 'items': itemval };
            $("#assigndata").val(itemval);
        }
        else {
            return false;
        }

    });



//Base price menu jquery//



var counter = 0;
$(document).ready(function () {

    $('#btsetprice').on('click', function (e) {
        var IsValid = true;
        e.preventDefault();
        var itemval = "";
        var catid = $("#CategoryId").val();


        $('#table_Baseprice tbody tr').each(function () {

            if ($(this).find('td:eq(1) input').val() != "") {
                $(this).find('td:eq(1) input').removeClass("errorSubmit");
                var ItemId = $(this).find('td:eq(0) input').val();
                itemval = itemval + ItemId + "^";

                var Name = $(this).find('td:eq(1) input').val();
                itemval = itemval + Name + "^";

                var FullPrice = $(this).find('td:eq(2) input').val();
                itemval = itemval + FullPrice + "^";

                var HalfPrice = $(this).find('td:eq(3) input').val();
                itemval = itemval + HalfPrice + "^";

                var Vat = $(this).find('td:eq(4) input').val();
                itemval = itemval + Vat + ";";

            }
            else {
                $(this).find('td:eq(1) input').addClass("errorSubmit");
                IsValid = false;
            }

        });

        if (IsValid == true) {
            itemval = itemval.substring(0, itemval.length - 1);
            var items = { 'items': itemval };
            $("#Basepricedata").val(itemval);
            $('#BasePriceform').submit();
        }
        else {
            return false;
        }

    });

    $('#btnEdit').click(function () {

        var totlRecord = $('#tblEdit_BasePrice >tbody >tr').length;
        var txtList = "";
        $('#tblEdit_BasePrice tbody tr').each(function () {
            var txtBasePriceId = $(this).attr('id');
            var txtFullPrice = $(this).find('td:eq(1) input').val();
            var txtHalfPrice = $(this).find('td:eq(2) input').val();
            txtList = txtList.trim();
            txtList = txtList + txtBasePriceId.trim() + '_' + txtFullPrice.replace('.', '^') + '_' + txtHalfPrice.replace('.', '^') + '-';
        });

        txtList = txtList.substring(0, txtList.length - 1);
        // alert(txtList);
        $.getJSON('/BasePrice/EditBasePrice/' + txtList, function (data) {

        });
    });

});
//-------------Stock Return jquery---------------------------//

$(document).ready(function () {
    var rquantity = $(this).find('td:eq(3) input').val();
    var urerquantity = $("#txtreturnquantity").val();
    if (parseInt(urerquantity) > parseInt(rquantity)) {
        $("#errormesg").show();
    }
    $('#btnreturnstock').click(function () {

        var totlRecord = $('#tblReturn_stockquantity >tbody >tr').length;
        var txtList = "";
        $('#tblReturn_stockquantity tbody tr').each(function () {
            var transferid = $(this).attr('id');
           // var rtype = $(this).find('td:eq(0) input').val();
            //var rcategory = $(this).find('td:eq(1) input').attr('id');
            var ritem = $(this).find('td:eq(0) input').attr('id');
            var rquantity = $(this).find('td:eq(1) input').val();
            var urerquantity = $("#txtreturnquantity").val();
            var less = parseInt(rquantity) - parseInt(urerquantity);
            var rdescription = $("#txtareareturn").val();
            var retoutid = $("#returntooutlet").val();
            if (parseInt(urerquantity) > parseInt(rquantity)) {
                $("#errormesg").show();
            }
            else {
                $("#errormesg").hide();
                txtList = txtList.trim();

                txtList = txtList + transferid + '^' + less + '^' + ritem + '^' + urerquantity + '^' + rquantity + '^' + rdescription + '^' + retoutid +';';
            }
        });

        txtList = txtList.substring(0, txtList.length - 1);
        //alert(txtList);
        $.getJSON('/StockTransfer/SetstockQuantity/' + txtList, function (data) {
            // alert(data);
        });
    });
});

//---------get table of outlet details on selecting outlet(stock transfer)//
$(document).ready(function () {
    $("select[name='OutletId']").on('change', function () {
        var name = $(this).val();

        $.getJSON('/StockTransfer/OutletDetails/' + name, function (data) {
            $("#tbloutletdetail").html(data);
        });
    });
    //$("input[name=TransferRadios]:radio").on('change', function () {
    //    var name = $(this).val();
    //    $.getJSON('/StockTransfer/Transfercategory/' + name, function (data) {
    //        var CatName = '<option value="">Select Category</option>';
    //        var CatID = $("#CategoryEditId").val();
    //        var catValue = CatName.Value;

    //        $.each(data, function (i, Categoryname) {
    //            if (CatID != null && CatID != "") {
    //                if (parseInt(CatID) == parseInt(catValue)) {
    //                    CatName += "<option Selected value='" + Categoryname.Value + "'>" + Categoryname.Text + "</option>";
    //                }
    //                else {
    //                    CatName += "<option value='" + Categoryname.Value + "'>" + Categoryname.Text + "</option>";
    //                }
    //            }
    //            else {
    //                CatName += "<option value='" + Categoryname.Value + "'>" + Categoryname.Text + "</option>";
    //            }
    //        });
    //        $("#StockCategoryId").html(CatName);
    //    });

    //});
    //$("select[name='StockCategoryId']").on('change', function () {
    //    var cname = $(this).val();
    //    $.getJSON('/StockTransfer/Transferforitem/' + cname, function (data) {
    //        var CatName = '<option value="">Select Item</option>';
    //        var CatID = $("#CategoryEditId").val();
    //        var catValue = CatName.Value;


    //        $.each(data, function (i, StockItemName) {
    //            if (CatID != null && CatID != "") {
    //                if (parseInt(CatID) == parseInt(catValue)) {
    //                    CatName += "<option Selected value='" + StockItemName.Value + "'>" + StockItemName.Text + "</option>";
    //                }
    //                else {
    //                    CatName += "<option value='" + StockItemName.Value + "'>" + StockItemName.Text + "</option>";
    //                }
    //            }
    //            else {
    //                CatName += "<option value='" + StockItemName.Value + "'>" + StockItemName.Text + "</option>";
    //            }
    //        });
    //        $("#StockItemId").html(CatName);
    //    });

    //});
    $("select[name='RawMaterialId']").on('change', function () {
        var stmd = $(this).val();

        $.getJSON('/StockTransfer/Stockavailability/' + stmd, function (data) {
            var res = data.split('^');
            $("#stock").val(res[0]);
            $("#Type_0").val(res[1])
            var stckvalue = $("#stock").val();
            if (stckvalue == 0) {
                $("#transferstock").val(0);
                $("#transferstock").attr('readonly', true);
            }
            else {
                $("#transferstock").attr('readonly', false);
            }
        });
    });



    $("#table_Stocktransfer").append('<thead><th></th><th>Item Name</th><th>Available Stock</th><th>Transfer Stock</th><th>Transfer Unit</th><th>Department</th><th></th><th>Action</th></thead>');
    //var stk = $("#stock").val();
    //alert("hi");
    //var tstk = $("#transferstock").val();
    //alert(tstk);
    
    
    $(document).ready(function () {

        $("#Category").change(function () {
            $("#item").empty();
            $.ajax({
                type: 'POST',
                url: '@Url.Action("getitem")',

                dataType: 'json',

                data: { id: $("#Category").val() },


                success: function (items) {


                    $.each(items, function (i, item) {
                        $("#item").append('<option value="' + item.Value + '">' +
                             item.Text + '</option>');

                    });
                },
                error: function (ex) {
                    alert('Failed to retrieve country states.' + ex);
                }
            });
            return false;
        })
    });

    
});

//----------------------purchase jquery-----------------//


$(document).ready(function () {
    $("input[name=PaymentMode]:radio").on('change', function () {
        var self = $(this).attr("id");
        if (self == "rbtcash") {
            $("#CashDiv").show();
            $("#CreditDiv").hide();
            $("#ChequeDiv").hide();
        }
        else if (self == "rbtcredit") {
            $("#CreditDiv").show();
            $("#CashDiv").hide();
            $("#ChequeDiv").hide();
            var NetAmount = $('#netamt').val();
            $('#txtcreditdeposite').val(NetAmount);
        }
        else {
            $("#ChequeDiv").show();
            $("#CashDiv").hide();
            $("#CreditDiv").hide();
        }


    });

    $("#txtTax").on('input', function () {
        var Tax = $(this).val();
        var Totalamount = $('#tamt').val();
        var Net = parseInt(Totalamount) + parseInt(Tax)
        $('#netamt').val(Net);
    });
    $("#extchrg").on('input', function () {
        var ext = $(this).val();
        var Totalamount = $('#tamt').val();
        var Tax = $('#txtTax').val();
        var Net = parseInt(Totalamount) + parseInt(ext) + parseInt(Tax)
        $('#netamt').val(Net);
    });
    $("#txtcashdeposite").on('input', function () {
        var ext = $(this).val();
        var NetAmount = $('#netamt').val();
        var remainingamt = parseInt(NetAmount) - parseInt(ext);
        $('#txtcashremaing').val(remainingamt);
    });
});

//-----------Purchase Return----------------------//


$(document).ready(function () {
    var rquantity = $(this).find('td:eq(2) input').val();
    var urerquantity = $("#txtreturnPurchase").val();
    if (parseInt(urerquantity) > parseInt(rquantity)) {
        $("#errormesgpur").show();
    }
    $('#btnreturnpurchase').click(function () {

        var totlRecord = $('#tblReturn_stockquantity >tbody >tr').length;
        var txtList = "";
        $('#tblReturn_purchasequantity tbody tr').each(function () {

            var purchasedetailid = $(this).attr('id');
            var vendorid = $(this).find('td:eq(0) input').attr('id');
            var ritem = $(this).find('td:eq(1) input').attr('id');
            var rquantity = $(this).find('td:eq(2) input').val();
            var urerquantity = $("#txtreturnPurchase").val();
            var less = parseInt(rquantity) - parseInt(urerquantity);
            var rdescription = $("#txtPurareareturn").val();
            if (parseInt(urerquantity) > parseInt(rquantity)) {
                $("#errormesgpur").show();
            }
            else {
                $("#errormesgpur").hide();
                txtList = txtList.trim();

                txtList = txtList + vendorid + '^' + purchasedetailid + '^' + less + '^' + ritem + '^' + urerquantity + '^' + rdescription + ';';
            }
        });

        txtList = txtList.substring(0, txtList.length - 1);
        //alert(txtList);
        $.getJSON('/Purchase/SetPurchaseQuantity/' + txtList, function (data) {
            // alert(data);
        });
    });
});

//-------------------Offer Creation jquery-----------------//

$(document).ready(function () {

    $('input[name="selectcheck"]').click(function () {
        if ($(this).prop("checked") == true) {
            $('#selecttype').show();

        }
        else if ($(this).prop("checked") == false) {
            $('#selecttype').hide();

        }
    });
    $("select[name='Categoryid']").on('change', function () {
        var cname = $(this).val();
        //alert(cname);
        $.getJSON('/OfferCreation/OfferItems/' + cname, function (data) {
            var offercatName = '<option value="">Select Item</option>';
            var CatID = $("#CategoryEditId").val();
            var catValue = offercatName.Value;


            $.each(data, function (i, OfferItemName) {
                if (CatID != null && CatID != "") {
                    if (parseInt(CatID) == parseInt(catValue)) {
                        offercatName += "<option Selected value='" + OfferItemName.Value + "'>" + OfferItemName.Text + "</option>";
                    }
                    else {
                        offercatName += "<option value='" + OfferItemName.Value + "'>" + OfferItemName.Text + "</option>";
                    }
                }
                else {
                    offercatName += "<option value='" + OfferItemName.Value + "'>" + OfferItemName.Text + "</option>";
                }
            });
            $("#OfferItemId").html(offercatName);
        });

    });

    $("select[name='OfferItemId']").on('change', function () {
        var name = $(this).val();

        $.getJSON('/OfferCreation/OfferItemPrices/' + name, function (data) {
            var itepric = data.split('-');
            var full = itepric[0];
            var half = itepric[1];
            $('#itemfullprice').val(full);
            $('#itemhalfpric').val(half);


            //=data.FullPrice;
            // = data.HalfPrice;
            //$("#tbloutletdetail").html(data);
        });
    });

    $("#tblofferdetails").append('<thead><th></th><th>Category Name</th><th>Item Name</th><th>Full Price</th><th>Half Price</th><th>Offer Full Price</th><th>Offer Half Price</th><th>Action</th></thead>');
    $('#btnOfferadd').on('click', function (e) {
        $("#tblofferdetails").attr('hidden', false);
        var catname = $("#Categoryid :selected").text();
        var catvalue = $("#Categoryid :selected").val();
        var itename = $("#OfferItemId :selected").text();
        var itemvalue = $("#OfferItemId :selected").val();
        var fullvalue = $('#itemfullprice').val();
        var halfvalue = $('#itemhalfpric').val();
        var offervalue = $("input[name='Offers']:checked").val();

        //var stockvalue = $("#stock").val();
        //var Outletvalue = $("#stockoutlet :selected").val();
        //var transfervalue = $("#transferstock").val();
        //var radiocat = $('input[name=TransferRadios]:checked').val();
        //$("#txtofferdiscount").on('input', function () {
        //    var dis = $(this).val();
        //    //var fullamount = $('#itemfullprice').val();
        //    //var halfamount = $('#itemhalfpric').val();
        //    var fullamount =parseFloat(fullvalue);
        //    var halfamount = parseFloat(halfvalue);
        //    var Nethalf = (parseFloat(halfamount) * parseFloat(dis)) / 100;
        //    var Netfull = (parseFloat(fullamount) * parseFloat(dis)) / 100;
        //    var fullof = parseFloat(fullamount) - parseFloat(Netfull);
        //    var halfof = parseFloat(halfamount) - parseFloat(Nethalf);
        //    $('#tdfull').val(fullof);
        //    $('#txthalf').val(halfof);
        //});

        $("#tblofferdetails").append('<tbody><tr><td><input type="hidden"  id="' + offervalue + '"></td><td><input type="text" class="form-control" id=' + catvalue + ' value=' + catname + ' readonly/></td><td><input type="text" class="form-control" id=' + itemvalue + ' value=' + itename + ' readonly/></td><td><input type="text" class="form-control" value=' + fullvalue + ' readonly/></td><td><input type="text" class="form-control" value=' + halfvalue + ' readonly/></td><td><input type="text" id="tdfull" value="0" class="form-control"/></td><td><input type="text" id=txthalf  value="0" class="form-control"/></td><td><input type="button" class="btn green" value="Delete" id="btndelete"</td></tr></tbody>');

        $('#btndelete').live('click', function () {
            $(this).closest('tr').remove();

        });



    });
    $("input[name=Offers]:radio").on('change', function () {
        var value = $(this).val();
        if (value == "Happy Hours") {
            $("#onhappyhours").show();
            $("#oncombooffer").hide();
            $("#dayhappy").show();
        }
        else {
            $("#onhappyhours").hide();
            $("#oncombooffer").show();
            $("#dayhappy").hide();
        }
    });


    //------------Save offerdata in table------//
    $('#btnoffersubmite').on('click', function (e) {
        var IsValid = true;
        e.preventDefault();

        var Offerval = "";
        var catid = $("#CategoryId").val();


        $('#tblofferdetails tbody tr').each(function () {

            $('input[name="alldiscount"]').click(function () {
                if ($(this).prop("checked") == true) {
                    var applyall = "T";
                    Offerval = Offerval + applyall + "^";

                }
                else if ($(this).prop("checked") == false) {
                    var applyall = "F";
                    Offerval = Offerval + applyall + "^";
                }
            });

            if ($(this).find('td:eq(1) input').val() != "") {
                $(this).find('td:eq(1) input').removeClass("errorSubmit");
                var type = $(this).find('td:eq(0) input').attr('id');
                Offerval = Offerval + type + "^";

                var Catid = $(this).find('td:eq(1) input').attr('id');
                Offerval = Offerval + Catid + "^";

                var itemid = $(this).find('td:eq(2) input').attr('id');
                Offerval = Offerval + itemid + "^";

                var offrfull = $(this).find('td:eq(5) input').val();
                Offerval = Offerval + offrfull + "^";

                var offrhalf = $(this).find('td:eq(6) input').val();
                Offerval = Offerval + offrhalf + ";";


            }
            else {
                $(this).find('td:eq(1) input').addClass("errorSubmit");
                IsValid = false;
            }

        });

        if (IsValid == true) {
            Offerval = Offerval.substring(0, Offerval.length - 1);
            var items = { 'items': Offerval };
            $("#offervaldata").val(Offerval);
            $("#form_sample_12").submit();
        }
        else {
            return false;
        }

    });


});

//-------------------------------Billing jquery-----------------------//

$(document).ready(function () {

    $(".BillingCategory").click(function () {
        var bilcatid = $(this).attr("id");
        $.getJSON('/Billing/Billingitms/' + bilcatid, function (data) {

            $("#tblbilling").html(data);
        });
    });

});
$(document).ready(function () {

    //$(document).on("click", ".btnbillgadd", function () {
    //    var talleno = $('#ttblno').val();
    //    var taqblenoId = '#tbl_' + $('#ttblno').val();
    //    $(taqblenoId).attr('hidden', false);
    //    var itemvalue = $(this).attr("id");
    //    $('#gid').val(itemvalue);
    //    var $td = $(this).parent('td').parent('tr').children('td');
    //    //var itename = $td.eq(0).find("input").val();
    //    var itename = $(this).val();
    //    $('#gname').val(itename);
    //    var radiocat = $('input[name=gradio]:checked').val();
    //    $.getJSON('/Billing/bilprice/' + itemvalue, function (data) {
    //        var arr = data.split('-');
    //        var full = arr[0];
    //        var half = arr[1];
    //        var vat = arr[2];
    //        var vatamt = 0;
    //        var Quantity = $("#graphicquty").val();
    //        var radiocat = $('input[name=gradio]:checked').val();
    //        var fulli = 0;
    //        var halfi = 0;
    //        var amount = 0;
    //        var amtte = 0;
    //        var netamount = 0;
    //        var subtotl = 0;
    //        var catd = 0;
    //        var ab = $(taqblenoId + ' ' + ' tbody').children().length;
    //        if (ab == 0) {
    //            $(taqblenoId).append('<tbody><tr id=' + itemvalue + '><td><input type="hidden" id="hiddentd" value=' + itemvalue + '></td><td><input type="text" class="form-control" id=' + itemvalue + ' value=' + itename + ' readonly/></td><td><input type="text" class="form-control" value=' + fulli + ' readonly/></td><td><input type="text" class="form-control" value=' + halfi + ' readonly/></td><td><input type="text" class="form-control" value=' + full + ' readonly/></td><td><input type="text" class="form-control" value=' + half + ' readonly/></td><td><input type="text" class="form-control" value=' + amount + ' readonly/></td><td><input type="text" class="form-control" value=' + vat + ' readonly/></td><td><input type="hidden" id="txtvatamount" value=' + vatamt + '></td><td><input type="button" class="btn green" value="Delete" name="btndelete" id=' + itemvalue + '></td></tr></tbody>');
    //        }
    //        else {

    //            $(taqblenoId + ' ' + ' tbody tr').each(function () {
    //                var rowid = $(this).closest('tr').attr('id');
    //                var itemvalue = $("#gid").val();
    //                if (parseInt(rowid) == parseInt(itemvalue)) {
    //                    catd = 1;
    //                    return false;
    //                }
    //                else {
    //                    catd = 0;
    //                }
    //                return;
    //            });
    //            if (parseInt(catd) == 0) {
    //                $(taqblenoId).append('<tr id=' + itemvalue + '><td><input type="hidden" id="hiddentd" value=' + itemvalue + '></td><td><input type="text" class="form-control" id=' + itemvalue + ' value=' + itename + ' readonly/></td><td><input type="text" class="form-control" value=' + fulli + ' readonly/></td><td><input type="text" class="form-control" value=' + halfi + ' readonly/></td><td><input type="text" class="form-control" value=' + full + ' readonly/></td><td><input type="text" class="form-control" value=' + half + ' readonly/></td><td><input type="text" class="form-control" value=' + amount + ' readonly/></td><td><input type="text" class="form-control" value=' + vat + ' readonly/></td><td><input type="hidden"  id="txtvatamount" value=' + vatamt + '></td><td><input type="button" class="btn green" value="Delete"  name="btndelete" id=' + itemvalue + '></td></tr>');
    //            }
    //        }
    //        $(taqblenoId + ' ' + ' tbody tr').each(function () {

    //            var row = $(this).closest('tr').attr('id');
    //            if ($(this).find('td:eq(0) input').val() == row) {
    //                var itename = $("#gname").val();
    //                var itemvalue = $("#gid").val();
    //                var Quantity = $("#graphicquty").val();
    //                var radiocat = $('input[name=gradio]:checked').val();
    //                var amtt;
    //                if ($(this).find('td:eq(0) input').val() == itemvalue) {
    //                    if (radiocat == "Full") {
    //                        var fulli = $(this).find('td:eq(2) input').val();
    //                        var halfi = $(this).find('td:eq(3) input').val();
    //                        fulli = parseInt(fulli) + parseInt(Quantity);
    //                        var ab = parseInt(fulli) * parseInt(full);
    //                        var bb = parseInt(halfi) * parseInt(half);
    //                        var vat = $(this).find('td:eq(7) input').val();
    //                        var amount = ab + bb;
    //                        var vatamt = ((parseInt(amount) * parseInt(vat)) / 100);
    //                        $(this).closest('tr').remove();
    //                        $(taqblenoId).append('<tr id=' + itemvalue + '><td><input type="hidden" id="hiddentd" value=' + itemvalue + '></td><td><input type="text" class="form-control" id=' + itemvalue + ' value=' + itename + ' readonly/></td><td><input type="text" class="form-control" value=' + fulli + ' readonly/></td><td><input type="text" class="form-control" value=' + halfi + ' readonly/></td><td><input type="text" class="form-control" value=' + full + ' readonly/></td><td><input type="text" class="form-control" value=' + half + ' readonly/></td><td><input type="text" class="form-control" value=' + amount + ' readonly/></td><td><input type="text" class="form-control" value=' + vat + ' readonly/></td><td><input type="hidden"  id="txtvatamount" value=' + vatamt + '></td><td><input type="button" class="btn green" value="Delete"  name="btndelete" id=' + itemvalue + '></td></tr>');
    //                    }
    //                    else {
    //                        var halfi = $(this).find('td:eq(3) input').val();
    //                        var fulli = $(this).find('td:eq(2) input').val();
    //                        halfi = parseInt(halfi) + parseInt(Quantity);
    //                        var ab = parseInt(fulli) * parseInt(full);
    //                        var bb = parseInt(halfi) * parseInt(half);
    //                        var vat = $(this).find('td:eq(7) input').val();
    //                        var amount = ab + bb;
    //                        var vatamt = ((parseInt(amount) * parseInt(vat)) / 100);
    //                        $(this).closest('tr').remove();
    //                        $(taqblenoId).append('<tr id=' + itemvalue + '><td><input type="hidden" id="hiddentd" value=' + itemvalue + '></td><td><input type="text" class="form-control" id=' + itemvalue + ' value=' + itename + ' readonly/></td><td><input type="text" class="form-control" value=' + fulli + ' readonly/></td><td><input type="text" class="form-control" value=' + halfi + ' readonly/></td><td><input type="text" class="form-control" value=' + full + ' readonly/></td><td><input type="text" class="form-control" value=' + half + ' readonly/></td><td><input type="text" class="form-control" value=' + amount + ' readonly/></td><td><input type="text" class="form-control" value=' + vat + ' readonly/></td><td><input type="hidden"  id="txtvatamount" value=' + vatamt + '></td><td><input type="button" class="btn green" value="Delete"  name="btndelete" id=' + itemvalue + '></td></tr>');
    //                    }


    //                }

    //            }


    //        });
    //        $('input[name=btndelete]').on('click', function () {
    //            var id = $(this).attr('id');
    //            $(taqblenoId + ' ' + ' tbody tr').each(function () {
    //                var loopid = $(this).find('td:eq(0) input').val();
    //                if (loopid == id) {
    //                    var delvat = $(this).find('td:eq(8) input').val();
    //                    var delamount = $(this).find('td:eq(6) input').val();

    //                    var delvatamt = $('#txtvatamt_' + talleno).val();
    //                    var vatreal = (parseFloat(delvatamt) - parseFloat(delvat));
    //                    $('#txtvatamt_' + talleno).val(vatreal);

    //                    var deltotamt = $("#txtsubtotal_" + talleno).val();
    //                    var totreal = (parseFloat(deltotamt) - parseFloat(delamount));
    //                    $("#txtsubtotal_" + talleno).val(totreal);

    //                    var delservic = $("#txtservice_" + talleno).val();
    //                    var servicreal = ((parseFloat(totreal) * 5) / 100);
    //                    $("#txtservice_" + talleno).val(servicreal);

    //                    var delnetamount = (parseFloat(vatreal) + parseFloat(totreal) + parseFloat(servicreal));
    //                    $("#txtnettotal_" + talleno).val(delnetamount);
    //                    $(this).closest('tr').remove();
    //                }
    //                else {
    //                    return;
    //                }

    //            });
    //        });

    //        $(taqblenoId + ' ' + ' tbody tr').each(function () {
    //            var amtt = $(this).find('td:eq(8) input').val();
    //            var total = $(this).find('td:eq(6) input').val();
    //            subtotl = (parseFloat(total) + parseFloat(subtotl));
    //            $("#txtsubtotal_" + talleno).val(subtotl);
    //            amtte = (parseFloat(amtt) + parseFloat(amtte));
    //            $("#txtvatamt_" + talleno).val(amtte);
    //            var serviceamount = ((parseFloat(subtotl) * 5) / 100);
    //            $("#txtservice_" + talleno).val(serviceamount);
    //            netamount = (parseFloat(amtte) + parseFloat(subtotl) + parseFloat(serviceamount));
    //            $("#txtnettotal_" + talleno).val(netamount);




    //            var optype = $("#Creteateoperator").val();
    //            if (optype == "Cashier") {
    //                $("#txtdiscount_" + talleno).prop('readonly', false);
    //                $("#txtdiscount_" + talleno).on('input', function () {
    //                    var val = $("#htxtdiscount").val();
    //                    if (parseFloat(val) >= 0 && parseFloat(val) <= 20.00) {
    //                        var discount = $(this).val();
    //                        var discountamt = ((parseFloat(netamount) * parseFloat(discount)) / 100);
    //                        var discountnet = parseFloat(netamount) - parseFloat(discountamt);
    //                        $("#txtnettotal_" + talleno).val(discountnet);
    //                        $("#txtdiscountprice_" + talleno).val(discountamt);
                           
    //                    }
    //                    else {
    //                        $("#msgerror").show('slow');
    //                    }
    //                });

    //            }
    //            else if (optype == "Manager") {
    //                $("#txtdiscount_" + talleno).prop('readonly', false);
    //                $("#txtdiscount_" + talleno).on('input', function () {
    //                    var discount = $(this).val();
    //                    var discountamt = ((parseFloat(netamount) * parseFloat(discount)) / 100);
    //                    var discountnet = parseFloat(netamount) - parseFloat(discountamt);
    //                    $("#txtnettotal_" + talleno).val(discountnet);
    //                    $("#txtdiscountprice_" + talleno).val(discountamt);
    //                });

    //            }



    //            //$("#txtdiscount_" + talleno).on('input', function () {
    //            //    var discount = $(this).val();
    //            //    var value = $("#operatortype").val();
    //            //    var discountamt = ((parseFloat(netamount) * parseFloat(discount)) / 100);
    //            //    var discountnet = parseFloat(netamount) - parseFloat(discountamt);
    //            //    $("#txtnettotal_" + talleno).val(discountnet);
    //            //    $("#txtdiscountprice_" + talleno).val(discountamt);
    //            //});

    //        });

    //    });

    //});
   
});



$(document).ready(function () {
    $("input[name=BillPrintType]:radio").on('change', function () {
        var value = $(this).val();
        //alert(value);
        $("#BillType").val(value);
    });
});

var arr = [];
$(document).ready(function () {
    var tttt1;
    var operate = false;
    var current_talbleno = 0;
    $(".tabltd").click(function () {
        tttt1 = $(this).val();
        current_talbleno = $(this).val();
        var tableno = $(this).val();
        var len = arr.length;
        if (len != 0) {
            $(this).css('color', 'red');
            var id = tttt1;
            if ($.inArray(id, arr) < 0) {
                arr.push(tttt1);
                for (var i = 0; i <= len; i++) {
                    var gid = $(".GraphicRight").attr('id');
                    var aaa = gid.split('_');
                    $(".GraphicRight").hide();
                    if (arr[i] == tttt1) {
                        var id = 'div_' + tttt1;
                        $(id).show();
                    }
                }
                $("#ttblno").val(tableno);
                $("#mainbillingdiv").append('<div class="GraphicRight" id="div_' + tttt1 + '">' +
                                        '<div class="right_mid">' +
                                            '<div class="col-md-12 right_mid2">' +
                                                '<label class="col-md-3" style="padding-top: 6px;">Customer Name</label>' +
                                                '<div class="col-md-6">' +
                                                '<input type="text" name="name" class="form-control" value=" " id="txtcname" />' +
                                                '</div>'+
                                           ' </div>' +
                                        ' </div>' +
                                        '<div class="col-md-12 right_mid_bottom">' +
                                           ' <table class="table table-bordered" id="tbl_' + tttt1 + '" hidden>' +
                                                '<thead>' +
                                                    '<tr>' +
                                                        '<th></th>' +
                                                        '<th>Item</th>' +
                                                        '<th>Full Qty</th>' +
                                                        '<th>Half Qty</th>' +
                                                        '<th>Full Price</th>' +
                                                        '<th>Half Price</th>' +
                                                        '<th>Amount</th>' +
                                                        '<th>Vat(%)</th>' +
                                                        '<th></th>' +
                                                        '<th>Action</th>' +
                                                    '</tr>' +
                                                '</thead>' +

                                            '</table>' +
                                        '</div>' +
                                        '<div class="col-md-12" style="padding-left:0px;padding-top: 15px;">' +
                                            '<div class="col-md-4 form-horizontal">' +
                                                '<div class="form-group">' +
                                                '<label class="col-md-7">Discount(%)</label>' +
                                                '<div class="col-md-5">' +
                                                '<input type="text" maxlength="3" name="name" value="" class="form-control" readonly id="txtdiscount_' + tttt1 + '" />' +
                                                '</div>' +
                                                '</div>' +//end
                                                '<div class="form-group">' +
                                                '<label class="col-md-7">Vat Amount</label>' +
                                                '<div class="col-md-5">' +
                                                '<input type="text" name="name" value="" class="form-control" readonly id="txtvatamt_' + tttt1 + '" />' +
                                                '</div>' +
                                                '</div>' +//end
                                                '<div class="form-group">' +
                                                '<label class="col-md-7">Service Charge</label>' +
                                                '<div class="col-md-5">' +
                                                '<input type="text" name="name" value="" class="form-control" readonly id="txtservice_' + tttt1 + '" />' +
                                                '</div>' +
                                                '</div>' +
                                                '</div>' +//end
                                                //--------
                                                '<div class="col-md-4 form-horizontal">' +
                                                '<div class="form-group">' +
                                                '<label class="col-md-7">Discount(Price)</label>' +
                                                '<div class="col-md-5">' +
                                                '<input type="text" name="name" value="" class="form-control" readonly id="txtdiscountprice_' + tttt1 + '" />' +
                                                '</div>' +
                                                '</div>' +//end
                                                '<div class="form-group">' +
                                                '<label class="col-md-7">Amount</label>' +
                                                '<div class="col-md-5">' +
                                                '<input type="text" name="name" value="" class="form-control" readonly id="txtsubtotal_' + tttt1 + '" />' +
                                                '</div>' +
                                                '</div>' +//end
                                                '<div class="form-group">' +
                                                '<label class="col-md-7">Net Amount</label>' +
                                                '<div class="col-md-5">' +
                                                '<input type="text" name="name" value="" class="form-control" readonly id="txtnettotal_' + tttt1 + '" />' +
                                                '</div>' +
                                                '</div>' +
                                                '</div>' +//end
                                                //--------
                                                '<div class="col-md-4 form-horizontal">' +
                                                '<input  type="button"  id="btndispatch_' + tttt1 + '" name="name" value="Dispatch Order" class="DispatchClick btn green" formtarget="_blank" />' +
                                                '<div class="clear margin-bottom-10"></div>' +
                                                '<input  type="button" style="width:127px;" name="name" value="Cancel Order" class="btn green canclegorder_Click" id="canclegorder_' + tttt1 + '" />' +
                                                '<div class="clear margin-bottom-10"></div>' +
                                                '<a  class="btn green " id="gbtnback_' + tttt1 + '"  target="_blank"  style="width:127px;">Print</a>' +
                                                '<div class="clear margin-bottom-10"></div>' +
                                            '</div>' +
                                            '</div>' +
                                         '</div>' +
                                         '</div>' +
                                     '</div>');


            }
            else {
                var gid = $(".GraphicRight").attr('id');
                var aaa = gid.split('_');

                for (var i = 0; i < len; i++) {
                    if (arr[i] != aaa[1]) {
                        $(".GraphicRight").hide();
                        //$(".GraphicRight").show();
                        //$("#ttblno").val(tableno);
                        //$(this).css('color', 'red');
                    }

                }
                var id = '#div_' + tttt1;
                $(id).show();
                $("#ttblno").val(tableno);

            }
        }
        else {
            arr.push(tttt1);
            $(this).css('color', 'red');
            $("#ttblno").val(tableno);
            $("#mainbillingdiv").append('<div class="GraphicRight" id="div_' + tttt1 + '">' +
                                        '<div class="right_mid">' +
                                            '<div class="col-md-12 right_mid2">' +
                                                '<label class="col-md-3" style="padding-top: 6px;">Customer Name</label>' +
                                                '<div class="col-md-6">' +
                                                '<input type="text" name="name" class="form-control" value=" " id="txtcname" />' +
                                                '</div>' +
                                           ' </div>' +
                                        ' </div>' +
                                        '<div class="col-md-12 right_mid_bottom">' +
                                           ' <table class="table table-bordered" id="tbl_' + tttt1 + '" hidden>' +
                                                '<thead>' +
                                                    '<tr>' +
                                                        '<th></th>' +
                                                        '<th>Item</th>' +
                                                        '<th>Full Qty</th>' +
                                                        '<th>Half Qty</th>' +
                                                        '<th>Full Price</th>' +
                                                        '<th>Half Price</th>' +
                                                        '<th>Amount</th>' +
                                                        '<th>Vat(%)</th>' +
                                                        '<th></th>' +
                                                        '<th>Action</th>' +
                                                    '</tr>' +
                                                '</thead>' +

                                            '</table>' +
                                        '</div>' +
                                        '<div class="col-md-12" style="padding-left:0px;padding-top: 15px;">' +
                                            '<div class="col-md-4 form-horizontal">' +
                                                '<div class="form-group">' +
                                                '<label class="col-md-7">Discount(%)</label>' +
                                                '<div class="col-md-5">' +
                                                '<input type="text" maxlength="3" name="name" value="" class="form-control" readonly id="txtdiscount_' + tttt1 + '" />' +
                                                '</div>' +
                                                '</div>' +//end
                                                '<div class="form-group">' +
                                                '<label class="col-md-7">Vat Amount</label>' +
                                                '<div class="col-md-5">' +
                                                '<input type="text" name="name" value="" class="form-control" readonly id="txtvatamt_' + tttt1 + '" />' +
                                                '</div>' +
                                                '</div>' +//end
                                                '<div class="form-group">' +
                                                '<label class="col-md-7">Service Charge</label>' +
                                                '<div class="col-md-5">' +
                                                '<input type="text" name="name" value="" class="form-control" readonly id="txtservice_' + tttt1 + '" />' +
                                                '</div>' +
                                                '</div>' +
                                                '</div>' +//end
                                                //--------
                                                '<div class="col-md-4 form-horizontal">' +
                                                '<div class="form-group">' +
                                                '<label class="col-md-7">Discount(Price)</label>' +
                                                '<div class="col-md-5">' +
                                                '<input type="text" name="name" value="" class="form-control" readonly id="txtdiscountprice_' + tttt1 + '" />' +
                                                '</div>' +
                                                '</div>' +//end
                                                '<div class="form-group">' +
                                                '<label class="col-md-7">Amount</label>' +
                                                '<div class="col-md-5">' +
                                                '<input type="text" name="name" value="" class="form-control" readonly id="txtsubtotal_' + tttt1 + '" />' +
                                                '</div>' +
                                                '</div>' +//end
                                                '<div class="form-group">' +
                                                '<label class="col-md-7">Net Amount</label>' +
                                                '<div class="col-md-5">' +
                                                '<input type="text" name="name" value="" class="form-control" readonly id="txtnettotal_' + tttt1 + '" />' +
                                                '</div>' +
                                                '</div>' +
                                                '</div>' +//end
                                                //--------
                                                '<div class="col-md-4 form-horizontal">' +
                                                '<input  type="button"  id="btndispatch_' + tttt1 + '" name="name" value="Dispatch Order" class="DispatchClick btn green" formtarget="_blank" />' +
                                                '<div class="clear margin-bottom-10"></div>' +
                                                '<input  type="button" style="width:127px;" name="name" value="Cancel Order" class="btn green canclegorder_Click" id="canclegorder_' + tttt1 + '" />' +
                                                '<div class="clear margin-bottom-10"></div>' +
                                                '<a  class="btn green " id="gbtnback_' + tttt1 + '"  target="_blank"  style="width:127px;">Print</a>' +
                                                '<div class="clear margin-bottom-10"></div>' +
                                            '</div>' +
                                            '</div>' +
                                         '</div>' +
                                         '</div>' +
                                     '</div>');
            var id = '#div_' + tttt1;
            $(id).show();
        }

        $('#btndispatch_' + tttt1).on('click', function (e) {
            var IsValid = true;
            e.preventDefault();
            var tableno = $('#ttblno').val();
            var taqblenoId = 'tbl_' + $('#ttblno').val();
            var net = $("#txtnettotal_" + tableno).val();
            var totalamount = $("#txtsubtotal_" + tableno).val();
            var vatamount = $("#txtvatamt_" + tableno).val();
            var servicamount = $("#txtservice_" + tableno).val();
            var discountamount = $("#txtdiscountprice_" + tableno).val();
            var netamount = $("#txtnettotal_" + tableno).val();
            var tableno = $("#ttblno").val();
            var custname = $("#txtcname").val();
            var mainitems = totalamount + "^" + vatamount + "^" + servicamount + "^" + discountamount + "^" + netamount + "^" + tttt1 + "^" + custname + ";";
            $("#masterdata").val(mainitems);
            var item = "";
            var abc = '#' + taqblenoId + ' ' + ' tbody tr';
            $(abc).each(function () {

                if ($(this).find('td:eq(0) input').val() != "") {

                    $(this).find('td:eq(1) input').removeClass("errorSubmit");
                    var ItemId = $(this).find('td:eq(0) input').val();
                    item = item + ItemId + "^";

                    var FullQty = $(this).find('td:eq(2) input').val();
                    item = item + FullQty + "^";

                    var HalfQty = $(this).find('td:eq(3) input').val();
                    item = item + HalfQty + "^";

                    var Amount = $(this).find('td:eq(6) input').val();
                    item = item + Amount + ";";

                }
                else {
                    $(this).find('td:eq(1) input').addClass("errorSubmit");
                    IsValid = false;
                }
            });
            if (IsValid == true && item != "") {
                item = item.substring(0, item.length - 1);
                var items = { 'items': item };
                $("#detailsitems").val(item);
                FormData();
                $('#gbtnback_' + tttt1).prop("disabled", false);
                var removeItem = tttt1;
                var $div = $("#tbl_" + tttt1 + ">tbody")
                $div.contents().remove();
                $("#" + tttt1).css('color', 'Black');
                $("#ttblno").val("");
                $("#txtdiscount_" + tttt1).val("0");
                $("#txtvatamt_" + tttt1).val("0");
                $("#txtservice_" + tttt1).val("0");
                $("#txtdiscountprice_" + tttt1).val("0");
                $("#txtsubtotal_" + tttt1).val("");
                $("#txtnettotal_" + tttt1).val("");
            }
            else {
                return false;
            }
        });

        function FormData() {
            var form = $("#form_sample_39");
            var url = form.attr("action");
            var formData = form.serialize();
            $.post(url, formData, function (data) {
                $('#gbtnback_' + tttt1).prop("disabled", true);
                // alert(data);
                $('#gbtnback_' + tttt1).attr('href', 'http://localhost:54622//Billing/RestroPrint?id=' + data)
                // $("#hiddenprintdiv").html(data);
            });

        }
        function printdiv() {
            // var hideid = document.getElementById("hiddenprintdiv");
            var w = window.open();
            var printOne = $('#hiddenprintdiv').html();
            //var printTwo = $('.termsToPrint').html();
            w.document.write('<html><head><title>Nibs Bill Printed</title></head><body><h1>Nibs</h1><hr />' + printOne) + '</body></html>';
            w.window.print();
            w.document.close();
            return false;
        }

        //----------------Sift tables in Restro Billing show div of sifting---------------------//
        $('#btnsift').on('click', function () {
            var tblvalue = $("#ttblno").val();
            $("#Shiftedtable").val(tblvalue);
            $("#Shiftedtable").css('color', 'red');
            $("#Shtable").val(tblvalue)
            $("#ShiftedBytable").val(null);
            $(".shitto").each(function () {
                $(this).css('color', 'black');
            });
            $("#shiftdiv").show();
        });
        //---------------On click merge button dialog box appears------------------------------//
        $("#btnmerge").on('click', function () {
            $("#Mergdivmain").show();
        });
       
    });
    $("#btnconfirm").on('click', function () {
        $("#choicemerge").show();
        var arrlengthmerge = mergearr.length;
        for (var i = 0; i < arrlengthmerge; i++) {
            $("#btnconfirmclick").append('<input type="button" name="TableNo" value=' + mergearr[i] + ' class="mergemaster" id="mergemaster" />');
        }
        $(".mergemaster").click(function () {
            var mstrtblno = $(this).val();
            $("#master").val(mstrtblno);
        });
        $("#btnconfirm").hide();
    });
});
//----------------Merge table no on dialog box------------------------//
var mergearr = [];
//---------------make an array when click on tables that we want to merge------------------//
$(".mergetblno").on('click', function () {
    var tblvalue = $(this).val();
    if ($.inArray(tblvalue, mergearr) < 0) {
        $(this).css('color', 'red');
        mergearr.push($(this).val());
    }
    else {
        $(this).css('color', 'black');
        mergearr.pop($(this).val());
        //alert(mergearr);
    }
   
});

//-------------------------button ok of merge click------------------------------//
$("#btnmergeok").click(function () {
    var masterttble = $("#master").val();
  
    $("#Mergdivmain").hide();
    var i = 0;
    var lengthmarry = mergearr.length;
    var arrayofmrg = [];
    while (i < lengthmarry) {
        if (i==0) {
            var tblid = '#tbl_' + masterttble + ' ' + ' tbody:last';
        }
        if (mergearr[i] != masterttble) {
            $(tblid).append($('#tbl_' + mergearr[i] + '' + '>' + 'tbody').html());
        }
        i++;
    }
    //$('#tbl_'+masterttble).tablesorter();
    var j = 0;
    while (j < lengthmarry) {
        if (mergearr[j]!=masterttble) {
            $('#tbl_' + mergearr[j]).remove();
            $(".tabltd").each(function () {
                var maintbltd = $(this).attr('id');
                if (maintbltd == mergearr[j]) {
                    $(this).css('color', 'black');
                    $(this).attr('disabled', 'disabled');
                }
            });
        }
        j++;
    }
    var mergetableid = 'tbl_' + masterttble;
   
    var duplicatetr = [];
    var dupli="";
    $('#' + mergetableid + '>tbody>tr').each(function () {
        var itmid = $(this).find('td:eq(1) input').attr('id');
        duplicatetr.push(itmid);
    });
    
    //var popno = 0;
    //for (var i = 0; i < duplicatetr.length; i++) {
    //    for (var j = 0; j < duplicatetr.length; j++) {
    //        if (i!=j) {
    //            if (duplicatetr[i] == duplicatetr[j]) {
    //                popno = 1;
    //            }
    //            else {
    //                popno = 0;
    //            }
    //        }
          
    //    }
    //    if (popno==0) {
    //        duplicatetr.pop(duplicatetr[i]);
    //    }
    //}
    //alert(duplicatetr);

    //-----------current merge solution-----------//

    //var full = 0;
    //var half = 0;
    //var fullprice = 0;
    //var halfprice = 0;
    //var amount = 0;
    //var vat = 0;
    //var vatamt = 0;
    //var itemname;
    //for (var i = 0; i < duplicatetr.length; i++) {
    //    for (var j = 1; j < duplicatetr.length; j++) {
    //        if (duplicatetr[i] == duplicatetr[j]) {
    //            $('#' + mergetableid + '>tbody>tr').each(function () {
    //                //if ($(this).find('td:eq(1) input').attr('id') == duplicatetr[i]) {
    //                    var itmval = duplicatetr[i];
    //                    full = parseInt(full) + parseInt($(this).find('td:eq(2) input').val());
    //                    half = parseInt(half) + parseInt($(this).find('td:eq(3) input').val());
    //                    fullprice = parseFloat($(this).find('td:eq(4) input').val());
    //                    halfprice = parseFloat($(this).find('td:eq(5) input').val());
    //                    amount = parseFloat(amount) + parseFloat($(this).find('td:eq(6) input').val());
    //                     vat = $(this).find('td:eq(7) input').val();
    //                     itemname = $(this).find('td:eq(1) input').val();
    //                     vatamt = parseFloat(vatamt) + parseFloat($(this).find('td:eq(8) input').val());
    //                     duplicatetr.pop(itmval);
    //                     //$(this).remove();
    //                //}
    //                //$('#' + mergetableid + '>tbody>tr').each(function () {
    //                //    if (itmval==($(this).find('td:eq(1) input').attr('id'))) {
    //                //        $(this).remove();
    //                //    }
    //                //});
    //                    // $('#' + mergetableid).append('<tr id=' + itmval + '><td><input type="hidden" id="hiddentd" value=' + itmval + '></td><td><input type="text" class="form-control" id=' + itmval + ' value=' + itemname + ' readonly/></td><td><input type="text" class="form-control" value=' + full + ' readonly/></td><td><input type="text" class="form-control" value=' + half + ' readonly/></td><td><input type="text" class="form-control" value=' + fullprice + ' readonly/></td><td><input type="text" class="form-control" value=' + halfprice + ' readonly/></td><td><input type="text" class="form-control" value=' + amount + ' readonly/></td><td><input type="text" class="form-control" value=' + vat + ' readonly/></td><td><input type="hidden"  id="txtvatamount" value=' + vatamt + '></td><td><input type="button" class="btn green" value="Delete"  name="btndelete" id=' + itmval + '></td></tr>');
    //            });
                

    //        }
    //    }
      
    //}
    
    var seen = {};
    $('#' + mergetableid + '>tbody>tr').each(function () {
        var txt = $(this).html();
        if (seen[txt]) {
            var halfval = $(txt).find('td:eq(3) input').val();
            alert(halfval);
            $(this).remove();
        }
        else {
            seen[txt] = true;
        }
    });
});

//----------------Sift tables in Restro Billing---------------------//
var abd = 0;
var flag = 0;
$(".shitto").on('click', function () {
    abd = $(this).val();
    var divid = 'div_' + abd;
    var flagshift = 0;
    $(".GraphicRight").each(function () {
        var freetable = $(this).attr('id');
        if (divid==freetable) {
            flagshift = 1;
        }
    });
    if (flagshift == 1) {
        alert("This table is running..!");
    }
    else {
        flag = 1;
        $(this).css('color', 'blue');
        $("#ShiftedBytable").val(abd);
        var number = $("#ttblno").val();
    }
    $("#btnok").on('click', function () {
        if (flag==1) {
            var number = $("#ttblno").val();
            $("#shiftdiv").hide();
            $("#ttblno").val(abd);
            $('#tbl_' + number).attr("id", 'tbl_' + abd);
            $('#div_' + number).attr("id", 'div_' + abd);
            var itemtoRemove = number;
            arr = $.grep(arr, function (value) {
                return value != itemtoRemove;
            });
        
            arr.push(abd);
            $('#txtdiscount_' + number).attr("id", 'txtdiscount_' + abd);
            $('#txtvatamt_' + number).attr("id", 'txtvatamt_' + abd);
            $('#txtservice_' + number).attr("id", 'txtservice_' + abd);
            $('#txtdiscountprice_' + number).attr("id", 'txtdiscountprice_' + abd);
            $('#txtsubtotal_' + number).attr("id", 'txtsubtotal_' + abd);
            $('#txtnettotal_' + number).attr("id", 'txtnettotal_' + abd);
            $('#btndispatch_' + number).attr("id", 'btndispatch_' + abd);
            $('#canclegorder_' + number).attr("id", 'canclegorder_' + abd);
            $('#gbtnback_' + number).attr("id", 'gbtnback_' + abd);
            $('.tabltd').each(function () {
                var ides = $(this).attr('id');
                if (ides == number) {
                    $(this).css('color', 'black');
                }
                else if (ides == abd) {
                    $(this).css('color', 'red');
                }
            });
            flag = 0;
        }
    });
    $("#btncancle").click(function () {
        $("#shiftdiv").hide();
    });
});
 
//--------------jquery for Take Away Billing---------------//
//$(document).ready(function () {
//    $(document).on("click", ".btnbillgadd", function () {
//        $("#tbltakeaway").attr('hidden', false);
//        var itemvalue = $(this).attr("id");
//        $('#hidenitemval').val(itemvalue);
//        var $td = $(this).parent('td').parent('tr').children('td');
//        var itename = $(this).val();
//        $('#hidenname').val(itename);
//        var radiocat = $('input[name=gradio]:checked').val();
//        $.getJSON('/Billing/bilprice/' + itemvalue, function (data) {
//            var arr = data.split('-');
//            var full = arr[0];
//            var half = arr[1];
//            var vat = arr[2];
//            var vatamt = 0;
//            var Quantity = $("#graphicquty").val();
//            var radiocat = $('input[name=gradio]:checked').val();
//            var fulli = 0;
//            var halfi = 0;
//            var amount = 0;
//            var amtte = 0;
//            var netamount = 0;
//            var subtotl = 0;
//            var catd = 0;
//            var ab = $('#tbltakeaway tbody').children().length;
//            if (ab == 0) {
//                $("#tbltakeaway").append('<tbody><tr id=' + itemvalue + '><td><input type="hidden" id="hiddentd" value=' + itemvalue + '></td><td><input type="text" class="form-control" id=' + itemvalue + ' value=' + itename + ' readonly/></td><td><input type="text" class="form-control" value=' + fulli + ' readonly/></td><td><input type="text" class="form-control" value=' + halfi + ' readonly/></td><td><input type="text" class="form-control" value=' + full + ' readonly/></td><td><input type="text" class="form-control" value=' + half + ' readonly/></td><td><input type="text" class="form-control" value=' + amount + ' readonly/></td><td><input type="text" class="form-control" value=' + vat + ' readonly/></td><td><input type="hidden" id="txtvatamount" value=' + vatamt + '></td><td><input type="button" class="btn green" value="Delete" name="btndelete" id=' + itemvalue + '></td></tr></tbody>');
//            }
//            else {

//                $('#tbltakeaway tbody tr').each(function () {
//                    var rowid = $(this).closest('tr').attr('id');
//                    var itemvalue = $("#hidenitemval").val();
//                    if (parseInt(rowid) == parseInt(itemvalue)) {
//                        catd = 1;
//                        return false;
//                    }
//                    else {
//                        catd = 0;
//                    }
//                    return;
//                });
//                if (parseInt(catd) == 0) {
//                    $("#tbltakeaway").append('<tr id=' + itemvalue + '><td><input type="hidden" id="hiddentd" value=' + itemvalue + '></td><td><input type="text" class="form-control" id=' + itemvalue + ' value=' + itename + ' readonly/></td><td><input type="text" class="form-control" value=' + fulli + ' readonly/></td><td><input type="text" class="form-control" value=' + halfi + ' readonly/></td><td><input type="text" class="form-control" value=' + full + ' readonly/></td><td><input type="text" class="form-control" value=' + half + ' readonly/></td><td><input type="text" class="form-control" value=' + amount + ' readonly/></td><td><input type="text" class="form-control" value=' + vat + ' readonly/></td><td><input type="hidden"  id="txtvatamount" value=' + vatamt + '></td><td><input type="button" class="btn green" value="Delete"  name="btndelete" id=' + itemvalue + '></td></tr>');
//                }
//            }
//            $('#tbltakeaway tbody tr').each(function () {

//                var row = $(this).closest('tr').attr('id');
//                if ($(this).find('td:eq(0) input').val() == row) {
//                    var itename = $("#hidenname").val();
//                    var itemvalue = $("#hidenitemval").val();
//                    var Quantity = $("#graphicquty").val();
//                    var radiocat = $('input[name=gradio]:checked').val();
//                    var amtt;
//                    if ($(this).find('td:eq(0) input').val() == itemvalue) {
//                        if (radiocat == "Full") {
//                            var fulli = $(this).find('td:eq(2) input').val();
//                            var halfi = $(this).find('td:eq(3) input').val();
//                            fulli = parseInt(fulli) + parseInt(Quantity);
//                            var ab = parseInt(fulli) * parseInt(full);
//                            var bb = parseInt(halfi) * parseInt(half);
//                            var vat = $(this).find('td:eq(7) input').val();
//                            var amount = ab + bb;
//                            var vatamt = ((parseInt(amount) * parseInt(vat)) / 100);
//                            $(this).closest('tr').remove();
//                            $("#tbltakeaway").append('<tr id=' + itemvalue + '><td><input type="hidden" id="hiddentd" value=' + itemvalue + '></td><td><input type="text" class="form-control" id=' + itemvalue + ' value=' + itename + ' readonly/></td><td><input type="text" class="form-control" value=' + fulli + ' readonly/></td><td><input type="text" class="form-control" value=' + halfi + ' readonly/></td><td><input type="text" class="form-control" value=' + full + ' readonly/></td><td><input type="text" class="form-control" value=' + half + ' readonly/></td><td><input type="text" class="form-control" value=' + amount + ' readonly/></td><td><input type="text" class="form-control" value=' + vat + ' readonly/></td><td><input type="hidden"  id="txtvatamount" value=' + vatamt + '></td><td><input type="button" class="btn green" value="Delete"  name="btndelete" id=' + itemvalue + '></td></tr>');
//                        }
//                        else {
//                            var halfi = $(this).find('td:eq(3) input').val();
//                            var fulli = $(this).find('td:eq(2) input').val();
//                            halfi = parseInt(halfi) + parseInt(Quantity);
//                            var ab = parseInt(fulli) * parseInt(full);
//                            var bb = parseInt(halfi) * parseInt(half);
//                            var vat = $(this).find('td:eq(7) input').val();
//                            var amount = ab + bb;
//                            var vatamt = ((parseInt(amount) * parseInt(vat)) / 100);
//                            $(this).closest('tr').remove();
//                            $("#tbltakeaway").append('<tr id=' + itemvalue + '><td><input type="hidden" id="hiddentd" value=' + itemvalue + '></td><td><input type="text" class="form-control" id=' + itemvalue + ' value=' + itename + ' readonly/></td><td><input type="text" class="form-control" value=' + fulli + ' readonly/></td><td><input type="text" class="form-control" value=' + halfi + ' readonly/></td><td><input type="text" class="form-control" value=' + full + ' readonly/></td><td><input type="text" class="form-control" value=' + half + ' readonly/></td><td><input type="text" class="form-control" value=' + amount + ' readonly/></td><td><input type="text" class="form-control" value=' + vat + ' readonly/></td><td><input type="hidden"  id="txtvatamount" value=' + vatamt + '></td><td><input type="button" class="btn green" value="Delete"  name="btndelete" id=' + itemvalue + '></td></tr>');
//                        }


//                    }

//                }


//            });
//            $('input[name=btndelete]').on('click', function () {
//                var id = $(this).attr('id');
//                $('#tbltakeaway tbody tr').each(function () {
//                    var loopid = $(this).find('td:eq(0) input').val();
//                    if (loopid == id) {
//                        var delvat = $(this).find('td:eq(8) input').val();
//                        var delamount = $(this).find('td:eq(6) input').val();

//                        var delvatamt = $('#txtvatamt').val();
//                        var vatreal = (parseFloat(delvatamt) - parseFloat(delvat));
//                        $('#txtvatamt').val(vatreal);

//                        var deltotamt = $("#txtsubtotal").val();
//                        var totreal = (parseFloat(deltotamt) - parseFloat(delamount));
//                        $("#txtsubtotal").val(totreal);

//                        var delservic = $("#txtservice").val();
//                        var servicreal = ((parseFloat(totreal) * 5) / 100);
//                        $("#txtservice").val(servicreal);

//                        var delnetamount = (parseFloat(vatreal) + parseFloat(totreal) + parseFloat(servicreal));
//                        $("#txtnettotal").val(delnetamount);
//                        $(this).closest('tr').remove();
//                    }
//                    else {
//                        return;
//                    }

//                });
//            });

//            $('#tbltakeaway tbody tr').each(function () {
//                var amtt = $(this).find('td:eq(8) input').val();
//                var total = $(this).find('td:eq(6) input').val();
//                subtotl = (parseFloat(total) + parseFloat(subtotl));
//                $("#txtsubtotal").val(subtotl);
//                amtte = (parseFloat(amtt) + parseFloat(amtte));
//                $("#txtvatamt").val(amtte);
//                var serviceamount = ((parseFloat(subtotl) * 5) / 100);
//                $("#txtservice").val(serviceamount);
//                netamount = (parseFloat(amtte) + parseFloat(subtotl) + parseFloat(serviceamount));
//                $("#txtnettotal").val(netamount);

//                var optype = $("#tkeawaytype").val();
//                if (optype == "Cashier") {
//                    $("#txtdiscount").prop('readonly', false);
//                    $("#txtdiscount").on('input', function () {
//                        var val = $("#txtdiscount").val();
//                        if (parseFloat(val) >= 0 && parseFloat(val) <= 20.00) {
//                            $("#msgerror").hide();
//                            var discount = $(this).val();
//                            var discountamt = ((parseFloat(netamount) * parseFloat(discount)) / 100);
//                            var discountnet = parseFloat(netamount) - parseFloat(discountamt);
//                            $("#txtnettotal").val(discountnet);
//                            $("#txtdiscountprice").val(discountamt);
//                        }
//                        else {
//                            $("#msgerror").show('slow');
//                        }
//                    });

//                }
//                else if (optype == "Manager") {
//                    $("#txtdiscount").prop('readonly', false);
//                    $("#txtdiscount").on('input', function () {
//                        var discount = $(this).val();
//                        var discountamt = ((parseFloat(netamount) * parseFloat(discount)) / 100);
//                        var discountnet = parseFloat(netamount) - parseFloat(discountamt);
//                        $("#txtnettotal").val(discountnet);
//                        $("#txtdiscountprice").val(discountamt);
//                    });

//                }
//            });

//        });

//    });


//    $("#btndispatch").on('click', function (e) {

//        var IsValid = true;
//        e.preventDefault();

//        var net = $("#txtnettotal").val();
//        if (net == " ") {
//            alert("Please Add Product in Billing..");
//        }
//        else {
//            var totalamount = $("#txtsubtotal").val();
//            var vatamount = $("#txtvatamt").val();
//            var servicamount = $("#txtservice").val();
//            var discountamount = $("#txtdiscountprice").val();
//            var netamount = $("#txtnettotal").val();
//            var custname = $("#txtcname").val();
//            var tokeno = $("#tokennotake").val();
//            var mainitems = totalamount + "^" + vatamount + "^" + servicamount + "^" + discountamount + "^" + netamount + "^" + tokeno + ";";
//            //alert(mainitems);
//            $("#takemasterdata").val(mainitems);
//            var item = "";

//            $("#tbltakeaway tbody tr").each(function () {
//                if ($(this).find('td:eq(0) input').val() != "") {
//                    $(this).find('td:eq(1) input').removeClass("errorSubmit");
//                    var ItemId = $(this).find('td:eq(0) input').val();
//                    item = item + ItemId + "^";

//                    var FullQty = $(this).find('td:eq(2) input').val();
//                    item = item + FullQty + "^";

//                    var HalfQty = $(this).find('td:eq(3) input').val();
//                    item = item + HalfQty + "^";

//                    var Amount = $(this).find('td:eq(6) input').val();
//                    item = item + Amount + ";";

//                }
//                else {
//                    $(this).find('td:eq(1) input').addClass("errorSubmit");
//                    IsValid = false;
//                }

//            });

//            if (IsValid == true) {
//                // alert(item);
//                item = item.substring(0, item.length - 1);
//                var items = { 'items': item };
//                $("#takedetailsitems").val(item);
//                //FormData();
//                $('#gbtnback').prop("disabled", false);
//                $("#txtdiscount").val("0");
//                $("#txtvatamt").val("0");
//                $("#txtservice").val("0");
//                $("#txtdiscountprice").val("0");
//                $("#txtsubtotal").val("");
//                $("#txtnettotal").val("");
//                $("#form_sample_18").submit();
//            }
//            else {
//                return false;
//            }


//        }
//    });
//});

//------------------------------jquery for Home Delivery Billing----------------------//

//$(document).ready(function () {
//    $(document).on("click", ".btnbillgadd", function () {
//        $("#tblhomedilivry").attr('hidden', false);
//        var itemvalue = $(this).attr("id");
//        $('#hiditemvalhdeliver').val(itemvalue);
//        var $td = $(this).parent('td').parent('tr').children('td');
//        var itename = $(this).val();
//        $('#hnamehdeliver').val(itename);
//        var radiocat = $('input[name=hradio]:checked').val();
//        $.getJSON('/Billing/bilprice/' + itemvalue, function (data) {
//            var arr = data.split('-');
//            var full = arr[0];
//            var half = arr[1];
//            var vat = arr[2];
//            var vatamt = 0;
//            var Quantity = $("#homedelqty").val();
//            var radiocat = $('input[name=hradio]:checked').val();
//            var fulli = 0;
//            var halfi = 0;
//            var amount = 0;
//            var amtte = 0;
//            var netamount = 0;
//            var subtotl = 0;
//            var catd = 0;
//            var ab = $('#tblhomedilivry tbody').children().length;
//            if (ab == 0) {
//                $("#tblhomedilivry").append('<tbody><tr id=' + itemvalue + '><td><input type="hidden" id="hiddentd" value=' + itemvalue + '></td><td><input type="text" class="form-control" id=' + itemvalue + ' value=' + itename + ' readonly/></td><td><input type="text" class="form-control" value=' + fulli + ' readonly/></td><td><input type="text" class="form-control" value=' + halfi + ' readonly/></td><td><input type="text" class="form-control" value=' + full + ' readonly/></td><td><input type="text" class="form-control" value=' + half + ' readonly/></td><td><input type="text" class="form-control" value=' + amount + ' readonly/></td><td><input type="text" class="form-control" value=' + vat + ' readonly/></td><td><input type="hidden" id="txtvatamount" value=' + vatamt + '></td><td><input type="button" class="btn green" value="Delete" name="btndelete" id=' + itemvalue + '></td></tr></tbody>');
//            }
//            else {

//                $('#tblhomedilivry tbody tr').each(function () {
//                    var rowid = $(this).closest('tr').attr('id');
//                    var itemvalue = $("#hiditemvalhdeliver").val();
//                    if (parseInt(rowid) == parseInt(itemvalue)) {
//                        catd = 1;
//                        return false;
//                    }
//                    else {
//                        catd = 0;
//                    }
//                    return;
//                });
//                if (parseInt(catd) == 0) {
//                    $("#tblhomedilivry").append('<tr id=' + itemvalue + '><td><input type="hidden" id="hiddentd" value=' + itemvalue + '></td><td><input type="text" class="form-control" id=' + itemvalue + ' value=' + itename + ' readonly/></td><td><input type="text" class="form-control" value=' + fulli + ' readonly/></td><td><input type="text" class="form-control" value=' + halfi + ' readonly/></td><td><input type="text" class="form-control" value=' + full + ' readonly/></td><td><input type="text" class="form-control" value=' + half + ' readonly/></td><td><input type="text" class="form-control" value=' + amount + ' readonly/></td><td><input type="text" class="form-control" value=' + vat + ' readonly/></td><td><input type="hidden"  id="txtvatamount" value=' + vatamt + '></td><td><input type="button" class="btn green" value="Delete"  name="btndelete" id=' + itemvalue + '></td></tr>');
//                }
//            }
//            $('#tblhomedilivry tbody tr').each(function () {

//                var row = $(this).closest('tr').attr('id');
//                if ($(this).find('td:eq(0) input').val() == row) {
//                    var itename = $("#hnamehdeliver").val();
//                    var itemvalue = $("#hiditemvalhdeliver").val();
//                    var Quantity = $("#homedelqty").val();
//                    var radiocat = $('input[name=hradio]:checked').val();
//                    var amtt;
//                    if ($(this).find('td:eq(0) input').val() == itemvalue) {
//                        if (radiocat == "Full") {
//                            var fulli = $(this).find('td:eq(2) input').val();
//                            var halfi = $(this).find('td:eq(3) input').val();
//                            fulli = parseInt(fulli) + parseInt(Quantity);
//                            var ab = parseInt(fulli) * parseInt(full);
//                            var bb = parseInt(halfi) * parseInt(half);
//                            var vat = $(this).find('td:eq(7) input').val();
//                            var amount = ab + bb;
//                            var vatamt = ((parseInt(amount) * parseInt(vat)) / 100);
//                            $(this).closest('tr').remove();
//                            $("#tblhomedilivry").append('<tr id=' + itemvalue + '><td><input type="hidden" id="hiddentd" value=' + itemvalue + '></td><td><input type="text" class="form-control" id=' + itemvalue + ' value=' + itename + ' readonly/></td><td><input type="text" class="form-control" value=' + fulli + ' readonly/></td><td><input type="text" class="form-control" value=' + halfi + ' readonly/></td><td><input type="text" class="form-control" value=' + full + ' readonly/></td><td><input type="text" class="form-control" value=' + half + ' readonly/></td><td><input type="text" class="form-control" value=' + amount + ' readonly/></td><td><input type="text" class="form-control" value=' + vat + ' readonly/></td><td><input type="hidden"  id="txtvatamount" value=' + vatamt + '></td><td><input type="button" class="btn green" value="Delete"  name="btndelete" id=' + itemvalue + '></td></tr>');
//                        }
//                        else {
//                            var halfi = $(this).find('td:eq(3) input').val();
//                            var fulli = $(this).find('td:eq(2) input').val();
//                            halfi = parseInt(halfi) + parseInt(Quantity);
//                            var ab = parseInt(fulli) * parseInt(full);
//                            var bb = parseInt(halfi) * parseInt(half);
//                            var vat = $(this).find('td:eq(7) input').val();
//                            var amount = ab + bb;
//                            var vatamt = ((parseInt(amount) * parseInt(vat)) / 100);
//                            $(this).closest('tr').remove();
//                            $("#tblhomedilivry").append('<tr id=' + itemvalue + '><td><input type="hidden" id="hiddentd" value=' + itemvalue + '></td><td><input type="text" class="form-control" id=' + itemvalue + ' value=' + itename + ' readonly/></td><td><input type="text" class="form-control" value=' + fulli + ' readonly/></td><td><input type="text" class="form-control" value=' + halfi + ' readonly/></td><td><input type="text" class="form-control" value=' + full + ' readonly/></td><td><input type="text" class="form-control" value=' + half + ' readonly/></td><td><input type="text" class="form-control" value=' + amount + ' readonly/></td><td><input type="text" class="form-control" value=' + vat + ' readonly/></td><td><input type="hidden"  id="txtvatamount" value=' + vatamt + '></td><td><input type="button" class="btn green" value="Delete"  name="btndelete" id=' + itemvalue + '></td></tr>');
//                        }


//                    }

//                }


//            });
//            $('input[name=btndelete]').on('click', function () {
//                var id = $(this).attr('id');
//                $('#tblhomedilivry tbody tr').each(function () {
//                    var loopid = $(this).find('td:eq(0) input').val();
//                    if (loopid == id) {
//                        var delvat = $(this).find('td:eq(8) input').val();
//                        var delamount = $(this).find('td:eq(6) input').val();

//                        var delvatamt = $('#htxtvatamt').val();
//                        var vatreal = (parseFloat(delvatamt) - parseFloat(delvat));
//                        $('#htxtvatamt').val(vatreal);

//                        var deltotamt = $("#htxtsubtotal").val();
//                        var totreal = (parseFloat(deltotamt) - parseFloat(delamount));
//                        $("#htxtsubtotal").val(totreal);

//                        var delservic = $("#htxtservice").val();
//                        var servicreal = ((parseFloat(totreal) * 5) / 100);
//                        $("#htxtservice").val(servicreal);

//                        var delnetamount = (parseFloat(vatreal) + parseFloat(totreal) + parseFloat(servicreal));
//                        $("#htxtnettotal").val(delnetamount);
//                        $(this).closest('tr').remove();
//                    }
//                    else {
//                        return;
//                    }

//                });
//            });

//            $('#tblhomedilivry tbody tr').each(function () {
//                var amtt = $(this).find('td:eq(8) input').val();
//                var total = $(this).find('td:eq(6) input').val();
//                subtotl = (parseFloat(total) + parseFloat(subtotl));
//                $("#htxtsubtotal").val(subtotl);
//                amtte = (parseFloat(amtt) + parseFloat(amtte));
//                $("#htxtvatamt").val(amtte);
//                var serviceamount = ((parseFloat(subtotl) * 5) / 100);
//                $("#htxtservice").val(serviceamount);
//                netamount = (parseFloat(amtte) + parseFloat(subtotl) + parseFloat(serviceamount));
//                $("#htxtnettotal").val(netamount);

//                var optype = $("#operatortype").val();
//                if (optype == "Cashier") {
//                    $("#htxtdiscount").prop('readonly', false);
//                    $("#htxtdiscount").on('input', function () {
//                        var val = $("#htxtdiscount").val();
//                        if (parseFloat(val) >= 0 && parseFloat(val) <= 20.00) {
//                            $("#msgerror").hide();
//                            var discount = $(this).val();
//                            var discountamt = ((parseFloat(netamount) * parseFloat(discount)) / 100);
//                            var discountnet = parseFloat(netamount) - parseFloat(discountamt);
//                            $("#htxtnettotal").val(discountnet);
//                            $("#htxtdiscountprice").val(discountamt);
//                        }
//                        else {
//                            $("#msgerror").show('slow');
//                        }
//                    });

//                }
//                else if (optype == "Manager") {
//                    $("#htxtdiscount").prop('readonly', false);
//                    $("#htxtdiscount").on('input', function () {
//                        var discount = $(this).val();
//                        var discountamt = ((parseFloat(netamount) * parseFloat(discount)) / 100);
//                        var discountnet = parseFloat(netamount) - parseFloat(discountamt);
//                        $("#htxtnettotal").val(discountnet);
//                        $("#htxtdiscountprice").val(discountamt);
//                    });

//                }
//            });
//        });


//    });


//    $("#hbtndispatch").on('click', function (e) {

//        var IsValid = true;
//        e.preventDefault();

//        var net = $("#htxtnettotal").val();
//        if (net == " ") {
//            alert("Please Add Product in Billing..");
//        }
//        else {
//            var totalamount = $("#htxtsubtotal").val();
//            var vatamount = $("#htxtvatamt").val();
//            var servicamount = $("#htxtservice").val();
//            var discountamount = $("#htxtdiscountprice").val();
//            var netamount = $("#htxtnettotal").val();
//            var custname = $("#txthdelivername").val();
//            var tokeno = $("#hometoken").val();
//            var address = $("#txthomeaddress").val();
//            var mainitems = totalamount + "^" + vatamount + "^" + servicamount + "^" + discountamount + "^" + netamount + "^" + tokeno + "^" + custname + "^" + address + ";";
//            // alert(mainitems);
//            $("#homemasterdata").val(mainitems);
//            var item = "";

//            $("#tblhomedilivry tbody tr").each(function () {
//                if ($(this).find('td:eq(0) input').val() != "") {
//                    $(this).find('td:eq(1) input').removeClass("errorSubmit");
//                    var ItemId = $(this).find('td:eq(0) input').val();
//                    item = item + ItemId + "^";

//                    var FullQty = $(this).find('td:eq(2) input').val();
//                    item = item + FullQty + "^";

//                    var HalfQty = $(this).find('td:eq(3) input').val();
//                    item = item + HalfQty + "^";

//                    var Amount = $(this).find('td:eq(6) input').val();
//                    item = item + Amount + ";";

//                }
//                else {
//                    $(this).find('td:eq(1) input').addClass("errorSubmit");
//                    IsValid = false;
//                }

//            });

//            if (IsValid == true) {
//                // alert(item);
//                item = item.substring(0, item.length - 1);
//                var items = { 'items': item };
//                $("#homedetailsitems").val(item);
//                //FormData();
//                $('#gbtnback').prop("disabled", false);
//                $("#htxtdiscount").val("0");
//                $("#htxtvatamt").val("0");
//                $("#htxtservice").val("0");
//                $("#htxtdiscountprice").val("0");
//                $("#htxtsubtotal").val("");
//                $("#htxtnettotal").val("");
//                $("#form_sample_19").submit();
//            }
//            else {
//                return false;
//            }


//        }
//    });
//});