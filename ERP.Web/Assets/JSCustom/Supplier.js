"use strict";

var Supplier_Module = function () {
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
                        return 'الموردين';
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
                    filename: "الموردين",
                    title: "الموردين",
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
                url: '/Suppliers/GetAll',
                type: 'GET',

            },
            columns: [  // column index changed 
                { data: 'Num', responsivePriority: 0 },
                { data: 'CountryName', title: 'الدولة', visible: false },
                { data: 'CityName', title: 'المدينة' },
                { data: 'AreaName', title: 'المنطقة' },
                { data: 'Name', title: 'المورد' },
                { data: 'StatusType', title: 'الحالة', visible: false },
                //{ data: 'Mob1', title: 'الهاتف المحمول 1' },
                //{ data: 'Mob2', title: 'الهاتف المحمول 2' },
                //{ data: 'Tel', title: 'التليفون' },
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
                        if (row.IsActive) {

                        return '\
							<div class="btn-group">\
							<a href="/Suppliers/Edit/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
								<i class="fa fa-edit"></i>\
                            </a><a href="javascript:;" onclick=Supplier_Module.UnActivePerson("'+ row.Id + '") class="btn btn-sm btn-clean btn-icUrln" title="ايقاف نشاط">\
								<i class="fa fa-unlock-alt"></i>\
							</a>\<a href="/UploadCenterTypeFiles/Index/?typ=' + row.typ + '&refGid=' + row.Id + '" class="btn btn-sm btn-clean btn-icUrln" title="رفع ملفات للمورد">\
								<i class="fa fa-upload"></i>\
							</a>\
							<a href="javascript:;" onclick=Supplier_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a></div>\
						';
                        }
                        else {
                            return '\
							<div class="btn-group">\
							<a href="/Suppliers/Edit/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
								<i class="fa fa-edit"></i>\
                            </a><a href="javascript:;" onclick=Supplier_Module.ActivePerson("'+ row.Id + '") class="btn btn-sm btn-clean btn-icUrln" title="تنشيط">\
								<i class="fas fa-times-circle"></i>\
							</a>\<a href="/UploadCenterTypeFiles/Index/?typ=' + row.typ + '&refGid=' + row.Id + '" class="btn btn-sm btn-clean btn-icUrln" title="رفع ملفات للمورد">\
								<i class="fa fa-upload"></i>\
							</a>\
							<a href="javascript:;" onclick=Supplier_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
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


    function SubmitForm(btn) {
        try {
            var form = document.getElementById('form1');
            var formData = new FormData(form);

            var dataSupplierResponsible = $('#kt_datatableSupplierResponsible').DataTable().rows().data().toArray();
            if (dataSupplierResponsible != null) {
                if (dataSupplierResponsible.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSupplierResponsible));
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
                            setTimeout(function () { window.location = "/Suppliers/Index" }, 3000);
                        } else
                            setTimeout(function () { window.location = "/Suppliers/CreateEdit" }, 3000);
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
                var url = '/Suppliers/Delete';
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
    function ActivePerson(id) {
        Swal.fire({
            title: 'تنشيط المورد',
            text: 'هل متأكد من تنشيط المورد؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/Suppliers/ActivePerson';
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
    function UnActivePerson(id) {
        Swal.fire({
            title: 'ايقاف تنشيط المورد',
            text: 'هل متأكد من ايقاف تنشيط المورد؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/Suppliers/UnActivePerson';
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
    function onCountryChange() {
        $.get("/SharedDataSources/onCountryChange", { id: $("#CountryId").val() }, function (data) {
            $("#CityId").empty();
            $("#AreaId").empty();
            $("#CityId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#CityId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        })
    };
    function onCityChange() {
        $.get("/SharedDataSources/onCityChange", { id: $("#CityId").val() }, function (data) {
            $("#AreaId").empty();
            $("#AreaId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#AreaId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        })
    };
    //#region Supplier Responsible
    var initDTSupplierResponsible = function () {
        var table = $('#kt_datatableSupplierResponsible');

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
                url: '/Suppliers/GetDSSupplierResponsible',
                type: 'GET',

            },
            columns: [
                //{ data: 'Id', title: 'م', visible: false },
                { data: 'Num', responsivePriority: 0 },
                { data: 'ResponsibleId', visible: false },
                { data: 'ResponsibleName', title: 'اسم المسئول' },
                { data: 'ResponsibleMob', title: 'رقم التليفون' },
                { data: 'ResponsibleTransfer', title: 'التحويلة' },
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
							<a href="javascript:;" onclick=Supplier_Module.deleteRowSupplierResponsible(\''+ row.Id + '\')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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
    //#region Supplier Responsible

    function SubmitFormSupplierResponsible() {
        try {
            var Name = document.getElementById('ResponsName').value;
            var Mob = document.getElementById('ResponsMob').value;
            var Tel = document.getElementById('ResponsJob').value;
            var Job = document.getElementById('ResponsEmail').value;
            var Email = document.getElementById('ResponsTel').value;
            var Transfer = document.getElementById('ResponsTransfer').value;
            var formData = new FormData();
            if (Name === '') {
                toastr.error('تأكد من ادخال اسم المسئول', '');
                return false;
            }
            if (Mob === '') {
                toastr.error('تأكد من رقم تليفون المسئول', '');
                return false;
            }

            formData.append('ResponsibleName', Name)
            formData.append('ResponsibleMob', Mob)
            formData.append('ResponsibleTel', Tel)
            formData.append('ResponsibleJob', Job)
            formData.append('ResponsibleEmail', Email)
            formData.append('ResponsibleTransfer', Transfer)
            var dataSet = $('#kt_datatableSupplierResponsible').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/Suppliers/AddSupplierResponsible',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_datatableSupplierResponsible').DataTable().ajax.reload();
                        $('#ResponsName').val('');
                        $('#ResponsMob').val('');
                        $('#ResponsJob').val('');
                        $('#ResponsEmail').val('');
                        $('#ResponsTel').val('');
                        $('#ResponsTransfer').val('');

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

    function deleteRowSupplierResponsible(id) {
        $('#kt_datatableSupplierResponsible tbody').on('click', 'a.deleteIcon', function () {
            $('#kt_datatableSupplierResponsible').DataTable().row($(this).parents('tr')).remove().draw();
        })

    };
    //#endregion
    function onPersonTypeChange() {
        if ($("#PersonTypeId").val() == '3') {
            $("#customerCategory").show();
        } else {
            $("#customerCategory").hide();
        }
    };
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        initSupplierResponsible: function () {
            initDTSupplierResponsible();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        SubmitFormSupplierResponsible: SubmitFormSupplierResponsible,
        onCountryChange: onCountryChange,
        onCityChange: onCityChange,
        deleteRowSupplierResponsible: deleteRowSupplierResponsible,
        ActivePerson: ActivePerson,
        UnActivePerson: UnActivePerson,
        onPersonTypeChange: onPersonTypeChange,

    };

}();


