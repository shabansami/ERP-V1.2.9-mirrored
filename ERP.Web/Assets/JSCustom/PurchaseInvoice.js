"use strict";

var PurchaseInvoice_Module = function () {

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
                        return 'البنوك';
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
                    filename: "فواتير التوريد",
                    title: "فواتير التوريد",
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
                url: '/PurchaseInvoices/GetAll',
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
            { data: 'InvoiceNumPaper', title: 'رقم الفاتورة الورقية' },
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
							<a href="/PurchaseInvoices/ShowPurchaseInvoice/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض فاتورة">\
								<i class="fa fa-search"></i>\
							</a>\<a href="#" onclick="PrintInvoice_Module.Print(\''+ row.Id + '\',\'purchase\',null);" class="btn btn-sm btn-clean btn-icon" title="طباعه فاتورة">\
								<i class="fa fa-print"></i>\
							</a>\<a href="#" onclick="PrintInvoice_Module.Print(\''+ row.Id + '\',\'purchase\',\'quantityOnly\');" class="btn btn-sm btn-clean btn-icon" title="طباعه فاتورة كميات">\
								<i class="fa fa-print"></i>\
							</a>\<a href="#" onclick="PrintInvoice_Module.DownloadInvoice(\''+ row.Id + '\',\'purchase\');" class="btn btn-sm btn-clean btn-icon" title="تنزيل فاتورة">\
								<i class="fa fa-download"></i>\
							</a>\
							<a href="/UploadCenterTypeFiles/Index/?typ=' + row.typ + '&refGid=' + row.Id + '" class="btn btn-sm btn-clean btn-icUrln" title="رفع ملفات الفاتورة">\
								<i class="fa fa-upload"></i>\
							</a><a href="/PurchaseInvoices/ShowHistory/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض الحالات">\
								<i class="fa fa-random"></i>\
							</a>\
						';
                    if (row.IsFinalApproval) {
                        ele += '<a href="/GeneralDailies/Index/?tranId=' + row.Id + '&tranTypeId=1" class="btn btn-sm btn-clean btn-icon" title="عرض القيود">\
								<i class="fa fa-money-bill"></i>\
							</a>\<ahref="javascript:;" onclick=PurchaseInvoice_Module.unApproval(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icon" title="فك الاعتماد">\
								<i class="fa fa-unlock-alt"></i>\
							</a>';

                    } else {
                        ele += '<a href="/PurchaseInvoices/Edit/?invoGuid=' + row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
								<i class="fa fa-edit"></i>\
							</a>\<a href="javascript:;" onclick=PurchaseInvoice_Module.deleteRow("'+ row.Id + '") class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a>';
                    }

                    return ele + '</div>';


      //                  return '\
						//	<div class="btn-group">\
						//	<a href="/PurchaseInvoices/Edit/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
						//		<i class="fa fa-edit"></i>\
						//	</a>\<a href="/PurchaseInvoices/ShowPurchaseInvoice/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض فاتورة">\
						//		<i class="fa fa-search"></i>\
						//	</a>\<a href="#" onclick="PrintInvoice_Module.Print(\''+ row.Id +'\',\'purchase\',null);" class="btn btn-sm btn-clean btn-icon" title="طباعه فاتورة">\
						//		<i class="fa fa-print"></i>\
						//	</a>\<a href="#" onclick="PrintInvoice_Module.Print(\''+ row.Id + '\',\'purchase\',\'quantityOnly\');" class="btn btn-sm btn-clean btn-icon" title="طباعه فاتورة كميات">\
						//		<i class="fa fa-print"></i>\
						//	</a>\<a href="#" onclick="PrintInvoice_Module.DownloadInvoice(\''+ row.Id +'\',\'purchase\');" class="btn btn-sm btn-clean btn-icon" title="تنزيل فاتورة">\
						//		<i class="fa fa-download"></i>\
						//	</a>\
						//	<a href="javascript:;" onclick=PurchaseInvoice_Module.deleteRow("'+ row.Id + '") class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
						//		<i class="fa fa-trash"></i>\
						//	</a><a href="/UploadCenterTypeFiles/Index/?typ=' + row.typ + '&refGid=' + row.Id+'" class="btn btn-sm btn-clean btn-icUrln" title="رفع ملفات الفاتورة">\
						//		<i class="fa fa-upload"></i>\
						//	</a><a href="/PurchaseInvoices/ShowHistory/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض الحالات">\
						//		<i class="fa fa-random"></i>\
						//	</a>\<a href="/GeneralDailies/Index/?tranId='+ row.Id + '&tranTypeId=1" class="btn btn-sm btn-clean btn-icon" title="عرض القيود">\
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
            var dataSetExpense = $('#kt_dtPurchaseInvoiceExpenses').DataTable().rows().data().toArray();
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
                                setTimeout(function () { window.location = "/PurchaseInvoices/Index" }, 3000);
                            } else
                                setTimeout(function () { window.location = "/PurchaseInvoices/CreateEdit" }, 3000);
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
                var url = '/PurchaseInvoices/Delete';
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
                var url = '/PurchaseInvoices/UnApproval';
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
                oPaginate: {
                    sNext: '<span class="pagination-default">التالى</span><span class="pagination-fa"><i class="fa fa-chevron-left" ></i></span>',
                    sPrevious: '<span class="pagination-default">السابق</span><span class="pagination-fa"><i class="fa fa-chevron-right" ></i></span>'
                }
            },

            ajax: {
                url: '/PurchaseInvoices/GetDSItemDetails',
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
							<a href="javascript:;" onclick=PurchaseInvoice_Module.deleteRowItemDetails('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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
                url: '/PurchaseInvoices/AddItemDetails',
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
                        $('#TotalQuantity').text(res.totalQuantity);
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

    //تغيير الصنف
    function onItemChange() {
        //اخر اسعار سعر شراء للصنف
        $.get("/SharedDataSources/GetPreviousPrices/", { itemId: $("#ItemId").val(), isSell:false}, function (data) {
            $("#prevouisPrice").empty();
            $("#prevouisPrice").append("<option value='0'>اختر سعر</option>");
            $.each(data, function (index, row) {
                $("#prevouisPrice").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });
        });
        //سياسة اسعار مورد محدد فى فاتورة التوريد 
        $.get("/SharedDataSources/GetItemPriceByCustomer", { id: $("#SupplierId").val(), itemId: $("#ItemId").val(),isCustomer:false }, function (data) {
            if (data.customeSell > 0) {
                $("#PricingPolicyId").val(data.pricingPolicyId);
                $("#Price").val(data.customeSell);
                $("#Amount").val(data.customeSell * $("#Quantity").val());
            } else {
                $("#PricingPolicyId").val(null);
                $("#Price").val(0);
                $("#Quantity").val(0);
                $("#Amount").val(0);

            }

        });
    };
    //تغيير سعر البيع حسب اختيار اسعار بيع سابقة
    function ChangeCurrentPice() {
        $("#Price").val($("#prevouisPrice option:selected").val());
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
    //تغيير سعر البيع حسب سياسة البيع المحدد
    function onPricingPolicyChange() {
        $.get("/SharedDataSources/GetPricePolicySellPrice/", { itemId: $("#ItemId").val(), pricePolicyId: $("#PricingPolicyId").val(), personId: $("#SupplierId").val(), isCustomer: false }, function (data) {
            $("#Price").val(data.data);
            onPriceOrQuanKeyUp();
        });
    };
    //#endregion ========= end Step 2 ==========


    //#region ======== Step 3 تسجيل المصروفات=================
    var initDTPurchaseInvoiceExpenses = function () {
        var table = $('#kt_dtPurchaseInvoiceExpenses');

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
                url: '/PurchaseInvoices/GetDStPurchaseInvoiceExpenses',
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
							<a href="javascript:;" onclick=PurchaseInvoice_Module.deleteRowPurchaseInvoiceExpenses('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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

    function addPurchaseInvoiceExpenses() {
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
            var dataSet = $('#kt_dtPurchaseInvoiceExpenses').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/PurchaseInvoices/AddPurchaseInvoiceExpenses',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtPurchaseInvoiceExpenses').DataTable().ajax.reload();
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

    function deleteRowPurchaseInvoiceExpenses(id) {
        $('#kt_dtPurchaseInvoiceExpenses tbody').on('click', 'a.deleteIcon', function () {
            var amountExpenseRemoved = $('#kt_dtPurchaseInvoiceExpenses').DataTable().row($(this).closest('tr')).data()['ExpenseAmount'];
            $("#TotalExpenses,#TotalExpenses2").text(Number.parseFloat($("#TotalExpenses").text()) - amountExpenseRemoved);
            $('#kt_dtPurchaseInvoiceExpenses').DataTable().row($(this).parents('tr')).remove().draw();
            getSafyInvoice();

        })

    };

    function onPriceOrQuanKeyUp() {
        if ($("#Price").val() < 0 || $("#Quantity").val() < 0 || $("#Price").val()==="" || $("#Quantity").val()==="") {
            toastr.error('تأكد من ادخال ارقام صحيحية للعدد والكمية', '');
        } else
            $("#Amount").val($("#Price").val() * $("#Quantity").val());
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
        onPricingPolicyChange: onPricingPolicyChange,
        //step3
        initPurchaseInvoiceExpenses: function () {
            initDTPurchaseInvoiceExpenses();
        },
        addPurchaseInvoiceExpenses: addPurchaseInvoiceExpenses,
        deleteRowPurchaseInvoiceExpenses: deleteRowPurchaseInvoiceExpenses,
        getSafyInvoice: getSafyInvoice,
        onInvoiceDiscountChange: onInvoiceDiscountChange,
        onPayedValueChange: onPayedValueChange,
        ChangeCurrentPice: ChangeCurrentPice,
        onItemChange: onItemChange,
        getSupplierOnCategoryChange: getSupplierOnCategoryChange,
        unApproval: unApproval,
    };

}();

