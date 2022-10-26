"use strict";

var CustomerCheque_Module = function () {
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
                        return 'استلام شيك من عميل';
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
                    "extend": "excelHtml5",
                    "filename": "استلام شيك من عميل",
                    "title": "استلام شيك من عميل",
                    exportOptions: {
                        columns: ':visible'
                    }
                },
            ],            language: {
                search: "البحث",
                lengthMenu: "عرض _MENU_ عنصر لكل صفحة",
                info: "العناصر من_START_ الي _END_ من اصل _TOTAL_ عنصر",
                processing: "جارى التحميل",
                zeroRecords: "لا يوجد سجلات لعرضها",
                infoFiltered: "",
                infoEmpty: 'لا يوجد سجلات متاحه',
            },

            ajax: {
                url: '/CustomerCheques/GetAll',
                type: 'GET',
               

            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'CustomerName', title: 'العميل' },
                { data: 'CheckNumber', title: 'رقم الشيك' },
                { data: 'CheckDate', title: 'اصدار الشيك' },
                { data: 'BankAccountName', title: 'الحساب البنكى' },
                { data: 'CollectionDate', title: 'تاريخ التحصيل' },
                { data: 'Amount', title: 'المبلغ' },
                { data: 'CheckStatus', title: 'حالة الشيك' },
                //{ data: 'IsRefusedStatus', title: 'حالة الشيك' },
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
                        if (!row.IsCollected && !row.IsRefused) {
                            if (row.BankAccountName===null) {
                                return '\
							<div class="btn-group">\
							<a  href="/CustomerChequeCollections/PaymentBank/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon"  title="تحديد الحساب البنكى">\
								<i class="la la-bank"></i>\
							</a></div>\
						';
                            } else
                            return '\
							<div class="btn-group">\
							<a  href="/CustomerChequeCollections/Collection/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon"  title="تحصيل">\
								<i class="la la-money"></i>\
							</a>\
                                <a href="/CustomerChequeRefuses/Refuse/'+ row.Id + '" class="btn btn-sm btn-clean btn-icUrln" title="رفض/إرجاع الشيك">\
								<i class="la la-ban"></i>\
							</a><a href="/GeneralDailies/Index/?tranId='+ row.Id + '&tranTypeId=12" class="btn btn-sm btn-clean btn-icon" title="عرض القيود">\
								<i class="la la-money"></i>\
							</a>\</div>\
						';
                        }  else {
                            return '\
							<div class="btn-group">\
                            <span class="label label-lg font-weight-bold label-light-success label-inline">تم اعتمادها</span><a href="/GeneralDailies/Index/?tranId='+ row.Id + '&tranTypeId=12" class="btn btn-sm btn-clean btn-icon" title="عرض القيود">\
								<i class="la la-money"></i>\
							</a>\</div>\
						';
                        }
                    },
                }

            ],

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
                            setTimeout(function () { window.location = "/CustomerCheques/Index" }, 3000);
                        } else
                            setTimeout(function () { window.location = "/CustomerCheques/CreateEdit" }, 3000);
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
            text: 'هل متأكد من حذف الشيك ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/CustomerCheques/Delete';
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

    function onRdoCollectionChanged() {
        if ($("#rdo_collection:checked").val()) {
            $('#CollectionDate').removeAttr('disabled');
            $('#CheckDueDate').attr('disabled', 'disabled');
            $('#IsCollected').val(true);
        }
    }
    function onRdoUnderCollectionChanged() {
        if ($("#rdo_underCollection:checked").val()) {
            $('#CheckDueDate').removeAttr('disabled');
            $('#CollectionDate').attr('disabled', 'disabled');
            $('#IsCollected').val(false);
        }
    };

    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        onRdoCollectionChanged: onRdoCollectionChanged,
        onRdoUnderCollectionChanged: onRdoUnderCollectionChanged
    };

}();


