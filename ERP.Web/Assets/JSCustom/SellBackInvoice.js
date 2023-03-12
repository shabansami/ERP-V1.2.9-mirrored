"use strict";

var SellBackInvoice_Module = function () {

    //#region ======== Save Sell back invoice ==================
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
                        return 'فواتير مرتجع البيع';
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
                    filename: "فواتير مرتجع البيع",
                    title: "فواتير مرتجع البيع",
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
                url: '/SellBackInvoices/GetAll',
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
            { data: 'CustomerName', title: 'اسم العميل' },
            { data: 'Safy', title: 'صافى الفاتورة' },
            { data: 'ApprovalAccountant', title: 'حالة الاعتماد محاسبيا' },
            { data: 'ApprovalStore', title: 'حالة الاعتماد مخزنيا' },
            { data: 'CaseName', title: 'اخر حالة للفاتورة' },
            { data: 'InvoType', title: 'حالة الفاتورة', visible: false },
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
       //         		< a href = "/SellBackInvoices/Edit/?invoGuid='+ row.Id + '" class= "btn btn-sm btn-clean btn-icon" title = "تعديل" >\
       //         <i class="fa fa-edit"></i>\
							//</a >
                targets: -1,
                title: 'عمليات',
                orderable: false,
                    render: function (data, type, row, meta) {
                        var ele = '\
							<div class="btn-group">\<a href="/PrintInvoices/ShowPrintInvoice/?id='+ row.Id + '&typ=sellBack" target="_blank" class="btn btn-sm btn-clean btn-icon" title="عرض وطباعة">\
								<i class="fa fa-print"></i>\
							</a><a href="/SellBackInvoices/ShowSellBackInvoice/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض فاتورة">\
								<i class="fa fa-search"></i>\
							</a>\<a href="#" onclick="PrintInvoice_Module.Print(\''+ row.Id + '\',\'sellBack\',null);" class="btn btn-sm btn-clean btn-icon" title="طباعه كاشير">\
								<i class="fa fa-print"></i>\
							</a>\<a href="#" onclick="PrintInvoice_Module.Print(\''+ row.Id + '\',\'sellBack\',\'quantityOnly\');" class="btn btn-sm btn-clean btn-icon" title="طباعه كاشير كميات">\
								<i class="fa fa-print"></i>\
							</a>\<a href="#" onclick="PrintInvoice_Module.DownloadInvoice(\''+ row.Id + '\',\'sellBack\');" class="btn btn-sm btn-clean btn-icon" title="تنزيل فاتورة">\
								<i class="fa fa-download"></i>\
							</a>\
							<a href="/UploadCenterTypeFiles/Index/?typ=' + row.typ + '&refGid=' + row.Id + '" class="btn btn-sm btn-clean btn-icUrln" title="رفع ملفات الفاتورة">\
								<i class="fa fa-upload"></i>\
							</a><a href="/SellBackInvoices/ShowHistory/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض الحالات">\
								<i class="fa fa-random"></i>\
							</a>\
						';
                        if (row.IsFinalApproval) {
                            ele += '<a href="/GeneralDailies/Index/?tranId=' + row.Id + '&tranTypeId=4" class="btn btn-sm btn-clean btn-icon" title="عرض القيود">\
								<i class="fa fa-money-bill"></i>\
							</a>\<ahref="javascript:;" onclick=SellBackInvoice_Module.unApproval(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icon" title="فك الاعتماد">\
								<i class="fa fa-unlock-alt"></i>\
							</a>';

                        } else {
                            ele += '<a href="/SellBackInvoices/Edit/?invoGuid=' + row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
								<i class="fa fa-edit"></i>\
							</a>\<a href="javascript:;" onclick=SellBackInvoice_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a>';
                        }

                        return ele + '</div>';

      //                  return '\
						//	<div class="btn-group">\
					 //       \<a href="/SellBackInvoices/ShowSellBackInvoice/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض فاتورة">\
						//		<i class="fa fa-search"></i>\
						//	</a>\<a href="#" onclick="PrintInvoice_Module.Print(\''+ row.Id + '\',\'sellBack\',null);" class="btn btn-sm btn-clean btn-icon" title="طباعه فاتورة">\
						//		<i class="fa fa-print"></i>\
						//	</a>\<a href="#" onclick="PrintInvoice_Module.Print(\''+ row.Id + '\',\'sellBack\',\'quantityOnly\');" class="btn btn-sm btn-clean btn-icon" title="طباعه فاتورة كميات">\
						//		<i class="fa fa-print"></i>\
						//	</a>\<a href="#" onclick="PrintInvoice_Module.DownloadInvoice(\''+ row.Id +'\',\'sellBack\');" class="btn btn-sm btn-clean btn-icon" title="تنزيل فاتورة">\
						//		<i class="fa fa-download"></i>\
						//	</a>\
						//	<a href="javascript:;" onclick=SellBackInvoice_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
						//		<i class="fa fa-trash"></i>\
						//	</a><a href="/UploadCenterTypeFiles/Index/?typ=' + row.typ + '&refGid=' + row.Id+'" class="btn btn-sm btn-clean btn-icUrln" title="رفع ملفات الفاتورة">\
						//		<i class="fa fa-upload"></i>\
						//	</a><a href="/SellBackInvoices/ShowHistory/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض الحالات">\
						//		<i class="fa fa-random"></i>\
						//	</a>\<a href="/GeneralDailies/Index/?tranId='+ row.Id + '&tranTypeId=4" class="btn btn-sm btn-clean btn-icon" title="عرض القيود">\
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
            var dataSetExpense = $('#kt_dtSellBackInvoiceExpenses').DataTable().rows().data().toArray();
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
                                setTimeout(function () { window.location = "/SellBackInvoices/Index" }, 3000);
                            } else
                                setTimeout(function () { window.location = "/SellBackInvoices/CreateEdit" }, 3000);
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
                var url = '/SellBackInvoices/Delete';
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
                var url = '/SellBackInvoices/UnApproval';
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
            $("#btnInstallment").hide();
           
        } else if ($("#PaymentTypeId").val() == "2") {
            $("#rdo_safe,#rdo_bank").prop('checked', false);
            $("#rdo_safe,#rdo_bank").attr("disabled", true);
            $("#divSafe").hide();
            $("#divBank").hide();
            $("#PayedValue").val(0);
            $("#btnInstallment").hide();
            onPayedValueChange();

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


    function getSafesOnBranchChanged() { // get safes and stores by branchId
        $.get("/SharedDataSources/getSafesOnBranchChanged", { id: $("#BranchId").val() }, function (data) {
            $("#SafeId").empty();
            $("#SafeId").append("<option value=>اختر عنصر من القائمة </option>");
            $.each(data, function (index, row) {
                $("#SafeId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            })
        });

        $.get("/SharedDataSources/getStoresOnBranchChanged", { id: $("#BranchId").val(), isDamage: false, userId: $("#Hdf_userId").val() }, function (data) {
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
                url: '/SellBackInvoices/GetDSItemDetails',
                type: 'GET',

            },
            columns: [
                { data: 'Id', title: 'م', visible: false },
                { data: 'ItemId', visible: false },
                { data: 'StoreId', visible: false },
                { data: 'SerialItemId', visible: false },
                { data: 'IsIntial', visible: false },
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
							<a href="javascript:;" onclick=SellBackInvoice_Module.deleteRowItemDetails('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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
            var productionOrderId = document.getElementById('productionOrderId').value;
            var isIntial = document.getElementById('isIntial').value;
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
            formData.append('SerialItemId', serialItemId)
            formData.append('ProductionOrderId', productionOrderId)
            formData.append('isIntial', isIntial)
            var dataSet = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/SellBackInvoices/AddItemDetails',
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
                        $('#productionOrderId').val(null);
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



    //#endregion ========= end Step 2 ==========


    //#region ======== Step 3 تسجيل الايرادات=================
    var initDTSellBackInvoiceExpenses = function () {
        var table = $('#kt_dtSellBackInvoiceExpenses');

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
                url: '/SellBackInvoices/GetDStSellBackInvoiceExpenses',
                type: 'GET',

            },
            columns: [
                { data: 'ExpenseTypeId', visible: false },
                { data: 'ExpenseTypeName', title: 'مسمى الايراد' },
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
							<a href="javascript:;" onclick=SellBackInvoice_Module.deleteRowSellBackInvoiceExpenses('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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

    function addSellBackInvoiceExpenses() {
        try {
            var expenseTypeId = document.getElementById('ExpenseTypeId').value;
            var expenseAmount = document.getElementById('ExpenseAmount').value;
            var formData = new FormData();
            if (ExpenseTypeId === '') {
                toastr.error('تأكد من اختيار مسمى الايراد', '');
                return false;
            };
            if (expenseAmount === '' || expenseAmount=="0") {
                toastr.error('تأكد من ادخال  قيمة الايراد', '');
                return false;
            };

            formData.append('ExpenseTypeId', expenseTypeId)
            formData.append('ExpenseAmount', expenseAmount)
            var dataSet = $('#kt_dtSellBackInvoiceExpenses').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/SellBackInvoices/AddSellBackInvoiceExpenses',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtSellBackInvoiceExpenses').DataTable().ajax.reload();
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

    function deleteRowSellBackInvoiceExpenses(id) {
        $('#kt_dtSellBackInvoiceExpenses tbody').on('click', 'a.deleteIcon', function () {
            var amountExpenseRemoved = $('#kt_dtSellBackInvoiceExpenses').DataTable().row($(this).closest('tr')).data()['ExpenseAmount'];
            $("#TotalExpenses,#TotalExpenses2").text(Number.parseFloat($("#TotalExpenses").text()) - amountExpenseRemoved);
            $('#kt_dtSellBackInvoiceExpenses').DataTable().row($(this).parents('tr')).remove().draw();
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

    function onItemChange() {
        //$('#StoreId').val(null);
        //$('#StoreId').select2({
        //    placeholder: "اختر عنصر من القائمة"
        //});
        $("#serialItemId").val(null);
        $("#productionOrderId").val(null);
        $("#isIntial").val(null);
        //سعر بيع الصنف الافتراضى المسجل 
        $.get("/SharedDataSources/GetDefaultSellPrice/", { itemId: $("#ItemId").val() }, function (data) {
            $("#Price").val(data.data);
            $("#Amount").val(data.data * $("#Quantity").val());
        });

    };
    //العملاء بدلالة فئة العملاء
    function getCustomerOnCategoryChange() {
        //فى حالة ان تم اختيار مندوب
        var selMen = null;
        if ($("#BySaleMen").is(":checked")) {
            if ($("#EmployeeId").val() === '' || $("#EmployeeId").val() === null) {
                toastr.error('لابد من اختيار المندوب اولا', ''); return false;
            }
            selMen = $("#EmployeeId").val();
        }
        $.get("/SharedDataSources/GetCustomerOnCategoryChange/", { id: $("#PersonCategoryId").val(), saleMenId: selMen }, function (data) {
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
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        //step 1
        onPaymentTypeChanged: onPaymentTypeChanged,
        onRdoBarcodeChanged: onRdoBarcodeChanged,
        onRdoSerialChanged: onRdoSerialChanged,
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
        //step3
        initSellBackInvoiceExpenses: function () {
            initDTSellBackInvoiceExpenses();
        },
        addSellBackInvoiceExpenses: addSellBackInvoiceExpenses,
        deleteRowSellBackInvoiceExpenses: deleteRowSellBackInvoiceExpenses,
        getSafyInvoice: getSafyInvoice,
        onInvoiceDiscountChange: onInvoiceDiscountChange,
        onPayedValueChange: onPayedValueChange,
        onItemChange: onItemChange,
        getCustomerOnCategoryChange: getCustomerOnCategoryChange,
        getSaleMenDepartmentChange: getSaleMenDepartmentChange,
        unApproval: unApproval,
    };

}();

