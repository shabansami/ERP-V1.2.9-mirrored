"use strict";

var MaintenanceItem_Module = function () {

    //#region ======== Save Purchase invoice ==================

    function SubmitForm(btn) {
        try {
            var form = document.getElementById('kt_form');
            var formData = new FormData(form);
            var dataSet = $('#kt_dtItemSpareParts').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_DatasourceItemSpareParts", JSON.stringify(dataSet));
                }
            }
            var dataSetDamage = $('#kt_dtItemDamages').DataTable().rows().data().toArray();
            if (dataSetDamage != null) {
                if (dataSetDamage.length > 0) {
                    formData.append("DT_DatasourceItemDamages", JSON.stringify(dataSetDamage));
                }
            }
            var dataSetExpense = $('#kt_dtMaintenanceIncomes').DataTable().rows().data().toArray();
            if (dataSetExpense != null) {
                if (dataSetExpense.length > 0) {
                    formData.append("DT_DatasourceItemIncomes", JSON.stringify(dataSetExpense));
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
                        toastr.success(res.message, '',)
                        setTimeout(function () { window.location = "/MaintenanceItems/Index/?invoGuid=" + res.id }, 3000);
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

    function deleteRow(invoGuid) {
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
                var url = '/MaintenanceItems/Delete';
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

    //#endregion ============== end ==============

    //#region ========== Step 1 البيانات الاساسية===============


    //#endregion ========== end Step 1 ============

    //#region ======== Step 2 تسجيل الاصناف ف الفاتورة=================
    var initDTItemSpareParts = function () {
        var table = $('#kt_dtItemSpareParts');

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
                url: '/MaintenanceItems/GetDSItemSpareParts',
                type: 'GET',

            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'ItemName', title: 'الصنف' },
                { data: 'Quantity', title: 'الكمية' },
                { data: 'Price', title: 'سعر البيع' },
                { data: 'Amount', title: 'القيمة' },
                { data: 'StoreName', title: 'المخزن' },
                { data: 'ItemDiscount', title: 'قيمة الخصم' },
                { data: 'ProductionOrderId', title: 'امر الانتاج' },
                { data: 'Actions', responsivePriority: -1 },

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
                        return '\
							<div class="btn-group">\
							<a href="javascript:;" onclick=MaintenanceItem_Module.deleteRowItemSpareParts("'+ row.Id + '")  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
								<i class="la la-trash"></i>\
							</a></div>\
						';
                    },
                }

            ],

            "order": [[0, "asc"]]
            //"order": [[0, "desc"]] 

        });
    };

    function addItemSpareParts() {
        try {
            var itemId = document.getElementById('ItemId').value;
            var price = document.getElementById('Price').value;
            var quantity = document.getElementById('Quantity').value;
            var amount = document.getElementById('Amount').value;
            var itemDiscount = document.getElementById('ItemDiscount').value;
            var storeId = document.getElementById('StoreId').value;
            var serialItemId = document.getElementById('serialItemId').value;
            var balanceVal = document.getElementById('balanceVal').value;
            var productionOrderId = document.getElementById('productionOrderId').value;
            var isIntial = document.getElementById('isIntial').value;
            var isDiscountItemVal = document.getElementById('IsDiscountItemVal').value;
            var formData = new FormData();
            if (itemId === '') {
                toastr.error('تأكد من اختيار الصنف', '');
                return false;
            };
            if (price === '' || price == '0') {
                toastr.error('تأكد من ادخال سعر البيع', '');
                return false;
            };
            if (quantity === '' || quantity == '0') {
                toastr.error('تأكد من ادخال الكمية', '');
                return false;
            };
            if (quantity > parseFloat(balanceVal)) {
                toastr.error('الكمية المدخلة اكبر من الرصيد المتاح', '');
                return false;
            };
            if (storeId === '') {
                toastr.error('تأكد من اختيار المخزن', '');
                return false;
            };
            if (Number.parseFloat(itemDiscount) > Number.parseFloat(amount) && isDiscountItemVal === 'true') {
                toastr.error('قيمة الخصم اكبر من قيمة الصنف', '');
                return false;
            }
            if ($("#IsDiscountItemVal").val === '') {
                toastr.error('تأكد من اختيار احتساب الخصم', '');
                return false;
            }

            formData.append('ItemId', itemId)
            formData.append('Price', price)
            formData.append('Quantity', quantity)
            formData.append('Amount', amount)
            formData.append('ItemDiscount', itemDiscount)
            formData.append('StoreId', storeId)
            formData.append('SerialItemId', serialItemId)
            formData.append('ProductionOrderId', productionOrderId)
            formData.append('IsIntial', isIntial)
            formData.append('IsDiscountItemVal', isDiscountItemVal)
            var dataSet = $('#kt_dtItemSpareParts').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/MaintenanceItems/AddItemSpareParts',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtItemSpareParts').DataTable().ajax.reload();
                        $('#Price').val(0);
                        $('#Quantity').val(0);
                        $('#Amount').val(0);
                        $('#ItemDiscount').val(0);
                        $('#PricePolicyId').val(null);
                        $('#balanceProductionOrder').empty();
                        $('#balanceVal').val(null);
                        $('#ItemBarcode').val(null);

                        $('#productionOrderId').val(null);
                        $('#isIntial').val(null);
                        $('#ItemId').val(null);
                        $('#ItemId').select2({
                            placeholder: "اختر عنصر من القائمة"
                        });
                        $('#StoreId').val(null);
                        $('#StoreId').select2({
                            placeholder: "اختر عنصر من القائمة"
                        });

                        $('#TotalItemSpareParts,#TotalItemSpareParts2').text(Number.parseFloat(res.totalAmount));
                        $("#TotalItemDiscount").text(Number.parseFloat(res.totalDiscountItems));
                        getSafyInvoice();
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

    function deleteRowItemSpareParts(id) {
        $('#kt_dtItemSpareParts tbody').on('click', 'a.deleteIcon', function () {
            var amountRemoved = $('#kt_dtItemSpareParts').DataTable().row($(this).closest('tr')).data()['Amount'];
            var itemDiscountRemoved = $('#kt_dtItemSpareParts').DataTable().row($(this).closest('tr')).data()['ItemDiscount'];
            $("#TotalItemSpareParts,#TotalItemSpareParts2").text(Number.parseFloat($("#TotalItemSpareParts").text()) - amountRemoved);
            $("#TotalItemDiscount").text(Number.parseFloat($("#TotalItemDiscount").text()) - itemDiscountRemoved);
            $('#kt_dtItemSpareParts').DataTable().row($(this).parents('tr')).remove().draw();
            getSafyInvoice();
        })

    };
    //الخصم على الاصناف
    function onRdoValChanged() {
        if ($("#rdo_val:checked").val()) {
            $("#IsDiscountItemVal").val(true)
        }
    };
    function onRdoPercentageChanged() {
        if ($("#rdo_percentage:checked").val()) {
            $("#IsDiscountItemVal").val(false)
        }
    };
    function onRdoBarcodeChanged() {
        if ($("#rdo_barcode:checked").val()) {
            $("#barcodeDiv").show();
            $('#ItemBarcode').removeAttr('disabled');
            $('#ItemSerial').attr('disabled', 'disabled');
            $("#serialDiv").hide();

            $("#serialItemId").val(null);
            $("#productionOrderId").val(null);
            $("#isIntial").val(null);

            $("#rdo_barcode:checked").prop('cheched', true);

        }
    }
    function onRdoSerialChanged() {
        if ($("#rdo_serial:checked").val()) {
            $("#barcodeDiv").hide();
            $('#ItemSerial').removeAttr('disabled');
            $('#ItemBarcode').attr('disabled', 'disabled');
            $("#serialDiv").show();

        }
    };
    //#endregion ========= end Step 2 ==========
    //#region ======== Step 3 تسجيل التوالف=================
    var initDTItemDamages = function () {
        var table = $('#kt_dtItemDamages');

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
                url: '/MaintenanceItems/GetDSItemDamages',
                type: 'GET',

            },
            columns: [
                { data: 'ItemId', visible: false },
                { data: 'ItemName', title: 'الصنف' },
                { data: 'Quantity', title: 'الكمية' },
                //{ data: 'Price', title: 'سعر البيع' },
                //{ data: 'Amount', title: 'القيمة' },
                { data: 'Actions', responsivePriority: -1 },

            ],
            columnDefs: [
                {
                    targets: -1,
                    title: 'عمليات',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return '\
							<div class="btn-group">\
							<a href="javascript:;" onclick=MaintenanceItem_Module.deleteRowItemDamages("'+ row.Id + '")  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
								<i class="la la-trash"></i>\
							</a></div>\
						';
                    },
                }

            ],

            "order": [[0, "asc"]]
            //"order": [[0, "desc"]] 

        });
    };

    function addItemDamages() {
        try {
            var itemId = document.getElementById('ItemDamageId').value;
            //var price = document.getElementById('Price').value;
            var quantity = document.getElementById('QuantityDamage').value;
            //var amount = document.getElementById('Amount').value;
            var formData = new FormData();
            if (itemId === '') {
                toastr.error('تأكد من اختيار الصنف', '');
                return false;
            };
            //if (price === '' || price=='0') {
            //    toastr.error('تأكد من ادخال سعر البيع', '');
            //    return false;
            //};
            if (quantity === '' || quantity == '0') {
                toastr.error('تأكد من ادخال الكمية', '');
                return false;
            };
            //if (quantity > parseFloat(balanceVal)) {
            //    toastr.error('الكمية المدخلة اكبر من الرصيد المتاح', '');
            //    return false;
            //};

            formData.append('ItemId', itemId)
            //formData.append('Price', price)
            formData.append('Quantity', quantity)
            //formData.append('Amount', amount)
            var dataSet = $('#kt_dtItemDamages').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/MaintenanceItems/AddItemDamages',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtItemDamages').DataTable().ajax.reload();
                        //$('#ItemId').val('');
                        //$('#Price').val(0);
                        $('#QuantityDamage').val(0);
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

    function deleteRowItemDamages(id) {
        $('#kt_dtItemDamages tbody').on('click', 'a.deleteIcon', function () {
            //var amountRemoved = $('#kt_dtItemDamages').DataTable().row($(this).closest('tr')).data()['Amount'];
            $('#kt_dtItemDamages').DataTable().row($(this).parents('tr')).remove().draw();
            getSafyInvoice();
        })

    };

    //#endregion ========= end Step 2 ==========


    //#region ======== Step 3 تسجيل الايرادات=================
    var initDTMaintenanceIncomes = function () {
        var table = $('#kt_dtMaintenanceIncomes');

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
                url: '/MaintenanceItems/GetDStMaintenanceIncomes',
                type: 'GET',

            },
            columns: [
                { data: 'ExpenseTypeName', title: 'مسمى الإيراد' },
                { data: 'ExpenseAmount', title: 'القيمة' },
                { data: 'Actions', responsivePriority: -1 },

            ],
            columnDefs: [
                {
                    targets: -1,
                    title: 'عمليات',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return '\
							<div class="btn-group">\
							<a href="javascript:;" onclick=MaintenanceItem_Module.deleteRowMaintenanceIncome("'+ row.Id + '")  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
								<i class="la la-trash"></i>\
							</a></div>\
						';
                    },
                }

            ],

            "order": [[0, "asc"]]
            //"order": [[0, "desc"]] 

        });
    };

    function addMaintenanceIncomes() {
        try {
            var ExpenseTypeName = document.getElementById('ExpenseTypeName').value;
            var expenseAmount = document.getElementById('ExpenseAmount').value;
            var formData = new FormData();
            if (ExpenseTypeName === '') {
                toastr.error('تأكد من ادخال بيان الإيراد', '');
                return false;
            };
            if (expenseAmount === '' || expenseAmount == "0") {
                toastr.error('تأكد من ادخال  قيمة الإيراد', '');
                return false;
            };

            formData.append('ExpenseTypeName', ExpenseTypeName)
            formData.append('ExpenseAmount', expenseAmount)
            var dataSet = $('#kt_dtMaintenanceIncomes').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/MaintenanceItems/AddMaintenanceIncomes',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtMaintenanceIncomes').DataTable().ajax.reload();
                        $('#ExpenseTypeName').val('');
                        $('#ExpenseAmount').val(0);
                        toastr.success(res.msg, '');
                        $("#TotalItemIncomes,#TotalItemIncomes2").text(res.totalItemIncomes);
                        getSafyInvoice();
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

    function deleteRowMaintenanceIncome(id) {
        $('#kt_dtMaintenanceIncomes tbody').on('click', 'a.deleteIcon', function () {
            var amountExpenseRemoved = $('#kt_dtMaintenanceIncomes').DataTable().row($(this).closest('tr')).data()['ExpenseAmount'];
            $("#TotalItemIncomes,#TotalItemIncomes2").text(Number.parseFloat($("#TotalItemIncomes").text()) - amountExpenseRemoved);
            $('#kt_dtMaintenanceIncomes').DataTable().row($(this).parents('tr')).remove().draw();
            getSafyInvoice();

        })

    };

    function onPriceOrQuanKeyUp() {
        if ($("#Price").val() < 0 || $("#Quantity").val() < 0 || isNaN($("#Price").val()) || isNaN($("#Quantity").val())) {
            toastr.error('تأكد من ادخال ارقام صحيحية للعدد والكمية', '');
        } else
            $("#Amount").val($("#Price").val() * $("#Quantity").val());
    };



    //#endregion ========= end Step 2 ==========
    //صافى قيمة الفاتورة= ( إجمالي قيمة البيمعات + إجمالي قيمة الايرادات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )
    function getSafyInvoice() {
        var TotalItemSpareParts = Number.parseFloat($("#TotalItemSpareParts").text());
        if (isNaN(TotalItemSpareParts))
            TotalItemSpareParts = 0;

        var TotalItemIncomes = Number.parseFloat($("#TotalItemIncomes").text());
        if (isNaN(TotalItemIncomes))
            TotalItemIncomes = 0;

        var TotalItemDiscount = Number.parseFloat($("#TotalItemDiscount").text());
        if (isNaN(TotalItemDiscount))
            TotalItemDiscount = 0;

        var safy = (TotalItemSpareParts + TotalItemIncomes) - (TotalItemDiscount);
        $("#ItemSafy").text(safy);
    };


    function getProductionOrdersOnStoreChange() {
        $.get("/SharedDataSources/GetProductionOrdersOnStoreChange/", { itemId: $("#ItemId").val(), storeId: $("#StoreId").val() }, function (data) {
            var isSelected = "";
            $("#balanceProductionOrder").empty();
            var t = data[0].Val
            var obj2 = JSON.parse(JSON.stringify(t));
            var obj = JSON.parse(obj2);
            var balance = obj.balance;
            var productionOrderId = obj.productOrder;
            var isIntial = obj.isIntial;
            $("#balanceVal").val(balance);
            $("#productionOrderId").val(productionOrderId);
            $("#isIntial").val(isIntial);
            if (data.length == 1) {
                isSelected = "selected";
            } else {
                $("#balanceProductionOrder").append("<option value=>اختر عنصر من القائمة</option>");
            };
            $.each(data, function (index, data) {
                $("#balanceProductionOrder").append("<option value='" + data.Val + "' " + isSelected + ">" + data.Text + "</option>");
            });


        });
        //to prevent default form submit event
        return false;

    };
    function onProductionOrderChange() {
        var t = $("#balanceProductionOrder").val();
        var obj2 = JSON.parse(JSON.stringify(t));
        var obj = JSON.parse(obj2);
        var balance = obj.balance;
        var productionOrderId = obj.productOrder;
        var isIntial = obj.isIntial;
        $("#balanceVal").val(balance);
        $("#productionOrderId").val(productionOrderId);
        $("#isIntial").val(isIntial);
    };
    function onItemChange() {
        $('#StoreId').val(null);
        $('#StoreId').select2({
            placeholder: "اختر عنصر من القائمة"
        });
        $("#balanceVal").val(null);
        $("#serialItemId").val(null);
        $("#productionOrderId").val(null);
        $("#isIntial").val(null);
        $("#balanceProductionOrder").val(null);
        //سعر بيع الصنف الافتراضى المسجل 
        $.get("/SharedDataSources/GetDefaultSellPrice/", { itemId: $("#ItemId").val() }, function (data) {
            $("#Price").val(data.data);
            $("#Amount").val(data.data * $("#Quantity").val());
        });
    };
    //تغيير سعر البيع حسب سياسة البيع المحدد
    function onPricingPolicyChange() {
        $.get("/SharedDataSources/GetPricePolicySellPrice/", { itemId: $("#ItemId").val(), pricePolicyId: $("#PricingPolicyId").val(), customerId: $("#CustomerId").val() }, function (data) {
            $("#Price").val(data.data);
        });
    };
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        //step 1
        //step2
        initItemSpareParts: function () {
            initDTItemSpareParts();
        },
        addItemSpareParts: addItemSpareParts,
        deleteRowItemSpareParts: deleteRowItemSpareParts,
        onPriceOrQuanKeyUp: onPriceOrQuanKeyUp,
        onRdoValChanged: onRdoValChanged,
        onRdoPercentageChanged: onRdoPercentageChanged,
        onRdoBarcodeChanged: onRdoBarcodeChanged,
        onRdoSerialChanged: onRdoSerialChanged,

        //step3
        initItemDamages: function () {
            initDTItemDamages();
        },
        addItemDamages: addItemDamages,
        deleteRowItemDamages: deleteRowItemDamages,
        //step4
        initMaintenanceIncomes: function () {
            initDTMaintenanceIncomes();
        },
        addMaintenanceIncomes: addMaintenanceIncomes,
        deleteRowMaintenanceIncome: deleteRowMaintenanceIncome,
        getSafyInvoice: getSafyInvoice,
        getProductionOrdersOnStoreChange: getProductionOrdersOnStoreChange,
        onProductionOrderChange: onProductionOrderChange,
        onItemChange: onItemChange,
        onPricingPolicyChange: onPricingPolicyChange,
    };

}();

