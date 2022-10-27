"use strict";

var CustomerPayment_Module = function () {
    var initDT = function () {
        var table = $('#kt_datatable');

        // begin first table
      table .DataTable({
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
                        return 'استلام نقدى من عميل';
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
                    "filename": "استلام نقدى من عميل",
                    "title": "استلام نقدى من عميل",
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
                url: '/CustomersPayments/GetAll',
                type: 'GET',
                data(d) {
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                }
            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'CustomerName', title: 'العميل' },
                { data: 'SafeName', title: 'الخزينة' },
                { data: 'Amount', title: 'المبلغ' },
                { data: 'PaymentDate', title: 'تاريخ العملية' },
                { data: 'EmployeeName', title: 'المندوب' },
                { data: 'Notes', title: 'الملاحظة' },
                { data: 'InvoType', title: 'حالة الفاتورة', visible: false },
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
							<a href="/CustomersPayments/Edit/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon"  title="تعديل">\
								<i class="fa fa-edit"></i>\
							</a>\
                                <a href="javascript:;" onclick=CustomerPayment_Module.Approval(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="اعتماد">\
								<i class="fa fa-check-square-o"></i>\
							</a><a href="javascript:;" onclick=CustomerPayment_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a></div>\
						';
                        } else {
                            return '\
							<div class="btn-group">\
                            <span class="label label-lg font-weight-bold label-light-success label-inline">تم اعتمادها</span>\
                                <a href = "javascript:;" onclick = CustomerPayment_Module.UnApproval(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title = "فك الاعتماد" >\
                            <i class="fa fa-check-square-o"></i>\
							</a ><a href="/GeneralDailies/Index/?tranId='+ row.Id + '&tranTypeId=6" class="btn btn-sm btn-clean btn-icon" title="عرض القيود">\
								<i class="fa fa-money"></i></a><a href="javascript:;" onclick="PrintInvoice_Module.PrintPayment(\''+ row.Id + '\',\'custPaymentCash\');" class="btn btn-sm btn-clean btn-icUrln" title="طباعه ايصال">\
								<i class="fa fa-print"></i>\
							</a>\</div >\
						';
                        }
                    },
                }

            ],
            drawCallback: function () {
                var html = ' <tr><th colspan ="11" style= "text-align:center" ><div class="row alert-success"><label>اجمالى المبلغ : ';
                var api = this.api();
                var totalAmount = api.column(3).data().sum();
                $(api.table().footer()).html(html + totalAmount + '</label></div></th>  </tr>');
            },
            //"order": [[0, "asc"]]
            "order": [[0, "desc"]] 

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
                            setTimeout(function () { window.location = "/CustomersPayments/Index" }, 3000);
                        } else
                            setTimeout(function () { window.location = "/CustomersPayments/CreateEdit" }, 3000);
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
                var url = '/CustomersPayments/Delete';
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

    function Approval(id) {
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
                var url = '/CustomersPayments/Approval';
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
                var url = '/CustomersPayments/UnApproval';
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
            $('#divCheck').hide();
        }
    }
    function onRdoBankChanged() {
        if ($("#rdo_bank:checked").val()) {
            $('#BankAccountId').removeAttr('disabled');
            $('#SafeId').attr('disabled', 'disabled');
            $('#divCheck').show();
        }
    };
    //العملاء بدلالة فئة العملاء
    function getCustomerOnCategoryChange() {
        //فى حالة ان تم اختيار مندوب
        var selMen = null;
        if ($("#BySaleMen").is(":checked")) {
            if ($("#EmployeeId").val() === '' || $("#EmployeeId").val() === null) {
                toastr.error('لابد من اختيار المندوب اولا', ''); return false;
            }
            selMen = $("#EmployeeId").val();
        }
        $.get("/SharedDataSources/GetCustomerOnCategoryChange/", { id: $("#PersonCategoryId").val(), saleMenId: selMen }, function (data) {
            $("#CustomerId").empty();
            $("#CustomerId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#CustomerId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });
        });
    };
    function getSaleMenDepartmentChange() {
        $.get("/SharedDataSources/GetSaleMenByDepartment", { id: $("#DepartmentId").val() }, function (data) {
            $("#EmployeeId").empty();
            $("#EmployeeId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#EmployeeId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        });
    };
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        getSafesOnBranchChanged: getSafesOnBranchChanged,
        onRdoSafeChanged: onRdoSafeChanged,
        onRdoBankChanged: onRdoBankChanged,
        Approval: Approval,
        UnApproval: UnApproval,
        getCustomerOnCategoryChange: getCustomerOnCategoryChange,
        getSaleMenDepartmentChange: getSaleMenDepartmentChange,
    };

}();


