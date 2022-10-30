"use strict";

var SellInvoiceInstallment_Module = function () {

    //#region ======== Index ==================
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
                        return 'فواتير التقسيط';
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
                    filename: "فواتير التقسيط",
                    title: "فواتير التقسيط",
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
                url: '/SellInvoiceInstallments/GetAll',
                type: 'GET',
                data(d) {
                    d.customerId = $("#CustomerId").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                }
                    },
        columns: [
            { data: 'Num',responsivePriority:0 },
            { data: 'InvoiceNum', title: 'رقم الفاتورة ' },
            { data: 'InvoiceDate', title: 'تاريخ الفاتورة' },
            { data: 'Safy', title: 'صافى الفاتورة' },
            { data: 'PayedValue', title: 'المدفوع' },
            { data: 'RemindValue', title: 'المتبقى' },
            { data: 'CustomerName', title: 'اسم العميل' },
            { data: 'PaymentTypeName', title: 'طريقة السداد' },
            { data: 'Actions', responsivePriority: -1, className: 'actions' },

        ],
            columnDefs: [
                {
                    targets: 0,
                    title: 'م',
                    render: function (data, type, row, meta) {
                        return  meta.row + meta.settings._iDisplayStart + 1;
                    },
                },
            {
                targets: -1,
                title: 'عمليات',
                orderable: false,
                render: function (data, type, row, meta) {
                    if (row.PaymentTypeId === 2 || row.PaymentTypeId === 3) { //اجل او جزئى (تغيير طريقة السداد الى تقسيط)
                        return '\
							<div class="btn-group">\
							<a href="javascript:void(0)" onclick="SellInvoiceInstallment_Module.ChangePaymentTypeToInstallment(\''+ row.InvoiceId + '\')" class="btn btn-sm btn-clean btn-icon" title="تغيير الى تقسيط">\
								<i class="fa fa-refresh"></i>\
							</a>\<a href="/CustomersPayments/CreateEdit/?invoGuid='+ row.InvoiceId + '" class="btn btn-sm btn-clean btn-icon" title="استلام نقدية من العميل">\
								<i class="fa fa-money-bill"></i>\
							</a>\
                            </div>\
						';
                    } else {
                        return '\
							<div class="btn-group">\
							<a href="/SellInvoiceInstallments/RegisterInstallments/?invoGuid='+ row.InvoiceId + '&typ=sell" class="btn btn-sm btn-clean btn-icon" title="انشاء/ادارة دفعات الفاتورة">\
								<i class="fa fa-money-bill"></i>\
							</a>\
                           </div>\
						';
                    }
                   
},
                        }

                    ],

