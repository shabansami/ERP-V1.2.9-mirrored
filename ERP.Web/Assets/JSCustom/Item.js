"use strict";

var Item_Module = function () {
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
                        return 'الاصناف';
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
                    filename: "الاصناف",
                    title: "الاصناف",
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
                url: '/Items/GetAll',
                type: 'GET',

                    },
        columns: [
            { data: 'Num',responsivePriority:0 },
            { data: 'GroupBasicName', title: 'المجموعة الاساسية' },
            { data: 'ItemTypeName', title: 'النوع' },
            { data: 'Name', title: 'اسم الصنف'},
            { data: 'SellPrice', title: 'سعر البيع'},
            { data: 'LastPurchasePrice', title: 'اخر سعر شراء'},
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
							<a href="/Items/Edit/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
								<i class="fa fa-edit"></i>\
							</a>\
							<a href="javascript:;" onclick=Item_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
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
        try {
            var form = document.getElementById('form1');
            var formData = new FormData(form);
            var dataSet = $('#kt_datatableTreePrice').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            };
            var dataSetUnits = $('#kt_datatableTreeUnits').DataTable().rows().data().toArray();
            if (dataSetUnits != null) {
                if (dataSetUnits.length > 0) {
                    formData.append("DT_DatasourceUnits", JSON.stringify(dataSetUnits));
                }
            };

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
                            setTimeout(function () { window.location = "/Items/Index" }, 3000);
                        } else
                            setTimeout(function () { window.location = "/Items/CreateEdit" }, 3000);
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
                var url = '/Items/Delete';
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

//#region Sell Pricing
    var initDTPricies = function () {
        var table = $('#kt_datatableTreePrice');

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
            },

            ajax: {
                url: '/Items/GetDSPricingSell',
                type: 'GET',

            },
            columns: [
                //{ data: 'Id', title: 'م', visible: false },
                //{ data: 'PricingPolicyId', visible: false },
                //{ data: 'CustomerId', visible: false },
                { data: 'Num',responsivePriority:0 },
                { data: 'PricingPolicyName', title: 'سياسة التسعير' },
                { data: 'SellPricePolicy', title: 'سعر البيع' },
                { data: 'CustomerName', title: 'اسم العميل', visible: false },
                { data: 'Actions', responsivePriority: -1 },

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
							<a href="javascript:;" onclick=Item_Module.deleteRowTreePrice(\''+ row.Id + '\')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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

    function SubmitFormTreePrice() {
        try {
            var pricingPolicyId = document.getElementById('PricingPolicyId').value;
            var customerId = document.getElementById('CustomerId').value;
            var sellPricePolicy = document.getElementById('SellPricePolicy').value;
            var formData = new FormData();
            if (pricingPolicyId === '') {
                toastr.error('تأكد من اختيار سياسة السعر', '');
                return false;
            }
            if (sellPricePolicy === '' || sellPricePolicy == '0') {
                toastr.error('تأكد من ادخال سعر البيع', '');
                return false;

            }

            formData.append('pricingPolicyId', pricingPolicyId)
            formData.append('customerId', customerId)
            formData.append('sellPricePolicy', sellPricePolicy)
            //var dataSet = $('#kt_datatableTreePrice').rows().data();
            var dataSet = $('#kt_datatableTreePrice').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/Items/AddSellPricies',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_datatableTreePrice').DataTable().ajax.reload();
                        $('#PricingPolicyId').val('');
                        $('#CustomerId').val(null);
                        $('#SellPricePolicy').val(0);
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

    function deleteRowTreePrice(id) {
        $('#kt_datatableTreePrice tbody').on('click', 'a.deleteIcon', function () {
            $('#kt_datatableTreePrice').DataTable().row($(this).parents('tr')).remove().draw();
        })

    };
    //#endregion 
//#region Item Unit
    var initDTUnits = function () {
        var table = $('#kt_datatableTreeUnits');

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
            },

            ajax: {
                url: '/Items/GetDSItemUnit',
                type: 'GET',

            },
            columns: [
                //{ data: 'Id', title: 'م', visible: false },
                { data: 'Num', responsivePriority: 0 },
                { data: 'UnitNewId', visible: false },
                { data: 'UnitName', title: 'الوحدة' },
                { data: 'UnitSellPrice', title: 'سعر الوحدة' },
                { data: 'Quantity', title: 'الكمية' },
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
                        return '\
							<div class="btn-group">\
							<a href="javascript:;" onclick=Item_Module.deleteRowTreeUnit(\''+ row.Id + '\')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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

    function SubmitFormTreeUnits() {
        try {
            var unitBaseId = document.getElementById('UnitId').value;
            var unitNewId = document.getElementById('UnitNewId').value;
            var quantity = document.getElementById('Quantity').value;
            var unitSellPrice = document.getElementById('UnitSellPrice').value;
            var formData = new FormData();
            if (unitBaseId === '' || unitBaseId == '0') {
                toastr.error('تأكد من اختيار الوحدة الاساسية للصنف اولا', '');
                return false;
            }
            if (unitNewId === '' || unitNewId == '0') {
                toastr.error('تأكد من اختيار الوحدة', '');
                return false;
            }
            if (quantity === '' || quantity == '0') {
                toastr.error('تأكد من ادخال الكمية', '');
                return false;
            }
            if (unitSellPrice === '' || unitSellPrice == '0') {
                toastr.error('تأكد من ادخال سعر الوحدة', '');
                return false;
            }

            formData.append('unitBaseId', unitBaseId)
            formData.append('unitNewId', unitNewId)
            formData.append('quantity', quantity)
            formData.append('unitSellPrice', unitSellPrice)
            var dataSet = $('#kt_datatableTreeUnits').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/Items/AddItemUnit',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_datatableTreeUnits').DataTable().ajax.reload();
                        $('#UnitNewId').val('');
                        $('#Quantity').val(0);
                        $('#UnitSellPrice').val(0);
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

    function deleteRowTreeUnit(id) {
        $('#kt_datatableTreeUnits tbody').on('click', 'a.deleteIcon', function () {
            $('#kt_datatableTreeUnits').DataTable().row($(this).parents('tr')).remove().draw();
        })

    };
    //#endregion 


   
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        initPricies: function () {
            initDTPricies();
        },
        initUnits: function () {
            initDTUnits();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        SubmitFormTreePrice: SubmitFormTreePrice,
        deleteRowTreePrice: deleteRowTreePrice  ,      
        SubmitFormTreeUnits: SubmitFormTreeUnits,
        deleteRowTreeUnit: deleteRowTreeUnit
    };

}();


//$(document).ready(function () {
//    Item_Module.init();
//    $('#kt_datatable_search_query').on('keyup', function () {
//        $('#kt_datatable').DataTable().search(this.value).draw();
//    });
//    $('#ItemGroupId').on('change', function () {
//        var textSelected = $("#ItemGroupId option:selected").text();
//        $('#kt_datatable').DataTable().search(textSelected).draw();
//    });
//    $('#ItemtypeId').on('change', function () {
//        var textSelected = $("#ItemtypeId option:selected").text();
//        $('#kt_datatable').DataTable().search(textSelected).draw();
//    });
//});