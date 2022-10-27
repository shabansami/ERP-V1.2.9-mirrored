"use strict";

var GeneralRecord_Module = function () {

    //#region index
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
                        return 'قيود حرة';
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
                    filename: "قيود حرة",
                    title: "قيود حرة",
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
                }
            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'AccountFrom', title: 'من حساب' },
                { data: 'AccountTo', title: 'إلي حساب' },
                { data: 'TransactionDate', title: 'تاريخ المعاملة' },
                { data: 'Amount', title: 'المبلغ' },
                { data: 'IsApprovalStatus', title: 'حالة الاعتماد' },
                { data: 'AccountTreeFromId', visible: false },
                { data: 'AccountTreeToId', visible: false },
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
								<i class="fa fa-check-square-o"></i>\</div>\
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
								<i class="fa fa-check-square-o"></i>\
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

    //#region add single general record
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
                            setTimeout(function () { window.location = "/GeneralRecords/Index" }, 3000);
                        } else
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
    function SubmitFormComplex(btn,isApproval) {
        try {
            var form = document.getElementById('form1');
            var formData = new FormData(form);
            var dataSet = $('#kt_datatableGenralComplex').DataTable().rows().data().toArray();
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
                        setTimeout(function () { window.location = "/GeneralRecordComplex/CreateEdit" }, 3000);
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


    function AddNewGeneralComplex() {
        try {
            var ComplexAccountTreeId = document.getElementById('ComplexAccountTreeId').value;
            var ComplexAmount = document.getElementById('ComplexAmount').value;
            var ComplexNotes = document.getElementById('ComplexNotes').value;
            var ComplexDebitCredit = document.getElementById('ComplexDebitCredit').value;
            var formData = new FormData();
            if (ComplexDebitCredit === '') {
                toastr.error('تأكد من اختيار حالة الحساب', '');
                return false;
            }
            if (ComplexAccountTreeId === '') {
                toastr.error('تأكد من اختيار الحساب', '');
                return false;
            }
            if (ComplexAmount === '' || ComplexAmount == '0') {
                toastr.error('تأكد من ادخال المبلغ', '');
                return false;

            }

            formData.append('ComplexAccountTreeId', ComplexAccountTreeId)
            formData.append('ComplexAmount', ComplexAmount)
            formData.append('ComplexNotes', ComplexNotes)
            formData.append('ComplexDebitCredit', ComplexDebitCredit)
            //var dataSet = $('#kt_datatableTreePrice').rows().data();
            var dataSet = $('#kt_datatableGenralComplex').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/GeneralRecordComplex/AddGenaralComplex',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_datatableGenralComplex').DataTable().ajax.reload();
                        $('#ComplexNotes').val('');
                        $('#ComplexAmount').val(0);
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
        $('#kt_datatableGenralComplex tbody').on('click', 'a.deleteIcon', function () {
            $('#kt_datatableGenralComplex').DataTable().row($(this).parents('tr')).remove().draw();
        })

    };

    //#endregion
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        approval: approval,
        Unapproval: Unapproval,
        SubmitFormComplex: SubmitFormComplex,
        AddNewGeneralComplex: AddNewGeneralComplex,
        deleteRowComplex: deleteRowComplex,
    };

}();


