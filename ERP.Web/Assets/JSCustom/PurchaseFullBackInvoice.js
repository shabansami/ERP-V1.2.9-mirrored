"use strict";

var PurchaseFullBackInvoice_Module = function () {

    //#region ادارة ارجاع جزءمن فاتورة توريد او كلها 
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
                        return 'ارجاع فاتورة توريد معينة';
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
                    filename: "ارجاع فاتورة توريد معينة",
                    title: "ارجاع فاتورة توريد معينة",
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
                url: '/PurchaseFullBackInvoices/GetAll',
                type: 'GET',
                data(d) {
                    d.invoId = $("#invoNumber").val();
                    d.dtFrom = $("#dtFrom").val();
                    d.dtTo = $("#dtTo").val();
                }

            },

            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'InvoiceNum', title: 'رقم فاتورة التوريد' },
                { data: 'InvoiceDate', title: 'تاريخ الفاتورة' },
                { data: 'SupplierName', title: 'اسم المورد' },
                { data: 'Safy', title: 'صافى الفاتورة' },
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
                    render: function (data, type, row, meta) {
                        return '\
							<div class="btn-group">\
							    <a href="/PurchaseFullBackInvoices/ReturnPatialInvoice/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="ارجاع بعض الاصناف">\
								<i class="la la-plus"></i>\
							</a>\
							<a href="javascript:;" onclick=PurchaseFullBackInvoice_Module.ReturnAllInvoice(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="ارجاع كامل الفاتورة">\
								<i class="la la-undo"></i>\
							</a></div>\
						';
                    },
                }

            ],

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
    function onSearch() {
        $('#kt_datatable').DataTable().ajax.reload();
    }

    //#endregion

    //#region ارجاع جزء من فاتورة توريد 
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
                            setTimeout(function () { window.location = "/PurchaseFullBackInvoices/ReturnPatialInvoice" }, 3000);
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


    //#endregion


    //#region ========== Step 1 البيانات الاساسية===============
    function onPaymentTypeChanged() {
        if ($("#PaymentTypeId").val() == "1" || $("#PaymentTypeId").val() == "3") {
            $("#rdo_safe,#rdo_bank").attr("disabled", false);

        } else {
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


    function getSafesOnBranchChanged() {  // get safes and stores by branchId
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
            },

            ajax: {
                url: '/PurchaseFullBackInvoices/GetDSItemDetails',
                type: 'GET',

            },
            columns: [
                { data: 'Id', title: 'م', visible: false },
                { data: 'ItemId', visible: false },
                { data: 'StoreId', visible: false },
/*                { data: 'ContainerId', visible: false },*/
                { data: 'ItemName', title: 'الصنف' },
                { data: 'Quantity', title: 'الكمية' },
                { data: 'Price', title: 'سعر الشراء' },
                { data: 'Amount', title: 'القيمة' },
                { data: 'StoreName', title: 'المخزن' },
/*                { data: 'ContainerName', title: 'الحاوية' },*/
                { data: 'ItemDiscount', title: 'قيمة الخصم' },
/*                { data: 'ItemEntryDate', title: 'تاريخ دخول الصنف' },*/
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
							<a href="javascript:;" onclick=PurchaseFullBackInvoice_Module.editRowItemDetails(\''+ row.Id + '\')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="تعديل">\
								<i class="la la-edit"></i>\
							</a><a href="javascript:;" onclick=PurchaseFullBackInvoice_Module.deleteRowItemDetails(\''+ row.Id + '\')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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
            var itemId = document.getElementById('ItemIdd').value;
            var price = document.getElementById('Price').value;
            var quantity = document.getElementById('Quantity').value;
            var quantityOld = document.getElementById('QuantityOld').value;
            var amount = document.getElementById('Amount').value;
            var itemDiscount = document.getElementById('ItemDiscount').value;
            var storeId = document.getElementById('StoreId').value;
            var purchaseDetailsId = document.getElementById('purchaseDetailsId').value;

            var formData = new FormData();
            if (purchaseDetailsId === '') {
                toastr.error('تأكد من اختيار الصنف', '');
                return false;
            };
            if (itemId === '') {
                toastr.error('تأكد من اختيار الصنف', '');
                return false;
            };
            if (price === '') {
                toastr.error('تأكد من ادخال سعر الشراء', '');
                return false;
            };
            if (quantity === '' || quantity == '0') {
                toastr.error('تأكد من ادخال الكمية', '');
                return false;
            };
            if (quantity > quantityOld) {
                toastr.error('الكمية المدخلة اكبر من الكمية الاصلية', '');
                return false;
            };
            if (storeId === '') {
                toastr.error('تأكد من اختيار المخزن', '');
                return false;
            };
            if (Number.parseFloat(itemDiscount) > Number.parseFloat(amount)) {
                toastr.error('قيمة الخصم اكبر من قيمة الصنف', '');
                return false;
            }

            formData.append('Id', purchaseDetailsId)
            formData.append('ItemId', itemId)
            formData.append('Price', price)
            formData.append('Quantity', quantity)
            formData.append('Amount', amount)
            formData.append('ItemDiscount', itemDiscount)
            formData.append('StoreId', storeId)
            var dataSet = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/PurchaseFullBackInvoices/AddItemDetails',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtItemDetails').DataTable().ajax.reload();
                        $('#purchaseDetailsId').val('');
                        $('#ItemName').val('');
                        $('#ItemIdd').val('');
                        $('#Price').val(0);
                        $('#Quantity').val(0);
                        $('#QuantityOld').val(0);
                        $('#Amount').val(0);
                        $('#ItemDiscount').val(0);
                        //$('#StoreId').val(null);
                        //$('#ContainerId').val('');
                        //$('#ItemEntryDate').val(null);


                        $('#TotalAmount,#TotalAmount2').text(res.totalAmount);
                        $('#TotalItemDiscount').text(res.totalDiscountItems);
                        $("#TotalDiscount").text(Number.parseFloat($("#TotalDiscount").text()) + Number.parseFloat(itemDiscount));
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

    function editRowItemDetails(id) {
        $('#kt_dtItemDetails tbody').on('click', 'a.deleteIcon', function () {
            var idRemoved = $('#kt_dtItemDetails').DataTable().row($(this).closest('tr')).data()['Id'];
            var itemRemoved = $('#kt_dtItemDetails').DataTable().row($(this).closest('tr')).data()['ItemId'];
            var itemNameRemoved = $('#kt_dtItemDetails').DataTable().row($(this).closest('tr')).data()['ItemName'];
            var priceRemoved = $('#kt_dtItemDetails').DataTable().row($(this).closest('tr')).data()['Price'];
            var quantityRemoved = $('#kt_dtItemDetails').DataTable().row($(this).closest('tr')).data()['Quantity'];
            var amountRemoved = $('#kt_dtItemDetails').DataTable().row($(this).closest('tr')).data()['Amount'];
            var itemDiscountRemoved = $('#kt_dtItemDetails').DataTable().row($(this).closest('tr')).data()['ItemDiscount'];
            var storeRemoved = $('#kt_dtItemDetails').DataTable().row($(this).closest('tr')).data()['StoreId'];
            $("#TotalAmount,#TotalAmount2").text(Number.parseFloat($("#TotalAmount").text()) - amountRemoved);
            $("#TotalItemDiscount").text(Number.parseFloat($("#TotalItemDiscount").text()) - itemDiscountRemoved);
            $('#kt_dtItemDetails').DataTable().row($(this).parents('tr')).remove().draw();
            $("#TotalDiscount").text(Number.parseFloat($("#TotalDiscount").text()) - itemDiscountRemoved);
            getSafyInvoice();

            $("#purchaseDetailsId").val(idRemoved);
            $("#ItemIdd").val(itemRemoved);
            $("#ItemName").val(itemNameRemoved);
            $("#Price").val(priceRemoved);
            $("#Quantity").val(quantityRemoved);
            $("#QuantityOld").val(quantityRemoved);
            $("#Amount").val(amountRemoved);
            $("#ItemDiscount").val(itemDiscountRemoved);
            $("#StoreId").val(storeRemoved).change();

        })

    };
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
            },

            ajax: {
                url: '/PurchaseFullBackInvoices/GetDStPurchaseBackInvoiceExpenses',
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
							<a href="javascript:;" onclick=PurchaseFullBackInvoice_Module.deleteRowPurchaseBackInvoiceExpenses('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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

    function addPurchaseBackInvoiceExpenses() {
        try {
            var expenseTypeId = document.getElementById('ExpenseTypeId').value;
            var expenseAmount = document.getElementById('ExpenseAmount').value;
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
            var dataSet = $('#kt_dtPurchaseBackInvoiceExpenses').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/PurchaseFullBackInvoices/AddPurchaseInvoiceExpenses',
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

    //#region العمليات الحسابية 
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
        if (isNaN(Number.parseFloat($("#InvoiceDiscount").val())) === false) {
            $("#TotalDiscount").text(Number.parseFloat($("#TotalItemDiscount").text()) + Number.parseFloat($("#InvoiceDiscount").val()));
            getSafyInvoice();
        };
    };
    function onPayedValueChange() {
        var safy = Number.parseFloat($("#SafyInvoice").text());
        if (isNaN(Number.parseFloat($("#PayedValue").val()))) {
            $("#RemindValue").val(safy);
        } else {
            if (Number.parseFloat($("#PayedValue").val()) > safy) {
                toastr.error('المبلغ المدفوع اكبر من صافى الفاتورة', '');
                return false;
            }

            $("#RemindValue").val(safy - Number.parseFloat($("#PayedValue").val()));

        };
    };

    function onPriceOrQuanKeyUp() {
        if ($("#Price").val() < 0 || $("#Quantity").val() < 0 || isNaN($("#Price").val()) || isNaN($("#Quantity").val())) {
            toastr.error('تأكد من ادخال ارقام صحيحية للعدد والكمية', '');
        } else
            $("#Amount").val($("#Price").val() * $("#Quantity").val());
    };

    //#endregion

    //#region ارجاع كامل الفاتورة 
    function ReturnAllInvoice(id) {
        Swal.fire({
            title: 'تأكيد إرجاع كامل الفاتورة',
            text: 'هل متأكد من إرجاع كامل فاتورة التوريد ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/PurchaseFullBackInvoices/ReturnAllInvoice';
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

    //#endregion 
   
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        onSearch: onSearch,
        SubmitForm: SubmitForm,
        ReturnAllInvoice: ReturnAllInvoice,
        //step 1
        onPaymentTypeChanged: onPaymentTypeChanged,
        onRdoSafeChanged: onRdoSafeChanged,
        onRdoBankChanged: onRdoBankChanged,
        getSafesOnBranchChanged: getSafesOnBranchChanged,

        //step2
        initItemDetails: function () {
            initDTItemDetails();
        },
        deleteRowItemDetails: deleteRowItemDetails,
        editRowItemDetails: editRowItemDetails,
        //step3
        initPurchaseBackInvoiceExpenses: function () {
            initDTPurchaseBackInvoiceExpenses();
        },
        deleteRowPurchaseBackInvoiceExpenses: deleteRowPurchaseBackInvoiceExpenses,
        getSafyInvoice: getSafyInvoice,
        onInvoiceDiscountChange: onInvoiceDiscountChange,
        onPayedValueChange: onPayedValueChange,
        addItemDetails: addItemDetails,
        addPurchaseBackInvoiceExpenses: addPurchaseBackInvoiceExpenses,
        onPriceOrQuanKeyUp: onPriceOrQuanKeyUp
    };

}();


