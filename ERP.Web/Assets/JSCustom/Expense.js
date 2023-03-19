"use strict";
var Expense_Module = function () {
    //#region Expense
    var initDTExpense = function () {
        var table = $('#kt_datatableExpense');

        // begin first table
        table.DataTable({
            //responsive: true,
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
                        return 'مصروفات';
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
                    "extend": "excelHtml5",
                    "filename": "مصروفات",
                    "title": "مصروفات",
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
                url: '/Expenses/GetAll',
                type: 'GET',
                data(d) {
                    d.expenseTypeId = $("#ExpenseTypeId").val();
                    d.isApprovalStatus = $("#IsApprovalStatus").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                }
            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'IsApprovalStatus', visible: false },
                { data: 'ExpenseTypeName', title: 'نوع المصروف' },
                { data: 'SafeName', title: 'الخزينة' },
                { data: 'Amount', title: 'المبلغ' },
                { data: 'PaymentDate', title: 'تاريخ العملية' },
                { data: 'PaidTo', title: 'المستلم من الخزنة' },
                { data: 'Notes', title: 'الملاحظة' },
                { data: 'Actions', responsivePriority: -1 },

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
                        if (!row.IsApproval) {
                            return '\
							<div class="btn-group">\
							<a href="/Expenses/Edit/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon"  title="تعديل">\
								<i class="fa fa-edit"></i>\
							</a>\
                                <a href="javascript:;" onclick=Expense_Module.ApprovalExpense(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="اعتماد">\
								<i class="fa fa-check"></i>\
							</a><a href="javascript:;" onclick=Expense_Module.deleteRowExpense(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							      </a>\<a href="/Expenses/Copy/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="نسخ">\
								<i class="fa fa-copy"></i>\
							</a>\</div>\
                                ';
                        } else {
                            return '\
							<div class="btn-group">\
                            <span class="label label-lg font-weight-bold label-light-success label-inline">تم اعتمادها</span><a href="/PrintInvoices/PrintGeneralRecord/?id='+ row.Id + '&typ=expense" target="_blank" class="btn btn-sm btn-clean btn-icon" title="عرض وطباعة">\
								<i class="fa fa-print"></i>\
							</a><a href="javascript:;" onclick=Expense_Module.UnApprovalExpense(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="فك الاعتماد">\
								<i class="fa fa-unlock-alt"></i>\<a href="/GeneralDailies/Index/?tranId='+ row.Id + '&tranTypeId=7" class="btn btn-sm btn-clean btn-icon" title="عرض القيود">\
								<i class="fa fa-money-bill"></i>\
							      </a>\<a href="/Expenses/Copy/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="نسخ">\
								<i class="fa fa-copy"></i>\
							</a>\</div>\
						';
                        }
                    },
                }

            ],
            drawCallback: function () {
                var html = ' <tr><th colspan ="11" style= "text-align:center" ><div class="row alert-success"><label>اجمالى المبلغ : ';
                var api = this.api();
                var totalAmount = api.column(4).data().sum();
                $(api.table().footer()).html(html + totalAmount + '</label></div></th>  </tr>');
            },

            "order": [[0, "asc"]]
            //"order": [[0, "desc"]]

        });
        $('#export_print').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableExpense').DataTable().button('.buttons-print').trigger();
        });

        $('#export_copy').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableExpense').DataTable().button('.buttons-copy').trigger();
        });

        $('#export_excel').on('click', function (e) {
            e.preventDefault();
            $('#kt_datatableExpense').DataTable().button('.buttons-excel').trigger();
        });
    };
    function SubmitFormExpense(btn) {
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
                            setTimeout(function () { window.location = "/Expenses/Index" }, 3000);
                        } else
                            setTimeout(function () { window.location = "/Expenses/CreateEdit" }, 3000);
                        //$('#kt_datatableExpenseLast').DataTable().ajax.reload();
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
    function deleteRowExpense(id) {
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
                var url = '/Expenses/Delete';
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
                            $('#kt_datatableExpense').DataTable().ajax.reload();
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
    function ApprovalExpense(id) {
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
                var url = '/Expenses/Approval';
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
                            $('#kt_datatableExpense').DataTable().ajax.reload();
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
    function UnApprovalExpense(id) {
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
                var url = '/Expenses/UnApproval';
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
                            $('#kt_datatableExpense').DataTable().ajax.reload();
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


    function getSafesOnBranchChanged() {  // get safes and stores by branchId
        $.get("/SharedDataSources/getSafesOnBranchChanged", { id: $("#BranchId").val(), userId: $("#Hdf_userId").val() }, function (data) {
            $("#SafeId").empty();
            $("#SafeId").append("<option value=>اختر عنصر من القائمة </option>");
            $.each(data, function (index, row) {
                $("#SafeId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            })
        });
    };
    function getBalanceAccountChanged() {  // get balance  by accountid
        $.get("/SharedDataSources/GetAccountBalance", { id: $("#SafeId").val(), isSafe:true }, function (data) {
            $("#AccountBalance").val(data);
           
        });
    };

    return {
        //main function to initiate the module
        initDTExpense: function () {
            initDTExpense();
        },
        SubmitFormExpense: SubmitFormExpense,
        deleteRowExpense: deleteRowExpense,
        ApprovalExpense: ApprovalExpense,
        UnApprovalExpense: UnApprovalExpense,
        getSafesOnBranchChanged: getSafesOnBranchChanged,
        getBalanceAccountChanged: getBalanceAccountChanged,
    };

}();


