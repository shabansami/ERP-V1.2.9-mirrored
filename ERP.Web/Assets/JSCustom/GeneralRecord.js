"use strict";

var GeneralRecord_Module = function () {

    //#region index
    var initDT = function () {
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
                        return 'قيود يومية';
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
                    filename: "قيود يومية",
                    title: "قيود يومية",
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
                url: '/GeneralRecords/GetAll',
                type: 'GET',
                data(d) {
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.accountId = $("#SelectedAccountTreeId").val();
                }
            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'TransactionDate', title: 'تاريخ المعاملة' },
                { data: 'Amount', title: 'المبلغ' },
                { data: 'IsApprovalStatus', title: 'حالة الاعتماد' },
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
                            <span class="label label-lg font-weight-bold label-light-success label-inline">تم اعتمادها</span><a href="javascript:;" onclick=GeneralRecord_Module.Unapproval(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="فك الاعتماد">\
								<i class="fa fa-unlock-alt"></i></a>\
                                <a href = "/GeneralDailies/Index/?tranId='+ row.Id + '&tranTypeId=9" class="btn btn-sm btn-clean btn-icon"  title = "استعراض القيد" >\
								<i class="fa fa-search"></i>\
							</a>\</div>\
						';
                        } else {
                            return '\
							<div class="btn-group">\
                                <a href="/GeneralRecords/Edit/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon"  title="تعديل">\
								<i class="fa fa-edit"></i>\
							</a>\
							<a href="javascript:;" onclick=GeneralRecord_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a><a href="javascript:;" onclick=GeneralRecord_Module.approval(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="اعتماد">\
								<i class="fa fa-unlock-alt"></i>\
							</a></div>\
						';
                        }
                    },
                }

            ],

            drawCallback: function () {
                var html = ' <tr><th colspan ="6" style= "text-align:center" ><label>اجمالى المبلغ : ';
                var api = this.api();
                var totalAmount = api.column(2).data().sum();
                $(api.table().footer()).html(html + totalAmount + '</label></th>  </tr>');
            },
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
                var url = '/GeneralRecords/Delete';
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

    //#region Un/approval
    function approval(id) {
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
                var url = '/GeneralRecords/Approval';
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
    function Unapproval(id) {
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
                var url = '/GeneralRecords/UnApproval';
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

    //#endregion

    //#region complex general record
    var initDetailsDT = function () {
        var table = $('#kt_datatableGenralDetails');
        table.DataTable({
            paging:false,
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
                url: '/GeneralRecords/GetDSGenaralComplex',
                type: 'GET',

            },
            columns: [
                { data: 'DebitAmount', title: 'مدين' },
                { data: 'CreditAmount', title: 'دائن' },
                { data: 'AccountTreeName', title: 'الحساب' },
                { data: 'AccountTreeNum', title: 'رقم الحساب' },
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
							<a href="javascript:;" onclick=GeneralRecord_Module.deleteRowComplex('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
								<i class="fas fa-trash"></i>\
							</a></div>\
						';
                    },
                }

            ],
            drawCallback: function () {
                var html = ' <tr><th colspan ="2" style= "text-align:center" ><div class="row alert alert-success"><label>اجمالى المدين : ';
                var api = this.api();
                var balanceStatusTxt = '';
                var debit = api.column(0).data().sum();
                var credit = api.column(1).data().sum();
                var balance = Number.parseFloat(debit) - Number.parseFloat(credit);
                if (balance > 0) {
                    balanceStatusTxt = 'الرصيد مدين : ' + balance;
                } else
                    if (balance < 0) {
                        balanceStatusTxt = 'الرصيد دائن : ' + balance;
                    } else {
                        balanceStatusTxt = 'الرصيد : 0';
                    }
                $(api.table().footer()).html(html + debit + '</label></div></th> <th colspan ="2" style= "text-align:center" ><div class="row alert alert-success"><label>' + balanceStatusTxt + '</label ></div ></th ><th colspan ="3" style= "text-align:center" ><div class="row alert alert-success"><label>اجمالى الدائن :' + credit + '</label ></div ></th > </tr>');
            },

            "order": [[0, "asc"]]
            //"order": [[0, "desc"]]

        });

    };

    function AddNewGeneralComplex() {
        try {
            var AccountTreeId = document.getElementById('SelectedAccountTreeId').value;
            var Amount = document.getElementById('InsertedAmount').value;
            var Notes = document.getElementById('InsertedNotes').value;
            var DebitCredit = document.getElementById('DebitCredit').value;
            var formData = new FormData();
            if (DebitCredit === '') {
                toastr.error('تأكد من اختيار حالة الحساب', '');
                return false;
            }
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
            formData.append('debitCredit', DebitCredit)
            //var dataSet = $('#kt_datatableTreePrice').rows().data();
            var dataSet = $('#kt_datatableGenralDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/GeneralRecords/AddGenaralComplex',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_datatableGenralDetails').DataTable().ajax.reload();
                        $('#SelectedAccountTreeId').val(null);
                        $('#accountId').val('');
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
        $('#kt_datatableGenralDetails tbody').on('click', 'a.deleteIcon', function () {
            $('#kt_datatableGenralDetails').DataTable().row($(this).parents('tr')).remove().draw();
        })

    };

    function SubmitForm(btn, isApproval) {
        try {
            var form = document.getElementById('form1');
            var formData = new FormData(form);
            var dataSet = $('#kt_datatableGenralDetails').DataTable().rows().data().toArray();
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
                        setTimeout(function () { window.location = "/GeneralRecords/CreateEdit" }, 3000);
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

    };

    //#endregion
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        initDetailsDT: function () {
            initDetailsDT();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        approval: approval,
        Unapproval: Unapproval,
        AddNewGeneralComplex: AddNewGeneralComplex,
        deleteRowComplex: deleteRowComplex,
    };

}();


