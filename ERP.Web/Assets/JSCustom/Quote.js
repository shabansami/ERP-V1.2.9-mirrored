"use strict";
let editor;
var Quote_Module = function () {
    ClassicEditor.create(document.querySelector("#Notes"), { language: { ui: 'en', content: 'ar' } }).then(function (e) { e.ui.view.editable.element.style.height = "200px"; editor = e; }).catch(function (e) { console.error(e) });

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
                        return 'عروض اسعار';
                    },
                    customize: function (win) {
                        $(win.document.body)
                            //.css('font-size', '20pt')
                            .prepend(
                                '<img src=' + localStorage.getItem("logo") + ' style="position:absolute; top:25%; right:10%;opacity: 0.2;" />'
                            );
                        $(win.document.body).find('table')
                            //.addClass('compact')
                            .css('font-size', 'inherit')
                            .css('direction', 'rtl')
                            .css('text-align', 'right')
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
                    filename: "عروض اسعار",
                    title: "عروض اسعار",
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
                url: '/Quotes/GetAll',
                type: 'GET',
                data(d) {
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.customerId = $("#CustomerId").val();
                }
            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'InvoiceNumber', title: 'رقم الفاتورة' },
                { data: 'CustomerName', title: 'العميل' },
                { data: 'InvoiceDate', title: 'تاريخ العملية' },
                { data: 'Safy', title: 'قيمة الفاتورة' },
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
                        var ele = '\
							<div class="btn-group">\<a href="/PrintInvoices/ShowPrintInvoice/?id='+ row.Id + '&typ=Quote" target="_blank" class="btn btn-sm btn-clean btn-icon" title="عرض وطباعة">\
								<i class="fa fa-print"></i>\
							</a>\<a href="/Quotes/Edit/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
								<i class="fa fa-edit"></i>\
							</a>\
							<a href="javascript:;" onclick=Quote_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a>';
                        if (row.OrderSellExist) {
                            ele += 'تم انشاء امر انتاج للعرض';
                        } else {
                            ele += '<a href="/OrderSells/CreateEdit/?quoteId=' + row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تسجيل أمر بيع">\
								<i class="fa fa-shopping-cart"></i>\
							</a>';
                        }
                        ele += '</div>';
                        return ele;
                    },
                }

            ],
            drawCallback: function () {
                var html = ' <tr><th colspan ="4" style= "text-align:center" >الاجمالى : <label>';
                var api = this.api();
                var balance = api.column(4).data().sum();
                $(api.table().footer()).html(html + balance + '</label></th>  </tr>');
            },

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

    function SubmitForm(btn, isSaveByParam) {
        try {
            var form = document.getElementById('form1');
            var DT_Datasource;
            var dataSet = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    DT_Datasource = JSON.stringify(dataSet);
                }
            }
            var formData = new FormData(form);
            formData.append('DT_Datasource', DT_Datasource);
            var isInvoiceDisVal = document.getElementById("isInvoiceDisVal").value;
            formData.append("isInvoiceDisVal", isInvoiceDisVal);
            formData.append("editorNotes", editor.getData())
            $.ajax({
                type: 'POST',
                url: form.action,
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        //$(btn).attr('disabled', 'disabled'); // disabled button after one clicke 
                        $(btn).css('pointer-events', 'none'); // disabled a link after one clicke 
                        toastr.success(res.message, '',)
                        if (isSaveByParam === 'print') { //فى حالةالضغط على حفظ وطباعه
                            setTimeout(function () { window.location = "/PrintInvoices/ShowPrintInvoice/?id=" + res.refGid + "&typ=Quote" }, 3000);
                        }else
                        if (!res.isInsert) {
                            setTimeout(function () { window.location = "/Quotes/Index" }, 3000);
                        } else
                            setTimeout(function () { window.location = "/Quotes/CreateEdit" }, 3000);
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
                var url = '/Quotes/Delete';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "id": id
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
    //تغيير سعر البيع حسب اختيار اسعار بيع سابقة
    function ChangeCurrentPice() {
        $("#Price").val($("#prevouisPrice option:selected").val());
        onPriceOrQuanKeyUp();
    };
    //تغيير سعر البيع حسب سياسة البيع المحدد
    function onPricingPolicyChange() {
        $.get("/SharedDataSources/GetPricePolicySellPrice/", { itemId: $("#ItemId").val(), pricePolicyId: $("#PricingPolicyId").val(), personId: $("#CustomerId").val(), isCustomer: true }, function (data) {
            $("#Price").val(data.data);
            onPriceOrQuanKeyUp();
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
                url: '/Quotes/GetDSItemDetails',
                type: 'GET',
               
            },
            columns: [
                { data: 'Id', title: 'م', visible: false },
                { data: 'ItemId', visible: false },
                { data: 'ItemName', title: 'الصنف' },
                { data: 'QuantityUnitName', title: 'الكمية بالوحدة' },
                { data: 'Quantity', title: 'الكمية' },
                { data: 'Price', title: 'السعر' },
                { data: 'Amount', title: 'القيمة' },
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
							<a href="javascript:;" onclick=Quote_Module.deleteRowItemDetails('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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
            var ItemUnitsId = document.getElementById('ItemUnitsId').value;

            var formData = new FormData();
            if (itemId === '') {
                toastr.error('تأكد من اختيار الصنف', '');
                return false;
            };
            if (price === '') {
                toastr.error('تأكد من ادخال السعر', '');
                return false;
            };
            if (quantity === '' || quantity == '0') {
                toastr.error('تأكد من ادخال الكمية', '');
                return false;
            };

            formData.append('ItemId', itemId)
            formData.append('Price', price)
            formData.append('Quantity', quantity)
            formData.append('Amount', amount)
            formData.append('ItemUnitsId', ItemUnitsId)

            var dataSet = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/Quotes/AddItemDetails',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtItemDetails').DataTable().ajax.reload();
                        $('#ItemId').val(null);
                        $('#ItemId').select2({
                            placeholder: "اختر عنصر من القائمة"
                        });
                        $('#Price').val(0);
                        $('#Quantity').val(0);
                        $('#Amount').val(0);
                        $('#PricePolicyId').val(null);
                        $('#ItemBarcode').val(null);
                        $('#TotalAmount,#TotalAmount2').text(res.totalAmount);
                        $('#TotalQuantity').text(res.totalQuantity);
                        $("#TotalDiscount").text(Number.parseFloat($("#TotalDiscount").text()));
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
            $('#kt_dtItemDetails').DataTable().row($(this).parents('tr')).remove().draw();
        })

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
    function onPriceOrQuanKeyUp() {
        if ($("#Price").val() < 0 || $("#Quantity").val() < 0 || isNaN($("#Price").val()) || isNaN($("#Quantity").val())) {
            toastr.error('تأكد من ادخال ارقام صحيحية للعدد والكمية', '');
        } else
            $("#Amount").val($("#Price").val() * $("#Quantity").val());
    };
    function onItemChange() {
        //سياسة اسعار عميل محدد فى فاتورة بيع 
        $.get("/SharedDataSources/GetItemPriceByCustomer", { id: $("#CustomerId").val(), itemId: $("#ItemId").val(), isCustomer: true }, function (data) {
            if (data.customeSell > 0) {
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
                        newPrice = data.data;
                        $("#PricingPolicyId").val(null);
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
        //وحدات الصنف 
        $.get("/SharedDataSources/GeItemUnits", { id: $("#ItemId").val() }, function (data) {
            $("#ItemUnitsId").empty();
            $("#ItemUnitsId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#ItemUnitsId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        });

    };
    function onItemUnitChange() {
        if ($("#ItemUnitsId").val() != null) {
            $.get("/SharedDataSources/GetItemUnitPrice/", { id: $("#ItemUnitsId").val() }, function (data) {
                $("#Quantity").val(1);
                $("#Price").val(data);
                $("#Amount").val(data * 1);
            });
        }
    };
    //#endregion ========= end Step 2 ==========

    //#region الضرائب والخصومات على الفاتورة
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
    function getSafyInvoice() {
        var TotalAmount = Number.parseFloat($("#TotalAmount").text());
        if (isNaN(TotalAmount))
            TotalAmount = 0;

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

        var safy = ((TotalAmount  + SalesTax) - (TotalDiscount + ProfitTax)).toFixed(2);
        $("#SafyInvoice").text(safy);
    };

    function onInvoiceDiscountChange() {
        if (isNaN(Number.parseFloat($("#InvoiceDiscount").val())) === false) {
            if ($("#rdo_valAllInvoice:checked").val()) {
                $("#TotalDiscount").text(Number.parseFloat($("#InvoiceDiscount").val()));
            } else if ($("#rdo_percentageAllInvoice:checked").val()) {
                var invoiceDis = (Number.parseFloat($("#TotalAmount").text()) * Number.parseFloat($("#DiscountPercentage").val())) / 100;
                $("#TotalDiscount").text( invoiceDis);
                $("#InvoiceDiscount").val(invoiceDis);
            } else {
                $("#TotalDiscount").text(Number.parseFloat($("#InvoiceDiscount").val()));
            }
            getSafyInvoice();
        };
    };
    //#endregion

    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        initItemDetails: function () {
            initDTItemDetails();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        addItemDetails: addItemDetails,
        deleteRowItemDetails: deleteRowItemDetails,
        onPriceOrQuanKeyUp: onPriceOrQuanKeyUp,
        onItemChange: onItemChange,
        onRdoBarcodeChanged: onRdoBarcodeChanged,
        onRdoSerialChanged: onRdoSerialChanged,
        onRdoInvoiceValChanged: onRdoInvoiceValChanged,
        onRdoInvoicePercentageChanged: onRdoInvoicePercentageChanged,
        onInvoiceDiscountChange: onInvoiceDiscountChange,
        ChangeCurrentPice: ChangeCurrentPice,
        onPricingPolicyChange: onPricingPolicyChange,
        onItemUnitChange: onItemUnitChange,
        getSafyInvoice: getSafyInvoice,
    };

}();


