"use strict";


var ContractSchedulingAbsence_Module = function () {
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
                        return 'غياب الموظفين';
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
                    filename: "غياب الموظفين",
                    title: "غياب الموظفين",
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
                url: '/ContractSchedulingAbsences/GetAll',
                type: 'GET',

            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'EmployeeName', title: 'اسم الموظف' },
                { data: 'Name', title: 'الشهر' },
                { data: 'FromDate', title: 'من تاريخ' },
                { data: 'ToDate', title: 'الى تاريخ' },
                { data: 'AbsenceDayNumber', title: 'عدد ايام الغياب' },
                { data: 'IsPenalty', title: 'حالة الغياب' },
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
                        if (row.IsPayed == false) {
                            return '\
							<div class="btn-group">\
							<a href="javascript:;" onclick=ContractSchedulingAbsence_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a></div>\
						';
                            //                     <a href="javascript:;" onclick=ContractSchedulingAbsence_Module.ApprovalContractSchedulingAbsence(\''+ row.Id + '\') class= "btn btn-sm btn-clean btn-icUrln" title="اعتماد السلفة" >\
                            //	< i class= "fa fa-unlock-alt" ></i >\
                            //</a >
                        } else {
                            return '\
							<div class="btn-group">\
                            <span class="label label-lg font-weight-bold label-light-success label-inline">تم اعتمادها</span></div>\
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
            var formData = new FormData(form);
            formData.append("balanceDays", $("#balanceDays").val());
            formData.append("txtPenaltyNumber", $("#txtPenaltyNumber").val());
            $.ajax({
                type: 'POST',
                url: "/ContractSchedulingAbsences/CreateEdit",
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        //$(btn).attr('disabled', 'disabled'); // disabled button after one clicke 
                        $(btn).css('pointer-events', 'none'); // disabled a link after one clicke 
                        toastr.success(res.message, '',)
                        setTimeout(function () { window.location = "/ContractSchedulingAbsences/CreateEdit" }, 3000);
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
                var url = '/ContractSchedulingAbsences/Delete';
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

    function ApprovalContractSchedulingAbsence(id) {
        Swal.fire({
            title: 'تأكيد اعتماد الغياب',
            text: 'هل متأكد من الاعتماد ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/ContractSchedulingAbsences/ApprovalContractSchedulingAbsence';
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


    function onRdoPenaltyChanged() {
        if ($("#rdo_penalty:checked").val()) {
            //$('#IsPenaltyTypeDay').removeAttr('disabled');
            $('#IsPenalty').val(true);
            $('#VacationTypeId').attr('disabled', 'disabled');
        }
    }

    //function onComboPenaltyChange() {
    //    if ($("#IsPenaltyTypeDay option:selected").val() === '1') {
    //        $('#divPenaltyNumber').show();
    //        $('#divPenaltyAmount').hide();
    //        $('#divBalance').hide();
    //        return;
    //    }
    //    if ($("#IsPenaltyTypeDay option:selected").val() === '2') {
    //        $('#divPenaltyNumber').hide();
    //        $('#divPenaltyAmount').show();
    //        $('#divBalance').hide();
    //        return;
    //    }
    //    $('#divPenaltyNumber').hide();
    //    $('#divPenaltyAmount').hide();
    //    $('#divBalance').hide();

    //}
    function onRdoNoPenaltyChanged() {
        if ($("#rdo_noPenalty:checked").val()) {
            $('#IsPenalty').val(false);
            $('#VacationTypeId').removeAttr('disabled');
            //$('#IsPenaltyTypeDay').attr('disabled', 'disabled');
        }
    };
    function onVacationTypeIdChange() {
        $('#divBalance').show();
        $('#divPenaltyNumber').hide();
        $('#divPenaltyAmount').hide();
        $.get("/SharedDataSources/GetBalanceVacationDays", { schedulingId: $("#ContractSchedulingId").val(), vcationTypeId: $("#VacationTypeId").val(), dayAbs: $("#AbsenceDayNumber").val() }, function (data) {
            if (data.isValid) {
                $("#balanceDays").val(data.val);
            } else {
                toastr.error(data.msg, '');
            }
        });
    }
    function onDatesChange() {
        $.get("/SharedDataSources/GetDiffDaysSchedulingAbsence", { dFrom: $("#FromDate").val(), dTo: $("#ToDate").val() }, function (data) {
            if (data.isValid) {
                $("#AbsenceDayNumber").val(data.val);
                $("#txtPenaltyNumber").val(data.val);
            } else {
                $("#AbsenceDayNumber").val(0);
                $("#txtPenaltyNumber").val(0);
                toastr.error(data.msg, '');
            }
        });
    }
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        getEmployeesDepartmentChange: getEmployeesDepartmentChange,
        ApprovalContractSchedulingAbsence: ApprovalContractSchedulingAbsence,
        onDatesChange: onDatesChange,
        getContractSchedulingEmpChange: getContractSchedulingEmpChange,
        onRdoPenaltyChanged: onRdoPenaltyChanged,
        //onComboPenaltyChange: onComboPenaltyChange,
        onRdoNoPenaltyChanged: onRdoNoPenaltyChanged,
        onVacationTypeIdChange: onVacationTypeIdChange,
    };

}();


