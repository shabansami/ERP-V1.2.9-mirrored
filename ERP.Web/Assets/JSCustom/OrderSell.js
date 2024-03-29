﻿"use strict";

var OrderSell_Module = function () {
    //#region ادارة امر البيع
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
                        return 'أمر بيع';
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
                    filename: "أمر بيع",
                    title: "أمر بيع",
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
                url: '/OrderSells/GetAll',
                type: 'GET',
                data(d) {
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.customerId = $("#CustomerId").val();
                }
            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'OrderSellInvoiceNumber', title: 'رقم أمر البيع' },
                { data: 'CustomerName', title: 'العميل' },
                { data: 'InvoiceDate', title: 'تاريخ العملية' },
                { data: 'TotalValue', title: 'القيمة' },
                { data: 'QuoteInvoiceNumber', title: 'رقم عرض السعر' },
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
							<div class="btn-group">\<a href="/PrintInvoices/ShowPrintInvoice/?id='+ row.Id + '&typ=OrderSell" target="_blank" class="btn btn-sm btn-clean btn-icon" title="عرض وطباعة">\
								<i class="fa fa-print"></i>\
							</a>\<a href="/OrderSells/Edit/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
								<i class="fa fa-edit"></i>\
							</a>\
							<a href="javascript:;" onclick=OrderSell_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a><a href="/OrderSells/OrderForSell/?orderId='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تجهيز بيع">\
								<i class="fa fa-shopping-cart"></i>\
							</a>\</div>\
						';
                        var print = '<a href="/PrintInvoices/ShowPrintInvoice/?id=' + row.Id + '&typ=OrderSell" target="_blank" class="btn btn-sm btn-clean btn-icon" title="عرض وطباعة">\
								<i class="fa fa-print"></i>\
							</a>';
                        if (row.RegSell && row.RegOrderProduction) {
                            return print+='<div class="btn-group"><div>';
                        }
                        if (row.RegSell) {
                            return print += '<div class="btn-group"><a href="/OrderSells/OrderForProduction/?orderId=' + row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تجهيز إنتاج">\
								<i class="fas fa-wrench"></i>\
							</a>\</div>'
                        }
                        if (row.RegOrderProduction) {
                            return print += '<div class="btn-group"><a href="/OrderSells/OrderForSell/?orderId=' + row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تجهيز بيع">\
								<i class="fa fa-shopping-cart"></i>\
							</a>\</div>';
                        }
                        return '\
							<div class="btn-group">\<a href="/PrintInvoices/ShowPrintInvoice/?id='+ row.Id + '&typ=OrderSell" target="_blank" class="btn btn-sm btn-clean btn-icon" title="عرض وطباعة">\
								<i class="fa fa-print"></i>\
							</a><a href="/OrderSells/Edit/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
								<i class="fa fa-edit"></i>\
							</a>\
							<a href="javascript:;" onclick=OrderSell_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a><a href="/OrderSells/OrderForSell/?orderId='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تجهيز بيع">\
								<i class="fa fa-shopping-cart"></i>\
							</a><a href="/OrderSells/OrderForProduction/?orderId='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تجهيز إنتاج">\
								<i class="fas fa-wrench"></i>\
							</a>\</div>\
						';
                    },
                }

            ],
            //drawCallback: function () {
            //    var html = ' <tr><th colspan ="4" style= "text-align:center" >الاجمالى : <label>';
            //    var api = this.api();
            //    var balance = api.column(4).data().sum();
            //    $(api.table().footer()).html(html + balance + '</label></th>  </tr>');
            //},

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
            var formData = new FormData(form);
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
                            setTimeout(function () { window.location = "/OrderSells/Index" }, 3000);
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
                var url = '/OrderSells/Delete';
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

    //#endregion

    //#region تجهيز لفاتورة بيع
    function SubmitFormForSell(btn) {
        try {
            var form = document.getElementById('form1');
            var formData = new FormData(form);
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
                        const urlParams = new URLSearchParams(window.location.search);
                        const orderId = urlParams.get('orderId');
                        setTimeout(function () { window.location = "/SellInvoices/CreateEdit/?orderSellId=" + orderId }, 3000);
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

    function getBalanceStoreChange(id) {  // OrderSellItems[1].StoreItemId
        //console.log("name='OrderSellItems[" + id + "].StoreItemId']");
        var itemId = $("[name='OrderSellItems[" + id + "].ItemId']").val();
        var storeId = $("[name='OrderSellItems[" + id + "].StoreId']").val();
        var currentBalance = $("[name='OrderSellItems[" + id + "].CurrentBalance']");
        $.get("/SharedDataSources/GetBalanceByStore", { itemId: itemId, storeId: storeId }, function (data) {
            currentBalance.val(data.balance);
        });
    };
    //function UpdateData() {
    //    if ($("#StoreId").val()==='') {
    //        toastr.error('تأكد من تحديد المخزن', ''); return false;
    //    }
    //    const urlParams = new URLSearchParams(window.location.search);
    //    const orderId = urlParams.get('orderId');
    //    window.location = "/OrderSells/OrderForSell/?orderId=" + orderId + "&storeId=" + $("#StoreId").val()
    //}
    //#endregion

    //#region تجهيز امر انتاج
    function SubmitFormForProduction(btn) {
        try {
            var form = document.getElementById('form1');
            var formData = new FormData(form);
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
                        const urlParams = new URLSearchParams(window.location.search);
                        const orderId = urlParams.get('orderId');
                        setTimeout(function () { window.location = "/SellInvoices/CreateEdit/?orderSellId=" + orderId }, 3000);
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
    function ExcuteProdictionOrder() {
        const urlParams = new URLSearchParams(window.location.search);
        const orderId = urlParams.get('orderId');
        window.location = "/OrderSells/OrderForProduction/?orderId=" + orderId + "&fi=1" 
    }
    //#endregion

    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        SubmitFormForSell: SubmitFormForSell,
        SubmitFormForProduction: SubmitFormForProduction,
        ExcuteProdictionOrder: ExcuteProdictionOrder,
        getBalanceStoreChange: getBalanceStoreChange,
    };

}();


