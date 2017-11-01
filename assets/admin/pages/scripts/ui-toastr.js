var UIToastr = function () {

    return {
        //main function to initiate the module
        init: function () {

            var i = -1,
                toastCount = 0,
                $toastlast,
                getMessage = function () {
                    var msgs = ['Hello, some notification sample goes here',
                        '<div><input class="form-control input-small" value="textbox"/>&nbsp;<a href="http://themeforest.net/item/metronic-responsive-admin-dashboard-template/4021469?ref=keenthemes" target="_blank">Check this out</a></div><div><button type="button" id="okBtn" class="btn blue">Close me</button><button type="button" id="surpriseBtn" class="btn default" style="margin: 0 8px 0 8px">Surprise me</button></div>',
                        'Did you like this one ? :)',
                        'Totally Awesome!!!',
                        'Yeah, this is the Metronic!',
                        'Explore the power of Metronic. Purchase it now!'
                    ];
                    i++;
                    if (i === msgs.length) {
                        i = 0;
                    }

                    return msgs[i];
                };
            function GetToster()
            {
                var shortCutFunction = 'error';
                var msg = 'Please Select Table No'
                var title = 'Error Notification';
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

                toastr.options.timeOut = 1000;

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
            //$('#showtoast').click(function () {
            //    var shortCutFunction = 'error';
            //    var msg = 'Please Select Table No'
            //    var title = 'Error Notification';
            //    //var $showDuration = $('#showDuration');
            //    //var $hideDuration = $('#hideDuration');
            //    //var $timeOut = $('#timeOut');
            //    //var $extendedTimeOut = $('#extendedTimeOut');
            //    //var $showEasing = $('#showEasing');
            //    //var $hideEasing = $('#hideEasing');
            //    //var $showMethod = $('#showMethod');
            //    //var $hideMethod = $('#hideMethod');
            //    var toastIndex = toastCount++;

            //    toastr.options = {
            //        closeButton: $('#closeButton').prop('checked'),
            //        debug: $('#debugInfo').prop('checked'),
            //        positionClass: $('#positionGroup input:checked').val() || 'toast-top-right',
            //        onclick: null
            //    };

            //    if ($('#addBehaviorOnToastClick').prop('checked')) {
            //        toastr.options.onclick = function () {
            //            alert('You can perform some custom action after a toast goes away');
            //        };
            //    }

               
            //        toastr.options.showDuration = 1000;
                
            //        toastr.options.hideDuration = 1000;
               
            //        toastr.options.timeOut =1000;
                
            //        toastr.options.extendedTimeOut = 1000;
                
            //        toastr.options.showEasing = 'swing';
                
            //        toastr.options.hideEasing = 'linear';
                
            //        toastr.options.showMethod = 'fadeIn';
               
            //        toastr.options.hideMethod = 'fadeOut';
            //    if (!msg) {
            //        msg = getMessage();
            //    }

            //    $("#toastrOptions").text("Command: toastr[" + shortCutFunction + "](\"" + msg + (title ? "\", \"" + title : '') + "\")\n\ntoastr.options = " + JSON.stringify(toastr.options, null, 2));

            //    var $toast = toastr[shortCutFunction](msg, title); // Wire up an event handler to a button in the toast, if it exists
            //    $toastlast = $toast;
            //    if ($toast.find('#okBtn').length) {
            //        $toast.delegate('#okBtn', 'click', function () {
            //            alert('you clicked me. i was toast #' + toastIndex + '. goodbye!');
            //            $toast.remove();
            //        });
            //    }
            //    if ($toast.find('#surpriseBtn').length) {
            //        $toast.delegate('#surpriseBtn', 'click', function () {
            //            alert('Surprise! you clicked me. i was toast #' + toastIndex + '. You could perform an action here.');
            //        });
            //    }

            //    $('#clearlasttoast').click(function () {
            //        toastr.clear($toastlast);
            //    });
            //});
            //$('#cleartoasts').click(function () {
            //    toastr.clear();
            //});

        }

    };

}();