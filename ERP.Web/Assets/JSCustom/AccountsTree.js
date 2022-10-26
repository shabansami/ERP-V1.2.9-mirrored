"use strict";

var AccountsTree_Module = function () {
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
                        return 'شجرة الحسابات';
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
                    filename: "شجرة الحسابات",
                    title: "شجرة الحسابات",
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
            },

            ajax: {
                url: '/AccountsTrees/GetAll',
                type: 'GET',

                    },
        columns: [
            { data: 'Num', responsivePriority: 0 },
            { data: 'ParentName', title: 'الحساب الرئيسى' },
            { data: 'AccountNumber', title: 'رقم الحساب' },
            { data: 'AccountName', title: 'اسم الحساب' },
            { data: 'TypeName', title: 'نوع الحساب' },
            { data: 'ParentId', title: 'م', visible: false },
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
                                return '\
							<div class="btn-group">\
							<a href="/AccountsTrees/Edit/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
								<i class="la la-edit"></i>\
							</a>\
							<a href="javascript:;" onclick=AccountsTree_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="la la-trash"></i>\
							</a></div>\
						';
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
        //حل مشكلة رجوع المجموعة الرئيسية بنل فى حالة اختيار مجموعة فرعية ثم الرجوع ومسحها لجعلها مجموعة رئيسية
        if ($('#parent').val() === '' || $('#parent').val() === null) {
            $('#ParentId').val(null);
        }

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
                            setTimeout(function () { window.location = "/AccountsTrees/Index" }, 3000);
                        } else
                            setTimeout(function () { window.location = "/AccountsTrees/CreateEdit" }, 3000);
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
                var url = '/AccountsTrees/Delete';
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

//    var initDTPricies = function () {
//        var table = $('#kt_datatableTreePrice');

//        // begin first table
//        table.DataTable({
//            responsive: true,
//            searchDelay: 500,
//            processing: true,
//            serverSide: false,
//            select: true,

//            // DOM Layout settings
//            dom: "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>",
//            language: {
//                search: "البحث",
//                lengthMenu: "عرض _MENU_ عنصر لكل صفحة",
//                info: "العناصر من_START_ الي _END_ من اصل _TOTAL_ عنصر",
//                processing: "جارى التحميل",
//                zeroRecords: "لا يوجد سجلات لعرضها",
//                infoFiltered: "",
//                infoEmpty: 'لا يوجد سجلات متاحه',
//            },

//            ajax: {
//                url: '/AccountsTrees/GetDSPricingSell',
//                type: 'GET',

//                    },
//        columns: [
//            { data: 'Id', title: 'م', visible: false },
//            { data: 'PricingPolicyId', visible: false },
//            { data: 'CustomerId', visible: false },
//            //{ data: 'Num',responsivePriority:1 },
//            { data: 'PricingPolicyName', title: 'سياسة التسعير'},
//            { data: 'SellPricePolicy', title: 'سعر البيع'},
//            { data: 'CustomerName', title: 'اسم العميل'},
//            { data: 'Actions', responsivePriority: -1},

//        ],
//            columnDefs: [
//                //{
//                //    targets: 1,
//                //    title: 'م',
//                //    orderable: false,
//                //    render: function (data, type, row, meta) {
//                //        return  meta.row + meta.settings._iDisplayStart + 1;
//                //    },
//                //},
//            {
//                targets: -1,
//                title: 'عمليات',
//                orderable: false,
//                render: function (data, type, row, meta) {
//                                return '\
//							<div class="btn-group">\
//							<a href="javascript:;" onclick=AccountsTree_Module.deleteRowTreePrice('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
//								<i class="la la-trash"></i>\
//							</a></div>\
//						';
//},
//                        }

//                    ],

//"order": [[0, "asc"]]
////"order": [[0, "desc"]] 

//        });
//            };

//    function SubmitFormTreePrice() {
//        try {
//            var pricingPolicyId = document.getElementById('PricingPolicyId').value;
//            var customerId = document.getElementById('CustomerId').value;
//            var sellPricePolicy = document.getElementById('SellPricePolicy').value;
//            var formData = new FormData();
//            if (pricingPolicyId==='') {
//                toastr.error('تأكد من اختيار سياسة السعر', '');
//                return false;
//            }
//            if (sellPricePolicy === ''||sellPricePolicy=='0') {
//                toastr.error('تأكد من ادخال سعر البيع', '');
//                return false;

