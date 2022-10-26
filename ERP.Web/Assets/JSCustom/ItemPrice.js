"use strict";

var ItemPrice_Module = function () {

    //#region ======== Save Purchase invoice ==================
    var initDTItemDetails = function () {
        var table = $('#kt_datatableTreePrice');

        // begin first table
        table.DataTable({
            paging: false,
            info: false,
            search: false,
            searching: false,
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

    //العملاء بدلالة فئة العملاء
    function getCustomerOnCategoryChange() {
        $.get("/SharedDataSources/GetCustomerOnCategoryChange/", { id: $("#PersonCategoryId").val(), saleMenId: null }, function (data) {
            $("#CustomerId").empty();
            $("#CustomerId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#CustomerId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });
        });
    };
    function SubmitForm(btn) { // اضافة
        var values = new Array();
        $.each($(".selectedDay"), function () {
            var data = $(this).parents('tr:eq(0)');
            values.push({ 'ItemPriceId': $(data).find('td:eq(0)').text(), 'ItemId': $(data).find('td:eq(1)').text(), 'ItemName': $(data).find('td:eq(2)').text(), 'SellPrice': $(data).find('td:eq(3)').text(), 'SellPriceCustome': $(data).find('td:eq(4) input[type="text"]').val() });
        });
        console.log(values);
        console.log(values.length);

        Swal.fire({
            title: 'تأكيد الاعتماد',
            text: 'هل متأكد من اعتماد تسجيل سياسة الاسعار لجميع الاصناف للعميل ؟',
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
                var PricingPolicyId = $("#PricingPolicyId option:selected").val();
                var CustomerId = $("#CustomerId option:selected").val();
                var url = '/ItemPrices/AddItemPriceCustomer';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "PricingPolicyId": PricingPolicyId,
                        "CustomerId": CustomerId,
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
                            setTimeout(function () { window.location = "/ItemPrices/CreateEdit" }, 3000);
                        } else {
                                toastr.error(data.message, '');
                        }

                    },
                    error: function (err) {
                        alert(0);
                        alert(err);
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
        getCustomerOnCategoryChange: getCustomerOnCategoryChange,
        SubmitForm: SubmitForm,
    };

}();

