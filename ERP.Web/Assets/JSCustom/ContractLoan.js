"use strict";


var ContractLoan_Module = function () {
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
                        return 'سٌلف وقروض الموظفين';
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
                            .css('text-align', 'right');
                        //توسيط عنوان التقرير
                        $(win.document.body).find('h1').css('text-align', 'center');
                    }
                },
                {
                    extend: "copyHtml5",
                    exportOptions: {
                        columns: ':visible'
                    }
                },
                {
                    "extend": "excelHtml5",
                    "filename": "سٌلف وقروض الموظفين",
                    "title": "سٌلف وقروض الموظفين",
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
                url: '/ContractLoans/GetAll',
                type: 'GET',
                data(d) {
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                }
                    },
        columns: [
            { data: 'Num', responsivePriority: 0 },
            { data: 'EmployeeName', title: 'اسم الموظف' },
            { data: 'Salary', title: 'الراتب' },
            //{ data: 'Notes', title: 'ملاحظة' },
            { data: 'Amount', title: 'قيمة السلفة/القرض' },
            { data: 'NumberMonths', title: 'عدد الأقساط' },
            { data: 'ContractSchedulingDate', title: 'بداية اول قسط' },
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
                    if (row.IsApproval == false) {
                        return '\
							<div class="btn-group">\
							<a href="/ContractLoans/Edit/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
								<i class="la la-edit"></i>\
							</a>\
							<a href="javascript:;" onclick=ContractLoan_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="la la-trash"></i>\
							</a><a href="javascript:;" onclick=ContractLoan_Module.ApprovalContractLoan(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="اعتماد السلفة">\
								<i class="la la-check-square-o"></i>\
							</a></div>\
						';
                    } else {
                        return '\
							<div class="btn-group">\
                            <span class="label label-lg font-weight-bold label-light-success label-inline">تم اعتمادها</span><a href="javascript:;" onclick=ContractLoan_Module.UnApproval(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="فك الاعتماد">\
								<i class="la la-check-square-o"></i></a>\</div>\
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
                            setTimeout(function () { window.location = "/ContractLoans/Index" }, 3000);
                        }else
                            setTimeout(function () { window.location = "/ContractLoans/CreateEdit" } , 3000);
                        //$('#kt_datatableLast').DataTable().ajax.reload();
                    } else {
                        toastr.error(res.message, '');
                    }
                    //document.getElementById('submit').disabled = false;
                    //document.getElementById('reset').disabled = false;
                },
                error: function (err) {
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
                var url = '/ContractLoans/Delete';
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
                            toastr.success(data.message,'');
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

    function ApprovalContractLoan(id) {
        Swal.fire({
            title: 'تأكيد اعتماد السلفه/القرض',
            text: 'هل متأكد من الاعتماد ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/ContractLoans/ApprovalContractLoan';
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
    function UnApproval(id) {
        Swal.fire({
            title: 'تأكيد فك الاعتماد',
            text: 'هل متأكد من فك الاعتماد ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/ContractLoans/UnApproval';
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
        $.get("/SharedDataSources/GetEmployeeByDepartment", { id: $("#DepartmentId").val(), showAll: false }, function (data) {
            $("#EmployeeId").empty();
            $("#EmployeeId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#EmployeeId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        });
    };
    function getContractSchedulingEmpChange() {
        $.get("/SharedDataSources/GetContractSchedulingEmployee", { id: $("#EmployeeId").val() }, function (data) {
            $("#ContractSchedulingId").empty();
            $("#ContractSchedulingId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data.list, function (index, row) {
                $("#ContractSchedulingId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        });
    };
    function getSafesOnBranchChanged() {  // get safes and stores by branchId
        $.get("/SharedDataSources/getSafesOnBranchChanged", { id: $("#BranchId").val() }, function (data) {
            $("#SafeId").empty();
            $("#SafeId").append("<option value=>اختر عنصر من القائمة </option>");
            $.each(data, function (index, row) {
                $("#SafeId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            })
        });
    };

    function onRdoSafeChanged() {
        if ($("#rdo_safe:checked").val()) {
            $('#SafeId').removeAttr('disabled');
            $('#BankAccountId').attr('disabled', 'disabled');
        }
    }
    function onRdoBankChanged() {
        if ($("#rdo_bank:checked").val()) {
            $('#BankAccountId').removeAttr('disabled');
            $('#SafeId').attr('disabled', 'disabled');
        }
    };
    function onAmountKeyUp() {
        var amount = Number.parseFloat($("#Amount").val());
        var monthNum = Number.parseFloat($("#NumberMonths").val());
        if (monthNum === 0 || monthNum === '' )
            $("#AmountMonth").val(0);
        else
            $("#AmountMonth").val(amount / monthNum);
    }
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        getEmployeesDepartmentChange: getEmployeesDepartmentChange,
        ApprovalContractLoan: ApprovalContractLoan,
        getContractSchedulingEmpChange: getContractSchedulingEmpChange,
        getSafesOnBranchChanged: getSafesOnBranchChanged,
        onRdoSafeChanged: onRdoSafeChanged,
        onRdoBankChanged: onRdoBankChanged,
        onAmountKeyUp: onAmountKeyUp,
        UnApproval: UnApproval,
    };

}();


