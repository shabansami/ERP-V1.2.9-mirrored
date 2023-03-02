﻿"use strict";

var SalaryApproval_Module = function () {
    //#region ========= الصفحة الرئيسية 
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
                url: '/SalaryApproval/GetAll',
                type: 'GET',

                    },
        columns: [
            { data: 'Id', title: 'م', visible: false },
            { data: 'IsApproval', visible: false },
            { data: 'IsPayed', visible: false },
            { data: 'Num',responsivePriority:1 },
            { data: 'Employee', title: 'الموظف' },
            { data: 'Month', title: 'الشهر'},
            { data: 'Year', title: 'السنة'},
            { data: 'Actions', responsivePriority: -1},

        ],
            columnDefs: [
                {
                    targets: 1,
                    title: 'م',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return  meta.row + meta.settings._iDisplayStart + 1;
                    },
                },
            {
                targets: -1,
                title: 'عمليات',
                orderable: false,
                render: function (data, type, row, meta) {
                    if (!row.IsApproval && !row.IsPayed) {
                        return '\
							<div class="btn-group" id="div'+row.Id+'">\
							<a href="/SalaryApproval/Edit/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="استعراض وتعديل">\
								<i class="fa fa-edit"></i>\
							</a>\<a href="javascript:;"  onclick=SalaryApproval_Module.ApprovalSalary("' + row.Id + '") class="btn btn-sm btn-clean btn-icon" title="اعتماد الراتب">\
								<i class="fa fa-check"></i>\
							</a>\</div>\
						';
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
            };
    function CalculationSalary(btn) {
        swal.fire({
            title:'جـــارى احتساب الرواتب',
            text: '',
            showConfirmButton: false,
            closeOnClickOutside: false,
            allowOutsideClick: false,
            animation: false
        });
        Swal.showLoading();
        try {
            //var form = document.getElementById('form1');
            var dt = document.getElementById('dtMonthYear');
            var formData = new FormData();
            formData.append("dtMonthYear", dt.value)
            $.ajax({
                type: 'POST',
                url: "/SalaryApproval/Index",
                data: formData,
                contentType: false,
/*                dataType: 'json',*/
                processData: false,
                success: function (res) {
                    swal.close();
                    if (res.isValid) {
                        toastr.success(res.message, '',)
                        $('#kt_datatable').DataTable().ajax.reload();
                    } else {
                        toastr.error(res.message, '');
                    }
                    //document.getElementById('submit').disabled = false;
                    //document.getElementById('reset').disabled = false;
                },
                error: function (err) {
                    swal.close();
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

    //#endregion ==========================
    //#region ======== البدلات والاضافات
    var initSalaryAllowance = function () {
        var table = $('#kt_dtSalaryAllowance');
        // begin first table
        table.DataTable({
            paging: false,
            info: false,
            search: false,
            searching: false,
            "order": [[0, "asc"]],
            // DOM Layout settings
            dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",
            language: {
                processing: "جارى التحميل",
                zeroRecords: "لا يوجد سجلات لعرضها",
                infoFiltered: "",
                infoEmpty: 'لا يوجد سجلات متاحه',
                oPaginate: {
                    sNext: '<span class="pagination-default">التالى</span><span class="pagination-fa"><i class="fa fa-chevron-left" ></i></span>',
                    sPrevious: '<span class="pagination-default">السابق</span><span class="pagination-fa"><i class="fa fa-chevron-right" ></i></span>'
                }
            },
        });
    };
    var initSalaryAddition = function () {
        var table = $('#kt_dtSalaryAddition');
        // begin first table
        table.DataTable({
            paging: false,
            info: false,
            search: false,
            searching: false,
            "order": [[0, "asc"]],
            // DOM Layout settings
            dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",
            language: {
                processing: "جارى التحميل",
                zeroRecords: "لا يوجد سجلات لعرضها",
                infoFiltered: "",
                infoEmpty: 'لا يوجد سجلات متاحه',
                oPaginate: {
                    sNext: '<span class="pagination-default">التالى</span><span class="pagination-fa"><i class="fa fa-chevron-left" ></i></span>',
                    sPrevious: '<span class="pagination-default">السابق</span><span class="pagination-fa"><i class="fa fa-chevron-right" ></i></span>'
                }
            },
        });
    };

    //الاستقطاعات الشهرية والخصومات
    var initSalaryEveryMonPenalty = function () {
        var table = $('#kt_dtSalaryEveryMonthPenalty');
        // begin first table
        table.DataTable({
            paging: false,
            info: false,
            search: false,
            searching: false,
            "order": [[0, "asc"]],
            // DOM Layout settings
            dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",
            language: {
                processing: "جارى التحميل",
                zeroRecords: "لا يوجد سجلات لعرضها",
                infoFiltered: "",
                infoEmpty: 'لا يوجد سجلات متاحه',
                oPaginate: {
                    sNext: '<span class="pagination-default">التالى</span><span class="pagination-fa"><i class="fa fa-chevron-left" ></i></span>',
                    sPrevious: '<span class="pagination-default">السابق</span><span class="pagination-fa"><i class="fa fa-chevron-right" ></i></span>'
                }
            },
        });
    };
    var initSalaryPenalty = function () {
        var table = $('#kt_dtSalaryPenalty');
        // begin first table
        table.DataTable({
            paging: false,
            info: false,
            search: false,
            searching: false,
            "order": [[0, "asc"]],
            // DOM Layout settings
            dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",
            language: {
                processing: "جارى التحميل",
                zeroRecords: "لا يوجد سجلات لعرضها",
                infoFiltered: "",
                infoEmpty: 'لا يوجد سجلات متاحه',
                oPaginate: {
                    sNext: '<span class="pagination-default">التالى</span><span class="pagination-fa"><i class="fa fa-chevron-left" ></i></span>',
                    sPrevious: '<span class="pagination-default">السابق</span><span class="pagination-fa"><i class="fa fa-chevron-right" ></i></span>'
                }
            },
        });
    };

    //الغياب
    var initAllowAbsenc = function () {
        var table = $('#kt_dtAllowAbsenc');
        // begin first table
        table.DataTable({
            paging: false,
            info: false,
            search: false,
            searching: false,
            "order": [[0, "asc"]],
            // DOM Layout settings
            dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",
            language: {
                processing: "جارى التحميل",
                zeroRecords: "لا يوجد سجلات لعرضها",
                infoFiltered: "",
                infoEmpty: 'لا يوجد سجلات متاحه',
                oPaginate: {
                    sNext: '<span class="pagination-default">التالى</span><span class="pagination-fa"><i class="fa fa-chevron-left" ></i></span>',
                    sPrevious: '<span class="pagination-default">السابق</span><span class="pagination-fa"><i class="fa fa-chevron-right" ></i></span>'
                }
            },
        });
    };
    var initPenaltyAbsenc = function () {
        var table = $('#kt_dtPenaltyAbsenc');
        // begin first table
        table.DataTable({
            paging: false,
            info: false,
            search: false,
            searching: false,
            "order": [[0, "asc"]],
            // DOM Layout settings
            dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",
            language: {
                processing: "جارى التحميل",
                zeroRecords: "لا يوجد سجلات لعرضها",
                infoFiltered: "",
                infoEmpty: 'لا يوجد سجلات متاحه',
                oPaginate: {
                    sNext: '<span class="pagination-default">التالى</span><span class="pagination-fa"><i class="fa fa-chevron-left" ></i></span>',
                    sPrevious: '<span class="pagination-default">السابق</span><span class="pagination-fa"><i class="fa fa-chevron-right" ></i></span>'
                }
            },
        });
    };
    //سلف وقروض الموظف
    var initLoan = function () {
        var table = $('#kt_dtLoan');
        // begin first table
        table.DataTable({
            paging: false,
            info: false,
            search: false,
            searching: false,
            "order": [[0, "asc"]],
            // DOM Layout settings
            dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",
            language: {
                processing: "جارى التحميل",
                zeroRecords: "لا يوجد سجلات لعرضها",
                infoFiltered: "",
                infoEmpty: 'لا يوجد سجلات متاحه',
                oPaginate: {
                    sNext: '<span class="pagination-default">التالى</span><span class="pagination-fa"><i class="fa fa-chevron-left" ></i></span>',
                    sPrevious: '<span class="pagination-default">السابق</span><span class="pagination-fa"><i class="fa fa-chevron-right" ></i></span>'
                }
            },
        });
    };

    //#endregion =========================

    //#region save edit salary
    function SubmitForm(btn) {
        try {
            var form = document.getElementById('kt_form');
            var formData = new FormData(form);

            var currentAllowances = new Array();
            $.each($(".contractAllowances"), function () {
                var data = $(this).parents('tr:eq(0)');
                currentAllowances.push({ 'Id': $(data).find('td:eq(0)').text(), 'AmountPayed': $(data).find('td:eq(4) input[type="text"]').val() });
            });

            var currentSalaryAdditions = new Array();
            $.each($(".contractSalaryAdditions"), function () {
                var data = $(this).parents('tr:eq(0)');
                currentSalaryAdditions.push({ 'Id': $(data).find('td:eq(0)').text(), 'AmountPayed': $(data).find('td:eq(4) input[type="text"]').val() });
            });

            var currentSalaryEveryMonthPenalties = new Array();
            $.each($(".salaryEveryMonthPenalties"), function () {
                var data = $(this).parents('tr:eq(0)');
                currentSalaryEveryMonthPenalties.push({ 'Id': $(data).find('td:eq(0)').text(), 'AmountPayed': $(data).find('td:eq(4) input[type="text"]').val() });
            });

            var currentSalaryPenalties = new Array();
            $.each($(".contractSalaryPenalties"), function () {
                var data = $(this).parents('tr:eq(0)');
                currentSalaryPenalties.push({ 'Id': $(data).find('td:eq(0)').text(), 'AmountPayed': $(data).find('td:eq(4) input[type="text"]').val() });
            });
            formData.append("currentAllowances", JSON.stringify(currentAllowances));
            formData.append("currentSalaryAdditions", JSON.stringify(currentSalaryAdditions));
            formData.append("currentSalaryEveryMonthPenalties", JSON.stringify(currentSalaryEveryMonthPenalties));
            formData.append("currentSalaryPenalties", JSON.stringify(currentSalaryPenalties));
            $.ajax({
                type: 'POST',
                url: form.action,
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        //$(btn).attr('disabled', 'disabled'); // disabled button after one clicke 
                        $(btn).css('pointer-events', 'none'); // disabled a link after one clicke 
                        toastr.success(res.message, '',)
                            setTimeout(function () { window.location = "/SalaryApproval/Index" }, 3000);
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


    };

    //#endregion

    //#region Approval Salary
    function ApprovalSalary(id) { // اعتماد فاتورة محاسبيا
        Swal.fire({
            title: 'تأكيد الاعتماد',
            text: 'هل متأكد من اعتماد الراتب بشكل نهائى ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/SalaryApproval/ApprovalSalary';
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
                            $("#div" + id).remove();
                            //setTimeout(function () { window.location = "/SalaryApproval/Index" }, 3000);
                            /*$('#kt_datatable').DataTable().ajax.reload();*/
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

    //#endregion

    function Calculation(val, typ) {
        var totalAllAmountAbsences = Number.parseFloat($("#TotalAllAmountAbsences").text());
        if (isNaN(totalAllAmountAbsences))
            totalAllAmountAbsences = 0;

        switch (typ) {
            //البدلات
            case 'TotalAllowances':
                var currentTotalAllowances = 0;
                $.each($(".contractAllowances"), function () {
                    var data = $(this).parents('tr:eq(0)');
                    currentTotalAllowances = currentTotalAllowances + Number.parseFloat($(data).find('td:eq(4) input[type="text"]').val());
                });
                $("#TotalAllowances,#TotalAllowances2").text(currentTotalAllowances);
                //اجمالى البدلات والاضافات
                $("#totalSalaryAdditionAllowances").text(Number.parseFloat(currentTotalAllowances) + Number.parseFloat($("#TotalSalaryAdditions").text()));
                getSafySalary();
                break;
            //الاضافات
            case 'TotalSalaryAdditions':
                var currentTotalSalaryAdditions = 0;
                $.each($(".contractSalaryAdditions"), function () {
                    var data = $(this).parents('tr:eq(0)');
                    currentTotalSalaryAdditions = currentTotalSalaryAdditions + Number.parseFloat($(data).find('td:eq(4) input[type="text"]').val());
                });
                $("#TotalSalaryAdditions,#TotalSalaryAdditions2").text(currentTotalSalaryAdditions);
                //اجمالى البدلات والاضافات
                $("#totalSalaryAdditionAllowances").text(Number.parseFloat(currentTotalSalaryAdditions) + Number.parseFloat($("#TotalAllowances").text()));
                getSafySalary();
                break;
            //الاستقطاعات الشهرية
            case 'TotalSalaryEveryMonthPenalties':
                var currentTotalSalaryEveryMonthPenalties = 0;
                $.each($(".salaryEveryMonthPenalties"), function () {
                    var data = $(this).parents('tr:eq(0)');
                    currentTotalSalaryEveryMonthPenalties = currentTotalSalaryEveryMonthPenalties + Number.parseFloat($(data).find('td:eq(4) input[type="text"]').val());
                });
                $("#TotalSalaryEveryMonthPenalties,#TotalSalaryEveryMonthPenalties2").text(currentTotalSalaryEveryMonthPenalties);
                //اجمالى الاستقطاعات والخصومات
                $("#totalAllSalaryPenalties").text(Number.parseFloat(currentTotalSalaryEveryMonthPenalties) + Number.parseFloat($("#TotalSalaryPenalties").text()) + totalAllAmountAbsences);
                getSafySalary();
                break;
            //الخصومات
            case 'TotalSalaryPenalties':
                var currentTotalSalaryPenalties = 0;
                $.each($(".contractSalaryPenalties"), function () {
                    var data = $(this).parents('tr:eq(0)');
                    currentTotalSalaryPenalties = currentTotalSalaryPenalties + Number.parseFloat($(data).find('td:eq(4) input[type="text"]').val());
                });
                $("#TotalSalaryPenalties,#TotalSalaryPenalties2").text(currentTotalSalaryPenalties);
                //اجمالى الاستقطاعات والخصومات
                $("#totalAllSalaryPenalties").text(Number.parseFloat(currentTotalSalaryPenalties) + Number.parseFloat($("#TotalSalaryEveryMonthPenalties").text()) + totalAllAmountAbsences);
                getSafySalary();
                break;

            default:
        }
    };

    function getSafySalary() {
        var totalAllowances = Number.parseFloat($("#TotalAllowances").text());
        if (isNaN(totalAllowances))
            totalAllowances = 0;
        var totalSalaryAdditions = Number.parseFloat($("#TotalSalaryAdditions").text());
        if (isNaN(totalSalaryAdditions))
            totalAllowances = 0;
        var totalSalaryEveryMonthPenalties = Number.parseFloat($("#TotalSalaryEveryMonthPenalties").text());
        if (isNaN(totalSalaryEveryMonthPenalties))
            totalSalaryEveryMonthPenalties = 0;
        var totalSalaryPenalties = Number.parseFloat($("#TotalSalaryPenalties").text());
        if (isNaN(totalSalaryPenalties))
            totalSalaryPenalties = 0;
        var totalAllAmountAbsences = Number.parseFloat($("#TotalAllAmountAbsences").text());
        if (isNaN(totalAllAmountAbsences))
            totalAllAmountAbsences = 0;
        var totalLoans = Number.parseFloat($("#TotalLoans").text());
        if (isNaN(totalLoans))
            totalLoans = 0;
        var salary = Number.parseFloat($("#Salary").val());
        if (isNaN(salary))
            salary = 0;
        //صافى الراتب (الراتب الاساسى+الاضافات والبدلات-الخصومات والسلف والغياب
        var safy = (salary + totalAllowances + totalSalaryAdditions) - (totalSalaryEveryMonthPenalties + totalSalaryPenalties + totalAllAmountAbsences + totalLoans);
        console.log(safy);
        $("#Safy").text(safy);

    }
    return {
        // Index page
        init: function () {
            initDT();
        },
        CalculationSalary: CalculationSalary,
        SubmitForm: SubmitForm,
        //البدلات والاضافات
        initSalaryAllowance: initSalaryAllowance,
        initSalaryAddition: initSalaryAddition,
        //الاستقطاعات والخصومات
        initSalaryEveryMonPenalty: initSalaryEveryMonPenalty,
        initSalaryPenalty: initSalaryPenalty,
        //الغياب
        initAllowAbsenc: initAllowAbsenc,
        initPenaltyAbsenc: initPenaltyAbsenc,
        //السلف /القروض
        initLoan: initLoan,
        Calculation: Calculation,

        getSafySalary: getSafySalary,
        ApprovalSalary: ApprovalSalary
    };

}();

