"use strict";

var SaleMenSellInvoice_Module = function () {

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
                        return 'فواتير البيع';
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
                    filename: "فواتير البيع",
                    title: "فواتير البيع",
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
                url: '/SaleMenSellInvoices/GetAll',
                type: 'GET',

                    },
            columns: [
                { data: 'Num', responsivePriority: 0 },
            { data: 'InvoiceNum', title: 'رقم الفاتورة' },
            { data: 'InvoiceDate', title: 'تاريخ الفاتورة' },
            { data: 'CustomerName', title: 'اسم العميل' },
            { data: 'Safy', title: 'صافى الفاتورة' },
            { data: 'ApprovalAccountant', title: 'حالة الاعتماد محاسبيا' },
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
							<a href="/SaleMenSellInvoices/Edit/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
								<i class="la la-edit"></i>\
							</a>\<a href="/SaleMenSellInvoices/ShowSaleMenSellInvoice/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض فاتورة">\
								<i class="la la-search"></i>\
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

    function SubmitForm(btn) {
        try {
            var form = document.getElementById('kt_form');
            var formData = new FormData(form);
            var dataSet = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_DatasourceItems", JSON.stringify(dataSet));
                }
            }

            var isInvoiceDisVal = document.getElementById("isInvoiceDisVal").value;
            formData.append("isInvoiceDisVal", isInvoiceDisVal);
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
                            if (!res.isInsert) {
                                setTimeout(function () { window.location = "/SaleMenSellInvoices/Index" }, 3000);
                            } else
                                setTimeout(function () { window.location = "/SaleMenSellInvoices/CreateEdit" }, 3000);
                       
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
                var url = '/SaleMenSellInvoices/Delete';
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

    //$.get("/SharedDataSources/getStoresOnBranchChanged", { id: $("#BranchId").val() }, function (data) {
    //    $("#SaleMenStoreId").empty();
    //    $("#SaleMenStoreId").append("<option value=>اختر عنصر من القائمة </option>");
    //    $.each(data, function (index, row) {
    //        $("#SaleMenStoreId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
    //    })
    //});


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
    //الخصم على قيمة القاتورة كلها 
    function onRdoInvoiceValChanged() {
        if ($("#rdo_valAllInvoice:checked").val()) {
            $("#isInvoiceDisVal").val(true);
            $("#TotalDiscount").text(Number.parseFloat($("#TotalDiscount").text()) - $("#InvoiceDiscount").val());
            $("#InvoiceDiscount").val(0);
            $("#DiscountPercentage").val(0);//edit
            $("#divDiscountPercentage").hide();//edit
            getSafyInvoice();
        }
    };
    //edit
    function onRdoInvoicePercentageChanged() {
        if ($("#rdo_percentageAllInvoice:checked").val()) {
            $("#isInvoiceDisVal").val(false);
            $("#divDiscountPercentage").show();
            $("#TotalDiscount").text(Number.parseFloat($("#TotalDiscount").text()) - Number.parseFloat($("#InvoiceDiscount").val()));
            $("#InvoiceDiscount").val(0);
            getSafyInvoice();

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
    //#endregion ========== end Step 1 ============

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
            },

            ajax: {
                url: '/SaleMenSellInvoices/GetDSItemDetails',
                type: 'GET',

            },
            columns: [
                { data: 'Id', title: 'م', visible: false },
                { data: 'ItemId', visible: false },
                { data: 'StoreId', visible: false },
                { data: 'SerialItemId', visible: false },
                { data: 'IsIntial', visible: false },
                { data: 'IsDiscountItemVal', visible: false },
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
							<a href="javascript:;" onclick=SaleMenSellInvoice_Module.deleteRowItemDetails('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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

    function addItemDetails() {
        try {
            var itemId = document.getElementById('ItemId').value;
            var price = document.getElementById('Price').value;
            var quantity = document.getElementById('Quantity').value;
            var amount = document.getElementById('Amount').value;
            var itemDiscount = document.getElementById('ItemDiscount').value;
            var storeId = document.getElementById('SaleMenStoreId').value;
            var serialItemId = document.getElementById('serialItemId').value;
            var balanceVal = document.getElementById('balanceVal').value;
            var productionOrderId = document.getElementById('productionOrderId').value;
            var isIntial = document.getElementById('isIntial').value;
            var isDiscountItemVal = document.getElementById('IsDiscountItemVal').value;
            var formData = new FormData();
            if (itemId === '') {
                toastr.error('تأكد من اختيار الصنف', '');
                $("#ItemId").select2('open');
                return false;
            };
            if (price === '' || price=='0') {
                toastr.error('تأكد من ادخال سعر البيع وفى حالة العينة تركها بصفر', '');
                $("#Price").focus().select();
                return false;
            };
            if (quantity === '' || quantity == '0') {
                toastr.error('تأكد من ادخال الكمية', '');
                $("#Quantity").focus().select();
                return false;
            };
            if (quantity > parseFloat(balanceVal)) {
                toastr.error('الكمية المدخلة اكبر من الرصيد المتاح', '');
                $("#Quantity").focus().select();
                return false;
            };
            if (storeId === '') {
                toastr.error('تأكد من اختيار المخزن', '');
                $("#StoreId").select2('open');
                return false;
            };
            if (Number.parseFloat(itemDiscount) > Number.parseFloat(amount) && isDiscountItemVal==='true') {
                toastr.error('قيمة الخصم اكبر من قيمة الصنف', '');
                $("#ItemDiscount").focus().select();
                return false;
            }
            if ($("#IsDiscountItemVal").val==='') {
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
            var dataSet = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/SaleMenSellInvoices/AddItemDetails',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtItemDetails').DataTable().ajax.reload();
                        //$('#ItemId').val('');
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
                        $('#serialItemId').val(null);
                        $('#ItemSerial').val(null);
                        $('#ItemId').val(null);
                        $('#ItemId').select2({
                            placeholder: "اختر عنصر من القائمة"
                        });


                        $('#TotalAmount,#TotalAmount2').text(res.totalAmount);
                        $('#TotalItemDiscount').text(res.totalDiscountItems);
                        $("#TotalDiscount").text(Number.parseFloat($("#TotalDiscount").text()) + Number.parseFloat(res.itemDiscount));
                        getSafyInvoice();
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

    function deleteRowItemDetails(id) {
        $('#kt_dtItemDetails tbody').on('click', 'a.deleteIcon', function () {
            var amountRemoved = $('#kt_dtItemDetails').DataTable().row($(this).closest('tr')).data()['Amount'];
            var itemDiscountRemoved = $('#kt_dtItemDetails').DataTable().row($(this).closest('tr')).data()['ItemDiscount'];
            $("#TotalAmount,#TotalAmount2").text(Number.parseFloat($("#TotalAmount").text()) - amountRemoved);
            $("#TotalItemDiscount").text(Number.parseFloat($("#TotalItemDiscount").text()) - itemDiscountRemoved);
            $('#kt_dtItemDetails').DataTable().row($(this).parents('tr')).remove().draw();
            $("#TotalDiscount").text(Number.parseFloat($("#TotalDiscount").text()) - itemDiscountRemoved);
            getSafyInvoice();
        })

    };

    //#endregion ========= end Step 2 ==========



    function onPriceOrQuanKeyUp() {
        if ($("#Price").val() < 0 || $("#Quantity").val() < 0 || isNaN($("#Price").val()) || isNaN($("#Quantity").val())) {
            toastr.error('تأكد من ادخال ارقام صحيحية للعدد والكمية', '');
        } else
            $("#Amount").val($("#Price").val() * $("#Quantity").val());
    };


    //صافى قيمة الفاتورة= ( إجمالي قيمة البيمعات + إجمالي قيمة الايرادات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )
    function getSafyInvoice() {
        var TotalAmount = Number.parseFloat($("#TotalAmount").text());
        if (isNaN(TotalAmount))
            TotalAmount = 0;

        //var TotalExpenses = Number.parseFloat($("#TotalExpenses").text());
        //if (isNaN(TotalExpenses))
        //    TotalExpenses = 0;

        var SalesTax = Number.parseFloat($("#SalesTax").val());
        if (isNaN(SalesTax))
            SalesTax = 0;

        var TotalDiscount = Number.parseFloat($("#TotalDiscount").text());
        if (isNaN(TotalDiscount))
            TotalDiscount = 0;

        var ProfitTax = Number.parseFloat($("#ProfitTax").val());
        if (isNaN(ProfitTax))
            ProfitTax = 0;

        var safy = (TotalAmount  + SalesTax) - (TotalDiscount + ProfitTax);
        $("#SafyInvoice").text(safy);

        $("#PayedValue").val(safy);
    };
    // //edit
    function onInvoiceDiscountChange() {
        if (isNaN(Number.parseFloat($("#InvoiceDiscount").val())) === false) {
            if ($("#rdo_valAllInvoice:checked").val()) {
                $("#TotalDiscount").text(Number.parseFloat($("#TotalItemDiscount").text()) + Number.parseFloat($("#InvoiceDiscount").val()));
            } else if ($("#rdo_percentageAllInvoice:checked").val()) {
                var invoiceDis = (Number.parseFloat($("#TotalAmount").text()) * Number.parseFloat($("#DiscountPercentage").val())) / 100;
                $("#TotalDiscount").text(Number.parseFloat($("#TotalItemDiscount").text()) + invoiceDis);
                $("#InvoiceDiscount").val(invoiceDis);
            } else {
                $("#TotalDiscount").text(Number.parseFloat($("#TotalItemDiscount").text()) + Number.parseFloat($("#InvoiceDiscount").val()));
            }
            getSafyInvoice();
        };
    };
    function onPayedValueChange() {
        var safy = Number.parseFloat($("#SafyInvoice").text());
        if (isNaN(Number.parseFloat($("#PayedValue").val()))) {
            $("#RemindValue").val(safy);
        } else {
            if (Number.parseFloat($("#PayedValue").val())>safy) {
                toastr.error('المبلغ المدفوع اكبر من صافى الفاتورة', '');
                return false;
            }

            $("#RemindValue").val(safy - Number.parseFloat($("#PayedValue").val()));

        };
    };

    function onProductionOrderChange() {
        var t = $("#balanceProductionOrder").val();
        var obj2 = JSON.parse(JSON.stringify(t));
        var obj = JSON.parse(obj2);
        var balance = obj.balance;
        var serialItemId = obj.serialItemId;
        var serialItemId = obj.serialItemId;
        var productionOrderId = obj.productOrder;
        var isIntial = obj.isIntial;
        $("#balanceVal").val(balance);
        $("#serialItemId").val(serialItemId);
        $("#serialItemId").val(serialItemId);
        $("#productionOrderId").val(productionOrderId);
        $("#isIntial").val(isIntial);
    };
    function onItemChange() {
        //$("#SaleMenStoreId").val(null);
        //$("#balanceVal").val(null);
        //$("#productionOrderId").val(null);
        //$("#isIntial").val(null);
        //$("#balanceProductionOrder").val(null);
        //سعر بيع الصنف الافتراضى المسجل 
        $.get("/SharedDataSources/GetDefaultSellPrice/", { itemId: $("#ItemId").val() }, function (data) {
            $("#Price").val(data.data);
        });
        $.get("/SharedDataSources/GetProductionOrdersOnStoreChange/", { itemId: $("#ItemId").val(), storeId: $("#SaleMenStoreId").val() }, function (data) {
            var isSelected = "";
            $("#balanceProductionOrder").empty();
            var t = data[0].Val
            var obj2 = JSON.parse(JSON.stringify(t));
            var obj = JSON.parse(obj2);
            var balance = obj.balance;
            var serialItemId = obj.serialItemId;
            var productionOrderId = obj.productOrder;
            var isIntial = obj.isIntial;
            $("#balanceVal").val(balance);
            $("#productionOrderId").val(productionOrderId);
            $("#isIntial").val(isIntial);
            $("#serialItemId").val(serialItemId);
            //if (data.length == 1) {
            //    isSelected = "selected";
            //} else {
            //    $("#balanceProductionOrder").append("<option value=>اختر عنصر من القائمة</option>");
            //};
            var count = 1;
            $.each(data, function (index, data) {
                if (count === 1)
                    $("#balanceProductionOrder").append("<option value='" + data.Val + "' selected>" + data.Text + "</option>");
                else
                    $("#balanceProductionOrder").append("<option value='" + data.Val + "'>" + data.Text + "</option>");
                count++;
            });


        });
    };
    //تغيير سعر البيع حسب سياسة البيع المحدد
    function onPricingPolicyChange() {
        $.get("/SharedDataSources/GetPricePolicySellPrice/", { itemId: $("#ItemId").val(), pricePolicyId: $("#PricingPolicyId").val(), customerId: $("#CustomerId").val() }, function (data) {
            $("#Price").val(data.data);
        });
    };
    //العملاء بدلالة فئة العملاء
    function getCustomerOnCategoryChange() {
        //فى حالة ان تم اختيار مندوب
        var selMen = $("#SaleMenEmployeeId").val();
        if (selMen === null ) {
            toastr.error('خطأ فى بيانات المندوب .. لا يمكن عرض هذه الشاشة', '');
            return false;
        }
        $.get("/SharedDataSources/GetCustomerOnCategoryChange/", { id: $("#PersonCategoryId").val(), saleMenId: selMen}, function (data) {
            $("#CustomerId").empty();
            $("#CustomerId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#CustomerId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });
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
        initItemDetails: function () {
            initDTItemDetails();
        },
        addItemDetails: addItemDetails,
        deleteRowItemDetails: deleteRowItemDetails,
        onPriceOrQuanKeyUp: onPriceOrQuanKeyUp,
        getSafyInvoice: getSafyInvoice,
        onInvoiceDiscountChange: onInvoiceDiscountChange,
        onPayedValueChange: onPayedValueChange,
        onProductionOrderChange: onProductionOrderChange,
        onItemChange: onItemChange,
        onPricingPolicyChange: onPricingPolicyChange,
        getCustomerOnCategoryChange: getCustomerOnCategoryChange,
        onRdoValChanged: onRdoValChanged,
        onRdoPercentageChanged: onRdoPercentageChanged,
        onRdoInvoiceValChanged: onRdoInvoiceValChanged,
        onRdoInvoicePercentageChanged: onRdoInvoicePercentageChanged,
        onRdoSerialChanged: onRdoSerialChanged,
        onRdoBarcodeChanged: onRdoBarcodeChanged,
    };

}();

