"use strict";

var RptDueInvoice_Module = function () {
    //#region ============== فواتير المتأخر تحصيلها
    var initDueSell = function () {
        var table = $('#kt_datatableDueSell');

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
                        return 'فواتير المبيعات المتأخر تحصيلها';
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
                    filename: "فواتير المبيعات المتأخر تحصيلها",
                    title: "فواتير المبيعات المتأخر تحصيلها",
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
                url: '/RptDueInvoices/GetDueSellInvo',
                type: 'GET',
                data(d) {
                    d.branchId = $("#BranchId").val();
                    d.customerId = $("#CustomerId").val();
                    d.employeeId = $("#EmployeeId").val();
                    d.allCustomers = $("#AllCustomers").is(":checked");
                }


            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'Id', visible: false },
                { data: 'InvoiceDate', title: 'التاريخ' },
                { data: 'InvoiceNum', title: 'رقم الفاتورة/العملية' },
                { data: 'CustomerName', title: 'العميل' },
                { data: 'Safy', title: 'صافى الفاتورة' },
                { data: 'AmountCollected', title: 'ما تم تحصيله' },
                { data: 'EmployeeName', title: 'المندوب' },

            ],
            columnDefs: [
                {
                    targets: 0,
                    title: 'م',
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    },
                },
                

            ],
            drawCallback: function () {
                var html = ' <tr><th colspan ="3" style= "text-align:center" ><div class="row alert-warning"><label>اجمالى صافى الفواتير : ';
                var api = this.api();
                $(api.table().footer()).html(html + api.column(5).data().sum() + '</label></div></th>  <th colspan ="3" style= "text-align:center" ><div class="row alert-warning"><label>المتبقى :' + (api.column(5).data().sum()-api.column(6).data().sum())+ ' </label ></div ></th > <th colspan ="6" style= "text-align:center" ><div class="row alert-warning"><label>اجمالى ما تم تحصيلة :' + api.column(6).data().sum()+ ' </label ></div ></th >  </tr>');
            },
            "order": [[0, "asc"]]
            //"order": [[0, "desc"]]

        });

        $('#export_print').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableDueSell').DataTable().button('.buttons-print').trigger();
        });

        $('#export_copy').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableDueSell').DataTable().button('.buttons-copy').trigger();
        });

        $('#export_excel').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableDueSell').DataTable().button('.buttons-excel').trigger();
        });
    };
//#endregion

    //#region ============== اعمار الديون
    var initAgesDebt = function () {
        var table = $('#kt_datatableAgesDebt');

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
                        return 'اعمار الديون';
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
                    filename: "اعمار الديون",
                    title: "اعمار الديون",
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
                url: '/RptDueInvoices/GetAgesDebts',
                type: 'GET',
                data(d) {
                    d.branchId = $("#BranchId").val();
                    d.customerId = $("#CustomerId").val();
                    d.allCustomers = $("#AllCustomers").is(":checked");
                }


            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'CustomerName', title: 'العميل' },
                { data: 'InvoiceNumber', title: 'رقم الفاتورة' },
                { data: 'InvoiceDate', title: 'تاريخ الفاتورة' },
                { data: 'AgeDebt', title: 'اعمار الديون بالايام' },
                { data: 'AgeDebtNotDue', title: 'لم تستحق بعد' },
                { data: 'AgeDebt30', title: 'خلال 30 يوم' },
                { data: 'AgeDebt60', title: 'خلال 60 يوم' },
                { data: 'AgeDebt90', title: 'خلال 90 يوم' },
                { data: 'AgeDebt120', title: 'خلال 120 يوم' },
                { data: 'AgeDebt150', title: 'خلال 150 يوم' },
                { data: 'AgeDebt180', title: 'خلال 180 يوم' },
                { data: 'AgeDebtOver180', title: 'اكتر من 180 يوم' },

            ],
            columnDefs: [
                {
                    targets: 0,
                    title: 'م',
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    },
                },
                

            ],
            drawCallback: function () {
                var html = ' <tr><th colspan ="5" ></th><th style= "text-align:center" ><div class="row alert-warning"><label>اجمالى لم تستحق بعد : ';
                var api = this.api();
                $(api.table().footer()).html(html + api.column(5).data().sum() + `</label></div></th> 
                    <th style="text-align:center"><div class="row alert-warning"><label>اجمالى خلال 30 يوم :` + api.column(6).data().sum() + ` </label ></div ></th >  
                    <th style="text-align:center"><div class="row alert-warning"><label>اجمالى خلال 60 يوم :` + api.column(7).data().sum() + ` </label ></div ></th >  
                    <th style="text-align:center"><div class="row alert-warning"><label>اجمالى خلال 90 يوم :` + api.column(8).data().sum() + ` </label ></div ></th >  
                    <th style="text-align:center"><div class="row alert-warning"><label>اجمالى خلال 120 يوم :` + api.column(9).data().sum() + ` </label ></div ></th >  
                    <th style="text-align:center"><div class="row alert-warning"><label>اجمالى خلال 150 يوم :` + api.column(10).data().sum() + ` </label ></div ></th >  
                    <th style="text-align:center"><div class="row alert-warning"><label>اجمالى خلال 180 يوم :` + api.column(11).data().sum() + ` </label ></div ></th >  
                    <th style="text-align:center"><div class="row alert-warning"><label>اجمالى اكتر من خلال 180 يوم :` + api.column(12).data().sum() + ` </label ></div ></th >  
</tr > `);
            },
            "order": [[0, "asc"]]
            //"order": [[0, "desc"]]

        });

        $('#export_print').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableAgesDebt').DataTable().button('.buttons-print').trigger();
        });

        $('#export_copy').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableAgesDebt').DataTable().button('.buttons-copy').trigger();
        });

        $('#export_excel').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableAgesDebt').DataTable().button('.buttons-excel').trigger();
        });
    };
//#endregion

    //العملاء بدلالة فئة العملاء
    function getCustomerOnCategoryChange() {
        var selMen = $("#EmployeeId").val();
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
        initDueSell: function () {
            initDueSell();
        },
        initAgesDebt: function () {
            initAgesDebt();
        },
        getCustomerOnCategoryChange: getCustomerOnCategoryChange,
        getSaleMenDepartmentChange: getSaleMenDepartmentChange,
    };

}();


