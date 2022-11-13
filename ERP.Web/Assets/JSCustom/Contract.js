"use strict";
var Contract_Module = function () {

    //#region ======== تسجيل عقد لموظف ==================
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
                        return 'عقود الموظفين';
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
                    filename: "عقود الموظفين",
                    title: "عقود الموظفين",
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
                url: '/Contracts/GetAll',
                type: 'GET',

            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'EmployeeName', title: 'اسم الموظف' },
                { data: 'ContractTypeName', title: 'نوع العقد' },
                { data: 'FromDate', title: 'من تاريخ' },
                { data: 'ToDate', title: 'الى تاريخ' },
                { data: 'Salary', title: 'الراتب' },
                { data: 'IsActiveStatus', title: 'حالة العقد' },
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
                        if (row.IsApproval) {
                            return '\
							<div class="btn-group">\
							</a><a href="/UploadCenterTypeFiles/Index/?typ=' + row.typ + '&refGid=' + row.EmpGuid + '" class="btn btn-sm btn-clean btn-icUrln" title="رفع ملفات العقد">\
								<i class="fa fa-upload"></i>\
							</a><a href = "javascript:;" onclick = Contract_Module.UnApprovalContract("'+ row.Id + '") class="btn btn-sm btn-clean btn-icUrln" title = "الغاء الاعتماد" >\
                            <i class="las la-times-circle"></i>\
                            </div >\
';
                        } else {
                            return '\
							<div class="btn-group">\
							<a href="/Contracts/Edit/?conGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
								<i class="fa fa-edit"></i>\
							</a>\
							<a href="javascript:;" onclick=Contract_Module.deleteRow("'+ row.Id + '") class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a><a href="javascript:;" onclick=Contract_Module.ApprovalContract("'+ row.Id + '") class="btn btn-sm btn-clean btn-icUrln" title="اعتماد">\
								<i class="fa fa-unlock-alt"></i>\
							</a><a href="/UploadCenterTypeFiles/Index/?typ=' + row.typ + '&refGid=' + row.EmpGuid + '" class="btn btn-sm btn-clean btn-icUrln" title="رفع ملفات العقد">\
								<i class="fa fa-upload"></i>\
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

    function SubmitForm(btn, isSaveUpload) {
        try {
            var salary = document.getElementById('Salary').value;
            if (salary === '' || salary === '0') {
                toastr.error('تأكد من ادخال قيمة الراتب الاساسى', '');
                return false;
            };

            var form = document.getElementById('kt_form');
            var formData = new FormData(form);
            var dataSet = $('#kt_dtSalaryAddition').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_DatasourceSalaryAdditions", JSON.stringify(dataSet));
                }
            }
            var salaryPenalty = $('#kt_dtSalaryPenalty').DataTable().rows().data().toArray();
            if (salaryPenalty != null) {
                if (salaryPenalty.length > 0) {
                    formData.append("DT_DatasourceSalaryPenaltys", JSON.stringify(salaryPenalty));
                }
            }
            var definitionVacation = $('#kt_dtDefinitionVacation').DataTable().rows().data().toArray();
            if (definitionVacation != null) {
                if (definitionVacation.length > 0) {
                    formData.append("DT_DatasourceDefinitionVacations", JSON.stringify(definitionVacation));
                }
            }

            $.ajax({
                type: 'POST',
                url: formData.action,
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $(btn).attr('disabled', 'disabled'); // disabled button after one clicke 
                        toastr.success(res.message, '',)
                        if (isSaveUpload) { //فى حالةالضغط على حفظ ورفع ملفات للفاتورة 
                            setTimeout(function () { window.location = "/UploadCenterTypeFiles/Index/?typ=" + res.typ + "&refGid=" + res.refGid }, 3000);
                        } else {
                            if (!res.isInsert) {
                                setTimeout(function () { window.location = "/Contracts/Index" }, 3000);
                            } else
                                setTimeout(function () { window.location = "/Contracts/CreateEdit" }, 3000);
                        }


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

    function deleteRow(Id) {
        Swal.fire({
            title: 'تأكيد الحذف',
            text: 'هل متأكد من الحدف ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/Contracts/Delete';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "conGuid": Id
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
    function ApprovalContract(Id) {
        Swal.fire({
            title: 'تأكيد الاعتماد',
            text: 'هل متأكد من الاعتماد ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/Contracts/ApprovalContract';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "conGuid": Id
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
    function UnApprovalContract(Id) {
        Swal.fire({
            title: 'تأكيد الغاء الاعتماد',
            text: 'هل متأكد من الغاء الاعتماد ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/Contracts/UnApprovalContract';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "conGuid": Id
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

    //#endregion ============== end ==============

    //#region ========== Step 1 البيانات الاساسية===============
    function getEmployeesDepartmentChange() {
        $.get("/SharedDataSources/GetEmployeeByDepartment", { id: $("#DepartmentId").val(), isContractReg: true }, function (data) {
            $("#EmployeeId").empty();
            $("#EmployeeId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#EmployeeId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        });
        removeClassInValidDepart()
    };

    function onSalaryChange() {
        $("#salary2").text($("#Salary").val());
        getSafySalary();
    };
    //#endregion ========== end Step 1 ============

    //#region ======== Step 2 تسجيل بدلات الموظف=================
    function onSalaryAdditionTypeChange() {
        $.get("/SharedDataSources/onSalaryAdditionTypeChange", { id: $("#SalaryAdditionTypeId").val() }, function (data) {
            $("#SalaryAdditionId").empty();
            $("#SalaryAdditionId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#SalaryAdditionId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        })
    };

    var initDTSalaryAdditions = function () {
        var table = $('#kt_dtSalaryAddition');

        // begin first table
        table.DataTable({
            responsive: true,
            searchDelay: 500,
            processing: true,
            serverSide: false,
            select: true,

            // DOM Layout settings
            dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",
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
                url: '/Contracts/GetDSSalaryAddition',
                type: 'GET',

            },
            columns: [
                { data: 'Id', title: 'م', visible: false },
                { data: 'SalaryAdditionId', visible: false },
                { data: 'SalaryAdditionTypeName', title: 'نوع الاضافة' },
                { data: 'SalaryAdditionName', title: 'مسمى البدل' },
                { data: 'SalaryAdditionAmount', title: 'القيمة' },
                { data: 'SalaryAdditionNotes', title: 'ملاحظات' },
                { data: 'Actions', responsivePriority: -1 },
            ],
            columnDefs: [
                //{
                //    targets: 1,
                //    title: 'م',
                //    orderable: false,
                //    render: function (data, type, row, meta) {
                //        return  meta.row + meta.settings._iDisplayStart + 1;
                //    },
                //},
                {
                    targets: -1,
                    title: 'عمليات',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return '\
							<div class="btn-group">\
							<a href="javascript:;" onclick=Contract_Module.deleteRowSalaryAddition('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a></div>\
						';
                    },
                }

            ],

            "order": [[0, "asc"]]
            //"order": [[0, "desc"]] 

        });
    };

    function addSalaryAddition() {
        try {
            var salaryAdditionId = document.getElementById('SalaryAdditionId').value;
            var amount = document.getElementById('SalaryAdditionAmount').value;
            var notes = document.getElementById('SalaryAdditionNotes').value;
            var formData = new FormData();
            if (salaryAdditionId === '') {
                toastr.error('تأكد من اختيار البدل', '');
                return false;
            };
            if (amount === '' || amount === '0') {
                toastr.error('تأكد من ادخال قيمة البدل', '');
                return false;
            };

            formData.append('SalaryAdditionId', salaryAdditionId)
            formData.append('SalaryAdditionAmount', amount)
            formData.append('SalaryAdditionNotes', notes)
            var dataSet = $('#kt_dtSalaryAddition').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/Contracts/AddSalaryAddition',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtSalaryAddition').DataTable().ajax.reload();
                        $('#SalaryAdditionAmount').val(0);
                        $('#SalaryAddition').val('');
                        $('#SalaryAdditionNotes').val('');

                        $('#TotalSalaryAddition,#TotalSalaryAddition2').text(res.totalAmount);
                        getSafySalary();
                        toastr.success(res.msg, '');

                    } else
                        toastr.error(res.msg, '');
                    return false;
                },
                error: function (err) {
                    toastr.error('حدث خطأ اثناء تنفيذ العملية', '');
                    console.log(err)
                }
            })
            //to prevent default form submit event
            return false;
            //$('#kt_datatableTreePrice').DataTable().ajax.reload();
            //to prevent default form submit event
            return false;
        } catch (ex) {
            console.log(ex)
        }

    }

    function deleteRowSalaryAddition(id) {
        $('#kt_dtSalaryAddition tbody').on('click', 'a.deleteIcon', function () {
            var amountRemoved = $('#kt_dtSalaryAddition').DataTable().row($(this).closest('tr')).data()['SalaryAdditionAmount'];
            $("#TotalSalaryAddition,#TotalSalaryAddition2").text(Number.parseFloat($("#TotalSalaryAddition").text()) - amountRemoved);
            $('#kt_dtSalaryAddition').DataTable().row($(this).parents('tr')).remove().draw();
            getSafySalary();
        })

    };

    //#endregion ========= end Step 2 ==========

    //#region ======== Step 3 تسجيل خصومات الموظف=================
    function onSalaryPenaltyTypeChange() {
        $.get("/SharedDataSources/onSalaryPenaltyTypeChange", { id: $("#SalaryPenaltyTypeId").val() }, function (data) {
            $("#SalaryPenaltyId").empty();
            $("#SalaryPenaltyId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#SalaryPenaltyId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        })
    };

    var initDTSalaryPenaltys = function () {
        var table = $('#kt_dtSalaryPenalty');

        // begin first table
        table.DataTable({
            responsive: true,
            searchDelay: 500,
            processing: true,
            serverSide: false,
            select: true,

            // DOM Layout settings
            dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",
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
                url: '/Contracts/GetDSSalaryPenalty',
                type: 'GET',

            },
            columns: [
                { data: 'Id', title: 'م', visible: false },
                { data: 'SalaryPenaltyId', visible: false },
                { data: 'SalaryPenaltyTypeName', title: 'نوع الخصم' },
                { data: 'SalaryPenaltyName', title: 'مسمى الخصم' },
                { data: 'SalaryPenaltyAmount', title: 'القيمة' },
                { data: 'SalaryPenaltyNotes', title: 'ملاحظات' },
                { data: 'Actions', responsivePriority: -1 },
            ],
            columnDefs: [
                //{
                //    targets: 1,
                //    title: 'م',
                //    orderable: false,
                //    render: function (data, type, row, meta) {
                //        return  meta.row + meta.settings._iDisplayStart + 1;
                //    },
                //},
                {
                    targets: -1,
                    title: 'عمليات',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return '\
							<div class="btn-group">\
							<a href="javascript:;" onclick=Contract_Module.deleteRowSalaryPenalty('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a></div>\
						';
                    },
                }

            ],

            "order": [[0, "asc"]]
            //"order": [[0, "desc"]] 

        });
    };

    function addSalaryPenalty() {
        try {
            var SalaryPenaltyId = document.getElementById('SalaryPenaltyId').value;
            var amount = document.getElementById('SalaryPenaltyAmount').value;
            var notes = document.getElementById('SalaryPenaltyNotes').value;
            var formData = new FormData();
            if (SalaryPenaltyId === '') {
                toastr.error('تأكد من اختيار الخصم', '');
                return false;
            };
            if (amount === '' || amount === '0') {
                toastr.error('تأكد من ادخال قيمة الخصم', '');
                return false;
            };

            formData.append('SalaryPenaltyId', SalaryPenaltyId)
            formData.append('SalaryPenaltyAmount', amount)
            formData.append('SalaryPenaltyNotes', notes)
            var dataSet = $('#kt_dtSalaryPenalty').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/Contracts/AddSalaryPenalty',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtSalaryPenalty').DataTable().ajax.reload();
                        $('#SalaryPenaltyAmount').val(0);
                        $('#SalaryPenaltyNotes').val('');

                        $('#TotalSalaryPenalty,#TotalSalaryPenalty2').text(res.totalPenalty);
                        getSafySalary();
                        toastr.success(res.msg, '');

                    } else
                        toastr.error(res.msg, '');
                    return false;
                },
                error: function (err) {
                    toastr.error('حدث خطأ اثناء تنفيذ العملية', '');
                    console.log(err)
                }
            })
            //to prevent default form submit event
            return false;
            //$('#kt_datatableTreePrice').DataTable().ajax.reload();
            //to prevent default form submit event
            return false;
        } catch (ex) {
            console.log(ex)
        }

    }

    function deleteRowSalaryPenalty(id) {
        $('#kt_dtSalaryPenalty tbody').on('click', 'a.deleteIcon', function () {
            var amountRemoved = $('#kt_dtSalaryPenalty').DataTable().row($(this).closest('tr')).data()['SalaryPenaltyAmount'];
            $("#TotalSalaryPenalty,#TotalSalaryPenalty2").text(Number.parseFloat($("#TotalSalaryPenalty").text()) - amountRemoved);
            $('#kt_dtSalaryPenalty').DataTable().row($(this).parents('tr')).remove().draw();
            getSafySalary();
        })

    };

    //#endregion ========= end Step 3 ==========
    //#region ======== Step 4 تسجيل الاجازات المسموح بها الموظف=================

    var initDTDefinitionVacations = function () {
        var table = $('#kt_dtDefinitionVacation');

        // begin first table
        table.DataTable({
            responsive: true,
            searchDelay: 500,
            processing: true,
            serverSide: false,
            select: true,

            // DOM Layout settings
            dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",
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
                url: '/Contracts/GetDSDefinitionVacations',
                type: 'GET',

            },
            columns: [
                { data: 'Id', title: 'م', visible: false },
                { data: 'VacationTypeId', visible: false },
                { data: 'VacationTypeName', title: 'نوع الاجازة' },
                { data: 'DayNumber', title: 'عدد الايام' },
                { data: 'Actions', responsivePriority: -1 },
            ],
            columnDefs: [
                //{
                //    targets: 1,
                //    title: 'م',
                //    orderable: false,
                //    render: function (data, type, row, meta) {
                //        return  meta.row + meta.settings._iDisplayStart + 1;
                //    },
                //},
                {
                    targets: -1,
                    title: 'عمليات',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return '\
							<div class="btn-group">\
							<a href="javascript:;" onclick=Contract_Module.deleteRowDefinitionVacation('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a></div>\
						';
                    },
                }

            ],

            "order": [[0, "asc"]]
            //"order": [[0, "desc"]] 

        });
    };

    function addDefinitionVacation() {
        try {
            var vacationTypeId = document.getElementById('VacationTypeId').value;
            var dayNumber = document.getElementById('DayNumber').value;
            var formData = new FormData();
            if (vacationTypeId === '') {
                toastr.error('تأكد من اختيار نوع الاجازة', '');
                return false;
            };
            if (dayNumber === '' || dayNumber === '0') {
                toastr.error('تأكد من ادخال عدد الايام', '');
                return false;
            };

            formData.append('VacationTypeId', vacationTypeId)
            formData.append('DayNumber', dayNumber)
            var dataSet = $('#kt_dtDefinitionVacation').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/Contracts/AddDefinitionVacation',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtDefinitionVacation').DataTable().ajax.reload();
                        $('#VacationTypeId').val('');
                        $('#DayNumber').val(0);

                        $('#TotalDefinitionVacations,#TotalDefinitionVacations2').text(res.totalDefinitionVacation);
                        getSafySalary();
                        toastr.success(res.msg, '');

                    } else
                        toastr.error(res.msg, '');
                    return false;
                },
                error: function (err) {
                    toastr.error('حدث خطأ اثناء تنفيذ العملية', '');
                    console.log(err)
                }
            })
            //to prevent default form submit event
            return false;
            //$('#kt_datatableTreePrice').DataTable().ajax.reload();
            //to prevent default form submit event
            return false;
        } catch (ex) {
            console.log(ex)
        }

    }

    function deleteRowDefinitionVacation(id) {
        $('#kt_dtDefinitionVacation tbody').on('click', 'a.deleteIcon', function () {
            var dayNumberRemoved = $('#kt_dtDefinitionVacation').DataTable().row($(this).closest('tr')).data()['DayNumber'];
            $("#TotalDefinitionVacations,#TotalDefinitionVacations2").text(Number.parseFloat($("#TotalDefinitionVacations").text()) - dayNumberRemoved);
            $('#kt_dtDefinitionVacation').DataTable().row($(this).parents('tr')).remove().draw();
            getSafySalary();
        })

    };

    //#endregion ========= end Step 3 ==========


    function getSafySalary() {
        var salary = Number.parseFloat($("#salary2").text());
        if (isNaN(salary))
            salary = 0;

        var totalSalaryAddioton = Number.parseFloat($("#TotalSalaryAddition").text());
        if (isNaN(totalSalaryAddioton))
            totalSalaryAddioton = 0;

        var totalSalaryPenalty = Number.parseFloat($("#TotalSalaryPenalty").text());
        if (isNaN(totalSalaryPenalty))
            totalSalaryPenalty = 0;

        var safy = (salary + totalSalaryAddioton) - totalSalaryPenalty;
        $("#SafySalary").text(safy);
    };

    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        //step 1
        getEmployeesDepartmentChange: getEmployeesDepartmentChange,
        onSalaryChange: onSalaryChange,
        //step2
        onSalaryAdditionTypeChange: onSalaryAdditionTypeChange,
        initSalaryAddition: function () {
            initDTSalaryAdditions();
        },
        addSalaryAddition: addSalaryAddition,
        deleteRowSalaryAddition: deleteRowSalaryAddition,
        //step3
        onSalaryPenaltyTypeChange: onSalaryPenaltyTypeChange,
        initSalaryPenalty: function () {
            initDTSalaryPenaltys();
        },
        addSalaryPenalty: addSalaryPenalty,
        deleteRowSalaryPenalty: deleteRowSalaryPenalty,
        //step4
        initDTDefinitionVacation: function () {
            initDTDefinitionVacations();
        },
        addDefinitionVacation: addDefinitionVacation,
        deleteRowDefinitionVacation: deleteRowDefinitionVacation,

        getSafySalary: getSafySalary,
        ApprovalContract: ApprovalContract,
        UnApprovalContract: UnApprovalContract
    };

}();

