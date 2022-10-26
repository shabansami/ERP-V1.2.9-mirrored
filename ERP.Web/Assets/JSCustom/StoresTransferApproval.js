"use strict";

var StoresTransferApproval_Module = function () {

    //#region ======== Save Purchase invoice ==================
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
                        return 'الاعتماد المخزنى للتحويلات المخزنية';
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
                    filename: 'الاعتماد المخزنى للتحويلات المخزنية',
                    title: 'الاعتماد المخزنى للتحويلات المخزنية',
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
                url: '/StoresTransferApprovals/GetAll',
                type: 'GET',

                    },
        columns: [
            { data: 'Num',responsivePriority:0 },
            { data: 'StoreTranNumber', title: 'رقم عملية التحويل' },
            { data: 'BranchFromName', title: 'من فرع' },
            { data: 'StoreFromName', title: 'من مخزن' },
            { data: 'BranchToName', title: 'الى فرع' },
            { data: 'StoreToName', title: 'الى مخزن' },
            { data: 'TransferDate', title: 'تاريخ العملية' },
            { data: 'cmbo_approval', title: 'حالة الاعتماد' },
/*            { data: 'ApprovalStore', title: 'حالة الاعتماد مخزنيا' },*/
            { data: 'cmbo_approval', visible: false },
            { data: 'Actions', responsivePriority: -1, className: 'actions' },

        ],
            columnDefs: [
                {
                    targets: 0,
                    title: 'م',
                    render: function (data, type, row, meta) {
                        return  meta.row + meta.settings._iDisplayStart + 1;
                    },
                },
            {
                targets: -1,
                title: 'عمليات',
                orderable: false,
                render: function (data, type, row, meta) {
                    if (row.IsApprovalStore || row.IsRefusStore) {
                        return '\
							<div class="btn-group">\
							<a href="/StoresTransferApprovals/ShowDetails/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض تفاصيل العملية">\
								<i class="fa fa-search"></i>\
							</a>\
                            </div>\
						';
                    } else {
                       return '\
							<div class="btn-group">\
							\<a href="/StoresTransferApprovals/ApprovalStore/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icUrln" title="اعتماد مخزنى">\
								<i class="fa fa-check-square-o"></i>\
							</a><a href="javascript:;" onclick=StoresTransferApproval_Module.Refused(\''+ row.Id + '\')  class="btn btn-sm btn-clean btn-icUrln" title="رفض عملية تحويل مخزنى">\
								<i class="fa fa-close"></i>\
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
    var initDTItemDetails = function () {
        var table = $('#kt_dtItemDetails');

        // begin first table
        table.DataTable({
            paging: false,
            info: false,
            search: false,
            searching:false,
            "order": [[0, "asc"]]
            //"order": [[0, "desc"]] 

        });
    };

    function ApprovalInvoice() { // اعتماد فاتورة

        //var data = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
        var values = new Array();
        $.each($(".selectedDay"), function () {
            var data = $(this).parents('tr:eq(0)');
            values.push({ 'Id': $(data).find('td:eq(0)').text(), 'ItemId': $(data).find('td:eq(1)').text(), 'ItemName': $(data).find('td:eq(2)').text(), 'Quantity': $(data).find('td:eq(3)').text(), 'QuantityReal': $(data).find('td:eq(4) input[type="text"]').val() });
        });
        console.log(values);
        console.log(values.length);

        Swal.fire({
            title: 'تأكيد الاعتماد',
            text: 'هل متأكد من اعتماد التحويل المخزنى ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                const params = new URLSearchParams(window.location.search)
                var queryString = '';
                if (params.has('invoGuid')) {
                    queryString = params.get('invoGuid');
                } else
                    document.location.replace("StoresTransferApprovals/Index")

                var url = '/StoresTransferApprovals/ApprovalInvoice';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "invoGuid": queryString,
                        "data": JSON.stringify(values)
                    },
                    //async: true,
                    //headers: { 'RequestVerificationToken': $('@Html.AntiForgeryToken()').val() },
                    success: function (data) {
                        if (data.isValid) {
                            toastr.success(data.message, '');
                            setTimeout(function () { window.location = "/StoresTransferApprovals/Index" }, 3000);
                        } else {
                            if (data.isQuantitDiff)
                                document.location.replace("/StoresTransferApprovals/Index");
                            else
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
  
    //#endregion ========= end Step 2 ==========
    function Refused(id) {
        Swal.fire({
            title: 'تأكيد الرفض',
            text: 'هل متأكد من رفض امر التحويل المخزنى ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/StoresTransferApprovals/Refused';
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


    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        initItemDetails: function () {
            initDTItemDetails();
        },
        ApprovalInvoice: ApprovalInvoice,
        Refused: Refused,
    };

}();

