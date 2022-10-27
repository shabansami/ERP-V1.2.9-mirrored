"use strict";

var Offer_Module = function () {
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
                        return 'العروض';
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
                    filename: "العروض",
                    title: "العروض",
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
                url: '/Offers/GetAll',
                type: 'GET',
                data(d) {
                    d.dtFrom = $('#dtFrom').val();
                    d.dtTo = $('#dtTo').val();
                }
            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'Name', title: 'مسمى العرض' },
                { data: 'OfferTypeName', title: 'نوع العرض' },
                { data: 'StartDate', title: 'من الفترة' },
                { data: 'EndDate', title: 'الى الفترة' },
                { data: 'AmountBefore', title: 'السعر قبل العرض' },
                { data: 'AmountAfter', title: 'السعر بعد العرض' },
                { data: 'Limit', title: 'الحد المسموح به لشراء العرض' },
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
							<a href="javascript:;" onclick=Offer_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
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

    function SubmitForm(btn) {
        try {
            var form = document.getElementById('form1');
            var IsDiscountItemVal = $('#IsDiscountItemVal').val();
            var formData = new FormData(form);
            if ($('#OfferTypeId').val()==='1') {
                var DT_Datasource;
                var dataSet = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
                if (dataSet != null) {
                    if (dataSet.length > 0) {
                        DT_Datasource = JSON.stringify(dataSet);
                        formData.append('DT_Datasource', DT_Datasource);
                    }
                }
            }
            formData.append('IsDiscountItemVal', IsDiscountItemVal);
            $.ajax({
                type: 'POST',
                url: form.action,
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $(btn).css('pointer-events', 'none'); // disabled a link after one clicke 
                        toastr.success(res.message, '',)
                        /*if (!res.isInsert) {*/
                            //setTimeout(function () { window.location = "/Offers/Index" }, 3000);
                        /*} else*/
                            setTimeout(function () { window.location = "/Offers/CreateEdit" }, 3000);
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
                var url = '/Offers/Delete';
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
                url: '/Offers/GetDSItemDetails',
                type: 'GET',

            },
            columns: [
                { data: 'Id', title: 'م', visible: false },
                { data: 'ItemId', visible: false },
                { data: 'ItemName', title: 'الصنف' },
                { data: 'Quantity', title: 'الكمية' },
                { data: 'Price', title: 'سعر الوحدة' },
                { data: 'Amount', title: 'القيمة' },
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
							<a href="javascript:;" onclick=Offer_Module.deleteRowItemDetails('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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
            var formData = new FormData();
            if (itemId === '') {
                toastr.error('تأكد من اختيار الصنف', '');
                return false;
            };
            if (price === '') {
                toastr.error('تأكد من ادخال سعر البيع', '');
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
            var dataSet = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/Offers/AddItemDetails',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtItemDetails').DataTable().ajax.reload();
                        $('#AmountBefore').val(res.totalAmount);
                        $('#ItemId').val(null);
                        $('#ItemId').select2({
                            placeholder: "اختر عنصر من القائمة"
                        });
                        $('#Price').val(0);
                        $('#Quantity').val(0);
                        $('#Amount').val(0);
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
            var amountBefore=$('#AmountBefore').val();
            $('#AmountBefore').val(amountBefore - amountRemoved);
            $('#kt_dtItemDetails').DataTable().row($(this).parents('tr')).remove().draw();
        })

    };

    //#endregion ========= end Step 2 ==========


    function onItemChange() {  
        //سعر بيع الصنف الافتراضى المسجل 
        $.get("/SharedDataSources/GetDefaultSellPrice/", { itemId: $("#ItemId").val() }, function (data) {
           var newPrice = data.data;
            $("#Price").val(newPrice);
            $("#Amount").val(newPrice * $("#Quantity").val());
        });

    };
    function onPriceOrQuanKeyUp() {
        if ($("#Price").val() < 0 || $("#Quantity").val() < 0 || isNaN($("#Price").val()) || isNaN($("#Quantity").val())) {
            toastr.error('تأكد من ادخال ارقام صحيحية للعدد والكمية', '');
        } else
            $("#Amount").val($("#Price").val() * $("#Quantity").val());
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
    function onOfferTypeChange() {
        if ($("#OfferTypeId").val() === '1') {
            $("#div_offerTypItems").show();
            $("#div_items").show();
            $("#div_offerTypInvoice").hide();
        }
        if ($("#OfferTypeId").val() === '2') {
            $("#div_offerTypInvoice").show();
            $("#div_offerTypItems").hide();
            $("#div_items").hide();
        }
    };
    function onDiscountChange() {
        if ($("#OfferTypeId").val() === '1') {
            var amountBefore = Number.parseFloat($("#AmountBefore").val());
            if (isNaN(amountBefore))
                return toastr.error('تأكد من ادخال قيمة الاصناف قبل العرض اولا', '');
            
            var discountVal = 0;
            console.log($('#IsDiscountItemVal').val());
            if ($('#IsDiscountItemVal').val() === 'true') {//الخصم بقيمة
                discountVal = Number.parseFloat($("#DiscountAmount").val());
                console.log(discountVal);
                if (isNaN(discountVal) || discountVal == 0)
                    return toastr.error('تأكد من ادخال قيمة الخصم', '');
            } else {
                var percentage = Number.parseFloat($("#DiscountAmount").val());
                if (isNaN(percentage) || percentage == 0)
                    return toastr.error('تأكد من ادخال نسبة الخصم', '');
                discountVal = amountBefore * percentage/100
            }
            if (amountBefore < discountVal) {
                return toastr.error('تأكد من ادخال قيمة الخصم اقل من قيمة الاصناف', '');
            }
            $('#AmountAfter').val(amountBefore - discountVal);
        }
    }
   
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
        onItemChange: onItemChange,
        addItemDetails: addItemDetails,
        deleteRowItemDetails: deleteRowItemDetails,
        onPriceOrQuanKeyUp: onPriceOrQuanKeyUp,
        onRdoValChanged: onRdoValChanged,
        onRdoPercentageChanged: onRdoPercentageChanged,
        onOfferTypeChange: onOfferTypeChange,
        onDiscountChange: onDiscountChange,
    };

}();


