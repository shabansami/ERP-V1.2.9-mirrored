"use strict";

var RptItemInvoice_Module = function () {
    //#region ============== المبيعات
    var initSell = function () {
        var table = $('#kt_datatableSell').DataTable({
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
                        return 'المبيعات';
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
                    filename: "المبيعات",
                    title: "المبيعات",
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
                url: '/RptItemInvoices/GetSellInvo',
                type: 'GET',
                data(d) {
                    d.branchId = $("#BranchId").val();
                    //d.customerId = $("#CustomerId").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.employeeId = $("#EmployeeId").val();
                    d.isFinalApproval = $("#IsFinalApproval").is(":checked");
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
                { data: 'TotalQuantity', title: 'اجمالى العدد' },
                { data: 'TotalAmount', title: 'الاجمالى' },

            ],
            drawCallback: function () {
                var api = this.api();
                console.log(api.column(4).data().sum());
                $("#totalSell").text(api.column(4).data().sum());
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

        // Array to track the ids of the details displayed rows
        var detailRows = [];

        $('#kt_datatableSell tbody').on('click', 'tr td.details-control', function () {
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

//#endregion
    //#region ============== مرتجع المبيعات
    var initSellBack = function () {
        var table = $('#kt_datatableSellBack').DataTable({
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
                        return 'مرتجع المبيعات';
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
                    filename: "مرتجع المبيعات",
                    title: "مرتجع المبيعات",
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
                url: '/RptItemInvoices/GetSellBackInvo',
                type: 'GET',
                data(d) {
                    d.branchId = $("#BranchId").val();
                    //d.customerId = $("#CustomerId").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.employeeId = $("#EmployeeId").val();
                    d.isFinalApproval = $("#IsFinalApproval").is(":checked");
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
                { data: 'TotalQuantity', title: 'اجمالى العدد' },
                { data: 'TotalAmount', title: 'الاجمالى' },

            ],
            drawCallback: function () {
                var api = this.api();
                $("#totalSellBack").text(api.column(4).data().sum());
            },
          
            "order": [[0, "asc"]]
            //"order": [[0, "desc"]]

        });
        $('#export_printBack').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableSellBack').DataTable().button('.buttons-print').trigger();
        });

        $('#export_copyBack').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableSellBack').DataTable().button('.buttons-copy').trigger();
        });

        $('#export_excelBack').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableSellBack').DataTable().button('.buttons-excel').trigger();
        });

        // Array to track the ids of the details displayed rows
        var detailBackRows = [];

        $('#kt_datatableSellBack tbody').on('click', 'tr td.details-control', function () {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var idx = $.inArray(tr.attr('id'), detailBackRows);
            if (row.child.isShown()) {
                tr.removeClass('details');
                row.child.hide();

                // Remove from the 'open' array
                detailBackRows.splice(idx, 1);
            }
            else {
                tr.addClass('details');
                row.child(format(row.data())).show();

                // Add to the 'open' array
                if (idx === -1) {
                    detailBackRows.push(tr.attr('id'));
                }
            }
        });

        // On each draw, loop over the `detailRows` array and show any child rows
        table.on('draw', function () {
            $.each(detailBackRows, function (i, id) {
                $('#' + id + ' td.details-control').trigger('click');
            });
        });

    };

//#endregion
    //#region ============== رصيد اول
    var initIntial = function () {
        var table = $('#kt_datatableIntial').DataTable({
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
                        return 'رصيد اول';
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
                    filename: "رصيد اول",
                    title: "رصيد اول",
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
                url: '/RptItemInvoices/GetIntial',
                type: 'GET',
                data(d) {
                    d.branchId = $("#BranchId").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
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
                { data: 'TotalQuantity', title: 'اجمالى العدد' },
                { data: 'TotalAmount', title: 'الاجمالى' },

            ],
            drawCallback: function () {
                var api = this.api();
                $("#totalIntial").text(api.column(4).data().sum());
            },
          
            "order": [[0, "asc"]]
            //"order": [[0, "desc"]]

        });
        $('#export_printIntial').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableIntial').DataTable().button('.buttons-print').trigger();
        });

        $('#export_copyIntial').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableIntial').DataTable().button('.buttons-copy').trigger();
        });

        $('#export_excelIntial').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableIntial').DataTable().button('.buttons-excel').trigger();
        });

        // Array to track the ids of the details displayed rows
        var detailBackRows = [];

        $('#kt_datatableIntial tbody').on('click', 'tr td.details-control', function () {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var idx = $.inArray(tr.attr('id'), detailBackRows);
            if (row.child.isShown()) {
                tr.removeClass('details');
                row.child.hide();

                // Remove from the 'open' array
                detailBackRows.splice(idx, 1);
            }
            else {
                tr.addClass('details');
                row.child(format(row.data())).show();

                // Add to the 'open' array
                if (idx === -1) {
                    detailBackRows.push(tr.attr('id'));
                }
            }
        });

        // On each draw, loop over the `detailRows` array and show any child rows
        table.on('draw', function () {
            $.each(detailBackRows, function (i, id) {
                $('#' + id + ' td.details-control').trigger('click');
            });
        });

    };