"order": [[0, "desc"]]
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

  
    //#endregion =========  ==========
    //#region =========== Generate Payments
    var initDTItemDetails = function () {
        var table = $('#kt_dtItemDetails');

        // begin first table
        table.DataTable({
            paging: false,
            info: false,
            search: false,
            searching: false,
            "order": [[0, "asc"]]
            //"order": [[0, "desc"]] 

        });
    };

    function RegisterInstallments(isSaveByParam) { // تسجيل فاتورة

        //var data = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
        var values = new Array();
        $.each($(".selectedRow"), function () {
            var data = $(this).parents('tr:eq(0)');
            values.push({ 'Id': $(data).find('td:eq(0)').text(), 'SellInvoiceId': $(data).find('td:eq(1)').text(), 'InstallmentDate': $(data).find('td:eq(2) input[type="date"]').val(), 'Amount': $(data).find('td:eq(3) input[type="number"]').val() });
        });
        console.log(values);
        console.log(values.length);

        Swal.fire({
            title: 'تأكيد تسجيل الاقساط',
            text: 'هل متأكد من تسجيل اقساط الفاتورة ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                const params = new URLSearchParams(window.location.search)
                var queryString = '';
                if (params.has('invoGuid')) {
                    queryString = params.get('invoGuid');
                } else
                    window.location = "/SellInvoiceInstallments/Index";

                var url = '/SellInvoiceInstallments/RegisterInstallments';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "data": JSON.stringify(values),
                        "finalSafy": $("#FinalSafy").val(),
                        "IsSell": $("#IsSell").val(),
                    },
                    //async: true,
                    //headers: { 'RequestVerificationToken': $('@Html.AntiForgeryToken()').val() },
                    success: function (data) {
                        if (data.isValid) {
                            toastr.success(data.message, '');
                            if (isSaveByParam === 'upload') { //فى حالةالضغط على حفظ ورفع ملفات للفاتورة 
                                setTimeout(function () { window.location = "/UploadCenterTypeFiles/Index/?typ=" + data.typ + "&refGid=" + data.refGid }, 3000);
                            }  else {
                                setTimeout(function () { window.location = "/SellInvoiceHasInstallments/Index" }, 3000);
                                        }

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
    function SubmitForm(btn,isSell) {
        try {
            var form = document.getElementById('form1');
            $.ajax({
                type: 'POST',
                url: "/SellInvoiceInstallments/GeneratePyments",
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        //$(btn).attr('disabled', 'disabled'); // disabled button after one clicke 
                        $(btn).css('pointer-events', 'none'); // disabled a link after one clicke 
                        toastr.success(res.message, '',)
                        if (isSell)
                            setTimeout(function () { window.location = "/SellInvoiceInstallments/RegisterInstallments/?invoGuid=" + res.invoGuid + "&typ=sell&fi=0" }, 3000);//fi firist init false لمعالجة الداتا سورس ستاتيك
                        else
                            setTimeout(function () { window.location = "/SellInvoiceInstallments/RegisterInstallments/?invoGuid=" + res.SellInvoiceId + "&typ=initial&fi=0" }, 3000);

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
    function ChangePaymentTypeToInstallment(id) { //تغيير طريقة السداد الى تقسيط
               Swal.fire({
            title: 'تأكيد التغيير ',
            text: 'هل متأكد من تغيير طريقة السداد الى تقسيط ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/SellInvoiceInstallments/ChangePaymentTypeToInstallment';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "id": id,
                    },
                    //async: true,
                    //headers: { 'RequestVerificationToken': $('@Html.AntiForgeryToken()').val() },
                    success: function (data) {
                        if (data.isValid) {
                            toastr.success(data.message, '');
                            setTimeout(function () { window.location = "/SellInvoiceInstallments/RegisterInstallments/?invoGuid=" + id+"&typ=sell"}, 3000);
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

    function calcPayValue(param) {
        var RemindValue = Number.parseFloat($("#RemindValue").val());
        if (isNaN(RemindValue))
            RemindValue = 0;

        var FinalSafy = Number.parseFloat($("#FinalSafy").val());
        if (isNaN(FinalSafy))
            FinalSafy = 0;

        if (param === 'total' || $("#rdo_total:checked").val()) {
            var TotalValue = Number.parseFloat($("#TotalValue").val());
            if (isNaN(TotalValue))
                TotalValue = 0;
            FinalSafy = TotalValue;
        } else if (param === 'commission' || $("#rdo_commissionPerc:checked").val()) {
            FinalSafy = RemindValue;
            var CommissionPerc = Number.parseFloat($("#CommissionPerc").val());
            if (isNaN(CommissionPerc))
                CommissionPerc = 0;
            var CommissionVal = (FinalSafy * CommissionPerc) / 100;
            $("#CommissionVal").val(CommissionVal);
            FinalSafy = Math.round(FinalSafy + CommissionVal, 2);
        } 
        $("#ProfitValue").val(FinalSafy - RemindValue);


        var Duration = Number.parseFloat($("#Duration").val());
        if (isNaN(Duration))
            Duration = 0;

        $("#FinalSafy").val(FinalSafy);
        var payVal = Math.round((FinalSafy / Duration)*100)/100;
        $("#PayValue").val(payVal);
    }

    function onRdoTotalChanged() {
        if ($("#rdo_total:checked").val()) {
            $('#TotalValue').removeAttr('disabled');
            $('#CommissionPerc').attr('disabled', 'disabled');
            $('#CommissionPerc').val(0);
            clearTxt();

        }
    }
    function clearTxt() {
        $('#PayValue').val(0);
        $('#ProfitValue').val(0);
        $('#FinalSafy').val(0);
    }
    function onRdoCommissionPercChanged() {
        if ($("#rdo_commissionPerc:checked").val()) {
            $('#CommissionPerc').removeAttr('disabled');
            $('#TotalValue').attr('disabled', 'disabled');
            $('#TotalValue').val(0);
            clearTxt();
        }
    };
    //#endregion ==================
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        initItemDetails: function () {
            initDTItemDetails();
        },
        SubmitForm: SubmitForm,
        RegisterInstallments: RegisterInstallments,
        calcPayValue: calcPayValue,
        onRdoTotalChanged: onRdoTotalChanged,
        onRdoCommissionPercChanged: onRdoCommissionPercChanged,
        ChangePaymentTypeToInstallment: ChangePaymentTypeToInstallment,
    };

}();

