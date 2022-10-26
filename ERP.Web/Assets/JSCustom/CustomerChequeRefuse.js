"use strict";

var CustomerChequeRefuse_Module = function () {
    function ChequeRefuse(btn) {
        try {
            var form = document.getElementById('form1');
            $.ajax({
                type: 'POST',
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $(btn).css('pointer-events', 'none'); // disabled a link after one clicke 
                        toastr.success(res.message, '',)
                        setTimeout(function () { window.location = "/CustomerCheques/Index" }, 3000);
                    } else {
                        toastr.error(res.message, '');
                    }
                },
                error: function (err) {
                    toastr.error('حدث خطأ اثناء تنفيذ العملية', '');
                    console.log(err)
                }
            })
            //to prevent default form submit event
            return false;
        } catch (ex) {
            console.log(ex)
        }

    }

    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        ChequeRefuse: ChequeRefuse
    };

}();


