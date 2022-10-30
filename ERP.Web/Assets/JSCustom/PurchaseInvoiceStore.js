"use strict";

var PurchaseInvoiceStore_Module = function () {

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
                        return 'الاعتماد المخزنى لفواتير التوريد';
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
                    filename: "الاعتماد المخزنى لفواتير التوريد",
                    title: "الاعتماد المخزنى لفواتير التوريد",
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
                url: '/PurchaseInvoiceStores/GetAll',
                type: 'GET',

                    },
        columns: [
            { data: 'Num',responsivePriority:0 },
            { data: 'InvoiceNum', title: 'رقم الفاتورة' },
            { data: 'InvoiceDate', title: 'تاريخ الفاتورة' },
            { data: 'SupplierName', title: 'اسم المورد' },
            { data: 'ApprovalStore', title: 'حالة الاعتماد' },
            { data: 'CaseName', title: 'اخر حالة للفاتورة' },
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
                    if (row.IsApprovalStore) {
                        return '\
							<div class="btn-group">\
							<a href="/PurchaseInvoices/ShowPurchaseInvoice/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض فاتورة">\
								<i class="fa fa-search"></i>\
							</a>\
                            </div>\
						';
                    } else {
                       return '\
							<div class="btn-group">\
							\<a href="/PurchaseInvoices/ShowPurchaseInvoice/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض فاتورة">\
								<i class="fa fa-search"></i>\
							</a>\
							<a href="/PurchaseInvoiceStores/ApprovalStore/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icUrln" title="اعتماد مخزنى">\
								<i class="fa fa-unlock-alt"></i>\
							</a></div>\
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
    var initDTItemDetails = function () {
        var table = $('#kt_dtItemDetails');

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

    function ApprovalInvoice() { // اعتماد فاتورة

        //var data = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
        var values = new Array();
        $.each($(".selectedDay"), function () {
            var data = $(this).parents('tr:eq(0)');
            values.push({ 'Id': $(data).find('td:eq(0)').text(), 'ItemId': $(data).find('td:eq(1)').text(), 'ItemName': $(data).find('td:eq(2)').text(), 'ContinerName': $(data).find('td:eq(3)').text(), 'Quantity': $(data).find('td:eq(4)').text(), 'QuantityReal': $(data).find('td:eq(5) input[type="text"]').val() });
        });
        console.log(values);
        console.log(values.length);

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
                const params = new URLSearchParams(window.location.search)
                var queryString = '';
                if (params.has('invoGuid')) {
                    queryString = params.get('invoGuid');
                } else
                    document.location.replace("PurchaseInvoiceStores/Index")

                var url = '/PurchaseInvoiceStores/ApprovalInvoice';
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
                            setTimeout(function () { window.location = "/PurchaseInvoiceStores/Index" }, 3000);
                        } else {
                            if (data.isQuantitDiff)
                                document.location.replace("/PurchaseInvoiceStores/Index");
                            else
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
        initItemDetails: function () {
            initDTItemDetails();
        },
        ApprovalInvoice: ApprovalInvoice
    };

}();

