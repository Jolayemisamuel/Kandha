$('#ddl_split').on('change', function () {
    var selected = $(this).val();
    if(selected=="Manually")
    {
        $('#Manual').show();
        $('#Divide').hide('fast');
    }
    else
    {
        $('#Manual').hide('fast');
        $('#Divide').show();
    }
});
var counter = 1;
$(document).on('change', '#Manul_No_of_Persion', function () {
    var val = $(this).val();
    var element = "";
    for (var i = 0; i < val; i++) {
        element += "<div class='form-group'><label class='col-md-2'>Name</label><div class='col-md-4'><input type='text' class='alphabte form-control' /></div><label class='col-md-2'>Amount</label><div class='col-md-4'><input type='text' class='number form-control'id='txt_" + counter + "' /></div></div>";
        counter++
    }
    $('#manul_content').html(element);
});
$(document).on('change', '#Divide_No_of_Persion', function () {
    var val = $(this).val();
    var NetAmount = $('#txtnettotal').val();
    var splitamt = parseFloat(NetAmount) / parseInt(val);
    var splitamount = splitamt.toFixed(2);
    var element = "";
    for (var i = 0; i < val; i++) {
        element += "<div class='form-group'><label class='col-md-2'>Name</label><div class='col-md-4'><input type='text' class='alphabte form-control' /></div><label class='col-md-2'>Amount</label><div class='col-md-4'><input type='text' class='number form-control' readonly value='" + splitamount + "'/></div></div>"
    }
    $('#divide_content').html(element);
});
$('#Tabsplit').on('click', function () {
    var TableNo = $('#RunningTable').val();
    $('#SplitTableNo').text(TableNo);
    var NetAmount = $('#txtnettotal').val();
    $('#SplitTotal').text(NetAmount);
    var multidropdown = "";
    //multidropdown += '<select multiple="multiple" size="10" name="duallistbox_demo1[]">';
                                                                            
                                                                       
    $('#BindXmlData table tbody tr').each(function () {
        var itemName = $(this).find('td:eq(1)').text();
        multidropdown += '  <option value="option1">'+itemName+'</option>'
        //multidropdown += ' <li class="ms-elem-selectable" id="-1890721249-selectable" style="display: list-item;"><span>New York Giants</span></li>';
    });
    $('select[name="my_multi_select1[]"]').html(multidropdown);
    $('#my_multi_select1').multiSelect();
});
var lessamount="";
var subval="";

$(document).on('change', '.number', function () {
    var NetAmount = $('#txtnettotal').val();
    var no = $('#Manul_No_of_Persion').val();
    var val = $(this).val();
    var id = $(this).attr('id');
    var el = id.split("_");
    var next_txt = parseInt(el[1]) + parseInt(1);
    if (subval != "")
    {
        subval = parseFloat(subval) - parseFloat(val);
        subval = subval.toFixed(2);
    }
    else
    {
        subval = parseFloat(NetAmount) - parseFloat(val);
        subval = subval.toFixed(2);
    }
    $('#txt_' + next_txt).val(subval);
    if (next_txt == no)
    {
        $('#txt_' + next_txt).attr('disabled', true);
    }
   

});
$(document).on('click', '#btn_split', function (event) {
    event.preventDefault();
    var NetAmount = $('#txtnettotal').val();
    var TableNo = $('#SplitTableNo').text();
    var Type = $('#ddl_split').val();
    var data = "";
    var amt=""
    var Number_of_persion
    if (Type == "Manually")
    {
        Number_of_persion = $('#Manul_No_of_Persion').val();
       
        $('#manul_content .form-group').each(function () {
            var name = $(this).find('input[type="text"].alphabte').val();
            var amount = $(this).find('input[type="text"].number').val();
            data += name + "," + amount + "^";
            if (amt == "")
            {
                amt = parseFloat(amount);
            }
            else
            {
                amt = parseFloat(amt) + parseFloat(amount);
            }
           
        });
    }
    else
    {
        Number_of_persion = $('#Divide_No_of_Persion').val();
        $('#divide_content .form-group').each(function () {
            var name = $(this).find('input[type="text"].alphabte').val();
            var amount = $(this).find('input[type="text"].number').val();
            data += name + "," + amount + "^";
            if (amt == "") {
                amt = parseFloat(amount);
            }
            else {
                amt = parseFloat(amt) + parseFloat(amount);
            }
        });
    }
    if (parseFloat(NetAmount) != parseFloat(amt))
    {
        alert('amount of all textboxs are not equal please enter correct amount');
        return false;
    }
    var FormData = { TableNo: TableNo, Type: Type, NoOfPersion: Number_of_persion, Detail: data };
    $.post("/Split/SaveSplitData", FormData, function (result) {
        $('#Manual').fadeOut();
        $('#manul_content').html("");
        $('#Divide').fadeOut();
        $('#divide_content').html("");
        $('#Manul_No_of_Persion').val("");
        $('#Divide_No_of_Persion').val("");
        $('.modal').modal('hide');
    });
   
   
});