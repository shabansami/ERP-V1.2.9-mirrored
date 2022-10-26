"use strict";

var RptQuantityItemProduction_Module = function () {
    //#region ============== التوريدات
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
                        return 'كمية انتاج صنف حسب الرصيد المتوفر';
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
                    filename: "كمية انتاج صنف حسب الرصيد المتوفر",
                    title: "كمية انتاج صنف حسب الرصيد المتوفر",
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
                url: '/RptQuantityItemProductions/GetDSItemMaterials',
                type: 'GET',
                data(d) {
                    d.itemFinalId = $("#ItemFinalId").val();
                    d.storeId = $("#StoreId").val();
                    d.allStores = $("#AllStores").val();
                    d.ItemCostCalculateId = $("#ItemCostCalculateId").val();
                    d.quantity = $("#itemQuantity").val();
                }


            },
            columns: [
                { data: 'IsAllQuantityDone', visible: false },
                { data: 'ItemId', visible: false },
                { data: 'StoreUnderId', visible: false },
                { data: 'ItemCostCalculateId', visible: false },
                { data: 'ItemName', title: 'المنتج الخام' },
                { data: 'ItemCost', title: 'تكلفة المنتج' },
                { data: 'QuantityRequired', title: 'الكمية المطلوبة' },
                { data: 'QuantityAvailable', title: 'الكمية المتاحة' },
                { data: 'Actions', responsivePriority: -1 },
                { data: 'ActionStatus', responsivePriority: -2 },

            ],
            columnDefs: [
                {
                    targets: 1,
                    title: 'م',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    },
                },
                {
                    targets: -1,
                    title: 'الحالة',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        var status = {
                            1: { 'title': 'Pending', 'class': 'label-light-primary' },
                            2: { 'title': 'Delivered', 'class': ' label-light-danger' },
                            3: { 'title': 'Canceled', 'class': ' label-light-primary' },
                            4: { 'title': 'الرصيد يكفى', 'class': ' label-light-success' },
                            5: { 'title': 'Info', 'class': ' label-light-info' },
                            6: { 'title': 'الرصيد لا يكفى', 'class': ' label-light-danger' },
                            7: { 'title': 'Warning', 'class': ' label-light-warning' },
                        };
                        if (row.IsAllQuantityDone) {
                            return '<span class="label label-lg font-weight-bold' + status[4].class + ' label-inline">' + status[4].title + '</span>';
                        } else {
                            return '<span class="label label-lg font-weight-bold' + status[6].class + ' label-inline">' + status[6].title + '</span>';
                        }
                    },

                    //              render: function (data, type, row, meta) {
                    //                  return '\
                    //	<div class="btn-group">\
                    //	<a href="#" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
                    //		<i class="fa fa-close"></i>\
                    //	</a>\
                    //	</div>\
                    //';
                    //              },
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
    var initDT2 = function () {
        var table = $('#kt_datatable2');

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
                        return 'انتاج صنف حسب كمية محددة';
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
                    filename: "انتاج صنف حسب كمية محددة",
                    title: "انتاج صنف حسب كمية محددة",
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
                url: '/RptQuantityItemProductions/GetDSItemMaterials',
                type: 'GET',
                data(d) {
                    d.itemFinalId = $("#ItemFinalId").val();
                    d.storeId = $("#StoreId").val();
                    d.allStores = $("#AllStores").val();
                    d.ItemCostCalculateId = $("#ItemCostCalculateId").val();
                    d.quantity = $("#itemQuantity").val();
                }


            },
            columns: [
                { data: 'IsAllQuantityDone', visible: false },
                { data: 'ItemId', visible: false },
                { data: 'StoreUnderId', visible: false },
                { data: 'ItemCostCalculateId', visible: false },
                { data: 'ItemName', title: 'المنتج الخام' },
                { data: 'ItemCost', title: 'تكلفة المنتج' },
                { data: 'QuantityRequired', title: 'الكمية المطلوبة' },
                { data: 'QuantityAvailable', title: 'الكمية المتاحة' },
                { data: 'Actions', responsivePriority: -1 },
                { data: 'ActionStatus', responsivePriority: -2 },

            ],
            columnDefs: [
                {
                    targets: 1,
                    title: 'م',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    },
                },
                {
                    targets: -1,
                    title: 'الحالة',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        var status = {
                            1: { 'title': 'Pending', 'class': 'label-light-primary' },
                            2: { 'title': 'Delivered', 'class': ' label-light-danger' },
                            3: { 'title': 'Canceled', 'class': ' label-light-primary' },
                            4: { 'title': 'الرصيد يكفى', 'class': ' label-light-success' },
                            5: { 'title': 'Info', 'class': ' label-light-info' },
                            6: { 'title': 'الرصيد لا يكفى', 'class': ' label-light-danger' },
                            7: { 'title': 'Warning', 'class': ' label-light-warning' },
                        };
                        if (row.IsAllQuantityDone) {
                            return '<span class="label label-lg font-weight-bold' + status[4].class + ' label-inline">' + status[4].title + '</span>';
                        } else {
                            return '<span class="label label-lg font-weight-bold' + status[6].class + ' label-inline">' + status[6].title + '</span>';
                        }
                    },

                    //              render: function (data, type, row, meta) {
                    //                  return '\
                    //	<div class="btn-group">\
                    //	<a href="#" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
                    //		<i class="fa fa-close"></i>\
                    //	</a>\
                    //	</div>\
                    //';
                    //              },
                }

            ],

            "order": [[0, "asc"]]
            //"order": [[0, "desc"]]

        });
        $('#export_print2').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatable2').DataTable().button('.buttons-print').trigger();
        });

        $('#export_copy2').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatable2').DataTable().button('.buttons-copy').trigger();
        });

        $('#export_excel2').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatable2').DataTable().button('.buttons-excel').trigger();
        });

    };
    function searchData(typ) {
        var url = '/RptQuantityItemProductions/GetQuantityItem';
        $.ajax({
            type: "POST",
            url: url,
            data: {
                "itemFinalId": $("#ItemFinalId").val(),
                "storeId": $("#StoreId").val(),
                "allStores": $("#AllStores").val()
            },
            //async: true,
            //headers: { 'RequestVerificationToken': $('@Html.AntiForgeryToken()').val() },
            success: function (data) {
                if (data.isValid) {
                    $('#itemQuantityExpected').text(data.quentityExpected);
                    if (typ === '1') { //البحث لمعرفة الكمية المتوقع انتاجها
                        $('#itemQuantity').val(data.quentityExpected);
                        $('#kt_datatable').DataTable().ajax.reload();

                    } else {     //البحث بدلالة كمية تم ادخالها من خلال المستخدم
                        if ($("#ItemQuantitySpc").val() == '' || $("#ItemQuantitySpc").val() === 0) {
                            toastr.error('تأكد من ادخال الكمية المطلوب انتاجها', '');
                            return false;
                        }
                        $("#itemQuantity").val($("#ItemQuantitySpc").val())
                        $('#kt_datatable2').DataTable().ajax.reload();

                    }

                } else {
                    toastr.error(data.message, '');
                }
            },
            error: function (err) {
                alert(err);
            }
        });
    };
//#endregion
    function onItemTypeChange(typ) {
        $.get("/SharedDataSources/OnItemTypeChange", { id: $("#ItemtypeId").val(), hasItemProduction:true }, function (data) {
            $("#ItemFinalId").empty();
            $("#ItemFinalId").append("<option value=>اختر عنصر من القائمة</option>");
                $.each(data, function (index, row) {
                    $("#ItemFinalId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
                });
            });
    };
    function getStoresOnBranchFromChanged() {  // get safes and stores by branchId
        $.get("/SharedDataSources/getStoresOnBranchChanged", { id: $("#BranchId").val() }, function (data) {
            $("#StoreId").empty();
            $("#StoreId").append("<option value=>اختر عنصر من القائمة </option>");
            $.each(data, function (index, row) {
                $("#StoreId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            })
        });
    };

    function onCheckBoxChange() {
        if ($("#AllStores").is(":checked")) {
            $("#AllStores").val(true);
        } else {
            $("#AllStores").val(false);
        }
    }
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        init2: function () {
            initDT2();
        },
        searchData: searchData,
        onItemTypeChange: onItemTypeChange,
        getStoresOnBranchFromChanged: getStoresOnBranchFromChanged,
        onCheckBoxChange: onCheckBoxChange,
    };

}();


