"use strict";

var ItemProduction_Module = function () {
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
                        return 'توليف منتج';
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
                    filename: "توليف منتج",
                    title: "توليف منتج",
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
                url: '/ItemProductions/GetAll',
                type: 'GET',

                    },
        columns: [
            { data: 'Num',responsivePriority:0 },
            //{ data: 'ItemFinalName', title: 'الصنف' },
            { data: 'ItemProductionName', title: 'مسمى التوليفة' },
            //{ data: 'ItemTypeId', title: 'نوع الصنف',visible:false },
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
							<div class="btn-group">\<a href="/ItemProductions/ShowItemProduction/?itemProId='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض تفاصيل التوليفة">\
								<i class="fa fa-search"></i>\
							</a>\<a href="javascript:;" onclick=ItemProduction_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
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
            var dataSetItem = $('#kt_dtProductionItems').DataTable().rows().data().toArray();
            if (dataSetItem != null) {
                if (dataSetItem.length > 0) {
                    formData.append("DT_DatasourceItems", JSON.stringify(dataSetItem));
                }
            }
            var dataSet = $('#kt_dtProductionDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_DatasourceDetails", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: form.action,
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $(btn).attr('disabled', 'disabled'); // disabled button after one clicke 
                        toastr.success(res.message, '',)
                            setTimeout(function () { window.location = "/ItemProductions/CreateEdit" }, 3000);
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
                var url = '/ItemProductions/Delete';
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
    //#region Production Item
    var initDTItems = function () {
        var table = $('#kt_dtProductionItems');

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
                url: '/ItemProductions/GetDSItemProItems',
                type: 'GET',

            },
            columns: [
                { data: 'ItemId', visible: false },
                { data: 'UnitName', title: 'الوحدة' },
                { data: 'ItemName', title: 'الصنف' },
                { data: 'Quantity', title: 'الكمية' },
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
							<a href="javascript:;" onclick=ItemProduction_Module.deleteRowProductionItems(\''+ row.Id + '\')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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
    function addProductionItems() {
        try {
            var itemId = document.getElementById('ItemInOutId').value;
            var formData = new FormData();
            if (itemId === '') {
                toastr.error('تأكد من اختيار الصنف', '');
                return false;
            }
            var quantityInOut = document.getElementById('QuantityInOut').value;
            if (quantityInOut === ''||quantityInOut==='0') {
                toastr.error('تأكد من ادخال الكمية', '');
                return false;
            }

            formData.append('ItemInOutId', itemId)
            formData.append('QuantityInOut', quantityInOut)
            var dataSet = $('#kt_dtProductionItems').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_DatasourceItem", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/ItemProductions/addProductionItems',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtProductionItems').DataTable().ajax.reload();
                        $('#ItemInOutId').val(null);
                        $('#ItemInOutId').select2({
                            placeholder: "اختر عنصر من القائمة"
                        });
                        toastr.success(res.msg, '');

                    } else
                        toastr.error(res.msg, '');
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
    function deleteRowProductionItems(id) {
        $('#kt_dtProductionItems tbody').on('click', 'a.deleteIcon', function () {
            $('#kt_dtProductionItems').DataTable().row($(this).parents('tr')).remove().draw();
        })
    };
    //#endregion

    //#region Production Detailas
    var initDTProductionDetails = function () {
        var table = $('#kt_dtProductionDetails');

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
                url: '/ItemProductions/GetDSItemProDetails',
                type: 'GET',

            },
            columns: [
                { data: 'Id', title: 'م', visible: false },
                { data: 'ItemId', visible: false },
                { data: 'UnitName', title: 'الوحدة' },
                { data: 'ItemName', title: 'الصنف' },
                { data: 'Quantity', title: 'الكمية' },
                { data: 'Actions', responsivePriority: -1 },

            ],
            columnDefs: [
                //{
                //    targets: 1,
                //    title: 'م',
                //    orderable: false,
                //    render: function (data, type, row, meta) {
                //        return  meta.row + meta.settings._iDisplayStart + 1;
                //    },
                //},
                {
                    targets: -1,
                    title: 'عمليات',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return '\
							<div class="btn-group">\
							<a href="javascript:;" onclick=ItemProduction_Module.deleteRowProductionDetails(\''+ row.Id + '\')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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
    function addProductionDetails() {
        try {
            var itemId = document.getElementById('ItemId').value;
            var quantity = document.getElementById('Quantity').value;
            var formData = new FormData();
            if (itemId === '') {
                toastr.error('تأكد من اختيار الصنف', '');
                return false;
            }
            if (quantity === '' || quantity == '0') {
                toastr.error('تأكد من ادخال الكمية', '');
                return false;

            }

            formData.append('itemId', itemId)
            formData.append('quantity', quantity)
            var dataSet = $('#kt_dtProductionDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/ItemProductions/AddProductionDetails',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtProductionDetails').DataTable().ajax.reload();
                        $('#ItemId').val(null);
                        $('#ItemId').select2({
                            placeholder: "اختر عنصر من القائمة"
                        });
                        $('#Quantity').val(0);
                        toastr.success(res.msg, '');

                    } else
                        toastr.error(res.msg, '');
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

    function deleteRowProductionDetails(id) {
        $('#kt_dtProductionDetails tbody').on('click', 'a.deleteIcon', function () {
            $('#kt_dtProductionDetails').DataTable().row($(this).parents('tr')).remove().draw();
        })
    };
    //#endregion





    function onItemTypeChange(typ) {
        if (typ == 1) {
            $.get("/SharedDataSources/OnItemTypeChange", { id: $("#ItemtypeId").val() }, function (data) {
                $("#ItemInOutId").empty();
                $("#ItemInOutId").append("<option value=>اختر عنصر من القائمة</option>");
                $.each(data, function (index, row) {
                    $("#ItemInOutId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
                });
            });
        } else {
            $.get("/SharedDataSources/OnItemTypeChange", { id: $("#ItemtypeIdDetails").val() }, function (data) {
                $("#ItemId").empty();
                $("#ItemId").append("<option value=>اختر عنصر من القائمة</option>");
                $.each(data, function (index, row) {
                    $("#ItemId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
                });
            });
        }

    };
   
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        initProductionDetails: function () {
            initDTProductionDetails();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        addProductionDetails: addProductionDetails,
        deleteRowProductionDetails: deleteRowProductionDetails,
        onItemTypeChange: onItemTypeChange,
        initDTItems: initDTItems,
        addProductionItems: addProductionItems,
        deleteRowProductionItems: deleteRowProductionItems,
    };

}();


//$(document).ready(function () {
//    ItemProduction_Module.init();
//    $('#kt_datatable_search_query').on('keyup', function () {
//        $('#kt_datatable').DataTable().search(this.value).draw();
//    });
//    $('#ItemProductionGroupId').on('change', function () {
//        var textSelected = $("#ItemProductionGroupId option:selected").text();
//        $('#kt_datatable').DataTable().search(textSelected).draw();
//    });
//    $('#ItemProductiontypeId').on('change', function () {
//        var textSelected = $("#ItemProductiontypeId option:selected").text();
//        $('#kt_datatable').DataTable().search(textSelected).draw();
//    });
//});