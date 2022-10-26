"use strict";

var DamageInvoice_Module = function () {

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
                        return 'فواتير الهالك';
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
                    filename: "فواتير الهالك",
                    title: "فواتير الهالك",
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
                url: '/DamageInvoices/GetAll',
                type: 'GET',

                    },
        columns: [
            { data: 'Num',responsivePriority:0 },
            { data: 'InvoiceDate', title: 'تاريخ الفاتورة' },
            { data: 'BranchName', title: 'الفرع' },
            { data: 'StoreName', title: 'المخزن' },
            { data: 'TotalQuantity', title: 'اجمالى الكمية' },
            { data: 'TotalCostQuantityAmount', title: 'اجمالى قيمة الكميات' },
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
                           <a href="/DamageInvoices/ShowDamageInvoice/?invoGuid='+ row.InvoiceGuid + '" class="btn btn-sm btn-clean btn-icon" title="عرض فاتورة">\
								<i class="fa fa-search"></i>\
							<a href="javascript:;" onclick=DamageInvoice_Module.deleteRow(\''+ row.InvoiceGuid + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a></div>\
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
    var initDTItemDetails = function () {
        var table = $('#kt_dtItemDetails');

        // begin first table
        table.DataTable({
            responsive: true,
            searchDelay: 500,
            processing: true,
            serverSide: false,
            select: true,

            // DOM Layout settings
            dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",
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
                url: '/DamageInvoices/SearchItemBalances',
                type: 'GET',
                data(d) {
                    d.itemCode = $("#ItemCode").val();
                    d.barCode = $("#BarCode").val();
                    d.groupId = $("#groupId").val();
                    d.itemtypeId = $("#ItemtypeId").val();
                    d.itemId = $("#ItemId").val();
                    d.branchId = $("#BranchId").val();
                    d.storeId = $("#StoreId").val();
                    d.isFirstInitPage = $("#isFirstInitPage").val();
                }


            },
            columns: [
                {
                    data: 'Id',
                    render: function (data, type, row){
                        return '<label class="form-control"  style="display:none"> ' + row.Id + '  </lable>';
                    }
                },
                { data: 'Num', responsivePriority: 1 },
                { data: 'GroupName', title: 'المجموعة' },
                { data: 'ItemTypeName', title: 'النوع' },
                { data: 'ItemName', title: 'الصنف' },
                { data: 'Balance', title: 'الرصيد' },
                {
                    data: 'BalanceReal', title: 'كمية الهالك',
                    render: function (data, type, row) {
                        return '<input class="form-control balanceInput" id="BalanceReal" name="BalanceReal" type="text"  value = ' + row.BalanceReal + '  >';
                    }
                },

            ],
            columnDefs: [
                {
                    targets: 1,
                    title: 'م',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    },
                },
            ],
            "order": [[0, "asc"]]
            //"order": [[0, "desc"]]

        });

    };

    function SaveInvoice() { // اعتماد فاتورة
        var values = new Array();
        $.each($(".balanceInput"), function () {
            var data = $(this).parents('tr:eq(0)');
            values.push({ 'Id': $(data).find('td:eq(0)').text(), 'ItemName': $(data).find('td:eq(4)').text(), 'Balance': $(data).find('td:eq(5)').text(),'BalanceReal': $(data).find('td:eq(6) input[type="text"]').val() });
        });
        var invoiceDate = document.getElementById('InvoiceDate').value;
        var storeId = document.getElementById('StoreId').value;
        var notes = document.getElementById('Notes').value;
        //var formData = new FormData();
        if (invoiceDate === ''||invoiceDate===null) {
            toastr.error('تأكد من اختيار التاريخ', '');
            return false;
        };
        if (storeId === '') {
            toastr.error('تأكد من اختيار المخزن', '');
            return false;
        };
        Swal.fire({
            title: 'تأكيد الحفظ',
            text: 'هل متأكد من حفظ الفاتورة ؟',
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
                //    document.location.replace("DamageInvoices/Index")

                var url = '/DamageInvoices/SaveInvoice';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "InvoiceDate": invoiceDate,
                        "StoreId": storeId,
                        "Notes": notes,
                        "data": JSON.stringify(values)
                    },
                    //async: true,
                    //headers: { 'RequestVerificationToken': $('@Html.AntiForgeryToken()').val() },
                    success: function (data) {
                        if (data.isValid) {
                            toastr.success(data.message, '');
                            setTimeout(function () { window.location = "/DamageInvoices/Index" }, 3000);
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
    function deleteRow(id) {
        Swal.fire({
            title: 'تأكيد الحذف',
            text: 'هل متأكد من الحذف ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/DamageInvoices/Delete';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "invoGuid": id
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


    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        initItemDetails: function () {
            initDTItemDetails();
        },
        SaveInvoice: SaveInvoice,
        deleteRow: deleteRow,

    };

}();

