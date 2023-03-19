"use strict";

var VoucherPayment_Module = function () {
    //#region VoucherPayment
    var initDTVoucherPayment = function () {
        var table = $('#kt_datatable');

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
                        return 'سندات صرف';
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
                    "filename": "سندات صرف",
                    "title": "سندات صرف",
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
                url: '/VoucherBankPayments/GetAll',
                type: 'GET',
                data(d) {
                    d.accountTreeFromId = $("#AccountTreeFromId").val();
                    d.isApprovalStatus = $("#IsApprovalStatus").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                }
            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'VoucherDate', title: 'تاريخ المعاملة' },
                { data: 'Amount', title: 'المبلغ' },
                { data: 'VoucherNumber', title: 'رقم السند' },
                { data: 'Notes', title: 'البيان' },
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
                        if (row.IsApproval) {
                            return '\
							<div class="btn-group">\
                            <span class="label label-lg font-weight-bold label-light-success label-inline">تم اعتمادها</span><a href="/PrintInvoices/PrintGeneralRecord/?id='+ row.Id + '&typ=voucherPayment" target="_blank" class="btn btn-sm btn-clean btn-icon" title="عرض وطباعة">\
								<i class="fa fa-print"></i>\
							</a><a href="javascript:;" onclick=VoucherPayment_Module.UnApproval(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="فك الاعتماد">\
								<i class="fa fa-unlock-alt"></i></a>\
                                <a href = "/GeneralDailies/Index/?tranId='+ row.Id + '&tranTypeId=22" class="btn btn-sm btn-clean btn-icon"  title = "استعراض القيد" >\
								<i class="fa fa-search"></i>\
                                </a>\<a href="/VoucherBankPayments/Copy/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="نسخ">\
								<i class="fa fa-copy"></i>\
							</a>\</div>\							';
                        } else {
                            return '\
							<div class="btn-group">\<a href="/VoucherBankPayments/Edit/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon"  title="تعديل">\
								<i class="fa fa-edit"></i>\
							</a>\
							<a href="javascript:;" onclick=VoucherPayment_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a><a href="javascript:;" onclick=VoucherPayment_Module.Approval(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="اعتماد">\
								<i class="fa fa-check"></i>\
                                </a>\<a href="/VoucherBankPayments/Copy/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="نسخ">\
								<i class="fa fa-copy"></i>\
							</a>\</div>\							';
                        }

                    },
                }

            ],
            drawCallback: function () {
                var html = ' <tr><th colspan ="6" style= "text-align:center" ><div class="row alert-success"><label>اجمالى المبلغ : ';
                var api = this.api();
                var totalAmount = api.column(2).data().sum();
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
                var url = '/VoucherBankPayments/Delete';
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
                var url = '/VoucherPayments/Approval';
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
                var url = '/VoucherPayments/UnApproval';
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

    //#endregion
    //#region complex voucher 
    var initDetailsDT = function () {
        var table = $('#kt_datatableVoucherDetails');
        table.DataTable({
            paging: false,
            dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",
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
                url: '/VoucherBankPayments/GetDSVoucherTransaction',
                type: 'GET',

            },
            columns: [
                { data: 'DebitAmount', title: 'مدين' },
                { data: 'CreditAmount', title: 'دائن' },
                { data: 'AccountTreeNameDT', title: 'الحساب' },
                { data: 'AccountTreeNumDT', title: 'رقم الحساب' },
                { data: 'Notes', title: 'البيان' },
                { data: 'Actions', responsivePriority: -1 },

            ],

            columnDefs: [

                {
                    targets: -1,
                    title: 'عمليات',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return '\
							<div class="btn-group">\
							<a href="javascript:;" onclick=VoucherPayment_Module.deleteRowComplex('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
								<i class="fas fa-trash"></i>\
							</a></div>\
						';
                    },
                }

            ],
            drawCallback: function () {
                var html = ' <tr><th colspan ="6" style= "text-align:center" ><div class="row alert alert-success"><label>اجمالى المدين : ';
                var api = this.api();
                var balanceStatusTxt = '';
                var debit = api.column(0).data().sum();

                $(api.table().footer()).html(html + debit + '</label></div></th>  </tr>');
            },

            "order": [[0, "asc"]]
            //"order": [[0, "desc"]]

        });

    };

    function AddVoucherTransaction() {
        try {
            var AccountTreeId = document.getElementById('SelectedAccountTreeId').value;
            var Amount = document.getElementById('InsertedAmount').value;
            var Notes = document.getElementById('InsertedNotes').value;
            var formData = new FormData();
            if (AccountTreeId === '') {
                toastr.error('تأكد من اختيار الحساب', '');
                return false;
            }
            if (Amount === '' || Amount == '0') {
                toastr.error('تأكد من ادخال المبلغ', '');
                return false;

            }

            formData.append('accountTreeTxtId', AccountTreeId)
            formData.append('amountTxt', Amount)
            formData.append('notes', Notes)
            //var dataSet = $('#kt_datatableTreePrice').rows().data();
            var dataSet = $('#kt_datatableVoucherDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/VoucherBankPayments/AddVoucherTransaction',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_datatableVoucherDetails').DataTable().ajax.reload();
                        $('#SelectedAccountTreeId').val(null);
                        $('#accountTreeFrom').val('');
                        $('#InsertedNotes').val('');
                        $('#InsertedAmount').val(0);
                        toastr.success('تم الاضافة بنجاح', '');

                    } else
                        toastr.error(res.message, '');
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

    function deleteRowComplex(id) {
        $('#kt_datatableVoucherDetails tbody').on('click', 'a.deleteIcon', function () {
            $('#kt_datatableVoucherDetails').DataTable().row($(this).parents('tr')).remove().draw();
        })

    };

    function SubmitForm(btn, isApproval) {
        try {
            var form = document.getElementById('form1');
            var formData = new FormData(form);
            var dataSet = $('#kt_datatableVoucherDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            };
            formData.append("isApproval", isApproval);
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
                        if (!res.isInsert) {
                            setTimeout(function () { window.location = "/VoucherBankPayments/Index" }, 3000);
                        } else
                            setTimeout(function () { window.location = "/VoucherBankPayments/CreateEdit" }, 3000);
                    } else {
                        toastr.error(res.message, '');
                    }
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

    function getBalanceAccountChanged() {  // get balance  by accountid
        $.get("/SharedDataSources/GetAccountBalance", { id: $("#AccountTreeId").val(), isSafe: false }, function (data) {
            $("#AccountBalance").val(data);

        });
    };

    return {
        //main function to initiate the module
        initDTVoucherPayment: function () {
            initDTVoucherPayment();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        Approval: Approval,
        UnApproval: UnApproval,
        getBalanceAccountChanged: getBalanceAccountChanged,
        initDetailsDT: initDetailsDT,
        AddVoucherTransaction: AddVoucherTransaction,
        deleteRowComplex: deleteRowComplex
    };

}();


