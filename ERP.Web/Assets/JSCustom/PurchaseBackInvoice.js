"use strict";
var PurchaseBackInvoice_Module = function () {

    //#region ======== Save Purchase invoice ==================
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
                        return 'فواتير مرتجع التوريد';
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
                    filename: "فواتير مرتجع التوريد",
                    title: "فواتير مرتجع التوريد",
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
                url: '/PurchaseBackInvoices/GetAll',
                type: 'GET',
                data(d) {
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                }
                    },
        columns: [
            { data: 'Num', responsivePriority: 0 },
            { data: 'InvoiceNumber', title: 'رقم الفاتورة' },
            { data: 'InvoiceDate', title: 'تاريخ الفاتورة' },
            { data: 'SupplierName', title: 'اسم المورد' },
            { data: 'Safy', title: 'صافى الفاتورة' },
            { data: 'ApprovalAccountant', title: 'حالة الاعتماد محاسبيا' },
            { data: 'ApprovalStore', title: 'حالة الاعتماد مخزنيا' },
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
                        return '\
							<div class="btn-group">\
							<a href="/PurchaseBackInvoices/Edit/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
								<i class="fa fa-edit"></i>\
							</a>\<a href="/PurchaseBackInvoices/ShowPurchaseBackInvoice/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض فاتورة">\
								<i class="fa fa-search"></i>\
							</a>\<a href="#" onclick="PrintInvoice_Module.Print(\''+ row.Id + '\',\'purchaseBack\',null);" class="btn btn-sm btn-clean btn-icon" title="طباعه فاتورة">\
								<i class="fa fa-print"></i>\
							</a>\<a href="#" onclick="PrintInvoice_Module.Print(\''+ row.Id + '\',\'purchaseBack\',\'quantityOnly\');" class="btn btn-sm btn-clean btn-icon" title="طباعه فاتورة كميات">\
								<i class="fa fa-print"></i>\
							</a>\<a href="#" onclick="PrintInvoice_Module.DownloadInvoice(\''+ row.Id +'\',\'purchaseBack\');" class="btn btn-sm btn-clean btn-icon" title="تنزيل فاتورة">\
								<i class="fa fa-download"></i>\
							</a>\
							<a href="javascript:;" onclick=PurchaseBackInvoice_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a><a href="/UploadCenterTypeFiles/Index/?typ=' + row.typ + '&refGid=' + row.Id+'" class="btn btn-sm btn-clean btn-icUrln" title="رفع ملفات الفاتورة">\
								<i class="fa fa-upload"></i>\
							</a><a href="/PurchaseBackInvoices/ShowHistory/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض الحالات">\
								<i class="fa fa-sliders"></i>\
							</a>\<a href="/GeneralDailies/Index/?tranId='+ row.Id + '&tranTypeId=3" class="btn btn-sm btn-clean btn-icon" title="عرض القيود">\
								<i class="fa fa-money"></i>\
							</a>\</div>\
						';
                   
},
                        }

                    ],
            drawCallback: function () {
                var html = ' <tr><th colspan ="11" style= "text-align:center" ><div class="row alert-success"><label>اجمالى المبلغ : ';
                var api = this.api();
                var totalAmount = api.column(4).data().sum();
                $(api.table().footer()).html(html + totalAmount + '</label></div></th>  </tr>');
            },
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

    function SubmitForm(btn, isSaveUpload) {
        try {
            var form = document.getElementById('kt_form');
            var formData = new FormData(form);
            var dataSet = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_DatasourceItems", JSON.stringify(dataSet));
                }
            }
            var dataSetExpense = $('#kt_dtPurchaseBackInvoiceExpenses').DataTable().rows().data().toArray();
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
                        toastr.success(res.message, '',)
                        if (isSaveUpload) { //فى حالةالضغط على حفظ ورفع ملفات للفاتورة 
                            setTimeout(function () { window.location = "/UploadCenterTypeFiles/Index/?typ=" + res.typ + "&refGid=" + res.refGid }, 3000);
                        } else {
                            if (!res.isInsert) {
                                setTimeout(function () { window.location = "/PurchaseBackInvoices/Index" }, 3000);
                            } else
                                setTimeout(function () { window.location = "/PurchaseBackInvoices/CreateEdit" }, 3000);
                        }

                       
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
                var url = '/PurchaseBackInvoices/Delete';
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
    function onPaymentTypeChanged() {
        if ($("#PaymentTypeId").val() != "2") {
            $("#rdo_safe,#rdo_bank").attr("disabled", false);

        } else if ($("#PaymentTypeId").val() == "2") {
            $("#rdo_safe,#rdo_bank").prop('checked', false);
            $("#rdo_safe,#rdo_bank").attr("disabled", true);
            $("#divSafe").hide();
            $("#divBank").hide();
            $("#PayedValue").val(0);
            onPayedValueChange();
        }
    };
    function onRdoSafeChanged() {
        if ($("#rdo_safe:checked").val()) {
            $("#divSafe").show();
            $('#SafeId').removeAttr('disabled');
            $('#BankAccountId').attr('disabled', 'disabled');
            $("#divBank").hide();
        }
    }
    function onRdoBankChanged() {
        if ($("#rdo_bank:checked").val()) {
            $("#divSafe").hide();
            $('#BankAccountId').removeAttr('disabled');
            $('#SafeId').attr('disabled', 'disabled');
            $("#divBank").show();
        }
    };


    function getSafesOnBranchChanged() { // get safes and stores by branchId
        $.get("/SharedDataSources/getSafesOnBranchChanged", { id: $("#BranchId").val() }, function (data) {
            $("#SafeId").empty();
            $("#SafeId").append("<option value=>اختر عنصر من القائمة </option>");
            $.each(data, function (index, row) {
                $("#SafeId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            })
        });

        $.get("/SharedDataSources/getStoresOnBranchChanged", { id: $("#BranchId").val() }, function (data) {
            $("#StoreId").empty();
            $("#StoreId").append("<option value=>اختر عنصر من القائمة </option>");
            $.each(data, function (index, row) {
                $("#StoreId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            })
        });
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
                oPaginate: {
                    sNext: '<span class="pagination-default">التالى</span><span class="pagination-fa"><i class="fa fa-chevron-left" ></i></span>',
                    sPrevious: '<span class="pagination-default">السابق</span><span class="pagination-fa"><i class="fa fa-chevron-right" ></i></span>'
                }
            },

            ajax: {
                url: '/PurchaseBackInvoices/GetDSItemDetails',
                type: 'GET',

            },
            columns: [
                { data: 'Id', title: 'م', visible: false },
                { data: 'ItemId', visible: false },
                { data: 'StoreId', visible: false },
                { data: 'ContainerId', visible: false },
                { data: 'ItemName', title: 'الصنف' },
                { data: 'Quantity', title: 'الكمية' },
                { data: 'Price', title: 'سعر الشراء' },
                { data: 'Amount', title: 'القيمة' },
                { data: 'StoreName', title: 'المخزن' },
                { data: 'ContainerName', title: 'الحاوية' },
                { data: 'ItemDiscount', title: 'قيمة الخصم' },
                { data: 'ItemEntryDate', title: 'تاريخ دخول الصنف' },
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
							<a href="javascript:;" onclick=PurchaseBackInvoice_Module.deleteRowItemDetails('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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
            var price = document.getElementById('Price').value;
            var quantity = document.getElementById('Quantity').value;
            var amount = document.getElementById('Amount').value;
            var itemDiscount = document.getElementById('ItemDiscount').value;
            var storeId = document.getElementById('StoreId').value;
            var containerId = document.getElementById('ContainerId').value;
            var itemEntryDate = document.getElementById('ItemEntryDate').value;
            var balanceVal = document.getElementById('balanceVal').value;

            var formData = new FormData();
            if (itemId === '') {
                toastr.error('تأكد من اختيار الصنف', '');
                $("#ItemId").select2('open');
                return false;
            };
            if (price === '') {
                toastr.error('تأكد من ادخال سعر الشراء وفى حالة العينة تركها بصفر', '');
                $("#Price").focus().select();
                return false;
            };
            if (quantity === '' || quantity == '0') {
                toastr.error('تأكد من ادخال الكمية', '');
                $("#Quantity").focus().select();
                return false;
            };
            if (quantity > parseFloat(balanceVal)) {
                if ($('#ItemAcceptNoBalance').val() === null || $('#ItemAcceptNoBalance').val() === '' || $('#ItemAcceptNoBalance').val() === 'undefined') {
                    toastr.error('الكمية المدخلة اكبر من الرصيد المتاح', '');
                    $("#Quantity").focus().select();
                    return false;
                } else if ($('#ItemAcceptNoBalance').val() === "0") {
                    toastr.error('الكمية المدخلة اكبر من الرصيد المتاح', '');
                    $("#Quantity").focus().select();
                    return false;
                }
            };

            if (storeId === '') {
                toastr.error('تأكد من اختيار المخزن', '');
                $("#StoreId").select2('open');
                return false;
            };
            if (Number.parseFloat(itemDiscount) > Number.parseFloat(amount)) {
                toastr.error('قيمة الخصم اكبر من قيمة الصنف', '');
                $("#ItemDiscount").focus().select();
                return false;
            }

            formData.append('ItemId', itemId)
            formData.append('Price', price)
            formData.append('Quantity', quantity)
            formData.append('Amount', amount)
            formData.append('ItemDiscount', itemDiscount)
            formData.append('StoreId', storeId)
            formData.append('ContainerId', containerId)
            formData.append('ItemEntryDate', itemEntryDate)
            var dataSet = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/PurchaseBackInvoices/AddItemDetails',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtItemDetails').DataTable().ajax.reload();
                        $('#Price').val(0);
                        $('#Quantity').val(0);
                        $('#Amount').val(0);
                        $('#ItemDiscount').val(0);
                        $('#ItemEntryDate').val(null);
                        $('#balanceVal').val(null);

                        //$('#ItemId').val(null);
                        //$('#ItemId').select2({
                        //    placeholder: "اختر عنصر من القائمة"
                        //});
                        //$('#StoreId').val(null);
                        //$('#StoreId').select2({
                        //    placeholder: "اختر عنصر من القائمة"
                        //});
                        $('#ContainerId').val(null);
                        $('#ContainerId').select2({
                            placeholder: "اختر عنصر من القائمة"
                        });

                        $('#TotalAmount,#TotalAmount2').text(res.totalAmount);
                        $('#TotalItemDiscount').text(res.totalDiscountItems);
                        $("#TotalDiscount").text(Number.parseFloat($("#TotalDiscount").text()) + Number.parseFloat(itemDiscount));
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

    function onPriceOrQuanKeyUp() {
        if ($("#Price").val() < 0 || $("#Quantity").val() < 0 || isNaN($("#Price").val()) || isNaN($("#Quantity").val())) {
            toastr.error('تأكد من ادخال ارقام صحيحة للعدد والكمية', '');
        } else {
            $("#Amount").val($("#Price").val() * $("#Quantity").val());
            /*    $('#TotalAmount').val($('#TotalAmount').val() * ($("#Price").val() * $("#Quantity").val()));*/
        }
    };


    function showUnitConvert() {
        $.get("/SharedDataSources/UnitConvert", { id: $("#ItemId").val() }, function (data) {
            if (data.isValid) {
                $("#divUnit").show();
                $("#UnitConvertFromName").val(data.unitConvertFromName);
                $("#UnitConvertFromCount").val(data.unitConvertFromCount);
                $("#Quantity").val(data.unitConvertFromCount);
            } else {
                $("#divUnit").hide();
                toastr.error('اختر صنف له وحدة تحويل', '');
            }

        })

    }
    function updateQuantity() {
        if ($("#UnitConvertFromCount").val() === '0' || $("#UnitConvertFromCount").val() === 0) {
            toastr.error('تأكد من وجود كمية الوحدة المحول منها', ''); return false;
        }
        $("#Quantity").val($("#UnitCount").val() * $("#UnitConvertFromCount").val());
        onPriceOrQuanKeyUp();
    }
    function getProductionOrdersOnStoreChange() {
        $.get("/SharedDataSources/GetProductionOrdersOnStoreChange/", { itemId: $("#ItemId").val(), storeId: $("#StoreId").val() }, function (data) {
            //var isSelected = "";
            $("#balanceProductionOrder").empty();
            if (data.length > 0) {
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

            }


        });
        //to prevent default form submit event
        return false;

    };

    function onProductionOrderChange() {
        var t = $("#balanceProductionOrder").val();
        var obj2 = JSON.parse(JSON.stringify(t));
        var obj = JSON.parse(obj2);
        var balance = obj.balance;
        var serialItemId = obj.serialItemId;
        var productionOrderId = obj.productOrder;
        var isIntial = obj.isIntial;
        $("#balanceVal").val(balance);
        $("#serialItemId").val(serialItemId);
        $("#productionOrderId").val(productionOrderId);
        $("#isIntial").val(isIntial);
    };

    function onItemChange() {
        if ($('#StoreId').val() != null) {
            getProductionOrdersOnStoreChange();
        } else {
            $('#StoreId').val(null);
            $('#StoreId').select2({
                placeholder: "اختر عنصر من القائمة"
            });
        }
        $("#balanceVal").val(null);
        $("#serialItemId").val(null);
        $("#productionOrderId").val(null);
        $("#isIntial").val(null);
        $("#balanceProductionOrder").val(null);

        //سياسة اسعار عميل محدد فى فاتورة بيع 
        $.get("/SharedDataSources/GetItemPriceByCustomer", { id: $("#CustomerId").val(), itemId: $("#ItemId").val() }, function (data) {
            if (data.customeSell > 0) {
                $("#PricingPolicyId").val(data.pricingPolicyId);
                $("#Price").val(data.customeSell);
                $("#Amount").val(data.customeSell * $("#Quantity").val());
            } else {
                var newPrice = 0;
                //السعر من جدول تحديد اسعار البيع تلقائيا حسب الفرع/الفئة/الصنف
                $.get("/SharedDataSources/GetItemCustomSellPrice", { itemId: $("#ItemId").val(), branchId: $("#BranchId").val() }, function (data) {
                    newPrice = data.data;
                    $("#Price").val(newPrice);
                    $("#Amount").val(newPrice * $("#Quantity").val());
                });
                if (newPrice === 0) {
                    //سعر بيع الصنف الافتراضى المسجل 
                    $.get("/SharedDataSources/GetDefaultSellPrice/", { itemId: $("#ItemId").val() }, function (data) {
                        console.log(data);
                        newPrice = data.data;
                        $("#Price").val(newPrice);
                        $("#Amount").val(newPrice * $("#Quantity").val());
                    });
                }


            }

        });
        //اخر اسعار سعر بيع للصنف
        $.get("/SharedDataSources/GetPreviousPrices/", { itemId: $("#ItemId").val(), isSell: true }, function (data) {
            $("#prevouisPrice").empty();
            $("#prevouisPrice").append("<option value='0'>اختر سعر</option>");
            $.each(data, function (index, row) {
                $("#prevouisPrice").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });
        });
        //اظهار تكلفة الصنف
        if ($('#ItemCostCalculateShowInSellRegId').val() === '1') {
            $.get("/SharedDataSources/GetPriceOnItemCostCalculateChange", { itemCostCalcId: $("#ItemCostCalculateId").val(), itemId: $("#ItemId").val() }, function (res) {
                $("#ItemCost").val(res.price);
            });
        } else
            $("#ItemCost").val(0);
        //وحدات الصنف 
        $.get("/SharedDataSources/GeItemUnits", { id: $("#ItemId").val() }, function (data) {
            $("#ItemUnitsId").empty();
            $("#ItemUnitsId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#ItemUnitsId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        });
    };

    //#endregion ========= end Step 2 ==========


    //#region ======== Step 3 تسجيل المصروفات=================
    var initDTPurchaseBackInvoiceExpenses = function () {
        var table = $('#kt_dtPurchaseBackInvoiceExpenses');

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
                url: '/PurchaseBackInvoices/GetDStPurchaseBackInvoiceExpenses',
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
							<a href="javascript:;" onclick=PurchaseBackInvoice_Module.deleteRowPurchaseBackInvoiceExpenses('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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

    function addPurchaseBackInvoiceExpenses() {
        try {
            var expenseTypeId = document.getElementById('ExpenseTypeId').value;
            var expenseAmount = document.getElementById('ExpenseAmount').value;
            var formData = new FormData();
            if (ExpenseTypeId === '') {
                toastr.error('تأكد من اختيار مسمى المصروف', '');
                return false;
            };
            if (expenseAmount === '' || expenseAmount=="0") {
                toastr.error('تأكد من ادخال  قيمة المصروف', '');
                return false;
            };

            formData.append('ExpenseTypeId', expenseTypeId)
            formData.append('ExpenseAmount', expenseAmount)
            var dataSet = $('#kt_dtPurchaseBackInvoiceExpenses').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/PurchaseBackInvoices/AddPurchaseBackInvoiceExpenses',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtPurchaseBackInvoiceExpenses').DataTable().ajax.reload();
                        $('#ExpenseTypeId').val('');
                        $('#accountTree').val(null);
                        $('#ExpenseAmount').val(0);
                        toastr.success(res.msg, '');
                        $("#TotalExpenses,#TotalExpenses2").text(res.totalExpenses);
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

    function deleteRowPurchaseBackInvoiceExpenses(id) {
        $('#kt_dtPurchaseBackInvoiceExpenses tbody').on('click', 'a.deleteIcon', function () {
            var amountExpenseRemoved = $('#kt_dtPurchaseBackInvoiceExpenses').DataTable().row($(this).closest('tr')).data()['ExpenseAmount'];
            $("#TotalExpenses,#TotalExpenses2").text(Number.parseFloat($("#TotalExpenses").text()) - amountExpenseRemoved);
            $('#kt_dtPurchaseBackInvoiceExpenses').DataTable().row($(this).parents('tr')).remove().draw();
            getSafyInvoice();

        })

    };


    //#endregion ========= end Step 2 ==========
    //صافى قيمة الفاتورة= ( إجمالي قيمة المشتريات + إجمالي قيمة المصروفات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )
    function getSafyInvoice() {
        var TotalAmount = Number.parseFloat($("#TotalAmount").text());
        if (isNaN(TotalAmount))
            TotalAmount = 0;

        var TotalExpenses = Number.parseFloat($("#TotalExpenses").text());
        if (isNaN(TotalExpenses))
            TotalExpenses = 0;

        var SalesTax = Number.parseFloat($("#SalesTax").val());
        if (isNaN(SalesTax))
            SalesTax = 0;

        var TotalDiscount = Number.parseFloat($("#TotalDiscount").text());
        if (isNaN(TotalDiscount))
            TotalDiscount = 0;

        var ProfitTax = Number.parseFloat($("#ProfitTax").val());
        if (isNaN(ProfitTax))
            ProfitTax = 0;

        var safy = (TotalAmount + TotalExpenses + SalesTax) - (TotalDiscount + ProfitTax);
        $("#SafyInvoice").text(safy);

        if ($("#PaymentTypeId").val() == "2") {
            $("#RemindValue").val(safy);
        } else
            $("#PayedValue").val(safy);
    };

    function onInvoiceDiscountChange() {
        if (isNaN(Number.parseFloat($("#InvoiceDiscount").val()))===false) {
            $("#TotalDiscount").text(Number.parseFloat($("#TotalItemDiscount").text()) + Number.parseFloat($("#InvoiceDiscount").val()));
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
    //الموردين بدلالة فئة الموردين
    function getSupplierOnCategoryChange() {
        $.get("/SharedDataSources/GetSupplierOnCategoryChange/", { id: $("#PersonCategoryId").val() }, function (data) {
            $("#SupplierId").empty();
            $("#SupplierId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#SupplierId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
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
        onPaymentTypeChanged: onPaymentTypeChanged,
        onRdoSafeChanged: onRdoSafeChanged,
        onRdoBankChanged: onRdoBankChanged,
        getSafesOnBranchChanged: getSafesOnBranchChanged,

        //step2
        initItemDetails: function () {
            initDTItemDetails();
        },
        addItemDetails: addItemDetails,
        deleteRowItemDetails: deleteRowItemDetails,
        onPriceOrQuanKeyUp: onPriceOrQuanKeyUp,
        showUnitConvert: showUnitConvert,
        updateQuantity: updateQuantity,
        //step3
        initPurchaseBackInvoiceExpenses: function () {
            initDTPurchaseBackInvoiceExpenses();
        },
        addPurchaseBackInvoiceExpenses: addPurchaseBackInvoiceExpenses,
        deleteRowPurchaseBackInvoiceExpenses: deleteRowPurchaseBackInvoiceExpenses,
        getSafyInvoice: getSafyInvoice,
        onInvoiceDiscountChange: onInvoiceDiscountChange,
        onPayedValueChange: onPayedValueChange,
        getSupplierOnCategoryChange: getSupplierOnCategoryChange,
        onItemChange: onItemChange,
        onProductionOrderChange: onProductionOrderChange,
    };

}();

