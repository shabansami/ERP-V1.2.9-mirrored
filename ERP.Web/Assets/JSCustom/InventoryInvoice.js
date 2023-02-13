"use strict";

var InventoryInvoice_Module = function () {

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
                        return 'فواتير الجرد';
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
                    filename: "فواتير الجرد",
                    title: "فواتير الجرد",
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
                url: '/InventoryInvoices/GetAll',
                type: 'GET',

                    },
        columns: [
   
            { data: 'Num',responsivePriority:0 },
            { data: 'InvoiceDate', title: 'تاريخ الفاتورة' },
            { data: 'BranchName', title: 'الفرع' },
            { data: 'StoreName', title: 'المخزن' },
            { data: 'TotalDifferenceAmount', title: 'اجمالى قيمة الفروق' },
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
                           <a href="/InventoryInvoices/ShowInventoryInvoice/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض فاتورة">\
								<i class="fa fa-search"></i>\
							<a href="javascript:;" onclick=InventoryInvoice_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
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
    function SaveInvoice() { // اعتماد فاتورة
        var dataSet = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
        var DT_DatasourceItems;
        if (dataSet != null) {
            if (dataSet.length > 0) {
               DT_DatasourceItems= JSON.stringify(dataSet);
            }
        }
        var invoiceDate = document.getElementById('InvoiceDate').value;
        var branchId = document.getElementById('BranchId').value;
        var storeId = document.getElementById('StoreId').value;
        var itemCostCalculateId = document.getElementById('ItemCostCalculateId').value;
        var notes = document.getElementById('Notes').value;
        //var formData = new FormData();
        if (invoiceDate === ''||invoiceDate===null) {
            toastr.error('تأكد من اختيار التاريخ', '');
            return false;
        };
        if (branchId === '') {
            toastr.error('تأكد من اختيار الفرع', '');
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
                //    document.location.replace("InventoryInvoices/Index")

                var url = '/InventoryInvoices/SaveInvoice';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "InvoiceDate": invoiceDate,
                        "BranchId": branchId,
                        "StoreId": storeId,
                        "ItemCostCalculateId": itemCostCalculateId,
                        "Notes": notes,
                        "data": DT_DatasourceItems
                    },
                    //async: true,
                    //headers: { 'RequestVerificationToken': $('@Html.AntiForgeryToken()').val() },
                    success: function (data) {
                        if (data.isValid) {
                            toastr.success(data.message, '');
                            setTimeout(function () { window.location = "/InventoryInvoices/Index" }, 3000);
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
                var url = '/InventoryInvoices/Delete';
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


    //#region ======== Step 2 تسجيل الاصناف ف الفاتورة=================
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
                oPaginate: {
                    sNext: '<span class="pagination-default">التالى</span><span class="pagination-fa"><i class="fa fa-chevron-left" ></i></span>',
                    sPrevious: '<span class="pagination-default">السابق</span><span class="pagination-fa"><i class="fa fa-chevron-right" ></i></span>'
                }
            },

            ajax: {
                url: '/InventoryInvoices/GetDSItemDetails',
                type: 'GET',

            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'GroupName', title: 'المجموعة' },
                { data: 'StoreName', title: 'المخزن' },
                { data: 'ItemName', title: 'الصنف' },
                { data: 'Balance', title: 'الرصيد' },
                { data: 'BalanceReal', title: 'الرصيد الفعلى' },
                { data: 'Actions', responsivePriority: -1 },

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
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return '\
							<div class="btn-group">\
							<a href="javascript:;" onclick=InventoryInvoice_Module.deleteRowItemDetails()  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a></div>\
						';
                    },
                }

            ],

            "order": [[0, "asc"]]
            //"order": [[0, "desc"]] 

        });
    };

    function addItemDetails() {
        try {
            var itemId = document.getElementById('ItemId').value;
            var storeId = document.getElementById('StoreId').value;
            var balance = document.getElementById('Balance').value;
            var balanceReal = document.getElementById('BalanceReal').value;
            var formData = new FormData();
            if (itemId === '') {
                toastr.error('تأكد من اختيار الصنف', '');
                $("#ItemId").select2('open');
                return false;
            };
            if (balanceReal === '') {
                toastr.error('تأكد من ادخال الكمية', '');
                $("#BalanceReal").focus().select();
                return false;
            };

            if (storeId === '') {
                toastr.error('تأكد من اختيار المخزن', '');
                $("#StoreId").select2('open');
                return false;
            };
           
            formData.append('Id', itemId)
            formData.append('StoreId', storeId)
            formData.append('Balance', balance)
            formData.append('BalanceReal', balanceReal)
            var dataSet = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/InventoryInvoices/AddItemDetails',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtItemDetails').DataTable().ajax.reload();
                        $('#Balance').val(0);
                        $('#BalanceReal').val(0);
                        $('#BarCode').val(null);

                        //$('#ItemCode').val(null);
                        $('#ItemId').val(null);
                        $('#ItemId').select2({
                            placeholder: "اختر عنصر من القائمة"
                        });
                        //$('#StoreId').val(null);
                        //$('#StoreId').select2({
                        //    placeholder: "اختر عنصر من القائمة"
                        //});
                        toastr.success(res.msg, '');
                        $('#ItemId').select2('open');
                    } else
                        toastr.error(res.msg, '');
                    return false;
                },
                error: function (err) {
                    toastr.error('حدث خطأ اثناء تنفيذ العملية', '');
                    console.log(err)
                }
            })
            //to prevent default form submit event
            return false;
            //$('#kt_datatableTreePrice').DataTable().ajax.reload();
            //to prevent default form submit event
            return false;
        } catch (ex) {
            console.log(ex)
        }

    }

    function deleteRowItemDetails() {
        $('#kt_dtItemDetails tbody').on('click', 'a.deleteIcon', function () {
            $('#kt_dtItemDetails').DataTable().row($(this).parents('tr')).remove().draw();
        })

    };
    function onItemChange() {
        if ($('#StoreId').val() != null) {
            $.get("/SharedDataSources/GetProductionOrdersOnStoreChange/", { itemId: $("#ItemId").val(), storeId: $("#StoreId").val() }, function (data) {
                //var isSelected = "";
                if (data.length > 0) {
                    var t = data[0].Val
                    var obj2 = JSON.parse(JSON.stringify(t));
                    var obj = JSON.parse(obj2);
                    var balance = obj.balance;
                    $("#Balance").val(balance);
                }


            });

        } else {
          toastr.error('تأكد من اختيار المخزن', '');
            return false;
        }
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
        SaveInvoice: SaveInvoice,
        deleteRow: deleteRow,
        addItemDetails: addItemDetails,
        deleteRowItemDetails: deleteRowItemDetails,
        onItemChange: onItemChange,

    };

}();

