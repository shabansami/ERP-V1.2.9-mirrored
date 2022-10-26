"use strict";
var MaintenancesAccountant_Module = function () {

    //#region ======== Save Sell back invoice ==================
    var initDT = function () {
        var table = $('#kt_datatable');

        // begin first table
        table.DataTable({
            responsive: true,
            searchDelay: 500,
            processing: true,
            serverSide: false,
            select: true,

            // DOM Layout settings
            dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",
            buttons: [
                {
                    extend: 'print',
                    title: function () {
                        return 'الاعتماد المحاسبى فواتير الصيانة';
                    },
                    customize: function (win) {
                        $(win.document.body)
                            //.css('font-size', '20pt')
                            .prepend(
                                '<img src=' + localStorage.getItem("logo") + ' style="position:absolute; top:25%; right:10%;opacity: 0.2;" />'
                            );
                        $(win.document.body).find('table')
                            //.addClass('compact')
                            .css('font-size', 'inherit')
                            .css('direction', 'rtl')
                            .css('text-align', 'right')
                            .find('.actions').css('display', 'none');
                        //توسيط عنوان التقرير
                        $(win.document.body).find('h1').css('text-align', 'center');
                    },
                    exportOptions: {
                        columns: ':visible'
                    }
                },
                {
                    extend: "copyHtml5",
                    exportOptions: {
                        columns: ':visible'
                    }
                },
                {
                    extend: "excelHtml5",
                    filename: "الاعتماد المحاسبى فواتير الصيانة",
                    title: "الاعتماد المحاسبى فواتير الصيانة",
                    exportOptions: {
                        columns: ':visible'
                    }
                },
            ],
            language: {
                search: "البحث",
                lengthMenu: "عرض _MENU_ عنصر لكل صفحة",
                info: "العناصر من_START_ الي _END_ من اصل _TOTAL_ عنصر",
                processing: "جارى التحميل",
                zeroRecords: "لا يوجد سجلات لعرضها",
                infoFiltered: "",
                infoEmpty: 'لا يوجد سجلات متاحه',
            },

            ajax: {
                url: '/MaintenancesAccountants/GetAll',
                type: 'GET',

            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'InvoiceNum', title: 'رقم الفاتورة' },
                { data: 'InvoiceDate', title: 'تاريخ الفاتورة' },
                { data: 'CustomerName', title: 'اسم العميل' },
                { data: 'CaseName', title: 'اخر حالة للفاتورة' },
                { data: 'InvoType', title: 'حالة الفاتورة', visible: false },
                { data: 'InvoAccountant', title: 'حالة الاعتماد', visible: false },
                { data: 'Actions', responsivePriority: -1, className: 'actions' },
            ],
            columnDefs: [
                {
                    targets: 0,
                    title: 'م',
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    },
                },

                {
                    //         		< a href = "/MaintenancesAccountants/Edit/?invoGuid='+ row.InvoiceGuid + '" class= "btn btn-sm btn-clean btn-icon" title = "تعديل" >\
                    //         <i class="fa fa-edit"></i>\
                    //</a >
                    targets: -1,
                    title: 'عمليات',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        if (row.IsApprovalAccountant) {
                            return '\
							<div class="btn-group">\
							<a href="/MaintenancesAccountants/ShowMaintenance/?invoGuid='+ row.InvoiceGuid + '" class="btn btn-sm btn-clean btn-icon" title="عرض فاتورة">\
								<i class="fa fa-search"></i>\
							</a>\
                            <span class="label label-lg font-weight-bold label-light-success label-inline">معتمده</span></div>\
						';
                        } else {
                            return '\
							<div class="btn-group">\
							<a href="/MaintenancesAccountants/ApprovalAccountant/?invoGuid='+ row.InvoiceGuid + '" class="btn btn-sm btn-clean btn-icon" title="اعتماد محاسبى">\
								<i class="fa fa-check"></i>\
							</a>\<a href="/MaintenancesAccountants/ShowMaintenance/?invoGuid='+ row.InvoiceGuid + '" class="btn btn-sm btn-clean btn-icon" title="عرض فاتورة">\
								<i class="fa fa-search"></i>\
							</a>\
                            </div>\
						';
                        }

                    },
                }

            ],

            "order": [[0, "desc"]]
            //"order": [[0, "desc"]] 

        });
        $('#export_print').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatable').DataTable().button('.buttons-print').trigger();
        });

        $('#export_copy').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatable').DataTable().button('.buttons-copy').trigger();
        });

        $('#export_excel').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatable').DataTable().button('.buttons-excel').trigger();
        });
    };


    //#endregion ============== end ==============

    //#region تسليم فاتورة صيانة 
    function AccountantInvo() {
        if ($("#PaymentTypeId").val() === '' || $("#PaymentTypeId").val() === null) {
            toastr.error('تأكد من اختيار طريقة السداد من شاشة ادارة فاتورة صيانة', '');
            return false;
        }
        if ($("#Safy").val() === '' || $("#Safy").val() === null || $("#Safy").val() === '0') {
            toastr.error('تأكد من ادخال المبلغ المدفوع فى شاشة ادارة فاتورة صيانة', '');
            return false;
        }
        var form = document.getElementById('form1');
        var formData = new FormData(form);
        formData.append('receiptStatus', $("#cmbo_ReceiptStatus").val());
        try {
            $.ajax({
                type: 'POST',
                url: form.action,
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        toastr.success(res.message, '',)
                        setTimeout(function () { window.location = "/MaintenancesAccountants/Index" }, 3000);

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

    };

    function onPaymentTypeChanged() {
        if ($("#PaymentTypeId").val() == "1" || $("#PaymentTypeId").val() == "3") {
            $("#rdo_safe,#rdo_bank").attr("disabled", false);

        } else {
            $("#rdo_safe,#rdo_bank").prop('checked', false);
            $("#rdo_safe,#rdo_bank").attr("disabled", true);
            $("#divSafe").hide();
            $("#divBank").hide();
        }
    };
    function onRdoSafeChanged() {
        if ($("#rdo_safe:checked").val()) {
            $("#divSafe").show();
            $('#SafeId').removeAttr('disabled');
            $('#BankAccountId').attr('disabled', 'disabled');
            $("#divBank").hide();
        }
    }
    function onRdoBankChanged() {
        if ($("#rdo_bank:checked").val()) {
            $("#divSafe").hide();
            $('#BankAccountId').removeAttr('disabled');
            $('#SafeId').attr('disabled', 'disabled');
            $("#divBank").show();
        }
    };

    function onReceiptStatusChange() {
        if ($('#cmbo_ReceiptStatus option:selected').val() === "2") {
            $('#divStore').show();
        } else {
            $('#divStore').hide();
            $('#StoreReceiptId').val(null);
        }

    };
    //#endregion
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        AccountantInvo: AccountantInvo,
        onPaymentTypeChanged: onPaymentTypeChanged,
        onRdoSafeChanged: onRdoSafeChanged,
        onRdoBankChanged: onRdoBankChanged,
        onReceiptStatusChange: onReceiptStatusChange,
    };

}();

