"use strict";
var SellInvoiceInstallmentSchedule_Module = function () {

    //#region ======== Index ==================
    var initDTPaid = function () {
        var table = $('#kt_datatablePaid');

        // begin first table
        table.DataTable({
            //responsive: true,
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
                        return 'اقساط تم تحصيلها';
                    },
                    customize: function (win) {
                        $(win.document.body)
                            //.css('font-size', '20pt')
                            .prepend(
                                '<img src=' + localStorage.getItem("logo")+' style="position:absolute; top:25%; right:10%;opacity: 0.2;" />'
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
                    filename: "اقساط تم تحصيلها",
                    title: "اقساط تم تحصيلها",
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
                url: '/SellInvoiceInstallmentSchedules/GetAll',
                type: 'GET',
                data(d) {
                    d.customerId = $("#CustomerId").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.isPaid =true;
                }
                    },
        columns: [
            { data: 'Num',responsivePriority:0 },
            { data: 'InvoiceNum', title: 'رقم فاتورة البيع' },
            { data: 'InvoiceDate', title: 'تاريخ فاتورة البيع' },
            { data: 'CustomerName', title: 'اسم العميل' },
            { data: 'Amount', title: 'قيمة القسط' },
            { data: 'InstallmentDate', title: 'تاريخ القسط' },
            { data: 'PaymentDate', title: 'تاريخ السداد' },
            { data: 'StatusTxt', title: 'نوع الفاتورة' },
            { data: 'Actions', responsivePriority: -1, className: 'actions' },

        ],
            columnDefs: [
                {
                    targets: 0,
                    title: 'م',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return  meta.row + meta.settings._iDisplayStart + 1;
                    },
                },
                {
                    targets: -1,
                    title: 'عمليات',
                    render: function (data, type, row, meta) {
                        if (row.PaidInTime) {
                         return `
							<div class="btn-group">
                            <span class="label label-lg font-weight-bold label-light-success label-inline">تم السداد فى موعدها</span>
                           <a href="javascript:;" onclick=SellInvoiceInstallmentSchedule_Module.UnApprovalSchedule('` + row.ScheduleId + `') class="btn btn-sm btn-clean btn-icUrln" title="فك الاعتماد">\
								<i class="fa fa-check-square-o"></i>
                            </div >
						`;
                        }else
                            return `
							<div class="btn-group">
                            <span class="label label-lg font-weight-bold label-light-warning label-inline">تم السداد بعد موعدها</span>
                            <a href="javascript:;" onclick=SellInvoiceInstallmentSchedule_Module.UnApprovalSchedule('` + row.ScheduleId + `') class="btn btn-sm btn-clean btn-icUrln" title="فك الاعتماد">
								<i class="fa fa-check-square-o"></i></a><a href="javascript:;" onclick="PrintInvoice_Module.PrintInstallmentSchedule('`+ row.ScheduleId + `');" class="btn btn-sm btn-clean btn-icUrln" title="طباعه ايصال">\
								<i class="fa fa-print"></i></a>
                            </div >
						`;


                    },
                }
            ],
            drawCallback: function () {
                var html = ' <tr><th colspan ="5" style= "text-align:center" ><div class="row alert-success"><label>اجمالى المبلغ : ';
                var api = this.api();
                var totalAmount = api.column(4).data().sum();
                var totalQuantity = api.column(4).data().count();
                $(api.table().footer()).html(html + totalAmount + '</label></div></th> <th colspan ="5" style= "text-align:center" ><div class="row alert-success"><label>اجمالى العدد :' + totalQuantity + '</label ></div ></th > </tr>');
            },

"order": [[0, "desc"]]
//"order": [[0, "desc"]] 

        });
        $('#export_print').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatablePaid').DataTable().button('.buttons-print').trigger();
        });

        $('#export_copy').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatablePaid').DataTable().button('.buttons-copy').trigger();
        });

        $('#export_excel').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatablePaid').DataTable().button('.buttons-excel').trigger();
        });
    };
    var initDTNotPaid = function () {
        var table = $('#kt_datatableNotPaid');

        // begin first table
        table.DataTable({
            //responsive: true,
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
                        return 'اقساط لم تم تحصيلها بعد';
                    },
                    customize: function (win) {
                        $(win.document.body)
                            //.css('font-size', '20pt')
                            .prepend(
                                '<img src=' + localStorage.getItem("logo")+' style="position:absolute; top:25%; right:10%;opacity: 0.2;" />'
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
                    filename: "اقساط لم تم تحصيلها بعد",
                    title: "اقساط لم تم تحصيلها بعد",
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
                url: '/SellInvoiceInstallmentSchedules/GetAll',
                type: 'GET',
                data(d) {
                    d.customerId = $("#CustomerId").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.isPaid =false;
                }
                    },
        columns: [
            { data: 'Num',responsivePriority:0 },
            { data: 'InvoiceNum', title: 'رقم الفاتورة' },
            { data: 'InvoiceDate', title: 'تاريخ الفاتورة' },
            { data: 'CustomerName', title: 'اسم العميل' },
            { data: 'Amount', title: 'قيمة القسط' },
            { data: 'InstallmentDate', title: 'تاريخ القسط' },
            { data: 'StatusTxt', title: 'نوع الفاتورة' },
            { data: 'Actions', responsivePriority: -1, className: 'actions' },

        ],
            columnDefs: [
                {
                    targets: 0,
                    title: 'م',
                    render: function (data, type, row, meta) {
                        return  meta.row + meta.settings._iDisplayStart + 1;
                    },
                },
            {
                targets: -1,
                title: 'عمليات',
                orderable: false,
                render: function (data, type, row, meta) {
                        return '\
							<div class="btn-group">\
							<a href="/SellInvoiceInstallmentSchedules/Paid?scId='+ row.ScheduleId + '" class="btn btn-sm btn-clean btn-icon" title=" سداد دفعة">\
								<i class="fa fa-credit-card"></i>\
							</a>\
                            </div>\
						';

},
                        }

                    ],
            drawCallback: function () {
                var html = ' <tr><th colspan ="5" style= "text-align:center" ><div class="row alert-success"><label>اجمالى المبلغ : ';
                var api = this.api();
                var balanceStatusTxt = '';
                var totalAmount = api.column(4).data().sum();
                var totalQuantity = api.column(4).data().count();
                $(api.table().footer()).html(html + totalAmount + '</label></div></th> <th colspan ="5" style= "text-align:center" ><div class="row alert-success"><label>اجمالى العدد :' + totalQuantity + '</label ></div ></th > </tr>');
            },
"order": [[0, "desc"]]
//"order": [[0, "desc"]] 

        });
        $('#export_printNotPaid').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableNotPaid').DataTable().button('.buttons-print').trigger();
        });

        $('#export_copyNotPaid').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableNotPaid').DataTable().button('.buttons-copy').trigger();
        });

        $('#export_excelNotPaid').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableNotPaid').DataTable().button('.buttons-excel').trigger();
        });
    };

  
    //#endregion =========  ==========
    //#region =========== Generate Payments


    function Paid(btn) { //دفع دفعة
            //   Swal.fire({
            //title: 'تأكيد التحصيل ',
            //text: 'هل متأكد من تحصيل القسط ؟',
            //icon: 'warning',
            //showCancelButton: true,
            //animation: true,
            //confirmButtonText: 'تأكيد',
            //cancelButtonText: 'إلغاء الامر'
            //   }).then((result) => {
            //       console.log(result);
            //       console.log(result.value);
            //if (result.value) {
                var form = document.getElementById('form1');
                var url = '/SellInvoiceInstallmentSchedules/Paid';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: new FormData(form),
                    contentType: false,
                    processData: false,
                    success: function (data) {
                        if (data.isValid) {
                            //$(btn).attr('disabled', 'disabled'); // disabled button after one clicke 
                            $(btn).css('pointer-events', 'none'); // disabled a link after one clicke 
                            toastr.success(data.message, '');
                            setTimeout(function () { window.location = "/SellInvoiceInstallmentSchedules/Index" }, 3000);
                        } else {
                            toastr.error(data.message, '');
                        }
                    },
                    error: function (err) {
                        alert(err);
                    }
                });
            //}
        //});
    };

    function onKeyupAmount() {
        var payedAmount = $("#PayedAmount").val(); //قيمة القسط المدفوعة
        var amount = $("#Amount").val();//قيمة القسط الاساسية
        if (payedAmount === "" || isNaN(payedAmount))
            payedAmount = 0;
        if (amount === "" || isNaN(amount))
            amount = 0;
        var dif = amount-payedAmount;
        $("#DifValue").val(dif);
        if (dif == 0)
            $("#divOtherSch").hide();
        else
            $("#divOtherSch").show();


    }
    function UnApprovalSchedule(id) {
        Swal.fire({
            title: 'تأكيد فك الاعتماد',
            text: 'هل متأكد من فك الاعتماد ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/SellInvoiceInstallmentSchedules/UnApprovalSchedule';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "id": id
                    },
                    //async: true,
                    //headers: { 'RequestVerificationToken': $('@Html.AntiForgeryToken()').val() },
                    success: function (data) {
                        if (data.isValid) {
                            toastr.success(data.message, '');
                            $('#kt_datatableNotPaid').DataTable().ajax.reload();
                            $('#kt_datatablePaid').DataTable().ajax.reload();
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

    //#endregion ==================
    return {
        //main function to initiate the module
        initPaid: function () {
            initDTPaid();
        },
        initNotPaid: function () {
            initDTNotPaid();
        },
      
        Paid: Paid,
        onKeyupAmount: onKeyupAmount,
        UnApprovalSchedule: UnApprovalSchedule,
      
    };

}();

