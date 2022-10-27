"use strict";

var UserRole_Module = function () {
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
                        return 'المستخدمين';
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
                    filename: "المستخدمين",
                    title: "المستخدمين",
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
                url: '/UserRoles/GetAll',
                type: 'GET',
                //data(d) {
                //    d.departId = $("#DepartmentId").val();
                //    d.jobId = $("#JobId").val();
                //},
            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'UserName', title: 'اسم المستخدم' },
                { data: 'DepartmentName', title: 'الادارة' },
                { data: 'EmployeeName', title: 'اسم الموظف' },
                { data: 'RoleName', title: 'الصلاحية/المجموعة' },
                { data: 'Status', responsivePriority: -2 },
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
                    targets: -2,
                    title: 'حالة التفعيل',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        var status = {
                            1: { 'title': 'Pending', 'class': 'label-light-primary' },
                            2: { 'title': 'Delivered', 'class': ' label-light-danger' },
                            3: { 'title': 'Canceled', 'class': ' label-light-primary' },
                            4: { 'title': 'مفعل', 'class': ' label-light-success' },
                            5: { 'title': 'Info', 'class': ' label-light-info' },
                            6: { 'title': 'غير مفعل', 'class': ' label-light-danger' },
                            7: { 'title': 'Warning', 'class': ' label-light-warning' },
                        };
                        if (row.IsActive) {
                            return '<span class="label label-lg font-weight-bold' + status[4].class + ' label-inline">' + status[4].title + '</span>';
                        } else {
                            return '<span class="label label-lg font-weight-bold' + status[6].class + ' label-inline">' + status[6].title + '</span>';
                        }
                    },
                },
                {
                    targets: -1,
                    title: 'عمليات',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        if (row.EmployeeName == null) {
                            return '\
							<div class="btn-group">\
							</div>\
						';
                        } else {
                            return '\
							<div class="btn-group">\
							<a href="/UserRoles/Edit/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
								<i class="fa fa-edit"></i>\
							</a>\
							<a href="javascript:;" onclick=UserRole_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a></div>\
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


    function SubmitForm(btn) {
        try {
            var form = document.getElementById('form1');
            $.ajax({
                type: 'POST',
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        //$(btn).attr('disabled', 'disabled'); // disabled button after one clicke 
                        $(btn).css('pointer-events', 'none'); // disabled a link after one clicke 
                        toastr.success(res.message, '',)
                        if (!res.isInsert) {
                            setTimeout(function () { window.location = "/UserRoles/Index" }, 3000);
                        } else
                            setTimeout(function () { window.location = "/UserRoles/CreateEdit" }, 3000);
                        //$('#kt_datatableLast').DataTable().ajax.reload();
                    } else {
                        toastr.error(res.message, '');
                    }
                    //document.getElementById('submit').disabled = false;
                    //document.getElementById('reset').disabled = false;
                },
                error: function (err) {
                    alert(0);
                    toastr.error('حدث خطأ اثناء تنفيذ العملية', '');
                    console.log(err)
                }
            })
            //to prevent default form submit event
            return false;
        } catch (ex) {
            console.log(ex)
        }

    }

    function deleteRow(id) {
        Swal.fire({
            title: 'تأكيد الحذف',
            text: 'هل متأكد من الحذف ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/UserRoles/Delete';
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
    function getEmployeesDepartmentChange() {
        $.get("/SharedDataSources/GetEmployeeByDepartment", { id: $("#DepartmentId").val(), isUserRole:true }, function (data) {
            $("#EmployeeId").empty();
            $("#EmployeeId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#EmployeeId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        })
    };
   

    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        getEmployeesDepartmentChange: getEmployeesDepartmentChange
    };

}();


