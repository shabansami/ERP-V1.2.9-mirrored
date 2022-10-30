"use strict";

var RptPurchaseInvoice_Module = function () {
    //#region ============== التوريدات
    var initPurchase = function () {
        var table = $('#kt_datatablePurchase');

        // begin first table
        table.DataTable({
            //responsive: true,
            //searchDelay: 500,
            processing: true,
            //serverSide: false,
            //select: true,

            // DOM Layout settings
            dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",
            buttons: [
                {
                    extend: 'print',
                    title: function () {
                        return 'فواتير التوريدات';
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
                    filename: "فواتير التوريدات",
                    title: "فواتير التوريدات",
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
                url: '/RptPurchaseInvoices/GetPurchases',
                type: 'GET',
                data(d) {
                    d.branchId = $("#BranchId").val();
                    d.supplierId = $("#SupplierId").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.isFinalApproval = $("#IsFinalApproval").is(":checked");
                    d.isDiscount = $("#IsDiscount").is(":checked");
                    d.itemId = $("#ItemId").val();
                    d.purchaseAmount = $("#PurchaseAmount").val();
                    d.PaymentTypeId = $("#PaymentTypeId").val();
                }


            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'InvoiceDate', title: 'التاريخ' },
                { data: 'InvoiceNum', title: 'رقم الفاتورة' },
                { data: 'SupplierName', title: 'المورد' },
                { data: 'TotalQuantity', title: 'اجمالى العدد' },
                { data: 'TotalValue', title: 'الاجمالى' },
                { data: 'Safy', title: 'الصافى' },
                { data: 'PaymentTypeName', title: 'طريقة السداد' },
                { data: 'CaseName', title: 'اخر حالة' },
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
					        \<a href="/PurchaseInvoices/ShowPurchaseInvoice/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض فاتورة">\
								<i class="fa fa-search"></i>\
							</a>\
                            <a href="/PurchaseInvoices/ShowHistory/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض الحالات">\
								<i class="fa fa-random"></i>\
							</a>\</div>\
						';

                    },
                }

            ],
            drawCallback: function () {
                var html = ' <tr><th colspan ="6" style= "text-align:center" ><div class="row alert-warning"><label>اجمالى قيمة المشتريات : ';
                var api = this.api();
                $(api.table().footer()).html(html + api.column(5).data().sum() + '</label></div></th>  <th colspan ="6" style= "text-align:center" ><div class="row alert-warning"><label>اجمالى صافى الفواتير :' + api.column(6).data().sum() + ' </label ></div ></th >  </tr>');
            },

            "order": [[0, "asc"]]
            //"order": [[0, "desc"]]

        });
        $('#export_print').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatablePurchase').DataTable().button('.buttons-print').trigger();
        });

        $('#export_copy').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatablePurchase').DataTable().button('.buttons-copy').trigger();
        });

        $('#export_excel').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatablePurchase').DataTable().button('.buttons-excel').trigger();
        });

    };

//#endregion
    //#region ============== مرتجع التوريدات
    var initBackPurchase = function () {
        var table = $('#kt_datatableBackPurchase');

        // begin first table
        table.DataTable({
            //responsive: true,
            //searchDelay: 500,
            processing: true,
            //serverSide: false,
            //select: true,

            // DOM Layout settings
            dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",
            buttons: [
                {
                    extend: 'print',
                    title: function () {
                        return 'فواتير مرتجع التوريدات';
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
                    filename: "فواتير مرتجع التوريدات",
                    title: "فواتير مرتجع التوريدات",
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
                url: '/RptPurchaseInvoices/GetBackPurchases',
                type: 'GET',
                data(d) {
                    d.branchId = $("#BranchId").val();
                    d.supplierId = $("#SupplierId").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.isFinalApproval = $("#IsFinalApproval").is(":checked");
                    d.isDiscount = $("#IsDiscount").is(":checked");
                    d.itemId = $("#ItemId").val();
                    d.purchaseAmount = $("#PurchaseAmount").val();
                    d.PaymentTypeId = $("#PaymentTypeId").val();
                }


            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'InvoiceDate', title: 'التاريخ' },
                { data: 'InvoiceNum', title: 'رقم الفاتورة' },
                { data: 'SupplierName', title: 'المورد' },
                { data: 'TotalQuantity', title: 'اجمالى العدد' },
                { data: 'TotalValue', title: 'الاجمالى' },
                { data: 'Safy', title: 'الصافى' },
                { data: 'PaymentTypeName', title: 'طريقة السداد' },
                { data: 'CaseName', title: 'اخر حالة' },
                { data: 'Actions', responsivePriority: -1, className: 'actions' },

            ],
            columnDefs: [
                {
                    targets: 0,
                    title: 'م',
                    orderable: false,
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
					        \<a href="/PurchaseBackInvoices/ShowPurchaseBackInvoice/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض فاتورة">\
								<i class="fa fa-search"></i>\
							</a>\
                            <a href="/PurchaseBackInvoices/ShowHistory/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض الحالات">\
								<i class="fa fa-random"></i>\
							</a>\</div>\
						';

                    },
                }

            ],
            drawCallback: function () {
                var html = ' <tr><th colspan ="6" style= "text-align:center" ><div class="row alert-warning"><label>اجمالى قيمة مرتجع المشتريات : ';
                var api = this.api();
                $(api.table().footer()).html(html + api.column(5).data().sum() + '</label></div></th>  <th colspan ="6" style= "text-align:center" ><div class="row alert-warning"><label>اجمالى صافى مرتجع الفواتير :' + api.column(6).data().sum() + ' </label ></div ></th >  </tr>');
            },

            "order": [[0, "asc"]]
            //"order": [[0, "desc"]]

        });
        $('#export_print').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableBackPurchase').DataTable().button('.buttons-print').trigger();
        });

        $('#export_copy').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableBackPurchase').DataTable().button('.buttons-copy').trigger();
        });

        $('#export_excel').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableBackPurchase').DataTable().button('.buttons-excel').trigger();
        });

    };

//#endregion
    function onItemTypeChange() {
        $.get("/SharedDataSources/OnItemTypeChange", { id: $("#ItemtypeId").val() }, function (data) {
            $("#ItemId").empty();
            $("#ItemId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#ItemId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });
        });
    };
    return {
        //main function to initiate the module
        initPurchase: function () {
            initPurchase();
        },
        initBackPurchase: function () {
            initBackPurchase();
        },
        onItemTypeChange: onItemTypeChange,

    };

}();


