"use strict";

var RptMaintenance_Module = function () {
    //#region ============== فواتير الصيانة
    var initMaintenance = function () {
        var table = $('#kt_datatable');

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
                        return 'فواتير الصيانة';
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
                    filename: "فواتير الصيانة",
                    title: "فواتير الصيانة",
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
                url: '/RptMaintenances/GetMaintenanceInvo',
                type: 'GET',
                data(d) {
                    d.branchId = $("#BranchId").val();
                    d.customerId = $("#CustomerId").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.receiptDate = $("#ReceiptDate").val();
                    d.employeeResponseId = $("#EmployeeResponseId").val();
                    d.employeeSaleMenId = $("#EmployeeSaleMenId").val();
                    d.isFinalApproval = $("#IsFinalApproval").is(":checked");
                    d.hasCost = $("#HasCost").is(":checked");
                    d.hasGuarantee = $("#HasGuarantee").is(":checked");
                    d.itemId = $("#ItemId").val();
                }


            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'Id', visible: false },
                { data: 'InvoiceGuid', visible: false },
                { data: 'InvoiceDate', title: 'التاريخ' },
                { data: 'InvoiceNum', title: 'رقم الفاتورة' },
                { data: 'CustomerName', title: 'العميل' },
                { data: 'EmployeeResponseName', title: 'الفنى المسئول' },
                { data: 'TotalQuantity', title: 'اجمالى العدد' },
                { data: 'Safy', title: 'الصافى' },
                { data: 'PaymentTypeName', title: 'طريقة السداد' },
                { data: 'CaseName', title: 'الحالة' },
                { data: 'Actions', responsivePriority: -1 },

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
					        <a href="/Maintenances/ShowHistory/?invoGuid='+ row.InvoiceGuid + '" class="btn btn-sm btn-clean btn-icon" title="عرض الحالات">\
								<i class="fa fa-random"></i>\
							</a>\<a href="/Maintenances/ShowMaintenance/?invoGuid='+ row.InvoiceGuid + '" class="btn btn-sm btn-clean btn-icon" title="استعراض بيانات الفاتورة">\
								<i class="fa fa-print"></i>\
							</a>\</div>\
						';

                    },
                }

            ],
            drawCallback: function () {
                var html = ' <tr><th colspan ="6" style= "text-align:center" ><div class="row alert-warning"><label>اجمالى العدد : ';
                var api = this.api();
                $(api.table().footer()).html(html + api.column(7).data().sum() + '</label></div></th>  <th colspan ="6" style= "text-align:center" ><div class="row alert-warning"><label>اجمالى صافى الفواتير :' + api.column(8).data().sum()+ ' </label ></div ></th >  </tr>');
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

//#endregion
    //#region ============== الاعطال
    var initMaintenanceProblem = function () {
        var table = $('#kt_datatableProblem').DataTable({
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
                        return 'اعطال الصيانة';
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
                    filename: "اعطال الصيانة",
                    title: "اعطال الصيانة",
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
                url: '/RptMaintenances/GetMaintenanceProblem',
                type: 'GET',
                data(d) {
                    d.branchId = $("#BranchId").val();
                    d.customerId = $("#CustomerId").val();
                    d.maintenProblemTypeId = $("#MaintenProblemTypeId").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.employeeResponseId = $("#EmployeeResponseId").val();
                    d.employeeSaleMenId = $("#EmployeeSaleMenId").val();
                    d.isFinalApproval = $("#IsFinalApproval").is(":checked");
                    d.hasCost = $("#HasCost").is(":checked");
                    d.hasGuarantee = $("#HasGuarantee").is(":checked");
                    d.itemId = $("#ItemId").val();
                }
            },
            columns: [
                {

                    class: "details-control",
                    orderable: false,
                    data: null,
                    defaultContent: ""
                },
                { data: 'Id', visible: false },
                { data: 'ItemName', title: 'الصنف' },
                { data: 'TotalItemSafy', title: 'تكلفة صيانه الصنف' },

            ],
            drawCallback: function () {
                var api = this.api();
                $("#totalItemSafy").text(api.column(3).data().sum());
            },

            "order": [[0, "asc"]]
            //"order": [[0, "desc"]]

        });
        $('#export_print').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableProblem').DataTable().button('.buttons-print').trigger();
        });

        $('#export_copy').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableProblem').DataTable().button('.buttons-copy').trigger();
        });

        $('#export_excel').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableProblem').DataTable().button('.buttons-excel').trigger();
        });

        // Array to track the ids of the details displayed rows
        var detailRows = [];

        $('#kt_datatableProblem tbody').on('click', 'tr td.details-control', function () {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var idx = $.inArray(tr.attr('id'), detailRows);
            if (row.child.isShown()) {
                tr.removeClass('details');
                row.child.hide();

                // Remove from the 'open' array
                detailRows.splice(idx, 1);
            }
            else {
                tr.addClass('details');
                row.child(format(row.data())).show();

                // Add to the 'open' array
                if (idx === -1) {
                    detailRows.push(tr.attr('id'));
                }
            }
        });

        // On each draw, loop over the `detailRows` array and show any child rows
        table.on('draw', function () {
            $.each(detailRows, function (i, id) {
                $('#' + id + ' td.details-control').trigger('click');
            });
        });

    };

    function format(d) {
        var tr = '';
        $.each(d.InvoicesData, function (index, data) {
            tr += ' <tr>' +
                ' <th scope="row">' + (index + 1) + '</th>' +
                ' <td>' + data.InvoiceNum + '</td>' +
                ' <td>' + data.CustomerName + '</td>' +
                '<td>' + data.InvoiceDate + '</td>' +
                '<td>' + data.ItemSafy + '</td>' +
                '<td>' + data.MaintenProblemTypeName + '</td>' +
                //'<td><a href="/UserProfileResults/ShowResult/?usQ=' + data.InvoiceGuid + '" class="btn btn-sm btn-clean btn-icon" title="عرض النتيجة">' +
                //'<i class="fa fa-eye"></i></a>\</td>' +
                '</tr>';


        });
        return '<table class="table table-primary mb-6"><thead>' +
            '  <tr>' +
            ' <th scope="col">م</th>' +
            '  <th scope="col">فاتورة الصيانة</th>' +
            '  <th scope="col">اسم العميل</th>' +
            '  <th scope="col">تاريخ الفاتورة</th>' +
            '  <th scope="col">الصافى</th>' +
            '  <th scope="col">نوع العطل</th>' +
            ' </tr>' +
            '</thead > ' +
            '<tbody>' +
            tr
        ' </tbody>' +
            '</table >';

    };
