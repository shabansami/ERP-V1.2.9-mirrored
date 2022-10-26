"use strict";

var ProductionOrderReceipt_Module = function () {

    //#region ======== Manage Receipts ==================
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
                        return 'أوامر التسليم';
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
                    filename: "أوامر التسليم",
                    title: "أوامر التسليم",
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
                url: '/ProductionOrderReceipts/GetAll',
                type: 'GET',

            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'OrderNumber', title: 'رقم امر الإنتاج' },
                { data: 'ProductionOrderDate', title: 'تاريخ امر الإنتاج' },
                { data: 'FinalItemName', title: 'الصنف النهائى' },
                { data: 'OrderQuantity', title: 'الكمية' },
                { data: 'IsDoneTitle', title: 'حالة التسليم للمخازن المنتج التام' },
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
                        if (row.IsDone) {
                            return '\
							<div class="btn-group">\
							<a href="/ProductionOrders/ShowProductionOrder/?ordrGud=' +
                                row.Id +
                                '" class="btn btn-sm btn-clean btn-icon" title="عرض ملخص أمر اللإنتاج">\
								<i class="la la-search"></i>\
							</a>\</div>\
						';
                        } else {
                            return '\
							<div class="btn-group">\
							\<a href="/ProductionOrders/ShowProductionOrder/?ordrGud=' +
                                row.Id + '" class="btn btn-sm btn-clean btn-icon" title=" عرض ملخص أمر اللإنتاج">\
								<i class="la la-search"></i>\
							</a>\
							<a href="/ProductionOrderReceipts/RegisterReceipts/?ordrGud='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تسليم">\
								<i class="la la-house-damage"></i>\
							</a></div>\
						';
                        }
                    },
                }

            ],

            "order": [[0, "desc"]]
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

    

    function RegisterReceipts() { // تسجيل الاستلام
        var form = document.getElementById('form1');
        //var data = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
        Swal.fire({
            title: 'تأكيد إستلام منتجات أمر الإنتاج',
            text: 'هل متأكد من الإستلام ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
               
                var url = '/ProductionOrderReceipts/RegisterReceipts';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: new FormData(form),
                    contentType: false,
                    processData: false,
                    success: function (data) {
                        if (data.isValid) {
                            toastr.success(data.message, '');
                            setTimeout(function () { window.location = "/ProductionOrderReceipts/Index" }, 3000);
                        } else
                            toastr.error(data.message, '');
                        
                    },
                    error: function (err) {
                        alert(err);
                    }
                });
            }
        });
    };

    function getStoresOnBranchChanged() {  // get safes and stores by branchId
        $.get("/SharedDataSources/getStoresOnBranchChanged", { id: $("#BranchId").val() }, function (data) {
            $("#FinalItemStoreId").empty();
            $("#FinalItemStoreId").append("<option value=>اختر عنصر من القائمة </option>");
            $.each(data, function (index, row) {
                $("#FinalItemStoreId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });
        });
    };
    //#endregion ========= end Step 2 ==========


    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        RegisterReceipts: RegisterReceipts,
        getStoresOnBranchChanged: getStoresOnBranchChanged
    };

}();

