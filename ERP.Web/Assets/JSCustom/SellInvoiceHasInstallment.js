"use strict";
var SellInvoiceHasInstallment_Module = function () {

    //#region ======== Index ==================
    var initDT = function () {
        var table = $('#kt_datatable');

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
                        return 'فواتير التقسيط';
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
                    filename: "فواتير التقسيط",
                    title: "فواتير التقسيط",
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
                url: '/SellInvoiceHasInstallments/GetAll',
                type: 'GET',
                data(d) {
                    d.customerId = $("#CustomerId").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                }
                    },
        columns: [
            { data: 'Num',responsivePriority:0 },
            { data: 'InvoiceNum', title: 'رقم فاتورة البيع' },
            { data: 'InvoiceDate', title: 'تاريخ فاتورة البيع' },
            { data: 'CustomerName', title: 'اسم العميل' },
            { data: 'Safy', title: 'صافى الفاتورة' },
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
                    var typ = "";
                    if (row.IsSell) {
                        typ = "sell";
                    }
                    else if (!typ.IsSell) {
                        typ = "initial";
                    };
                    if (row.AnySchedules) {
                        return '\
							<div class="btn-group">\
							<a href="/SellInvoiceHasInstallments/Installments/?invoGuid='+ row.InvoiceId + '&typ=' + typ + '" class="btn btn-sm btn-clean btn-icon" title="استعراض اقساط الفاتورة">\
								<i class="fa fa-search"></i>\
							</a><a href="javascript:;" onclick=SellInvoiceHasInstallment_Module.deleteRow(\''+ row.InvoiceId + '\',\'' + typ + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a>\
                           </div>\
						';

                    } else

                    return '\
							<div class="btn-group">\
							                          </div>\
						';

                   
},
                        }

                    ],

            drawCallback: function () {
                var html = ' <tr><th colspan ="8" style= "text-align:center" ><div class="row alert-success"><label>اجمالى المبلغ : ';
                var api = this.api();
                var totalAmount = api.column(4).data().sum();
                $(api.table().footer()).html(html + totalAmount + '</label></div></th>  </tr>');
            },
            "order": [[0, "asc"]]
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

    function deleteRow(id,typ) {
        Swal.fire({
            title: 'تأكيد الحذف',
            text: 'هل متأكد من حذف جميع الاقساط ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/SellInvoiceHasInstallments/Delete';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "invoGuid": id,
                        "typ": typ,
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

    //#endregion =========  ==========
    //#region =========== Generate Payments

    var tablePaidInTime = $('#kt_datatablePaidInTime').DataTable({
            paging: false,
            info: false,
            search: false,
            searching: false,
            "order": [[0, "asc"]]
            //"order": [[0, "desc"]] 

        });
    var tablePaidNotInTime = $('#kt_datatablePaidNotInTime').DataTable({
            paging: false,
            info: false,
            search: false,
            searching: false,
            "order": [[0, "asc"]]
            //"order": [[0, "desc"]] 

        });
    var tableNotPaid = $('#kt_datatableNotPaid').DataTable({
            paging: false,
            info: false,
            search: false,
            searching: false,
            "order": [[0, "asc"]]
            //"order": [[0, "desc"]] 

        });

    //#endregion ==================
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        tablePaidInTime: tablePaidInTime,
        tablePaidNotInTime: tablePaidNotInTime,
        tableNotPaid: tableNotPaid,
        deleteRow: deleteRow,
    };

}();