//            }

//            formData.append('pricingPolicyId', pricingPolicyId)
//            formData.append('customerId', customerId)
//            formData.append('sellPricePolicy', sellPricePolicy)
//            //var dataSet = $('#kt_datatableTreePrice').rows().data();
//            var dataSet = $('#kt_datatableTreePrice').DataTable().rows().data().toArray();
//            if (dataSet != null) {
//                if (dataSet.length > 0) {
//                    formData.append("DT_Datasource", JSON.stringify(dataSet));
//                }
//            }
//            $.ajax({
//                type: 'POST',
//                url: '/AccountsTrees/AddSellPricies',
//                data: formData,
//                contentType: false,
//                processData: false,
//                success: function (res) {
//                    if (res.isValid) {
//                    $('#kt_datatableTreePrice').DataTable().ajax.reload();
//                    $('#PricingPolicyId').val('');
//                    $('#customerId').val('');
//                    $('#sellPricePolicy').val(0);
//                    } else
//                        toastr.error('سياسة الاسعار المدخلة موجودة بالفعل مسبقا', '');
//                    return false;
//                },
//                error: function (err) {
//                    toastr.error('حدث خطأ اثناء تنفيذ العملية', '');
//                    console.log(err)
//                }
//            })
//            //to prevent default form submit event
//            return false;
//            //$('#kt_datatableTreePrice').DataTable().ajax.reload();
//            //to prevent default form submit event
//            return false;
//        } catch (ex) {
//            console.log(ex)
//        }

//    }

//    function deleteRowTreePrice(id) {
//        $('#kt_datatableTreePrice tbody').on('click', 'a.deleteIcon', function () {
//            $('#kt_datatableTreePrice').DataTable().row($(this).parents('tr')).remove().draw();
//        })
//        //Swal.fire({
//        //    title: 'تأكيد الحذف',
//        //    text: 'هل متأكد من الحدف ؟',
//        //    icon: 'warning',
//        //    showCancelButton: true,
//        //    animation: true,
//        //    confirmButtonText: 'تأكيد',
//        //    cancelButtonText: 'إلغاء الامر'
//        //}).then((result) => {
//        //    if (result.value) {
//        //        alert(111);
//        //        $('#kt_datatableTreePrice tbody').on('click', 'a.deleteIcon', function () {
//        //            $('#kt_datatableTreePrice').DataTable().row($(this).parents('tr')).remove().draw();
//        //        })
//        //        //$('#kt_datatableTreePrice').on('click', 'tbody tr ', function () {
//        //        //    $('#kt_datatableTreePrice').DataTable().row($(this).parents('tr')).remove().draw();
//        //        //})
//        //        //$('#kt_datatableTreePrice').DataTable().ajax.reload();
//        //        //var url = '/AccountsTrees/Delete';
//        //        //$.ajax({
//        //        //    type: "POST",
//        //        //    url: url,
//        //        //    data: {
//        //        //        "id": id
//        //        //    },
//        //        //    //async: true,
//        //        //    //headers: { 'RequestVerificationToken': $('@Html.AntiForgeryToken()').val() },
//        //        //    success: function (data) {
//        //        //        if (data.isValid) {
//        //        //            toastr.success(data.message, '');
//        //        //            $('#kt_datatable').DataTable().ajax.reload();
//        //        //        } else {
//        //        //            toastr.error(data.message, '');
//        //        //        }
//        //        //    },
//        //        //    error: function (err) {
//        //        //        alert(err);
//        //        //    }
//        //        //});
//        //    }
//        //});
//    };


   
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        //initPricies: function () {
        //    initDTPricies();
        //},
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        //SubmitFormTreePrice: SubmitFormTreePrice,
        //deleteRowTreePrice: deleteRowTreePrice
    };

}();


//$(document).ready(function () {
//    AccountsTree_Module.init();
//    $('#kt_datatable_search_query').on('keyup', function () {
//        $('#kt_datatable').DataTable().search(this.value).draw();
//    });
//    $('#AccountsTreeGroupId').on('change', function () {
//        var textSelected = $("#AccountsTreeGroupId option:selected").text();
//        $('#kt_datatable').DataTable().search(textSelected).draw();
//    });
//    $('#AccountsTreetypeId').on('change', function () {
//        var textSelected = $("#AccountsTreetypeId option:selected").text();
//        $('#kt_datatable').DataTable().search(textSelected).draw();
//    });
//});