//#endregion
    //#region ============== المشتريات
    var initPurchase = function () {
        var table = $('#kt_datatablePurchase').DataTable({
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
                        return 'التوريدات';
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
                    filename: "التوريدات",
                    title: "التوريدات",
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
                url: '/RptItemInvoices/GetPurchaseInvo',
                type: 'GET',
                data(d) {
                    d.branchId = $("#BranchId").val();
                    //d.customerId = $("#CustomerId").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.employeeId = $("#EmployeeId").val();
                    d.isFinalApproval = $("#IsFinalApproval").is(":checked");
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
                { data: 'TotalQuantity', title: 'اجمالى العدد' },
                { data: 'TotalAmount', title: 'الاجمالى' },

            ],
            drawCallback: function () {
                var api = this.api();
                console.log(api.column(4).data().sum());
                $("#totalPurchase").text(api.column(4).data().sum());
            },

            "order": [[0, "asc"]]
            //"order": [[0, "desc"]]

        });
        $('#export_printPurchase').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatablePurchase').DataTable().button('.buttons-print').trigger();
        });

        $('#export_copyPurchase').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatablePurchase').DataTable().button('.buttons-copy').trigger();
        });

        $('#export_excelPurchase').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatablePurchase').DataTable().button('.buttons-excel').trigger();
        });

        // Array to track the ids of the details displayed rows
        var detailRows = [];

        $('#kt_datatablePurchase tbody').on('click', 'tr td.details-control', function () {
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

    //#endregion
    //#region ============== مرتجع التوريد
    var initPurchaseBack = function () {
        var table = $('#kt_datatablePurchaseBack').DataTable({
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
                        return 'مرتجع التوريد';
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
                    filename: "مرتجع التوريد",
                    title: "مرتجع التوريد",
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
                url: '/RptItemInvoices/GetPurchaseBackInvo',
                type: 'GET',
                data(d) {
                    d.branchId = $("#BranchId").val();
                    //d.customerId = $("#CustomerId").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.employeeId = $("#EmployeeId").val();
                    d.isFinalApproval = $("#IsFinalApproval").is(":checked");
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
                { data: 'TotalQuantity', title: 'اجمالى العدد' },
                { data: 'TotalAmount', title: 'الاجمالى' },

            ],
            drawCallback: function () {
                var api = this.api();
                $("#totalPurchaseBack").text(api.column(4).data().sum());
            },

            "order": [[0, "asc"]]
            //"order": [[0, "desc"]]

        });
        $('#export_printPurchaseBack').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatablePurchaseBack').DataTable().button('.buttons-print').trigger();
        });

        $('#export_copyPurchaseBack').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatablePurchaseBack').DataTable().button('.buttons-copy').trigger();
        });

        $('#export_excelPurchaseBack').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatablePurchaseBack').DataTable().button('.buttons-excel').trigger();
        });

        // Array to track the ids of the details displayed rows
        var detailBackRows = [];

        $('#kt_datatablePurchaseBack tbody').on('click', 'tr td.details-control', function () {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var idx = $.inArray(tr.attr('id'), detailBackRows);
            if (row.child.isShown()) {
                tr.removeClass('details');
                row.child.hide();

                // Remove from the 'open' array
                detailBackRows.splice(idx, 1);
            }
            else {
                tr.addClass('details');
                row.child(format(row.data())).show();

                // Add to the 'open' array
                if (idx === -1) {
                    detailBackRows.push(tr.attr('id'));
                }
            }
        });

        // On each draw, loop over the `detailRows` array and show any child rows
        table.on('draw', function () {
            $.each(detailBackRows, function (i, id) {
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
                '<td>' + data.ItemAmount + '</td>' +
                '<td class=d-none>' + data.InvoiceGuid + '</td>' +
                //'<td><a href="/UserProfileResults/ShowResult/?usQ=' + data.InvoiceGuid + '" class="btn btn-sm btn-clean btn-icon" title="عرض النتيجة">' +
                //'<i class="fa fa-eye"></i></a>\</td>' +
                '</tr>';


        });
        return '<table class="table table-primary mb-6"><thead>' +
            '  <tr>' +
            ' <th scope="col">م</th>' +
            '  <th scope="col">رقم الفاتورة</th>' +
            '  <th scope="col">اسم العميل/المورد</th>' +
            '  <th scope="col">تاريخ الفاتورة</th>' +
            '  <th scope="col">القيمة</th>' +
            '  <th scope="col" class="d-none"></th>' +
            //'  <th scope="col">عرض النتيجة</th>' +
            ' </tr>' +
            '</thead > ' +
            '<tbody>' +
            tr
        ' </tbody>' +
            '</table >';

    };

//#endregion
    //العملاء بدلالة فئة العملاء
    //function getCustomerOnCategoryChange() {
    //    var selMen = $("#EmployeeId").val();
    //    $.get("/SharedDataSources/GetCustomerOnCategoryChange/", { id: $("#PersonCategoryId").val(), saleMenId: selMen }, function (data) {
    //        $("#CustomerId").empty();
    //        $("#CustomerId").append("<option value=>اختر عنصر من القائمة</option>");
    //        $.each(data, function (index, row) {
    //            $("#CustomerId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
    //        });
    //    });
    //};
    //function getSaleMenDepartmentChange() {
    //    $.get("/SharedDataSources/GetSaleMenByDepartment", { id: $("#DepartmentId").val() }, function (data) {
    //        $("#EmployeeId").empty();
    //        $("#EmployeeId").append("<option value=>اختر عنصر من القائمة</option>");
    //        $.each(data, function (index, row) {
    //            $("#EmployeeId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
    //        });

    //    });
    //};
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
        initSell: function () {
            initSell();
        },
        initSellBack: function () {
            initSellBack();
        },
        initPurchase: function () {
            initPurchase();
        },
        initPurchaseBack: function () {
            initPurchaseBack();
        },
        initIntial: function () {
            initIntial();
        },
        //getCustomerOnCategoryChange: getCustomerOnCategoryChange,
        //getSaleMenDepartmentChange: getSaleMenDepartmentChange,
        onItemTypeChange: onItemTypeChange,
    };

}();