//#endregion
    //#region ============== احصاء بالاعطال الاكثر شيوعا
    var initMaintenanceProblemStatistic = function () {
        var table = $('#kt_datatableProblemStatistic').DataTable({
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
                        return 'احصاء الاعطال الصيانة الاكثر شيوعا';
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
                    filename: "احصاء الاعطال الصيانة الاكثر شيوعا",
                    title: "احصاء الاعطال الصيانة الاكثر شيوعا",
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
                url: '/RptMaintenances/GetMaintenanceProblemStatistics',
                type: 'GET',
                data(d) {
                    d.branchId = $("#BranchId").val();
                    d.maintenProblemTypeId = $("#MaintenProblemTypeId").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.isFinalApproval = $("#IsFinalApproval").is(":checked");
                    d.hasCost = $("#HasCost").is(":checked");
                    d.hasGuarantee = $("#HasGuarantee").is(":checked");
                    d.itemId = $("#ItemId").val();
                }


            },
            columns: [
                //{ data: 'Id', visible: false },
                { data: 'ItemName', title: 'الصنف' },
                { data: 'MaintenProblemTypeName', title: 'نوع العطل الاكثر شيوعا' },
                { data: 'MaintenProblemTypeTheMostCount', title: 'عدد الاعطال' },
            ],

            "order": [[0, "asc"]]
            //"order": [[0, "desc"]]

        });
        $('#export_print').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableProblemStatistic').DataTable().button('.buttons-print').trigger();
        });

        $('#export_copy').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableProblemStatistic').DataTable().button('.buttons-copy').trigger();
        });

        $('#export_excel').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableProblemStatistic').DataTable().button('.buttons-excel').trigger();
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
    function getSaleMenDepartmentChange() { // كل المناديب فقط بدلالة الادارة 
        $.get("/SharedDataSources/GetSaleMenByDepartment", { id: $("#DepartmentSaleMenId").val() }, function (data) {
            $("#EmployeeSaleMenId").empty();
            $("#EmployeeSaleMenId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#EmployeeSaleMenId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        });
    };
    function getEmployeeDepartmentChange() { //كل الموظفين بدلالة الادارة
        $.get("/SharedDataSources/GetEmployeeByDepartment", { id: $("#DepartmentResponseId").val() }, function (data) {
            $("#EmployeeResponseId").empty();
            $("#EmployeeResponseId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#EmployeeResponseId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        });
    };
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
        initMaintenance: function () {
            initMaintenance();
        },
        initMaintenanceProblem: function () {
            initMaintenanceProblem();
        },
        initMaintenanceProblemStatistic: function () {
            initMaintenanceProblemStatistic();
        },
        getCustomerOnCategoryChange: getCustomerOnCategoryChange,
        getSaleMenDepartmentChange: getSaleMenDepartmentChange,
        getEmployeeDepartmentChange: getEmployeeDepartmentChange,
        onItemTypeChange: onItemTypeChange,
    };

}();


