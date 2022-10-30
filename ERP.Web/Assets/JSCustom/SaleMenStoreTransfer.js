"use strict";

var SaleMenStoreTransfer_Module = function () {
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
                        return 'اعتماد التحويلات المخزنية';
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
                    filename: "اعتماد التحويلات المخزنية",
                    title: "اعتماد التحويلات المخزنية",
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
                url: '/SaleMenStoreTransfers/GetAll',
                type: 'GET',

            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'Status', visible: false },
                { data: 'StoreFromName', title: 'من المخزن' },
                { data: 'TransferDate', title: 'تاريخ التحويل' },
                { data: 'Notes', title: 'الملاحظة' },
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
                        if (!row.SaleMenStatus) {
                            return '\
							<div class="btn-group">\
                                <a href="javascript:;" onclick=SaleMenStoreTransfer_Module.Approval("'+ row.Id + '","1") class="btn btn-sm btn-clean btn-icUrln" title="اعتماد وقبول">\
								<i class="fa fa-unlock-alt"></i>\
							</a><a href="javascript:;" onclick=SaleMenStoreTransfer_Module.Approval("'+ row.Id + '","2") class="btn btn-sm btn-clean btn-icUrln" title="رفض التحويل">\
								<i class="fa fa-ban"></i>\
							</a>\<a href="/SaleMenStoreTransfers/Show/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض تفاصيل العملية">\
								<i class="fa fa-search"></i>\
							</a>\</div>\
						';
                        } else if (row.SaleMenIsApproval){
                            return '\
							<div class="btn-group">\
                            <span class="label label-lg font-weight-bold label-light-success label-inline">تم اعتماد وقبول التحويل</span>\
                            <a href = "/SaleMenStoreTransfers/Show/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title = "عرض تفاصيل العملية" >\
                            <i class="fa fa-search"></i>\
							</a>\</div >\
						';
                        }else{
                            return '\
							<div class="btn-group">\
                            <span class="label label-lg font-weight-bold label-light-danger label-inline">تم رفض التحويل</span>\
                            <a href = "/SaleMenStoreTransfers/Show/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title = "عرض تفاصيل العملية" >\
                            <i class="fa fa-search"></i>\
							</a>\</div >\
						';
                        }
                    },
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


    function Approval(id, app) {
        var msgTitle = '';
        var msgText = '';
        if (app === '1') {
            msgTitle = "تأكيد الاعتماد وقبول التحويل";
            msgText = "هل متأكد من الاعتماد ؟";
        } else {
            msgTitle = "تأكيد رفض التحويل";
            msgText = "هل متأكد من الرفض ؟";
        }
        Swal.fire({
            title: msgTitle,
            text: msgText,
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/SaleMenStoreTransfers/Approval';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "id": id,
                        "app":app
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
        Approval: Approval,
    };

}();


