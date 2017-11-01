var LockValidation = function () {

    var handleLogin = function () {

        $('.form-inline').validate({
            errorElement: 'span', //default input error message container
            errorClass: 'help-block', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            rules: {
                
                Password: {
                    required: true
                }
            },

            messages: {
                Password: {
                    required: "Password is required."
                }
            },

            invalidHandler: function (event, validator) { //display error alert on form submit   
                $('.alert-danger', $('.form-inline')).show();
            },

            highlight: function (element) { // hightlight error inputs
                $(element)
                    .closest('.form-group').addClass('has-error'); // set error class to the control group
            },

            success: function (label) {
                label.closest('.form-group').removeClass('has-error');
                label.remove();
            },

            errorPlacement: function (error, element) {
                error.insertAfter(element.closest('.input-icon'));
            },

            submitHandler: function (form) {
                form.submit(); // form validation success, call ajax form submit
            }
        });

        $('.form-inline input').keypress(function (e) {
            if (e.which == 13) {
                if ($('.form-inline').validate().form()) {
                    $('.form-inline').submit(); //form validation success, call ajax form submit
                }
                return false;
            }
        });
    }

    

    return {
        //main function to initiate the module
        init: function () {
            handleLogin();
        }
    };

}();