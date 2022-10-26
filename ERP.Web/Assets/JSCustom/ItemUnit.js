"use strict";

var ItemUnit_Module = function () {

    //#region ======== Save Purchase invoice ==================
    var initDTItemDetails = function () {
        var table = $('#kt_datatableTreePrice');

        // begin first table
        table.DataTable({
            paging: false,
            //info: false,
            //search: false,
            //searching: false,
            //responsive: true,
            searchDelay: 500,
            processing: true,
            serverSide: false,
            select: true,

            // DOM Layout settings
            dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",

            language: {
                lengthMenu: "عرض _MENU_ عنصر لكل صفحة",
                info: "العناصر من_START_ الي _END_ من اصل _TOTAL_ عنصر",
                processing: "جارى التحميل",
                zeroRecords: "لا يوجد سجلات لعرضها",
                infoFiltered: "",
                infoEmpty: 'لا يوجد سجلات متاحه',
            },
            "order": [[0, "asc"]]
            //"order": [[0, "desc"]] 

        });
    };


  
    //#endregion ========= end Step 2 ==========

    function SubmitForm(btn) { // اضافة
        var values = new Array();
        $.each($(".selectedDay"), function () {
            var data = $(this).parents('tr:eq(0)');
            values.push({ 'ItemUnitId': $(data).find('td:eq(0)').text(), 'ItemId': $(data).find('td:eq(1)').text(), 'ItemUnitBase': $(data).find('td:eq(2)').text(), 'ItemName': $(data).find('td:eq(3)').text(), 'SellPriceCustome': $(data).find('td:eq(4) input[type="text"]').val(), 'Quantity': $(data).find('td:eq(5) input[type="text"]').val() });
        });
        console.log(values);
        console.log(values.length);

        Swal.fire({
            title: 'تأكيد الحفظ',
            text: 'هل متأكد من تسجيل واحدات لجميع الاصناف  ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                //const params = new URLSearchParams(window.location.search)
                //var queryString = '';
                //if (params.has('invoGuid')) {
                //    queryString = params.get('invoGuid');
                //} else
                //    document.location.replace("PurchaseInvoiceStores/Index")
                //var form = document.getElementById('form1');
                //var formData = new FormData();
                //formData.append('PricingPolicyId', $("#PricingPolicyId option:selected").val());
                //formData.append('CustomerId', $("#CustomerId option:selected").val());
                //formData.append('data', JSON.stringify(values));
                var UnitId = $("#UnitId option:selected").val();
                var url = '/ItemUnits/AddItemUnit';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "UnitId": UnitId,
                        "data": JSON.stringify(values)
                    },
                    //data: formData,
                    //async: true,
                    //headers: { 'RequestVerificationToken': $('@Html.AntiForgeryToken()').val() },
                    success: function (data) {
                        console.log(data);
                        if (data.isValid) {
                            $(btn).css('pointer-events', 'none'); // disabled a link after one clicke 
                            toastr.success(data.message, '');
                            setTimeout(function () { window.location = "/ItemUnits/CreateEdit" }, 3000);
                        } else {
                                toastr.error(data.message, '');
                        }

                    },
                    error: function (err) {
                    //    alert(0);
                    //    alert(err);
                    }
                });
            }
        });
    };

    return {
        //main function to initiate the module

        initItemDetails: function () {
            initDTItemDetails();
        },
        SubmitForm: SubmitForm,
    };

}();

