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
                url: '/ProductionOrderFirst/GetAll',
                type: 'GET',

            },
            columns: [
                { data: 'Id', title: 'رقم أمر الإنتاج' },
                { data: 'OrderNumber', visible: false },
                { data: 'IsDone', visible: false },
                { data: 'ProductionOrderDate', title: 'تاريخ امر الإنتاج' },
                { data: 'FinalItemName', title: 'الصنف النهائى' },
                { data: 'OrderQuantity', title: 'الكمية' },
                { data: 'IsDoneTitle', title: 'حالة التسليم لمخازن المنتج التام' },
                { data: 'Actions', responsivePriority: -1 },

            ],
            columnDefs: [

                {
                    targets: -1,
                    title: 'عمليات',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        if (row.IsDone) {
                            return '\
							<div class="btn-group">\
							<a href="/ProductionOrders/ShowProductionOrder/?ordrGud=' +
                                row.Id +
                                '" class="btn btn-sm btn-clean btn-icon" title="عرض ملخص أمر اللإنتاج">\
								<i class="fa fa-search"></i>\
							</a>\</div>\
						';
                        } else {
                            return '\
							<div class="btn-group">\
							<a href="/ProductionOrders/ShowProductionOrder/?ordrGud=' +
                                row.Id +
                                '" class="btn btn-sm btn-clean btn-icon" title="عرض ملخص أمر اللإنتاج">\
								<i class="fa fa-search"></i>\
							</a>\<a href="/ProductionOrderDamages/?ordrGud=' +
                                row.Id +
                                '" class="btn btn-sm btn-clean btn-icon" title="تسجيل توالف">\
								<i class="fa fa-house-damage"></i>\
							</a>\
							<a href="javascript:;" onclick=ProductionOrder_Module.deleteRow("' +
                                row.Id +
                                '") class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a></div>\
						';
                        }


                    },
                }

            ],

            "order": [[0, "desc"]]
            //"order": [[0, "desc"]] 

        });
    };

    function SubmitForm(btn) {
        try {
            var form = document.getElementById('kt_form');
            var formData = new FormData(form);
            var dataSetItemProduction = $('#kt_datatableItemProduction').DataTable().rows().data().toArray();
            if (dataSetItemProduction != null) {
                if (dataSetItemProduction.length > 0) {
                    formData.append("DT_DatasourceItemProduction", JSON.stringify(dataSetItemProduction));
                }
            };
            var dataSet = $('#kt_datatableItemMaterial').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_DatasourceItemMaterials", JSON.stringify(dataSet));
                }
            }
            var dataSetExpense = $('#kt_dtProductionOrderExpenses').DataTable().rows().data().toArray();
            if (dataSetExpense != null) {
                if (dataSetExpense.length > 0) {
                    formData.append("DT_DatasourceExpenses", JSON.stringify(dataSetExpense));
                }
            }

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
                        setTimeout(function () { window.location = "/ProductionOrderFirst/RegisterOrder" }, 3000);



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
                var url = '/ProductionOrderFirst/Delete';
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


    //#region ======== Step 1 عرض توليفة منتج نهائى=================
    var initItemProductionDT = function () {
        var table = $('#kt_datatableItemProduction');

        // begin first table
        table.DataTable({
            responsive: true,
            searchDelay: 500,
            processing: true,
            serverSide: false,
            select: true,
            searching: false,
            paging: false,
            info: false,
            // DOM Layout settings
            //dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",
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
                url: '/ProductionOrderFirst/GetDSItemProduction',
                type: 'GET',
                data(d) {
                    d.itemFinalId = $("#FinalItemId").val();
                    d.quantity = $("#OrderQuantity").val();
                    d.branchId = $("#BranchId").val();
                    d.isFirstInit = $("#isFirstInit").val();
                }

            },
            columns: [
                { data: 'ItemProductionId', visible: false },
                { data: 'ItemProductionId', visible: false },
                { data: 'ItemId', visible: false },
                { data: 'ItemName', title: 'المنتج الخام' },
                { data: 'QuantityRequired', title: 'الكمية المطلوبة' },
                { data: 'QuantityAvailable', title: 'الكمية المتاحة' },
                { data: 'Actions', responsivePriority: -1 },


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
                {
                    targets: -1,
                    title: 'الحالة',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        var status = {
                            1: { 'title': 'Pending', 'class': 'label-light-primary' },
                            2: { 'title': 'Delivered', 'class': ' label-light-danger' },
                            3: { 'title': 'Canceled', 'class': ' label-light-primary' },
                            4: { 'title': 'مكتمل', 'class': ' label-light-success' },
                            5: { 'title': 'Info', 'class': ' label-light-info' },
                            6: { 'title': 'غير مكتمل', 'class': ' label-light-danger' },
                            7: { 'title': 'Warning', 'class': ' label-light-warning' },
                        };
                        if (row.IsAllQuantityDone) {
                            return '<span class="label label-lg font-weight-bold' + status[4].class + ' label-inline">' + status[4].title + '</span>';
                        } else {
                            return '<span class="label label-lg font-weight-bold' + status[6].class + ' label-inline">' + status[6].title + '</span>';
                        }
                    },

                    //              render: function (data, type, row, meta) {
                    //                  return '\
                    //	<div class="btn-group">\
                    //	<a href="#" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
                    //		<i class="fa fa-close"></i>\
                    //	</a>\
                    //	</div>\
                    //';
                    //              },
                }

            ],

            "order": [[0, "desc"]]
            //"order": [[0, "desc"]] 

        });
    };
    var initItemMaterialDT = function () {
        var table = $('#kt_datatableItemMaterial');

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
                url: '/ProductionOrderFirst/GetDSItemMaterials',
                type: 'GET',

            },
            columns: [
                { data: 'Id', visible: false },
                { data: 'ItemId', visible: false },
                { data: 'StoreId', visible: false },
                { data: 'ItemCostCalculateId', visible: false },
                { data: 'Quantitydamage', visible: false },
                { data: 'ItemName', title: 'المنتج الخام' },
                { data: 'StoreName', title: 'المخزن' },
                { data: 'ItemCostCalculateName', title: 'طريقة احتساب التكلفة' },
                { data: 'ItemCost', title: 'تكلفة المنتج' },
                { data: 'Quantity', title: 'الكمية' },
                { data: 'Actions', responsivePriority: -1 },

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
                {
                    targets: -1,
                    title: 'عمليات',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return '\
							<div class="btn-group">\
							<a href="javascript:;" onclick=ProductionOrder_Module.deleteRowItemMaterial()  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a></div>\
						';
                    },
                }

            ],

            "order": [[0, "desc"]]
            //"order": [[0, "desc"]] 

        });
    };

    function addItemMaterials() {
        try {
            var itemId = document.getElementById('ItemId').value;
            var storeId = document.getElementById('StoreId').value;
            var quantity = document.getElementById('Quantity').value;
            var itemCostCalculateId = document.getElementById('ItemCostCalculateId').value;
            var itemCost = document.getElementById('ItemCost').value;
            var orderQuantity = document.getElementById('OrderQuantity').value;
            var finalItemId = document.getElementById('FinalItemId').value;
            var formData = new FormData();
            if (itemId === '') {
                toastr.error('تأكد من اختيار الصنف', '');
                return false;
            };
            if (storeId === '') {
                toastr.error('تأكد من اختيار المخزن', '');
                return false;
            };
            if (quantity === '' || quantity == '0') {
                toastr.error('تأكد من ادخال الكمية', '');
                return false;
            };
            if (itemCost === '') {
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
            if (finalItemId === '') {
                toastr.error('تأكد من اختيار المنتج النهائى', '');
                return false;
            };
            formData.append('ItemId', itemId)
            formData.append('StoreId', storeId)
            formData.append('Quantity', quantity)
            formData.append('ItemCostCalculateId', itemCostCalculateId)
            formData.append('ItemCost', itemCost)
            formData.append('orderQuantity', orderQuantity)
            formData.append('finalItemId', finalItemId)
            //اصناف المواد الخام التى تم توليفها سابقا
            var dataSetItemProduction = $('#kt_datatableItemProduction').DataTable().rows().data().toArray();
            if (dataSetItemProduction != null) {
                if (dataSetItemProduction.length > 0) {
                    formData.append("DT_DatasourceItemProduction", JSON.stringify(dataSetItemProduction));
                }
            };

            //اصناف المواد الخام التى تم اضافتها
            var dataSet = $('#kt_datatableItemMaterial').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_DatasourceMaterial", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/ProductionOrderFirst/AddItemMaterials',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_datatableItemMaterial').DataTable().ajax.reload();
                        $('#StoreId').val('');
                        $('#Quantity').val(0);
                        $('#ItemCostCalculateId').val(null);
                        $('#ItemCost').val(0);
                        $('#balanceCurrentStore').text(0);

                        //فى حالة اكتمال العدد المطلوب من المواد الخام يتم تحديث جريد اصناف التوليفة 
                        if (res.isAllQuantityDone != 'undefined') {
                            if (res.isAllQuantityDone) {
                                $("#isFirstInit").val('1');
                                $('#kt_datatableItemProduction').DataTable().ajax.reload();
                            }
                        }
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

    function deleteRowItemMaterial() {
        $('#kt_datatableItemMaterial tbody').on('click', 'a.deleteIcon', function () {
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
                    var itemIdRemoved = $('#kt_datatableItemMaterial').DataTable().row($(this).closest('tr')).data()['ItemId'];
                    $('#kt_datatableItemMaterial').DataTable().row($(this).parents('tr')).remove().draw();


                    var url = '/ProductionOrderFirst/UpdateItemProduction';
                    //اصناف المواد الخام التى تم توليفها سابقا
                    var DT_DatasourceItemProduction;
                    var dataSetItemProduction = $('#kt_datatableItemProduction').DataTable().rows().data().toArray();
                    if (dataSetItemProduction != null) {
                        if (dataSetItemProduction.length > 0) {
                            DT_DatasourceItemProduction = JSON.stringify(dataSetItemProduction);
                        }
                    };
                    //اصناف المواد الخام التى تم توليفها سابقا
                    var DT_DatasourceMaterial;
                    var dataSetItemMaterial = $('#kt_datatableItemMaterial').DataTable().rows().data().toArray();
                    if (dataSetItemMaterial != null) {
                        /* if (dataSetItemMaterial.length > 0) {*/
                        DT_DatasourceMaterial = JSON.stringify(dataSetItemMaterial);
                        //}
                    };
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: {
                            "itemId": itemIdRemoved,
                            'DT_DatasourceItemProduction': DT_DatasourceItemProduction,
                            'DT_DatasourceMaterial': DT_DatasourceMaterial
                        },
                        //async: true,
                        //headers: { 'RequestVerificationToken': $('@Html.AntiForgeryToken()').val() },
                        success: function (data) {
                            if (data.isValid) {
                                toastr.success(data.message, '');
                                //$('#kt_datatableItemMaterial').DataTable().ajax.reload();

                                if (data.updateItemOrderDT) {
                                    $("#isFirstInit").val('1');
                                    $('#kt_datatableItemProduction').DataTable().ajax.reload();
                                }
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
        })
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
            },

            ajax: {
                url: '/ProductionOrderFirst/GetDStProductionOrderExpenses',
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
                url: '/ProductionOrderFirst/AddProductionOrderExpenses',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtProductionOrderExpenses').DataTable().ajax.reload();
                        $('#ExpenseTypeId').val('');
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

    function getStoresOnBranchChanged() {  // get safes and stores by branchId
        $.get("/SharedDataSources/getStoresOnBranchChanged", { id: $("#BranchId").val() }, function (data) {
            $("#StoreId").empty();
            $("#StoreId").append("<option value=>اختر عنصر من القائمة </option>");
            $.each(data, function (index, row) {
                $("#StoreId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });
        });
    };

    function onOrderQuantityChange() {
        if ($("#OrderQuantity").val() === '0' || $("#OrderQuantity").val() == '') {
            toastr.error('تأكد من ادخال كمية', '');
            $('#kt_datatableItemProduction').dataTable().fnClearTable();
            $('#kt_datatableItemMaterial').dataTable().fnClearTable();
            $("#isFirstInit").val('0');
        } else {
            $("#isFirstInit").val('0');
            $('#kt_datatableItemProduction').DataTable().ajax.reload();
            $('#kt_datatableItemMaterial').dataTable().fnClearTable();
        }

    };

    function onOrderColorChange() {
        $('#OrderColor').val($("#OrderColorIn").val());
    };
    function getBalanceOnStoreChange() {
        $.get("/SharedDataSources/GetBalanceByStore/", { itemId: $("#ItemId").val(), storeId: $("#StoreId").val() }, function (data) {
            $("#balanceCurrentStore").text(data.balance);
        });
        //to prevent default form submit event
        return false;

    };
    function onQuantityChangeDiffBalance() { // التأكد من ان رصيد المخزن يسمح بالكمية المدخلة 
        if ($("#Quantity").val() === '' || $("#Quantity").val() === 'undefined') {
            toastr.error('تأكد من ادخال الكمية ', '');
            return false;
        }
        if ($("#Quantity").val() > parseFloat($("#balanceCurrentStore").text())) {
            toastr.error('رصيد المخزن لا يسمح بالكمية المدخلة ', '');
            return false;
        }
    };

    function getPriceOnItemCostCalculateChange() { // تكلفة المنتج بدلالة (متوسط سعر الشراء - اخر سعر شراء - اعلى او اقل سعر شراء )
        $.get("/SharedDataSources/GetPriceOnItemCostCalculateChange", { itemCostCalcId: $("#ItemCostCalculateId").val(), itemId: $("#ItemId").val() }, function (res) {
            $("#ItemCost").val(res.price);

        });
        return false;
    };
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        //step2
        initItemProduction: function () {
            initItemProductionDT();
        },
        initItemMaterial: function () {
            initItemMaterialDT();
        },
        initProductionOrderExpense: function () {
            initProductionOrderExpenseDT();
        },
        addItemMaterials: addItemMaterials,
        deleteRowItemMaterial: deleteRowItemMaterial,
        getStoresOnBranchChanged: getStoresOnBranchChanged,
        onOrderQuantityChange: onOrderQuantityChange,
        onOrderColorChange: onOrderColorChange,
        getBalanceOnStoreChange: getBalanceOnStoreChange,
        onQuantityChangeDiffBalance: onQuantityChangeDiffBalance,
        getPriceOnItemCostCalculateChange: getPriceOnItemCostCalculateChange,

        addProductionOrderExpenses: addProductionOrderExpenses,
        deleteRowProductionOrderExpenses: deleteRowProductionOrderExpenses
    };

}();

