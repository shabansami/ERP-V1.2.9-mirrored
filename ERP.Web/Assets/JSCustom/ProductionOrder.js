"use strict";
var ProductionOrder_Module = function () {

    //#region ======== Save Production Order ==================
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
                        return 'أوامر الإنتاج';
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
                    filename: "أوامر الإنتاج",
                    title: "أوامر الإنتاج",
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
                url: '/ProductionOrders/GetAll',
                type: 'GET',

            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'OrderNumber', title: 'رقم أمر الإنتاج' },
                { data: 'ProductionOrderDate', title: 'تاريخ امر الإنتاج' },
                { data: 'ItemProduction', title: 'التوليفة' },
                //{ data: 'ItemOutName', title: 'الصنف/الاصناف الخارجة' },
                //{ data: 'OrderQuantity', title: 'الكمية' },
                //{ data: 'IsDoneTitle', visible: false, title: 'حالة التسليم لمخازن المنتج التام' },
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
                    render: function(data, type, row, meta) {
      //                  if (row.IsDone) {
      //                      return '\
						//	<div class="btn-group">\
						//	<a href="/ProductionOrders/ShowProductionOrder/?ordrGud=' +
      //                          row.Id +
      //                          '" class="btn btn-sm btn-clean btn-icon" title="عرض ملخص أمر اللإنتاج">\
						//		<i class="fa fa-search"></i>\
						//	</a>\</div>\
						//';
                        //<a href="/ProductionOrderDamages/Index/?ordrGud=' +
                        //        row.Id +
                        //        '" class="btn btn-sm btn-clean btn-icon" title="تسجيل توالف">\
                        //    <i class="fa fa-house-damage"></i>\
                        //</a>\
      //                  } else {
                            return '\
							<div class="btn-group">\
							<a href="/ProductionOrders/ShowProductionOrder/?ordrGud=' +
                                row.Id +
                                '" class="btn btn-sm btn-clean btn-icon" title="عرض ملخص أمر اللإنتاج">\
								<i class="fa fa-search"></i>\
							</a>\
							<a href="javascript:;" onclick=ProductionOrder_Module.deleteRow(\'' +
                                row.Id +
                                '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a></div>\
						';
                        //}

                    
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

    function SubmitForm(btn) {
        try {
            var form = document.getElementById('form1');
            var formData = new FormData(form);
            var dataSet = $('#kt_datatableItemIn').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_DatasourceItemIn", JSON.stringify(dataSet));
                }
            }
            var dataSetOut = $('#kt_datatableItemOut').DataTable().rows().data().toArray();
            if (dataSetOut != null) {
                if (dataSetOut.length > 0) {
                    formData.append("DT_DatasourceItemOut", JSON.stringify(dataSetOut));
                }
            }
            var dataSetExpense = $('#kt_dtProductionOrderExpenses').DataTable().rows().data().toArray();
            if (dataSetExpense != null) {
                if (dataSetExpense.length > 0) {
                    formData.append("DT_DatasourceExpenses", JSON.stringify(dataSetExpense));
                }
            }
            formData.append("orderQuantity", $("#OrderQuantity").val());
            $.ajax({
                type: 'POST',
                url: formData.action,
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $(btn).attr('disabled', 'disabled'); // disabled button after one clicke 
                        toastr.success(res.message, '')
                        setTimeout(function () { window.location = "/ProductionOrders/RegisterOrder" }, 3000);



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

    function deleteRow(ordrGud) {
        Swal.fire({
            title: 'تأكيد الحذف',
            text: 'هل متأكد من الحدف ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/ProductionOrders/Delete';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "ordrGud": ordrGud
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

    //#endregion ============== end ==============


    //#region ======== Step 1توليف الاصناف الداخلة=================
    var initItemIn = function () {
        var table = $('#kt_datatableItemIn');

        // begin first table
        table.DataTable({
            responsive: true,
            searchDelay: 500,
            processing: true,
            serverSide: false,
            paging: false,
            info: false,
            lengthChange: false,
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
                url: '/ProductionOrders/GetDSItemIn',
                type: 'GET',
                data(d) {
                    d.itemProductionId = $("#ItemProductionId").val();
                    d.quantity = $("#OrderQuantity").val();
                    d.storeUnderId = $("#ProductionUnderStoreId").val();
                    d.isFirstInit = $("#isFirstInit").val();
                    d.ItemCostCalculateId = $("#ItemCostCalculateId").val();
                }
            },
            columns: [
                //{ data: 'Num', responsivePriority:1},
                { data: 'ItemProductionId', visible: false },
                { data: 'IsAllQuantityDone', visible: false },
                { data: 'ItemId', visible: false },
                { data: 'StoreUnderId', visible: false },
                { data: 'ItemCostCalculateId', visible: false },
                { data: 'ItemName', title: 'المنتج الخام' },
                { data: 'ItemCost', title: 'تكلفة المنتج' },
                { data: 'QuantityRequired', title: 'الكمية المطلوبة' },
                { data: 'QuantityAvailable', title: 'الكمية المتاحة' },
                { data: 'Actions', responsivePriority: -1 },
                { data: 'ActionStatus', responsivePriority: -2 },

            ],
            columnDefs: [
                //{
                //    targets: 1,
                //    title: 'م',
                //    orderable: false,
                //    render: function (data, type, row, meta) {
                //        return meta.row + meta.settings._iDisplayStart + 1;
                //    },
                //},
                {
                    targets: -1,
                    title: 'عمليات',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return '\
							<div class="btn-group">\
							<a href="javascript:;" onclick=ProductionOrder_Module.deleteRowItemIn()  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a></div>\
						';
                    },
                },{
                    targets: -2,
                    title: 'الحالة',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        var status = {
                            1: { 'title': 'Pending', 'class': 'label-light-primary' },
                            2: { 'title': 'Delivered', 'class': ' label-light-danger' },
                            3: { 'title': 'Canceled', 'class': ' label-light-primary' },
                            4: { 'title': 'الرصيد يكفى', 'class': ' label-light-success' },
                            5: { 'title': 'Info', 'class': ' label-light-info' },
                            6: { 'title': 'الرصيد لا يكفى', 'class': ' label-light-danger' },
                            7: { 'title': 'Warning', 'class': ' label-light-warning' },
                        };
                        if (row.IsAllQuantityDone) {
                            return '<span class="label label-lg font-weight-bold' + status[4].class + ' label-inline">' + status[4].title + '</span>';
                        } else {
                            return '<span class="label label-lg font-weight-bold' + status[6].class + ' label-inline">' + status[6].title + '</span>';
                        }
                    },
                }

            ],

            //"order": [[0, "desc"]]
            //"order": [[0, "desc"]] 

        });
    };


    function addItemProductionDetails() {
        if ($("#OrderQuantity").val() === '0' || $("#OrderQuantity").val() === '' || $("#ItemProductionId").val() === '' || $("#ProductionOrderDate").val() === null) {
            toastr.error('تأكد من اختيار توليفة او ادخال كمية', '');
            //$('#kt_datatableItemProduction').dataTable().fnClearTable();
            $('#kt_datatableItemIn').dataTable().fnClearTable();
            $('#kt_datatableItemOut').dataTable().fnClearTable();
            $("#isFirstInit").val('0');
        } else {
            $("#isFirstInit").val('0');
            $('.div_itemPro').show();
            $('#kt_datatableItemIn').DataTable().ajax.reload();
            $('#kt_datatableItemOut').DataTable().ajax.reload();
        }

    };
    function addOtherItemIn() {
        try {
            var itemId = document.getElementById('ItemId').value;
            var storeUnderId = document.getElementById('ProductionUnderStoreId').value;
            var QuantityRequired = document.getElementById('Quantity').value;
            var itemCostCalculateId = document.getElementById('ItemCostCalculateId').value;
            var itemCost = document.getElementById('ItemCost').value;
            var orderQuantity = document.getElementById('OrderQuantity').value;
            var balanceCurrentStore = parseFloat($("#balanceCurrentStore").text());
            var itemAcceptNoBalance = parseFloat($("#ItemAcceptNoBalance").val());
            var formData = new FormData();
            if (itemId === '') {
                toastr.error('تأكد من اختيار الصنف', '');
                $("#ItemId").select2('open');
                return false;
            };
            if (storeUnderId === '') {
                toastr.error('تأكد من اختيار المخزن', '');
                return false;
            };
            if (QuantityRequired === '' || QuantityRequired == '0') {
                toastr.error('تأكد من ادخال الكمية', '');
                $("#Quantity").focus().select();
                return false;
            };
            if (itemCost === '' ) {
                toastr.error('تأكد من وجود التكلفة', '');
                return false;
            };
            if (itemCostCalculateId === '') {
                toastr.error('تأكد من اختيار طريقة احتساب تكلفة المنتج ', '');
                return false;
            };

            if (orderQuantity === '' || orderQuantity == '0') {
                toastr.error('تأكد من ادخال الكمية المطلوب انتاجها', '');
                return false;
            };

            formData.append('ItemId', itemId)
            formData.append('StoreUnderId', storeUnderId)
            formData.append('QuantityRequired', QuantityRequired)
            formData.append('ItemCostCalculateId', itemCostCalculateId)
            formData.append('ItemCost', itemCost)
            formData.append('orderQuantity', orderQuantity)
            formData.append('balanceCurrentStore', balanceCurrentStore)
            formData.append('itemAcceptNoBalance', itemAcceptNoBalance)

            //اصناف المواد الداخلة التى تم اضافتها
            var dataSet = $('#kt_datatableItemIn').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_DatasourceIn", JSON.stringify(dataSet));
                }
            }
            //الاصناف الخارجة
            var dataSetOut = $('#kt_datatableItemOut').DataTable().rows().data().toArray();
            if (dataSetOut != null) {
                if (dataSetOut.length > 0) {
                    formData.append("DT_DatasourceOut", JSON.stringify(dataSetOut));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/ProductionOrders/addOtherItemIn',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $("#isFirstInit").val('1');
                        $('#kt_datatableItemIn').DataTable().ajax.reload();
                        $('#Quantity').val(0);
                        $('#ItemCost').val(0);
                        $('#balanceCurrentStore').text(0);

                        ////فى حالة اكتمال العدد المطلوب من المواد الخام يتم تحديث جريد اصناف التوليفة 
                        //if (res.isAllQuantityDone !='undefined') {
                        //    if (res.isAllQuantityDone) {
                        //        $("#isFirstInit").val('1');
                        //        $('#kt_datatableItemProduction').DataTable().ajax.reload();
                        //    }
                        //}
                        toastr.success(res.msg, '');
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

    function deleteRowItemIn() {
            $('#kt_datatableItemIn tbody').on('click', 'a.deleteIcon', function () {
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
                        //var itemIdRemoved = $('#kt_datatableItemIn').DataTable().row($(this).closest('tr')).data()['ItemId'];
                        $('#kt_datatableItemIn').DataTable().row($(this).parents('tr')).remove().draw();

                    }
                });
            })
    };

    //#endregion ========= end Step 2 ==========
    //#region ======== Step 1توليف الاصناف الخارجة=================
    var initItemOut = function () {
        var table = $('#kt_datatableItemOut');

        // begin first table
        table.DataTable({
            responsive: true,
            searchDelay: 500,
            processing: true,
            serverSide: false,
            select: true,
            paging: false,
            info: false,
            lengthChange: false,

            // DOM Layout settings
            dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",
            language: {
                search: "البحث",
                lengthMenu: "عرض _MENU_ عنصر لكل صفحة",
                info: "العناصر من_START_ الي _END_ من اصل _TOTAL_ عنصر",
                processing: "جارى التحميل",
                zeroRecords: "لا يوجد سجلات لعرضها",
                infoFiltered: "",
                oPaginate: {
                    sNext: '<span class="pagination-default">التالى</span><span class="pagination-fa"><i class="fa fa-chevron-left" ></i></span>',
                    sPrevious: '<span class="pagination-default">السابق</span><span class="pagination-fa"><i class="fa fa-chevron-right" ></i></span>'
                }
                //infoEmpty: 'لا يوجد سجلات متاحه',
            },

            ajax: {
                url: '/ProductionOrders/GetDSItemOut',
                type: 'GET',
                data(d) {
                    d.itemProductionId = $("#ItemProductionId").val();
                    d.quantity = $("#OrderQuantity").val();
                    d.storeUnderId = $("#ProductionUnderStoreId").val();
                    d.isFirstInit = $("#isFirstInit").val();
                    d.ItemCostCalculateId = $("#ItemCostCalculateId").val();

                }
            },
            columns: [
                //{ data: 'Num', responsivePriority=1},
                //{ data: 'ItemProductionId', visible: false },
                //{ data: 'IsAllQuantityDone', visible: false },
                //{ data: 'ItemId', visible: false },
                //{ data: 'StoreUnderId', visible: false },
                //{ data: 'ItemCostCalculateId', visible: false },
                { data: 'ItemName', title: 'المنتج ' },
                //{ data: 'ItemCost', title: 'تكلفة المنتج' },
                { data: 'QuantityRequired', title: 'الكمية المطلوبة' },
                //{ data: 'QuantityAvailable', title: 'الكمية المتاحة' },

            ],
            columnDefs: [
               
                
            ],

            "order": [[0, "desc"]]
            //"order": [[0, "desc"]] 

        });
    };




    //#endregion ========= end Step 2 ==========

    //#region ======== Step 3 تسجيل المصروفات=================
    var initProductionOrderExpenseDT = function () {
        var table = $('#kt_dtProductionOrderExpenses');

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
                url: '/ProductionOrders/GetDStProductionOrderExpenses',
                type: 'GET',

            },
            columns: [
                { data: 'ExpenseTypeId', visible: false },
                { data: 'ExpenseTypeName', title: 'مسمى المصروف' },
                { data: 'ExpenseAmount', title: 'القيمة' },
                { data: 'Actions', responsivePriority: -1 },

            ],
            columnDefs: [
                //{
                //    targets: 1,
                //    title: 'م',
                //    orderable: false,
                //    render: function (data, type, row, meta) {
                //        return  meta.row + meta.settings._iDisplayStart + 1;
                //    },
                //},
                {
                    targets: -1,
                    title: 'عمليات',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return '\
							<div class="btn-group">\
							<a href="javascript:;" onclick=ProductionOrder_Module.deleteRowProductionOrderExpenses()  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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

    function addProductionOrderExpenses() {
        try {
            var expenseTypeId = document.getElementById('ExpenseTypeId').value;
            var expenseAmount = document.getElementById('ExpenseAmount').value;
            var notes = document.getElementById('Note').value;
            var formData = new FormData();
            if (ExpenseTypeId === '') {
                toastr.error('تأكد من اختيار مسمى المصروف', '');
                return false;
            };
            if (expenseAmount === '' || expenseAmount == "0") {
                toastr.error('تأكد من ادخال  قيمة المصروف', '');
                return false;
            };

            formData.append('ExpenseTypeId', expenseTypeId)
            formData.append('ExpenseAmount', expenseAmount)
            formData.append('Notes', notes)
            var dataSet = $('#kt_dtProductionOrderExpenses').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/ProductionOrders/AddProductionOrderExpenses',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtProductionOrderExpenses').DataTable().ajax.reload();
                        $('#ExpenseTypeId').val('');
                        $('#accountTree').val(null);
                        $('#Note').val('');
                        $('#ExpenseAmount').val(0);
                        toastr.success(res.msg, '');
                        $("#TotalExpenses").text(res.totalExpenses);
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

    function deleteRowProductionOrderExpenses() {
        $('#kt_dtProductionOrderExpenses tbody').on('click', 'a.deleteIcon', function () {
            var amountExpenseRemoved = $('#kt_dtProductionOrderExpenses').DataTable().row($(this).closest('tr')).data()['ExpenseAmount'];
            $("#TotalExpenses").text(Number.parseFloat($("#TotalExpenses").text()) - amountExpenseRemoved);
            $('#kt_dtProductionOrderExpenses').DataTable().row($(this).parents('tr')).remove().draw();
        })

    };



    //#endregion ========= end Step 2 ==========

    //remove notes 18-10-2021
    //function getStoresOnBranchChanged() {  // get safes and stores by branchId
    //    $.get("/SharedDataSources/getStoresOnBranchChanged", { id: $("#BranchId").val() }, function (data) {
    //        $("#StoreId").empty();
    //        $("#StoreId").append("<option value=>اختر عنصر من القائمة </option>");
    //        $.each(data, function (index, row) {
    //            $("#StoreId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
    //        });
    //    });
    //};


    function onOrderColorChange() {
        $('#OrderColor').val($("#OrderColorIn").val());
    };
    function getBalanceOnStoreChange() {
        $.get("/SharedDataSources/GetBalanceByStore/", { itemId: $("#ItemId").val(), storeId: $("#ProductionUnderStoreId").val() }, function (data) {
            $("#balanceCurrentStore").text(data.balance);
        });
         $.get("/SharedDataSources/GetPriceOnItemCostCalculateChange", { itemCostCalcId: $("#ItemCostCalculateId").val(), itemId: $("#ItemId").val() }, function (res) {
              $("#ItemCost").val(res.price);
              });
        //to prevent default form submit event
        return false;

    };
    function onQuantityChangeDiffBalance() { // التأكد من ان رصيد المخزن يسمح بالكمية المدخلة 
        if ($("#Quantity").val() === '' || $("#Quantity").val() === 'undefined') {
            toastr.error('تأكد من ادخال الكمية ', '');
            return false;
        }
        if ($("#Quantity").val() > parseFloat($("#balanceCurrentStore").text()) ) {
            toastr.error('رصيد المخزن لا يسمح بالكمية المدخلة ', '');
            return false;
        }
        addOtherItemIn();
    };

    //function getPriceOnItemCostCalculateChange() { // تكلفة المنتج بدلالة (متوسط سعر الشراء - اخر سعر شراء - اعلى او اقل سعر شراء )
    //    $.get("/SharedDataSources/GetPriceOnItemCostCalculateChange", { itemCostCalcId: $("#ItemCostCalculateId").val(), itemId: $("#ItemId").val() }, function (res) {
    //        $("#ItemCost").val(res.price);

    //    });
    //    return false;
    //};
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        //step2
        initItemIn: initItemIn,
        initItemOut: initItemOut,
        initProductionOrderExpense: function () {
            initProductionOrderExpenseDT();
        },
        addOtherItemIn: addOtherItemIn,
        deleteRowItemIn: deleteRowItemIn,
        addItemProductionDetails: addItemProductionDetails,
        onOrderColorChange: onOrderColorChange,
        getBalanceOnStoreChange: getBalanceOnStoreChange,
        onQuantityChangeDiffBalance: onQuantityChangeDiffBalance,
        addProductionOrderExpenses: addProductionOrderExpenses,
        deleteRowProductionOrderExpenses: deleteRowProductionOrderExpenses
    };

}();

