"use strict";

var RptCustomerPayment_Module = function () {
    //#region ============== التحصيلات النقدية
    var initPayment = function () {
        var table = $('#kt_datatableCustPayment');

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
                        return 'التحصيلات النقدية';
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
                    filename: "التحصيلات النقدية",
                    title: "التحصيلات النقدية",
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
                url: '/RptCustomerPayments/GetPayments',
                type: 'GET',
                data(d) {
                    d.branchId = $("#BranchId").val();
                    d.customerId = $("#CustomerId").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.employeeId = $("#EmployeeId").val();
                    d.isFinalApproval = $("#IsFinalApproval").is(":checked");
                    d.areaId = $("#AreaId").val();
                }


            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'Id', visible: false },
                { data: 'PaymentDate', title: 'التاريخ' },
                { data: 'CustomerName', title: 'العميل' },
                { data: 'SafeName', title: 'الخزينة' },
                { data: 'Amount', title: 'المبلغ' },
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

            ],
            drawCallback: function () {
                var html = ' <tr><th colspan ="12" style= "text-align:center" ><div class="row alert-warning"><label>اجمالى المبلغ : ';
                var api = this.api();
                $(api.table().footer()).html(html + api.column(5).data().sum() + '</label></div></th>  </tr>');
            },
            "order": [[0, "asc"]]
            //"order": [[0, "desc"]]

        });
        $('#export_print').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableCustPayment').DataTable().button('.buttons-print').trigger();
        });

        $('#export_copy').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableCustPayment').DataTable().button('.buttons-copy').trigger();
        });

        $('#export_excel').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableCustPayment').DataTable().button('.buttons-excel').trigger();
        });

    };

    //#endregion
    //#region ============== الشيكات
    var initCheque = function () {
        var table = $('#kt_datatableCustCheque');

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
                        return 'الشيكات من العملاء';
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
                    filename: "الشيكات من العملاء",
                    title: "الشيكات من العملاء",
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
                url: '/RptCustomerPayments/GetCheques',
                type: 'GET',
                data(d) {
                    d.bankAccountId = $("#BankAccountId").val();
                    d.customerId = $("#CustomerId").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.employeeId = $("#EmployeeId").val();
                    d.isCollected = $("#IsCollected").is(":checked");
                    d.isRefused = $("#IsRefused").is(":checked");
                    d.areaId = $("#AreaId").val();
                }


            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'Id', visible: false },
                { data: 'CheckDate', title: 'التاريخ' },
                { data: 'CustomerName', title: 'العميل' },
                { data: 'CheckNumber', title: 'رقم الشيك' },
                { data: 'Amount', title: 'المبلغ' },
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

            ],
            drawCallback: function () {
                var html = ' <tr><th colspan ="12" style= "text-align:center" ><div class="row alert-warning"><label>اجمالى المبلغ : ';
                var api = this.api();
                $(api.table().footer()).html(html + api.column(5).data().sum() + '</label></div></th>  </tr>');
            },
            "order": [[0, "asc"]]
            //"order": [[0, "desc"]]

        });

        $('#export_print').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableCustCheque').DataTable().button('.buttons-print').trigger();
        });

        $('#export_copy').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableCustCheque').DataTable().button('.buttons-copy').trigger();
        });

        $('#export_excel').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableCustCheque').DataTable().button('.buttons-excel').trigger();
        });
    };

    //#endregion

    //#region الدوال المستخدمة
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
    function onCountryChange() {
        $.get("/SharedDataSources/onCountryChange", { id: $("#CountryId").val() }, function (data) {
            $("#CityId").empty();
            $("#AreaId").empty();
            $("#CityId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#CityId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        })
    };
    function onCityChange() {
        $.get("/SharedDataSources/onCityChange", { id: $("#CityId").val() }, function (data) {
            $("#AreaId").empty();
            $("#AreaId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#AreaId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        })
    };

    //#endregion
    //العملاء بدلالة فئة العملاء

    return {
        //main function to initiate the module
        initPayment: function () {
            initPayment();
        },
        initCheque: function () {
            initCheque();
        },
        getCustomerOnCategoryChange: getCustomerOnCategoryChange,
        getSaleMenDepartmentChange: getSaleMenDepartmentChange,
        onCountryChange: onCountryChange,
        onCityChange: onCityChange,
    };

}();


