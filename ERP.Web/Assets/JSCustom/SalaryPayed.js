"use strict";

var SalaryPayed_Module = function () {
    var initDT = function () {
        var table = $('#kt_datatable');
        // begin first table
        table.DataTable({
            paging: false,
            info: false,
            search: false,
            searching: false,
            "order": [[0, "asc"]],
            // DOM Layout settings
            dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",
            language: {
                processing: "جارى التحميل",
                zeroRecords: "لا يوجد سجلات لعرضها",
                infoFiltered: "",
                infoEmpty: 'لا يوجد سجلات متاحه',
                oPaginate: {
                    sNext: '<span class="pagination-default">التالى</span><span class="pagination-fa"><i class="fa fa-chevron-left" ></i></span>',
                    sPrevious: '<span class="pagination-default">السابق</span><span class="pagination-fa"><i class="fa fa-chevron-right" ></i></span>'
                }
            },
                });
            };


    function SalaryPaid(id,txt) {
        var remind = $('#remindId' + id).val();
        var payedValue = $("#payedId"+id).val();
        var dtPayed = $("#dtPayed").val();
        var safeId = $("#SafeId").val();
        var bankAccountId = $("#BankAccountId").val();

        Swal.fire({
            title: 'تأكيد الصرف',
            text: 'هل متأكد من صرف الراتب ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/SalaryPayed/SalaryPaid';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "id": id,
                        "payedValue": payedValue,
                        "remindValue": remind,
                        "dtPayed": dtPayed,
                        "safeId": safeId,
                        "bankAccountId": bankAccountId,
                    },
                    //async: true,
                    //headers: { 'RequestVerificationToken': $('@Html.AntiForgeryToken()').val() },
                    success: function (data) {
                        if (data.isValid) {
                            toastr.success(data.message,'');
                            setTimeout(function () { window.location = "/SalaryPayed/Index" }, 3000);
                        } else {
                            toastr.error(data.message, '');
                        }
                    },
                    error: function (err) {
                        alert(err);
                    }
                });
            }
        });
    };
    function Calculation(id) {
        var payed = $('#payedId' + id).val();
        console.log(payed);
        var safy = $('#safyId' + id).text();
        console.log(safy);

        if (payed > Number.parseFloat(safy)) {
            toastr.error('المدفوع اكبر من الصافى', '');
            return false;
        }
        $('#remindId' + id).val(Math.round(safy - payed,2));
    };
    function getSafesOnBranchChanged() {  // get safes and stores by branchId
        $.get("/SharedDataSources/getSafesOnBranchChanged", { id: $("#BranchId").val() }, function (data) {
            $("#SafeId").empty();
            $("#SafeId").append("<option value=>اختر عنصر من القائمة </option>");
            $.each(data, function (index, row) {
                $("#SafeId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            })
        });
    };
    function onRdoSafeChanged() {
        if ($("#rdo_safe:checked").val()) {
            $('#SafeId').removeAttr('disabled');
            $('#BankAccountId').attr('disabled', 'disabled');
            $('#BankAccountId').val(null);
        }
    }
    function onRdoBankChanged() {
        if ($("#rdo_bank:checked").val()) {
            $('#BankAccountId').removeAttr('disabled');
            $('#SafeId').attr('disabled', 'disabled');
            $('#SafeId').val(null);
        }
    };

    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        SalaryPaid: SalaryPaid,
        getSafesOnBranchChanged: getSafesOnBranchChanged,
        onRdoSafeChanged: onRdoSafeChanged,
        onRdoBankChanged: onRdoBankChanged,
        Calculation: Calculation,
    };

}();


