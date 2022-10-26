"use strict";

var ProductionOrderDamage_Module = function () {

    //#region ======== damages ==================
    var initDTItemMatertials = function () {
        var table = $('#kt_dtItemMatertials');

        // begin first table
        table.DataTable({
            paging: false,
            info: false,
            search: false,
            searching:false,
            "order": [[0, "asc"]]
            //"order": [[0, "desc"]] 

        });
    };

    function RegisterDamages() { // تسجيل التوالف

        //var data = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
        var values = new Array();
        $.each($(".selectedRow"), function () {
            var data = $(this).parents('tr:eq(0)');
            values.push({ 'Id': $(data).find('td:eq(0)').text(), 'ItemId': $(data).find('td:eq(1)').text(), 'ItemName': $(data).find('td:eq(2)').text(), 'ItemCost': $(data).find('td:eq(3)').text(), 'StoreName': $(data).find('td:eq(4)').text(), 'Quantity': $(data).find('td:eq(5)').text(), 'Quantitydamage': $(data).find('td:eq(6) input[type="text"]').val(), 'StoreId': $(data).find('td:eq(7)').text() });
        });
        console.log(values);
        console.log(values.length);

        Swal.fire({
            title: 'تأكيد تسجيل التوالف',
            text: 'هل متأكد من تسجيل التوالف ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                const params = new URLSearchParams(window.location.search)
                var queryString = '';
                if (params.has('ordrGud')) {
                    queryString = params.get('ordrGud');
                } else
                    document.location.replace("ProductionOrders/Index")

                var url = '/ProductionOrderDamages/RegisterDamages';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "ordrGud": queryString,
                        "data": JSON.stringify(values)
                    },
                    //async: true,
                    //headers: { 'RequestVerificationToken': $('@Html.AntiForgeryToken()').val() },
                    success: function (data) {
                        if (data.isValid) {
                            toastr.success(data.message, '');
                            setTimeout(function () { window.location = "/ProductionOrders/Index" }, 3000);
                        } else
                            toastr.error(data.message, '');
                        
                    },
                    error: function (err) {
                        alert(err);
                    }
                });
            }
        });
    };
  
    //#endregion ========= end Step 2 ==========


    return {
        //main function to initiate the module
        init: function () {
            initDTItemMatertials();
        },
        RegisterDamages: RegisterDamages
    };

}();

