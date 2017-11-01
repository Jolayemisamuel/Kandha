//----------------------------------Billing Creation jquery---------------------//
$(document).ready(function () {
   
    $(".BillingCategory").click(function () {
        var bilcatid = $(this).attr("id");
    $.getJSON('/Billing/Billingitms/' + bilcatid, function (data) {
        $("#tblbilling").html(data);
        $("#btnbillgadd").click(function () {
            $(".billingcheckbox").on('click', function () {
                if ($(this).prop("checked") == true) {
                    id = $(this).val();
                }
                $.getJSON('/Billing/bilprice/' + id, function (data) {
                    var itepric = data.split('-');
                    var full = itepric[0];
                    var half = itepric[1];
                    alert(half);
                    alert(full);
                });
            });
        });
    });
    });
    $(".tabltd").click(function () {
        var tableno = $(this).val();
        alert(tableno);
    });
  
});
