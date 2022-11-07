"use strict";

var ItemPriceSupplier_Module = function () {

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
                oPaginate: {
                    sNext: '<span class="pagination-default">التالى</span><span class="pagination-fa"><i class="fa fa-chevron-left" ></i></span>',
                    sPrevious: '<span class="pagination-default">السابق</span><span class="pagination-fa"><i class="fa fa-chevron-right" ></i></span>'
                }
            },
            "order": [[0, "asc"]]
            //"order": [[0, "desc"]] 

        });
    };


  
    //#endregion ========= end Step 2 ==========

    //العملاء بدلالة فئة العملاء
    function getSupplierOnCategoryChange() {
        $.get("/SharedDataSources/GetSupplierOnCategoryChange/", { id: $("#PersonCategoryId").val(), saleMenId: null }, function (data) {
            $("#SupplierId").empty();
            $("#SupplierId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#SupplierId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
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
            text: 'هل متأكد من اعتماد تسجيل سياسة الاسعار لجميع الاصناف للمورد ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var PricingPolicyId = $("#PricingPolicyId option:selected").val();
                var supplierId = $("#SupplierId option:selected").val();
                var url = '/ItemPriceSuppliers/AddItemPriceSupplier';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "PricingPolicyId": PricingPolicyId,
                        "SupplierId": supplierId,
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
                            setTimeout(function () { window.location = "/ItemPriceSuppliers/CreateEdit" }, 3000);
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
        getSupplierOnCategoryChange: getSupplierOnCategoryChange,
        SubmitForm: SubmitForm,
    };

}();

