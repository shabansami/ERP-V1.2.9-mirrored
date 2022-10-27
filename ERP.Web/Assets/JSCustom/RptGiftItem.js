"use strict";

var RptGiftItem_Module = function () {
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
                        return 'اجمالى اصناف بونص وعينات من مورد';
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
                    filename: "اجمالى اصناف بونص وعينات من مورد",
                    title: "اجمالى اصناف بونص وعينات من مورد",
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
                url: '/RptGiftItems/GetItemGiftPurchases',
                type: 'GET',
                data(d) {
                    d.branchId = $("#BranchId").val();
                    d.supplierId = $("#SupplierId").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.isFinalApproval = $("#IsFinalApproval").is(":checked");
                    d.itemTypeId = $("#ItemtypeId").val();
                    d.itemId = $("#ItemId").val();
                }


            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'Id', visible: false },
                { data: 'ItemCode', title: 'كود الصنف' },
                { data: 'ItemName', title: 'اسم الصنف' },
                { data: 'UnitName', title: 'الوحدة' },
                { data: 'Quantity', title: 'الكمية' },
                { data: 'CostItem', title: 'متوسط تكلفة' },
                { data: 'Amount', title: 'القيمة' },

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

            ],
            drawCallback: function () {
                var html = ' <tr><th colspan ="6" style= "text-align:center" ><div class="row alert-warning"><label>اجمالى الكمية : ';
                var api = this.api();
                $(api.table().footer()).html(html + api.column(5).data().sum() + '</label></div></th>  <th colspan ="6" style= "text-align:center" ><div class="row alert-warning"><label>اجمالى القيمة :' + api.column(7).data().sum() + ' </label ></div ></th >  </tr>');
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
    //#region ============== المبيعات
    var initSell = function () {
        var table = $('#kt_datatableSell');

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
                        return 'اجمالى اصناف بونص وعينات لعميل';
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
                    filename: "اجمالى اصناف بونص وعينات لعميل",
                    title: "اجمالى اصناف بونص وعينات لعميل",
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
                url: '/RptGiftItems/GetItemGiftSells',
                type: 'GET',
                data(d) {
                    d.branchId = $("#BranchId").val();
                    d.customerId = $("#CustomerId").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.isFinalApproval = $("#IsFinalApproval").is(":checked");
                    d.itemTypeId = $("#ItemtypeId").val();
                    d.itemId = $("#ItemId").val();
                }


            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'Id', visible: false },
                { data: 'ItemCode', title: 'كود الصنف' },
                { data: 'ItemName', title: 'اسم الصنف' },
                { data: 'UnitName', title: 'الوحدة' },
                { data: 'Quantity', title: 'الكمية' },
                { data: 'CostItem', title: 'متوسط تكلفة' },
                { data: 'Amount', title: 'القيمة' },

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

            ],
            drawCallback: function () {
                var html = ' <tr><th colspan ="6" style= "text-align:center" ><div class="row alert-warning"><label>اجمالى الكمية : ';
                var api = this.api();
                $(api.table().footer()).html(html + api.column(5).data().sum() + '</label></div></th>  <th colspan ="6" style= "text-align:center" ><div class="row alert-warning"><label>اجمالى القيمة :' + api.column(7).data().sum() + ' </label ></div ></th >  </tr>');
            },

            "order": [[0, "asc"]]
            //"order": [[0, "desc"]]

        });
        $('#export_print').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableSell').DataTable().button('.buttons-print').trigger();
        });

        $('#export_copy').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableSell').DataTable().button('.buttons-copy').trigger();
        });

        $('#export_excel').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableSell').DataTable().button('.buttons-excel').trigger();
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
        initSell: function () {
            initSell();
        },
        onItemTypeChange: onItemTypeChange,

    };

}();


