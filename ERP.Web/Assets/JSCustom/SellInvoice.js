"use strict";

var SellInvoice_Module = function () {

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
                oPaginate: {
                    sNext: '<span class="pagination-default">التالى</span><span class="pagination-fa"><i class="fa fa-chevron-left" ></i></span>',
                    sPrevious: '<span class="pagination-default">السابق</span><span class="pagination-fa"><i class="fa fa-chevron-right" ></i></span>'
                }
            },

            ajax: {
                url: '/SellInvoices/GetAll',
                type: 'GET',
                data(d) {
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.brnchId = $("#BranchId").val();
                }
                    },
        columns: [
            { data: 'Num', responsivePriority: 0 },
            { data: 'InvoiceNumber', title: 'رقم الفاتورة' },
            { data: 'InvoiceDate', title: 'تاريخ الفاتورة' },
            { data: 'CustomerName', title: 'اسم العميل' },
            { data: 'Safy', title: 'صافى الفاتورة' },
            { data: 'ApprovalAccountant', title: 'حالة الاعتماد محاسبيا' },
            { data: 'ApprovalStore', title: 'حالة الاعتماد مخزنيا' },
            { data: 'CaseName', title: 'اخر حالة للفاتورة' },
            { data: 'InvoType', title: 'حالة الفاتورة', visible: false },
            { data: 'InvoiceNumPaper', title: 'رقم الفاتورة الورقية'},
            { data: 'PaymentTypeName', title: 'طريقة السداد' },
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
                    var ele = '\
							<div class="btn-group">\
							<a href="/PrintInvoices/ShowPrintInvoice/?id='+ row.Id + '&typ=sell" target="_blank" class="btn btn-sm btn-clean btn-icon" title="عرض وطباعة">\
								<i class="fa fa-print"></i>\
							</a>\<a href="/SellInvoices/ShowSellInvoice/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض فاتورة">\
								<i class="fa fa-search"></i>\
							</a>\<a href="#" onclick="PrintInvoice_Module.Print(\''+ row.Id + '\',\'sell\',null);" class="btn btn-sm btn-clean btn-icon" title="طباعه كاشير">\
								<i class="fa fa-print"></i>\
							</a><a href="#" onclick="PrintInvoice_Module.Print(\''+ row.Id + '\',\'sell\',\'quantityOnly\');" class="btn btn-sm btn-clean btn-icon" title="طباعه كاشير كميات">\
								<i class="fa fa-print"></i>\
							</a>\<a href="#" onclick="PrintInvoice_Module.DownloadInvoice(\''+ row.Id + '\',\'sell\');" class="btn btn-sm btn-clean btn-icon" title="تنزيل فاتورة">\
								<i class="fa fa-download"></i>\
							</a>\
							<a href="/UploadCenterTypeFiles/Index/?typ=' + row.typ + '&refGid=' + row.Id + '" class="btn btn-sm btn-clean btn-icUrln" title="رفع ملفات الفاتورة">\
								<i class="fa fa-upload"></i>\
							</a><a href="/SellInvoices/ShowHistory/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض الحالات">\
								<i class="fa fa-random"></i>\
							</a>\
						';
                    if (row.IsFinalApproval) {
                        ele += '<a href="/GeneralDailies/Index/?tranId=' + row.Id + '&tranTypeId=2" class="btn btn-sm btn-clean btn-icon" title="عرض القيود">\
								<i class="fa fa-money-bill"></i>\
							</a>\<a href="javascript:;" onclick=SellInvoice_Module.unApproval(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icon" title="فك الاعتماد">\
								<i class="fa fa-unlock-alt"></i>\
							</a>';
                        
                    } else {
                        ele += '<a href="/SellInvoices/Edit/?invoGuid=' + row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
								<i class="fa fa-edit"></i>\
							</a>\<a href="javascript:;" onclick=SellInvoice_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a>';
                    }

                    return ele +='</div>';
      //                  return '\
						//	<div class="btn-group">\
						//	<a href="/SellInvoices/Edit/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
						//		<i class="fa fa-edit"></i>\
						//	</a>\<a href="/SellInvoices/ShowSellInvoice/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض فاتورة">\
						//		<i class="fa fa-search"></i>\
						//	</a>\<a href="#" onclick="PrintInvoice_Module.Print(\''+ row.Id + '\',\'sell\',null);" class="btn btn-sm btn-clean btn-icon" title="طباعه فاتورة">\
						//		<i class="fa fa-print"></i>\
						//	</a><a href="#" onclick="PrintInvoice_Module.Print(\''+ row.Id + '\',\'sell\',\'quantityOnly\');" class="btn btn-sm btn-clean btn-icon" title="طباعه فاتورة كميات">\
						//		<i class="fa fa-print"></i>\
						//	</a>\<a href="#" onclick="PrintInvoice_Module.DownloadInvoice(\''+ row.Id +'\',\'sell\');" class="btn btn-sm btn-clean btn-icon" title="تنزيل فاتورة">\
						//		<i class="fa fa-download"></i>\
						//	</a>\
						//	<a href="javascript:;" onclick=SellInvoice_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
						//		<i class="fa fa-trash"></i>\
						//	</a><a href="/UploadCenterTypeFiles/Index/?typ=' + row.typ + '&refGid=' + row.Id+'" class="btn btn-sm btn-clean btn-icUrln" title="رفع ملفات الفاتورة">\
						//		<i class="fa fa-upload"></i>\
						//	</a><a href="/SellInvoices/ShowHistory/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض الحالات">\
						//		<i class="fa fa-random"></i>\
						//	</a>\<a href="/GeneralDailies/Index/?tranId='+ row.Id + '&tranTypeId=2" class="btn btn-sm btn-clean btn-icon" title="عرض القيود">\
						//		<i class="fa fa-money-bill"></i>\
						//	</a>\</div>\
						//';
                   
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

    function SubmitForm(btn, isSaveByParam) {
        try {
            var form = document.getElementById('kt_form');
            var formData = new FormData(form);
            var dataSet = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_DatasourceItems", JSON.stringify(dataSet));
                }
            }
            var dataSetExpense = $('#kt_dtSellInvoiceExpenses').DataTable().rows().data().toArray();
            if (dataSetExpense != null) {
                if (dataSetExpense.length > 0) {
                    formData.append("DT_DatasourceExpenses", JSON.stringify(dataSetExpense));
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
                        toastr.success(res.message, '',);
                        if (isSaveByParam === 'upload') { //فى حالةالضغط على حفظ ورفع ملفات للفاتورة 
                            setTimeout(function () { window.location = "/UploadCenterTypeFiles/Index/?typ=" + res.typ + "&refGid=" + res.refGid }, 3000);
                            //}else 
                            //if (isSaveByParam === 'payment') { //فى حالةالضغط على حفظ وانشاء دفعات
                            //    //setTimeout(function () { window.location = "/SellInvoicePayments/RegisterPyments/?invoGuid=" + res.refGid }, 3000);
                            //    setTimeout(function () { window.location = "/SellInvoiceInstallments/RegisterInstallments/?invoGuid=" + res.refGid }, 3000);
                            //} 
                        }else
                            if (isSaveByParam === 'installment') { //فى حالةالضغط على حفظ وانشاء اقساط
                                setTimeout(function () { window.location = "/SellInvoiceInstallments/RegisterInstallments/?invoGuid=" + res.refGid+"&typ=sell" }, 3000);
                        }else
                            if (isSaveByParam === 'print') { //فى حالةالضغط على حفظ وطباعه
                                setTimeout(function () { window.location = "/PrintInvoices/ShowPrintInvoice/?id="+ res.refGid + "&typ=sell" }, 3000);
                        }else
                        if (!res.isInsert) {
                            setTimeout(function () { window.location = "/SellInvoices/Index" }, 3000);
                        } else {
                            setTimeout(function () { window.location = "/SellInvoices/CreateEdit" }, 3000);

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
                var url = '/SellInvoices/Delete';
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
    function unApproval(invoGuid) {
        Swal.fire({
            title: 'تأكيد فك الاعتماد',
            text: 'هل متأكد من فك الاعتماد ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/SellInvoices/UnApproval';
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
        if ($("#PaymentTypeId").val() != "2" ) {
            $("#rdo_safe,#rdo_bank").attr("disabled", false);
            $("#btnInstallment").hide();
            //فى حالة التقسيط
            if ($("#PaymentTypeId").val() == "4") {
                $("#btnInstallment").show();
            } else
                $("#btnInstallment").hide();
        }else if ($("#PaymentTypeId").val() == "2" ) {
            $("#rdo_safe,#rdo_bank").prop('checked', false);
            $("#rdo_safe,#rdo_bank").attr("disabled", true);
            $("#divSafe").hide();
            $("#divBank").hide();
            $("#PayedValue").val(0);
            $("#btnInstallment").hide();
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
    function onRdoInvoicePercentageChanged() {
        if ($("#rdo_percentageAllInvoice:checked").val()) {
            $("#isInvoiceDisVal").val(false);
            $("#divDiscountPercentage").show();
            $("#TotalDiscount").text(Number.parseFloat($("#TotalDiscount").text()) - Number.parseFloat($("#InvoiceDiscount").val()));
            $("#InvoiceDiscount").val(0);

            getSafyInvoice();

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
                oPaginate: {
                    sNext: '<span class="pagination-default">التالى</span><span class="pagination-fa"><i class="fa fa-chevron-left" ></i></span>',
                    sPrevious: '<span class="pagination-default">السابق</span><span class="pagination-fa"><i class="fa fa-chevron-right" ></i></span>'
                }
            },

            ajax: {
                url: '/SellInvoices/GetDSItemDetails',
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
                { data: 'QuantityUnitName', title: 'الكمية بالوحدة' },
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
							<a href="javascript:;" onclick=SellInvoice_Module.deleteRowItemDetails('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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
            var serialItemId = document.getElementById('serialItemId').value;
            var balanceVal = document.getElementById('balanceVal').value;
            var productionOrderId = document.getElementById('productionOrderId').value;
            var isIntial = document.getElementById('isIntial').value;
            var isDiscountItemVal = document.getElementById('IsDiscountItemVal').value;
            var ItemUnitsId = document.getElementById('ItemUnitsId').value;
            var formData = new FormData();
            if (itemId === '') {
                toastr.error('تأكد من اختيار الصنف', '');
                $("#ItemId").select2('open');
                return false;
            };
            if (price === '') {
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
                if ($('#ItemAcceptNoBalance').val() === null || $('#ItemAcceptNoBalance').val() === '' || $('#ItemAcceptNoBalance').val() === 'undefined') {
                    toastr.error('تأكد من تحديد قبول اصناف بدون رصيد من الاعدادات', '');
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
            if (Number.parseFloat(itemDiscount) > Number.parseFloat(amount) && isDiscountItemVal === 'true') {
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
            formData.append('ItemUnitsId', ItemUnitsId)
            var dataSet = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/SellInvoices/AddItemDetails',
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
                        $('#PricePolicyId').val(null);
                        $('#balanceProductionOrder').empty();
                        $('#balanceVal').val(null);
                        $('#ItemBarcode').val(null);

                        $('#productionOrderId').val(null);
                        $('#isIntial').val(null);
                        $('#serialItemId').val(null);
                        $('#ItemSerial').val(null);
                        //$('#ItemId').val(null);
                        //$('#ItemId').select2({
                        //    placeholder: "اختر عنصر من القائمة"
                        //});
                        //$('#StoreId').val(null);
                        //$('#StoreId').select2({
                        //    placeholder: "اختر عنصر من القائمة"
                        //});

                        $('#TotalAmount,#TotalAmount2').text(res.totalAmount);
                        $('#TotalQuantity').text(res.totalQuantity);
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
            var quantityRemoved = $('#kt_dtItemDetails').DataTable().row($(this).closest('tr')).data()['Quantity'];
            var itemDiscountRemoved = $('#kt_dtItemDetails').DataTable().row($(this).closest('tr')).data()['ItemDiscount'];
            $("#TotalAmount,#TotalAmount2").text(Number.parseFloat($("#TotalAmount").text()) - amountRemoved);
            $("#TotalQuantity").text(Number.parseFloat($("#TotalQuantity").text()) - quantityRemoved);
            $("#TotalItemDiscount").text(Number.parseFloat($("#TotalItemDiscount").text()) - itemDiscountRemoved);
            $('#kt_dtItemDetails').DataTable().row($(this).parents('tr')).remove().draw();
            $("#TotalDiscount").text(Number.parseFloat($("#TotalDiscount").text()) - itemDiscountRemoved);
            getSafyInvoice();
        })

    };

    //#endregion ========= end Step 2 ==========


    //#region ======== Step 3 تسجيل الايرادات=================
    var initDTSellInvoiceExpenses = function () {
        var table = $('#kt_dtSellInvoiceExpenses');

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
                url: '/SellInvoices/GetDStSellInvoiceExpenses',
                type: 'GET',

            },
            columns: [
                { data: 'ExpenseTypeId', visible: false },
                { data: 'ExpenseTypeName', title: 'مسمى الإيراد' },
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
							<a href="javascript:;" onclick=SellInvoice_Module.deleteRowSellInvoiceExpenses('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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

    function addSellInvoiceExpenses() {
        try {
            var expenseTypeId = document.getElementById('ExpenseTypeId').value;
            var expenseAmount = document.getElementById('ExpenseAmount').value;
            var formData = new FormData();
            if (ExpenseTypeId === '') {
                toastr.error('تأكد من اختيار مسمى الإيراد', '');
                return false;
            };
            if (expenseAmount === '' || expenseAmount=="0") {
                toastr.error('تأكد من ادخال  قيمة الإيراد', '');
                return false;
            };

            formData.append('ExpenseTypeId', expenseTypeId)
            formData.append('ExpenseAmount', expenseAmount)
            var dataSet = $('#kt_dtSellInvoiceExpenses').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/SellInvoices/AddSellInvoiceExpenses',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtSellInvoiceExpenses').DataTable().ajax.reload();
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

    function deleteRowSellInvoiceExpenses(id) {
        $('#kt_dtSellInvoiceExpenses tbody').on('click', 'a.deleteIcon', function () {
            var amountExpenseRemoved = $('#kt_dtSellInvoiceExpenses').DataTable().row($(this).closest('tr')).data()['ExpenseAmount'];
            $("#TotalExpenses,#TotalExpenses2").text(Number.parseFloat($("#TotalExpenses").text()) - amountExpenseRemoved);
            $('#kt_dtSellInvoiceExpenses').DataTable().row($(this).parents('tr')).remove().draw();
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
        var TotalAmount = Number.parseFloat($("#TotalAmount").text());
        if (isNaN(TotalAmount))
            TotalAmount = 0;

        var TotalExpenses = Number.parseFloat($("#TotalExpenses").text());
        if (isNaN(TotalExpenses))
            TotalExpenses = 0;

        var SalesTax = Number.parseFloat($("#SalesTax").val());
        if (isNaN(SalesTax))
            SalesTax = 0;
        var SalesTaxPercentage = Number.parseFloat($("#SalesTaxPercentage").val());
        if (isNaN(SalesTaxPercentage))
            SalesTaxPercentage = 0;
        else {
            SalesTax = (TotalAmount * SalesTaxPercentage) / 100
            $("#SalesTax").val(SalesTax);
        }
        

        var TotalDiscount = Number.parseFloat($("#TotalDiscount").text());
        if (isNaN(TotalDiscount))
            TotalDiscount = 0;

        var ProfitTax = Number.parseFloat($("#ProfitTax").val());
        if (isNaN(ProfitTax))
            ProfitTax = 0;
        var ProfitPercentageTax = Number.parseFloat($("#ProfitTaxPercentage").val());
        if (isNaN(ProfitPercentageTax))
            ProfitPercentageTax = 0;
        else {
            ProfitTax = (TotalAmount * ProfitPercentageTax) / 100
            $("#ProfitTax").val(ProfitTax)
        }

        var safy =((TotalAmount + TotalExpenses + SalesTax) - (TotalDiscount + ProfitTax)).toFixed(2);
        $("#SafyInvoice").text(safy);

        if ($("#PaymentTypeId").val() == "2" || $("#PaymentTypeId").val() == "4") {
            $("#RemindValue").val(safy);
        } else
            $("#PayedValue").val(safy);
    };

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
                    if(count===1)
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
        $.get("/SharedDataSources/GetItemPriceByCustomer", { id: $("#CustomerId").val(), itemId: $("#ItemId").val(), isCustomer: true }, function (data) {
            if (data.customeSell > 0 ) {
                $("#PricingPolicyId").val(data.pricingPolicyId);
                $("#Price").val(data.customeSell);
                $("#Amount").val(data.customeSell * $("#Quantity").val());
            } else {
                var newPrice = 0;
                //السعر من جدول تحديد اسعار البيع تلقائيا حسب الفرع/الفئة/الصنف
                $.get("/SharedDataSources/GetItemCustomSellPrice", { itemId: $("#ItemId").val(), branchId: $("#BranchId").val() }, function (data) {
                    newPrice = data.data;
                    $("#PricingPolicyId").val(null);
                    $("#Price").val(newPrice);
                    $("#Amount").val(newPrice * $("#Quantity").val());
                });
                if (newPrice === 0) {
                    //سعر بيع الصنف الافتراضى المسجل 
                    $.get("/SharedDataSources/GetDefaultSellPrice/", { itemId: $("#ItemId").val() }, function (data) {
                        console.log(data);
                        newPrice = data.data;
                        $("#PricingPolicyId").val(null);
                        $("#Price").val(newPrice);
                        $("#Amount").val(newPrice * $("#Quantity").val());
                    });
                }

               
            }

        });
        //اخر اسعار سعر بيع للصنف
        $.get("/SharedDataSources/GetPreviousPrices/", { itemId: $("#ItemId").val(), isSell:true }, function (data) {
            $("#prevouisPrice").empty();
            $("#prevouisPrice").append("<option value='0'>اختر سعر</option>");
            $.each(data, function (index, row) {
                $("#prevouisPrice").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });
        });
        //اظهار تكلفة الصنف
        if ($('#ItemCostCalculateShowInSellRegId').val()==='1') {
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
    //تغيير سعر البيع حسب اختيار اسعار بيع سابقة
    function ChangeCurrentPice() {
        console.log(1);
        $("#Price").val($("#prevouisPrice option:selected").val());
        onPriceOrQuanKeyUp();
      };
    //تغيير سعر البيع حسب سياسة البيع المحدد
    function onPricingPolicyChange() {
        console.log(11);

        $.get("/SharedDataSources/GetPricePolicySellPrice/", { itemId: $("#ItemId").val(), pricePolicyId: $("#PricingPolicyId").val(), personId: $("#CustomerId").val(),isCustomer:true }, function (data) {
            $("#Price").val(data.data);
            onPriceOrQuanKeyUp();
        });
    };

    //العملاء بدلالة فئة العملاء
    function getCustomerOnCategoryChange() {
        //فى حالة ان تم اختيار مندوب
        var selMen = null;
        if ($("#BySaleMen").is(":checked")) {
            if ($("#EmployeeId").val() === '' || $("#EmployeeId").val()===null) {
                toastr.error('لابد من اختيار المندوب اولا', ''); return false;
            }
            selMen = $("#EmployeeId").val();
        }
        $.get("/SharedDataSources/GetCustomerOnCategoryChange/", { id: $("#PersonCategoryId").val(), saleMenId: selMen}, function (data) {
            $("#CustomerId").empty();
            $("#CustomerId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#CustomerId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });
        });
    };
    function getSaleMenDepartmentChange() {
        $.get("/SharedDataSources/GetSaleMenByDepartment", { id: $("#DepartmentId").val() }, function (data) {
            $("#EmployeeId").empty();
            $("#EmployeeId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#EmployeeId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        });
    };
    function onItemUnitChange() {
        if ($("#ItemUnitsId").val()!=null) {
            $.get("/SharedDataSources/GetItemUnitPrice/", { id: $("#ItemUnitsId").val() }, function (data) {
                $("#Quantity").val(1);
                $("#Price").val(data);
                $("#Amount").val(data * 1);
            });
        }
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
        onRdoBarcodeChanged: onRdoBarcodeChanged,
        onRdoSerialChanged: onRdoSerialChanged,

        //step2
        initItemDetails: function () {
            initDTItemDetails();
        },
        addItemDetails: addItemDetails,
        deleteRowItemDetails: deleteRowItemDetails,
        onPriceOrQuanKeyUp: onPriceOrQuanKeyUp,
        //step3
        initSellInvoiceExpenses: function () {
            initDTSellInvoiceExpenses();
        },
        addSellInvoiceExpenses: addSellInvoiceExpenses,
        deleteRowSellInvoiceExpenses: deleteRowSellInvoiceExpenses,
        getSafyInvoice: getSafyInvoice,
        onInvoiceDiscountChange: onInvoiceDiscountChange,
        onPayedValueChange: onPayedValueChange
        , getProductionOrdersOnStoreChange: getProductionOrdersOnStoreChange,
        onProductionOrderChange: onProductionOrderChange,
        onItemChange: onItemChange,
        ChangeCurrentPice: ChangeCurrentPice,
        onPricingPolicyChange: onPricingPolicyChange,
        getCustomerOnCategoryChange: getCustomerOnCategoryChange,
        onRdoValChanged: onRdoValChanged,
        onRdoPercentageChanged: onRdoPercentageChanged,
        onRdoInvoiceValChanged: onRdoInvoiceValChanged,
        onRdoInvoicePercentageChanged: onRdoInvoicePercentageChanged,
        getSaleMenDepartmentChange: getSaleMenDepartmentChange,
        onItemUnitChange: onItemUnitChange,
        unApproval: unApproval,
    };

}();

