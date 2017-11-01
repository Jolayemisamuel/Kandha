
//=====happy hours change====//
$(document).on('change', '#happy_hours_type', function () {
    var val = $(this).val();
    $('.happy_hours_show').hide();
    $('#Div_' + val).show();
    $('#Div_' + val).css('opacity', '1');
    $('.HHOfferType_Id').val(val);
});
$(document).on('change', '#From_happy_Days', function () {
    var val = $(this).val();
    $("select#To_happy_Days option").prop('disabled', false);
    $("select#To_happy_Days option[value*='" + val + "']").prop('disabled', true);
});
$(document).on('change', '', function () {
    var val=$(this).val();
});

$(document).on('change', '#UserId', function () {
    var val = $(this).val();
    var text = $("#UserId option:selected").text();
   
    var FormData = { Id: val };
    $.post("/AssignOffer/getOffer", FormData, function (data) {
        $('#User_Name').html(text);
        $('.add_class').attr('id', val);
        $('#Offer_Show').show();
        $('#offerData').html(data);
        var table = $('#'+val);

        table.dataTable({
            "lengthMenu": [
                [5, 15, 20, -1],
                [5, 15, 20, "All"] // change per page values here
            ],
            // set the initial value
            "pageLength": 5,
            "language": {
                "lengthMenu": " _MENU_ records",
                "paging": {
                    "previous": "Prev",
                    "next": "Next"
                }
            },
            "columnDefs": [{  // set default column settings
                'orderable': false,
                'targets': [0]
            }, {
                "searchable": false,
                "targets": [0]
            }],
            "order": [
                [1, "asc"]
            ] // set first column as a default sort by asc
        });

        var tableWrapper = jQuery('#sample_2_wrapper');

        table.find('.group-checkable').change(function () {
            var set = jQuery(this).attr("data-set");
            var checked = jQuery(this).is(":checked");
            jQuery(set).each(function () {
                if (checked) {
                    $(this).attr("checked", true);
                } else {
                    $(this).attr("checked", false);
                }
            });
            jQuery.uniform.update(set);
        });

        tableWrapper.find('.dataTables_length select').select2(); // initialize select2 dropdown
        //$('select[name="my_multi_select2[]"]').html(data);
        //$('#my_multi_select2').multiSelect({
        //    selectableOptgroup: true
        //});
    });
});

//$(document).on('click')