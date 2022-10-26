"use strict";
var Maintenance_Module = function () {

    //#region ======== Save Sell back invoice ==================
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
                        return 'فواتير الصيانة';
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
                    filename: "فواتير الصيانة",
                    title: "فواتير الصيانة",
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
                url: '/Maintenances/GetAll',
                type: 'GET',

            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'InvoiceNum', title: 'رقم الفاتورة' },
                { data: 'InvoiceDate', title: 'تاريخ الفاتورة' },
                { data: 'CustomerName', title: 'اسم العميل' },
                { data: 'CaseName', title: 'اخر حالة للفاتورة' },
                { data: 'InvoType', title: 'حالة الفاتورة', visible: false },
                { data: 'IsApprovalAccountant', visible: false },
                { data: 'IsFinalApproval', visible: false },
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
                    //         		< a href = "/Maintenances/Edit/?invoGuid='+ row.InvoiceGuid + '" class= "btn btn-sm btn-clean btn-icon" title = "تعديل" >\
                    //         <i class="la la-edit"></i>\
                    //</a >
                    targets: -1,
                    title: 'عمليات',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        if (row.IsApprovalAccountant || row.IsFinalApproval) {
                            return '\
							<div class="btn-group">\
					        <a href="/Maintenances/ShowHistory/?invoGuid='+ row.InvoiceGuid + '" class="btn btn-sm btn-clean btn-icon" title="عرض الحالات">\
								<i class="la la-sliders"></i>\
							</a>\<a href="/Maintenances/ShowMaintenance/?invoGuid='+ row.InvoiceGuid + '" class="btn btn-sm btn-clean btn-icon" title="استعراض بيانات الفاتورة">\
								<i class="la la-print"></i>\
							</a>\</div>\
						';
                        } else {
                            return '\
							<div class="btn-group">\
					        <a href="/MaintenanceItems/Index/?invoGuid='+ row.InvoiceGuid + '" class="btn btn-sm btn-clean btn-icon" title="البدأ فى صيانة اصناف الفاتورة">\
								<i class="la la-search"></i>\
							</a>\
							<a href="javascript:;" onclick=Maintenance_Module.deleteRow("'+ row.InvoiceGuid + '") class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="la la-trash"></i>\
							</a><a href="/Maintenances/ShowHistory/?invoGuid='+ row.InvoiceGuid + '" class="btn btn-sm btn-clean btn-icon" title="عرض الحالات">\
								<i class="la la-sliders"></i>\
							</a>\<a href="/Maintenances/ShowMaintenance/?invoGuid='+ row.InvoiceGuid + '" class="btn btn-sm btn-clean btn-icon" title="استعراض بيانات الفاتورة">\
								<i class="la la-print"></i>\
							</a>\</div>\
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

    function SubmitForm(btn, isSaveUpload) {
        try {
            var form = document.getElementById('kt_form');
            var formData = new FormData(form);
            var dataSet = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_DatasourceItems", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: formData.action,
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $(btn).attr('disabled', 'disabled'); // disabled button after one clicke 
                        toastr.success(res.message, '',)
                        if (!res.isInsert) {
                            setTimeout(function () { window.location = "/Maintenances/Index" }, 3000);
                        } else
                            setTimeout(function () { window.location = "/Maintenances/CreateEdit" }, 3000);



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

    function deleteRow(invoGuid) {
        Swal.fire({
            title: 'تأكيد الحذف',
            text: 'هل متأكد من الحذف .. سيتم حذف كل البيانات المرتبطة بالفاتورة ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/Maintenances/Delete';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "invoGuid": invoGuid
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

    //#endregion ============== end ==============

    //#region ========== Step 1 البيانات الاساسية===============

    function onRdoBarcodeChanged() {
        if ($("#rdo_barcode:checked").val()) {
            $("#barcodeDiv").show();
            $('#ItemBarcode').removeAttr('disabled');
            $('#ItemSerial').attr('disabled', 'disabled');
            $("#serialDiv").hide();

            $("#rdo_barcode:checked").prop('cheched', true);
            //$("#Quantity").val(0);
            //$("#Quantity").removeAttr('readonly');

        }
    }
    function onRdoSerialChanged() {
        if ($("#rdo_serial:checked").val()) {
            $("#barcodeDiv").hide();
            $('#ItemSerial').removeAttr('disabled');
            $('#ItemBarcode').attr('disabled', 'disabled');
            $("#serialDiv").show();
            //    $("#Quantity").val(1);
            //    $("#Quantity").attr('readonly','readonly');
        }
    };


    function getSafesOnBranchChanged() { // get safes and stores by branchId
        $.get("/SharedDataSources/getStoresOnBranchChanged", { id: $("#BranchId").val() }, function (data) {
            $("#StoreId").empty();
            $("#StoreId").append("<option value=>اختر عنصر من القائمة </option>");
            $.each(data, function (index, row) {
                $("#StoreId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            })
        });
    };
    //#endregion ========== end Step 1 ============

    //#region ======== Step 2 تسجيل الاصناف ف الفاتورة=================
    var initDTItemDetails = function () {
        var table = $('#kt_dtItemDetails');

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
                url: '/Maintenances/GetDSItemDetails',
                type: 'GET',

            },
            columns: [
                { data: 'Id', title: 'م', visible: false },
                { data: 'ItemId', visible: false },
                { data: 'SerialItemId', visible: false },
                { data: 'IsIntial', visible: false },
                { data: 'ItemName', title: 'الصنف' },
                { data: 'Quantity', title: 'الكمية' },
                { data: 'ProductionOrderId', title: 'امر الانتاج' },
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
							<a href="javascript:;" onclick=Maintenance_Module.deleteRowItemDetails('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
								<i class="la la-trash"></i>\
							</a></div>\
						';
                    },
                }

            ],

            "order": [[0, "asc"]]
            //"order": [[0, "desc"]] 

        });
    };

    function addItemDetails() {
        try {
            var itemId = document.getElementById('ItemId').value;
            var quantity = document.getElementById('Quantity').value;
            var serialItemId = document.getElementById('serialItemId').value;
            var productionOrderId = document.getElementById('productionOrderId').value;
            var isIntial = document.getElementById('isIntial').value;
            var formData = new FormData();
            if (itemId === '') {
                toastr.error('تأكد من اختيار الصنف', '');
                return false;
            };
            if (quantity === '' || quantity == '0') {
                toastr.error('تأكد من ادخال الكمية', '');
                return false;
            };

            formData.append('ItemId', itemId)
            formData.append('Quantity', quantity)
            formData.append('SerialItemId', serialItemId)
            formData.append('ProductionOrderId', productionOrderId)
            formData.append('isIntial', isIntial)
            var dataSet = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/Maintenances/AddItemDetails',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtItemDetails').DataTable().ajax.reload();
                        $('#Quantity').val(0);
                        $('#productionOrderId').val(null);
                        $('#serialItemId').val(null);
                        $('#ItemSerial').val(null);
                        $('#ItemId').val(null);
                        $('#ItemId').select2({
                            placeholder: "اختر عنصر من القائمة"
                        });
                        toastr.success(res.msg, '');

                    } else {
                        toastr.error(res.msg, '');
                        $('#productionOrderId').val(null);
                        $('#serialItemId').val(null);
                        $('#ItemSerial').val(null);
                    }
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

    function deleteRowItemDetails(id) {
        $('#kt_dtItemDetails tbody').on('click', 'a.deleteIcon', function () {
            $('#kt_dtItemDetails').DataTable().row($(this).parents('tr')).remove().draw();
        })

    };

    //#endregion ========= end Step 2 ==========


    function onItemChange() {
        $("#serialItemId").val(null);
        $("#productionOrderId").val(null);
        $("#isIntial").val(null);
        //سعر بيع الصنف الافتراضى المسجل 
        //$.get("/SharedDataSources/GetDefaultSellPrice/", { itemId: $("#ItemId").val() }, function (data) {
        //    $("#Price").val(data.data);
        //});
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
    function getSaleMenDepartmentChange() { // كل المناديب فقط بدلالة الادارة 
        $.get("/SharedDataSources/GetSaleMenByDepartment", { id: $("#DepartmentSaleMenId").val() }, function (data) {
            $("#EmployeeSaleMenId").empty();
            $("#EmployeeSaleMenId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#EmployeeSaleMenId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        });
    };
    function getEmployeeDepartmentChange() { //كل الموظفين بدلالة الادارة
        $.get("/SharedDataSources/GetEmployeeByDepartment", { id: $("#DepartmentResponseId").val() }, function (data) {
            $("#EmployeeResponseId").empty();
            $("#EmployeeResponseId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#EmployeeResponseId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
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
        //step 1
        onRdoBarcodeChanged: onRdoBarcodeChanged,
        onRdoSerialChanged: onRdoSerialChanged,
        getSafesOnBranchChanged: getSafesOnBranchChanged,

        //step2
        initItemDetails: function () {
            initDTItemDetails();
        },
        addItemDetails: addItemDetails,
        deleteRowItemDetails: deleteRowItemDetails,

        onItemChange: onItemChange,
        getCustomerOnCategoryChange: getCustomerOnCategoryChange,
        getSaleMenDepartmentChange: getSaleMenDepartmentChange,
        getEmployeeDepartmentChange: getEmployeeDepartmentChange,
    };

}();

