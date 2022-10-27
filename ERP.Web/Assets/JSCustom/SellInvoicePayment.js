"use strict";

var SellInvoicePayment_Module = function () {

    //#region ======== Index ==================
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
                        return 'فواتير البيع الاجل';
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
                    filename: "فواتير البيع الاجل",
                    title: "فواتير البيع الاجل",
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
                url: '/SellInvoicePayments/GetAll',
                type: 'GET',

                    },
        columns: [
            { data: 'Id', title: 'م', visible: false },
            { data: 'InvoiceGuid', visible: false },
            { data: 'AnyPaid', visible: false },
            { data: 'Num',responsivePriority:1 },
            { data: 'InvoiceNum', title: 'رقم الفاتورة' },
            { data: 'InvoiceDate', title: 'تاريخ الفاتورة' },
            { data: 'CustomerName', title: 'اسم العميل' },
            { data: 'Actions', responsivePriority: -1, className: 'actions' },

        ],
            columnDefs: [
                {
                    targets: 1,
                    title: 'م',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return  meta.row + meta.settings._iDisplayStart + 1;
                    },
                },
            {
                targets: -1,
                title: 'عمليات',
                orderable: false,
                render: function (data, type, row, meta) {
                    if (row.AnyPaid) {
                        return '\
							<div class="btn-group">\
							<a href="/SellInvoicePayments/RegisterPyments/?invoGuid='+ row.InvoiceGuid + '" class="btn btn-sm btn-clean btn-icon" title=" سداد دفعة">\
								<i class="fa fa-credit-card"></i>\
							</a>\
                            </div>\
						';
                    }else
                        return '\
							<div class="btn-group">\
							<a href="/SellInvoicePayments/RegisterPyments/?invoGuid='+ row.InvoiceGuid + '" class="btn btn-sm btn-clean btn-icon" title="انشاء/ادارة دفعات الفاتورة">\
								<i class="fa fa-money"></i>\
							</a>\<a href="/SellInvoicePayments/RegisterPyments/?pay=1&invoGuid='+ row.InvoiceGuid + '" class="btn btn-sm btn-clean btn-icon" title="البدأ فى سداد دفعة">\
								<i class="fa fa-credit-card"></i>\
							</a>\
                            </div>\
						';

                   
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

  
    //#endregion =========  ==========
    //#region =========== Generate Payments
    var initDTItemDetails = function () {
        var table = $('#kt_dtItemDetails');

        // begin first table
        table.DataTable({
            paging: false,
            info: false,
            search: false,
            searching: false,
            "order": [[0, "asc"]]
            //"order": [[0, "desc"]] 

        });
    };

    function RegisterPyments() { // تسجيل فاتورة

        //var data = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
        var values = new Array();
        $.each($(".selectedRow"), function () {
            var data = $(this).parents('tr:eq(0)');
            values.push({ 'Id': $(data).find('td:eq(0)').text(), 'SellInvoiceId': $(data).find('td:eq(1)').text(), 'DueDate': $(data).find('td:eq(2) input[type="date"]').val(), 'Amount': $(data).find('td:eq(3) input[type="number"]').val() });
        });
        console.log(values);
        console.log(values.length);

        Swal.fire({
            title: 'تأكيد تسجيل الدفعات',
            text: 'هل متأكد من تسجيل دفعات الفاتورة ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                const params = new URLSearchParams(window.location.search)
                var queryString = '';
                if (params.has('invoGuid')) {
                    queryString = params.get('invoGuid');
                } else
                    document.location.replace("SellInvoicePayments/Index")

                var url = '/SellInvoicePayments/RegisterPyments';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "invoGuid": queryString,
                        "data": JSON.stringify(values)
                    },
                    //async: true,
                    //headers: { 'RequestVerificationToken': $('@Html.AntiForgeryToken()').val() },
                    success: function (data) {
                        if (data.isValid) {
                            toastr.success(data.message, '');
                            setTimeout(function () { window.location = "/SellInvoicePayments/Index" }, 3000);
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
    function SubmitForm(btn) {
        try {
            var form = document.getElementById('form1');
            $.ajax({
                type: 'POST',
                url: "/SellInvoicePayments/GeneratePyments",
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        //$(btn).attr('disabled', 'disabled'); // disabled button after one clicke 
                        $(btn).css('pointer-events', 'none'); // disabled a link after one clicke 
                        toastr.success(res.message, '',)
                        setTimeout(function () { window.location = "/SellInvoicePayments/RegisterPyments?invoGuid=" + res.invoGuid  }, 3000);
                        //$('#kt_datatableLast').DataTable().ajax.reload();
                    } else {
                        toastr.error(res.message, '');
                    }
                    //document.getElementById('submit').disabled = false;
                    //document.getElementById('reset').disabled = false;
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
    function Paid(id) { //دفع دفعة
       

        Swal.fire({
            title: 'تأكيد دفع ',
            text: 'هل متأكد من سداد الدفعة ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/SellInvoicePayments/Paid';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "id": id,
                        "SafeId":$("#SafeId").val()
                    },
                    //async: true,
                    //headers: { 'RequestVerificationToken': $('@Html.AntiForgeryToken()').val() },
                    success: function (data) {
                        if (data.isValid) {
                            toastr.success(data.message, '');
                            setTimeout(function () { window.location = "/SellInvoicePayments/Index" }, 3000);
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

    function calcPayValue() {
        var RemindValue = Number.parseFloat($("#RemindValue").val());
        if (isNaN(RemindValue))
            RemindValue = 0;

        var Duration = Number.parseFloat($("#Duration").val());
        if (isNaN(Duration))
            Duration = 0;

        var safy = RemindValue / Duration;
        $("#PayValue").val(safy);
    }

    //#endregion ==================
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        initItemDetails: function () {
            initDTItemDetails();
        },
        SubmitForm: SubmitForm,
        RegisterPyments: RegisterPyments,
        calcPayValue: calcPayValue,
        Paid: Paid,
    };

}();

