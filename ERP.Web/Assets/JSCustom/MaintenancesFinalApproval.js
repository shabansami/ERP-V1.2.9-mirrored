"use strict";

var MaintenancesFinalApproval_Module = function () {

    //#region ======== Save Purchase invoice ==================
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
                        return 'الاعتماد النهائى لفواتير الصيانة';
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
                    filename: "الاعتماد النهائى لفواتير الصيانة",
                    title: "الاعتماد النهائى لفواتير الصيانة",
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
                oPaginate: {
                    sNext: '<span class="pagination-default">التالى</span><span class="pagination-fa"><i class="fa fa-chevron-left" ></i></span>',
                    sPrevious: '<span class="pagination-default">السابق</span><span class="pagination-fa"><i class="fa fa-chevron-right" ></i></span>'
                }
            },

            ajax: {
                url: '/MaintenancesFinalApprovals/GetAll',
                type: 'GET',

            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'InvoiceNum', title: 'رقم الفاتورة' },
                { data: 'InvoiceDate', title: 'تاريخ الفاتورة' },
                { data: 'CustomerName', title: 'اسم العميل' },
                { data: 'CaseName', title: 'اخر حالة للفاتورة' },
                { data: 'InvoType', title: 'حالة الفاتورة', visible: false },
                { data: 'InvoFinal', title: 'حالة الاعتماد', visible: false },
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
                    targets: -1,
                    title: 'عمليات',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        if (row.IsFinalApproval) {
                            return '\
							<div class="btn-group">\
							<a href="/MaintenancesFinalApprovals/PrintBarCode/?invoGuid='+ row.InvoiceGuid + '" class="btn btn-sm btn-clean btn-icUrln" title="طباعة سيريال الاصناف">\
                       	<i class="fa fa-print"></i>\
                        	</a><a href="/Maintenances/ShowMaintenance/?invoGuid='+ row.InvoiceGuid + '" class="btn btn-sm btn-clean btn-icon" title="استعراض بيانات الفاتورة">\
								<i class="fa fa-search"></i>\
							</a>\</div>\
						';
                        } else {
                            return '\
							<div class="btn-group">\
							<a href="javascript:;" onclick=MaintenancesFinalApproval_Module.ApprovalInvoice("'+ row.InvoiceGuid + '") class="btn btn-sm btn-clean btn-icUrln" title="اعتماد الفاتورة بشكل نهائى">\
							<i class="fa fa-check"></i>\
							</a>\<a href="/Maintenances/ShowMaintenance/?invoGuid='+ row.InvoiceGuid + '" class="btn btn-sm btn-clean btn-icon" title="استعراض بيانات الفاتورة">\
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

    function ApprovalInvoice(invoGuid) { // اعتماد فاتورة محاسبيا
        Swal.fire({
            title: 'تأكيد الاعتماد',
            text: 'هل متأكد من اعتماد الفاتورة ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/MaintenancesFinalApprovals/ApprovalInvoice';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "invoGuid": invoGuid
                    },
                    //async: true,
                    //headers: { 'RequestVerificationToken': $('@Html.AntiForgeryToken()').val() },
                    success: function (data) {
                        if (data.isValid) {
                            toastr.success(data.message, '');
                            $('#kt_datatable').DataTable().ajax.reload();
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

    //#endregion ========= end Step 2 ==========


    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        ApprovalInvoice: ApprovalInvoice
    };

}();

