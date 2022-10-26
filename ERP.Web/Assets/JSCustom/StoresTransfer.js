"use strict";



var StoresTransfer_Module = function () {

    //#region ======== Save Purchase invoice ==================
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
                        return 'التحويلات المخزنية';
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
                    filename: "التحويلات المخزنية",
                    title: "التحويلات المخزنية",
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
                url: '/StoresTransfers/GetAll',
                type: 'GET',
                data(d) {
                    d.storeFromId = $("#StoreFromId").val();
                    d.storeToId = $("#StoreToId").val();
                    d.dtFrom = $("#dtFrom").val();
                    d.dtTo = $("#dtTo").val();
                    d.cmbo_approvalStore = $("#cmbo_approvalStore").val();
                    d.cmbo_forSaleMen = $("#cmbo_forSaleMen").val();
                }
                    },
        columns: [
            //{ data: 'Id', title: 'م', visible: false },
            //{ data: 'SaleMenIsApproval', visible: false },
            //{ data: 'SaleMenStatus', visible: false },
            //{ data: 'Status', visible: false },
            { data: 'Num', responsivePriority: 0 },
            { data: 'BranchFromName', title: 'من فرع' },
            { data: 'StoreFromName', title: 'من مخزن' },
            { data: 'BranchToName', title: 'الى فرع' },
            { data: 'StoreToName', title: 'الى مخزن' },
            { data: 'TransferDate', title: 'تاريخ العملية' },
            { data: 'EmployeeFromName', title: 'من المندوب' },
            { data: 'EmployeeToName', title: 'الى المندوب' },
            { data: 'ApprovalStore', title: 'حالة الاعتماد مخزنيا' },
            { data: 'CaseName', title: 'اخر حالة للتحويل' },
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
                                <a href="/StoresTransfers/Edit/'+ row.Id + '" class="btn btn-sm btn-clean btn-icUrln" title="تعديل">\
								<i class="fa fa-edit"></i>\
							</a><a href="/StoresTransfers/ShowHistory/?invoGuid='+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض الحالات">\
								<i class="fa fa-sliders"></i>\
							</a>\<a href="/StoresTransfers/ShowDetails/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="عرض تفاصيل العملية">\
								<i class="fa fa-search"></i>\
							</a>\<a href="javascript:;" onclick=StoresTransfer_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a></div>\
						';
                       
                       
                    }
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

    function SubmitForm(btn) {
        try {
            var form = document.getElementById('kt_form');
            var formData = new FormData(form);
            var dataSet = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_DatasourceItems", JSON.stringify(dataSet));
                }
            }
            var dataSetExpense = $('#kt_dtStoresTransferExpenses').DataTable().rows().data().toArray();
            if (dataSetExpense != null) {
                if (dataSetExpense.length > 0) {
                    formData.append("DT_DatasourceExpenses", JSON.stringify(dataSetExpense));
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
                                setTimeout(function () { window.location = "/StoresTransfers/Index" }, 3000);
                            } else
                                setTimeout(function () { window.location = "/StoresTransfers/CreateEdit" }, 3000);
                        

                       
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
            text: 'هل متأكد من الحدف ؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/StoresTransfers/Delete';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "id": invoGuid
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

    function onSearch() {
        $('#kt_datatable').DataTable().ajax.reload();
    }

    function Approval(id, app) {
        var msgTitle = '';
        var msgText = '';
        if (app === '1') {
            msgTitle = "تأكيد الاعتماد وقبول التحويل";
            msgText = "هل متأكد من الاعتماد ؟";
        } else {
            msgTitle = "تأكيد رفض التحويل";
            msgText = "هل متأكد من الرفض ؟";
        }
        Swal.fire({
            title: msgTitle,
            text: msgText,
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/StoresTransfers/Approval';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: {
                        "id": id,
                        "app": app
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
    function getStoresOnBranchFromChanged() {  // get safes and stores by branchId
        $.get("/SharedDataSources/getStoresOnBranchChanged", { id: $("#BranchFromId").val() }, function (data) {
            $("#StoreFromId").empty();
            $("#StoreFromId").append("<option value=>اختر عنصر من القائمة </option>");
            $.each(data, function (index, row) {
                $("#StoreFromId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            })
        });
    };
    function getStoresOnBranchToChanged() {  // get safes and stores by branchId
        $.get("/SharedDataSources/getStoresOnBranchChanged", { id: $("#BranchToId").val() }, function (data) {
            $("#StoreToId").empty();
            $("#StoreToId").append("<option value=>اختر عنصر من القائمة </option>");
            $.each(data, function (index, row) {
                $("#StoreToId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            })
        });
    };

    function getSaleMenFromChanged() {  //
        $.get("/SharedDataSources/GetSaleMenByStore", { id: $("#StoreFromId").val() }, function (data) {
            console.log(data);
            $("#saleMenFromName").text(data.saleMenName);
        });
    };
    function getSaleMenToChanged() {  //
        $.get("/SharedDataSources/GetSaleMenByStore", { id: $("#StoreToId").val() }, function (data) {
            $("#saleMenToName").text(data.saleMenName);
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
                url: '/StoresTransfers/GetDSItemDetails',
                type: 'GET',

            },
            columns: [
                { data: 'Id', title: 'م', visible: false },
                { data: 'ItemId', visible: false },
                { data: 'IsItemIntial', visible: false },
                { data: 'ItemName', title: 'الصنف' },
                { data: 'Quantity', title: 'الكمية' },
                { data: 'ProductionOrderId', title: 'امر الانتاج' },
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
							<a href="javascript:;" onclick=StoresTransfer_Module.deleteRowItemDetails('+ row.Id + ')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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

    //#region  On Item change (balance,production order .... ================ 

    function onProductionOrderChange() {
        var t = $("#balanceProductionOrder").val();
        var obj2 = JSON.parse(JSON.stringify(t));
        var obj = JSON.parse(obj2);
        var balance = obj.balance;
        var productionOrderId = obj.productOrder;
        var isIntial = obj.isIntial;
        $("#balanceVal").val(balance);
        $("#productionOrderId").val(productionOrderId);
        $("#isIntial").val(isIntial);
    };
    function onItemChange() {

        $.get("/SharedDataSources/GetProductionOrdersOnStoreChange/", { itemId: $("#ItemId").val(), storeId: $("#StoreFromId").val() }, function (data) {
            $("#balanceProductionOrder").empty();
            if (data.length > 0) {
                var t = data[0].Val
                var obj2 = JSON.parse(JSON.stringify(t));
                var obj = JSON.parse(obj2);
                console.log(obj);
                var balance = obj.balance;
                var serialItemId = obj.serialItemId;
                var productionOrderId = obj.productOrder;
                var isIntial = obj.isIntial;
                $("#balanceVal").val(balance);
                $("#productionOrderId").val(productionOrderId);
                $("#isIntial").val(isIntial);
                $("#serialItemId").val(serialItemId);
                //if (data.length == 1) {
                //    isSelected = "selected";
                //} else {
                //    $("#balanceProductionOrder").append("<option value=>اختر عنصر من القائمة</option>");
                //};
                var count = 1;
                $.each(data, function (index, data) {
                    if (count === 1)
                        $("#balanceProductionOrder").append("<option value='" + data.Val + "' selected>" + data.Text + "</option>");
                    else
                        $("#balanceProductionOrder").append("<option value='" + data.Val + "'>" + data.Text + "</option>");
                    count++;
                });

            }
            //var isSelected = "";
            //$("#balanceProductionOrder").empty();
            //var t = data[0].Val
            //var obj2 = JSON.parse(JSON.stringify(t));
            //var obj = JSON.parse(obj2);
            //var balance = obj.balance;
            //var productionOrderId = obj.productOrder;
            //var isIntial = obj.isIntial;
            //$("#balanceVal").val(balance);
            //$("#productionOrderId").val(productionOrderId);
            //$("#isIntial").val(isIntial);
            //if (data.length == 1) {
            //    isSelected = "selected";
            //} else {
            //    $("#balanceProductionOrder").append("<option value=>اختر عنصر من القائمة</option>");
            //};
            //$.each(data, function (index, data) {
            //    $("#balanceProductionOrder").append("<option value='" + data.Val + "' " + isSelected + ">" + data.Text + "</option>");
            //});


        });

        return false;
        //$("#StoreId").val(null);
        //$("#balanceVal").val(null);
        //$("#productionOrderId").val(null);
        //$("#isIntial").val(null);
        //$("#balanceProductionOrder").val(null);
        ////سعر بيع الصنف الافتراضى المسجل 
        //$.get("/SharedDataSources/GetDefaultSellPrice/", { itemId: $("#ItemId").val() }, function (data) {
        //    $("#Price").val(data.data);
        //});
    };

    //#endregion =============== end 
    function addItemDetails() {
        try {
            var itemId = document.getElementById('ItemId').value;
            var quantity = document.getElementById('Quantity').value;
            var storeFromId = document.getElementById('StoreFromId').value;
            var serialItemId = document.getElementById('serialItemId').value;
            var storeToId = document.getElementById('StoreToId').value;
            var balanceVal = document.getElementById('balanceVal').value;
            var productionOrderId = document.getElementById('productionOrderId').value;
            var isIntial = document.getElementById('isIntial').value;
            var formData = new FormData();
            if (itemId === '') {
                toastr.error('تأكد من اختيار الصنف', '');
                $("#ItemId").select2('open');
                return false;
            };

            if (quantity > parseFloat(balanceVal)) {
                if ($('#ItemAcceptNoBalance').val() === null || $('#ItemAcceptNoBalance').val() === '' || $('#ItemAcceptNoBalance').val() === 'undefined') {
                    toastr.error('الكمية المدخلة اكبر من الرصيwwد المتاح', '');
                    $("#Quantity").focus().select();
                    return false;
                } else if ($('#ItemAcceptNoBalance').val() === "0") {
                        toastr.error('الكمية المدخلة اكبر من الرصيد المتاح', '');
                        $("#Quantity").focus().select();
                        return false;
                    } 
         };

            if (storeFromId === '') {
                toastr.error('تأكد من اختيار المخزن(من)', '');
                return false;
            };
            if (storeToId === '') {
                toastr.error('تأكد من اختيار المخزن(إلى)', '');
                return false;
            };
           
            formData.append('ItemId', itemId)
            formData.append('Quantity', quantity)
            formData.append('SerialItemId', serialItemId)
            formData.append('ProductionOrderId', productionOrderId)
            formData.append('IsIntial', isIntial)
            var dataSet = $('#kt_dtItemDetails').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/StoresTransfers/AddItemDetails',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_dtItemDetails').DataTable().ajax.reload();
                        $('#Quantity').val(0);
                        $('#balanceProductionOrder').empty();
                        $('#balanceVal').val(null);
                        $('#productionOrderId').val(null);
                        $('#balanceVal').val(null);
                        $('#ItemBarcode').val(null);

                        $('#isIntial').val(null);
                        $('#serialItemId').val(null);
                        $('#ItemSerial').val(null);
                        //$('#ItemId').val(null);
                        //$('#ItemId').select2({
                        //    placeholder: "اختر عنصر من القائمة"
                        //});
                        $('#ItemId').select2('open');
                        toastr.success(res.msg, '');

                        $('#TotalQuantity').text(res.totalQuantity);

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

    function deleteRowItemDetails(id) {
        $('#kt_dtItemDetails tbody').on('click', 'a.deleteIcon', function () {
            var quantityRemoved = $('#kt_dtItemDetails').DataTable().row($(this).closest('tr')).data()['Quantity'];
            $("#TotalQuantity").text(Number.parseFloat($("#TotalQuantity").text()) - quantityRemoved);

            $('#kt_dtItemDetails').DataTable().row($(this).parents('tr')).remove().draw();
        })

    };


    //#endregion ============= step 2 
    function onPriceOrQuanKeyUp() {
        if ( $("#Quantity").val() < 0 ||  isNaN($("#Quantity").val())) {
            toastr.error('تأكد من ادخال ارقام صحيحية للعدد والكمية', '');
        } 
    };

    //function getSaleMenDepartmentChange(typ) {
    //    if (typ==='1') { // اختيار الادارة من 
    //        $.get("/SharedDataSources/GetSaleMenByDepartment", { id: $("#DepartmentFromId").val() }, function (data) {
    //            $("#EmployeeFromId").empty();
    //            $("#EmployeeFromId").append("<option value=>اختر عنصر من القائمة</option>");
    //            $.each(data, function (index, row) {
    //                $("#EmployeeFromId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
    //            });
    //        });
    //    } else {
    //        $.get("/SharedDataSources/GetSaleMenByDepartment", { id: $("#DepartmentToId").val() }, function (data) {
    //            $("#EmployeeToId").empty();
    //            $("#EmployeeToId").append("<option value=>اختر عنصر من القائمة</option>");
    //            $.each(data, function (index, row) {
    //                $("#EmployeeToId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
    //            });
    //        });

    //    }

    //};

    function onRdoBarcodeChanged() {
        if ($("#rdo_barcode:checked").val()) {
            $("#barcodeDiv").show();
            $('#ItemBarcode').removeAttr('disabled');
            $('#ItemSerial').attr('disabled', 'disabled');
            $("#serialDiv").hide();

            $("#serialItemId").val(null);
            $("#productionOrderId").val(null);
            $("#isIntial").val(null);

            $("#rdo_barcode:checked").prop('cheched', true);

        }
    }
    function onRdoSerialChanged() {
        if ($("#rdo_serial:checked").val()) {
            $("#barcodeDiv").hide();
            $('#ItemSerial').removeAttr('disabled');
            $('#ItemBarcode').attr('disabled', 'disabled');
            $("#serialDiv").show();

        }
    };
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        onSearch: onSearch,
        Approval: Approval,
        //step 1
        getStoresOnBranchFromChanged: getStoresOnBranchFromChanged,
        getStoresOnBranchToChanged: getStoresOnBranchToChanged,
        getSaleMenFromChanged: getSaleMenFromChanged,
        getSaleMenToChanged: getSaleMenToChanged,
        //step2
        initItemDetails: function () {
            initDTItemDetails();
        },
        addItemDetails: addItemDetails,
        deleteRowItemDetails: deleteRowItemDetails,
        onProductionOrderChange: onProductionOrderChange,
        onItemChange: onItemChange,
        onPriceOrQuanKeyUp: onPriceOrQuanKeyUp,
        //getSaleMenDepartmentChange: getSaleMenDepartmentChange,
        onRdoBarcodeChanged: onRdoBarcodeChanged,
        onRdoSerialChanged: onRdoSerialChanged,
    };

}();

