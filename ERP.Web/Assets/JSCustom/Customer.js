"use strict";

var Customer_Module = function () {
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
                        return 'العملاء';
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
                    "extend": "excelHtml5",
                    "filename": "العملاء",
                    "title": "العملاء",
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
                url: '/Customers/GetAll',
                type: 'GET',

            },
            columns: [
                { data: 'Num', responsivePriority: 0 },
                { data: 'CityName', title: 'المدينة' },
                { data: 'AreaName', title: 'المنطقة' },
                { data: 'Name', title: 'العميل' },
                { data: 'Mob1', title: 'رقم الهاتف' },
                { data: 'PersonCategoryName', title: 'الفئة' },
                { data: 'RegionName', title: 'المنطقة', visible: false },
                { data: 'DistrictName', title: 'الحي', visible: false },
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
							<a href="/Customers/Edit/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
								<i class="fa fa-edit"></i>\
                        		</a><a href="javascript:;" onclick=Customer_Module.UnActivePerson("'+ row.Id + '") class="btn btn-sm btn-clean btn-icUrln" title="ايقاف نشاط">\
								<i class="fa fa-unlock-alt"></i>\
							</a>\<a href="/UploadCenterTypeFiles/Index/?typ=' + row.typ + '&refGid=' + row.Id + '" class="btn btn-sm btn-clean btn-icUrln" title="رفع ملفات للعميل">\
								<i class="fa fa-upload"></i>\
							</a>\
							<a href="javascript:;" onclick=Customer_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="fa fa-trash"></i>\
							</a></div>\
						';
                        }
                        else {
                            return '\
							<div class="btn-group">\
							<a href="/Customers/Edit/'+ row.Id + '" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
								<i class="fa fa-edit"></i>\
                        		</a><a href="javascript:;" onclick=Customer_Module.ActivePerson("'+ row.Id + '") class="btn btn-sm btn-clean btn-icUrln" title="تنشيط">\
								<i class="fas fa-times-circle"></i>\
							</a>\<a href="/UploadCenterTypeFiles/Index/?typ=' + row.typ + '&refGid=' + row.Id + '" class="btn btn-sm btn-clean btn-icUrln" title="رفع ملفات للعميل">\
								<i class="fa fa-upload"></i>\
							</a>\
							<a href="javascript:;" onclick=Customer_Module.deleteRow(\''+ row.Id + '\') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
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

            var dataCustomerResponsible = $('#kt_datatableCustomerResponsible').DataTable().rows().data().toArray();
            if (dataCustomerResponsible != null) {
                if (dataCustomerResponsible.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataCustomerResponsible));
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
                            setTimeout(function () { window.location = "/Customers/Index" }, 3000);
                        } else
                            setTimeout(function () { window.location = "/Customers/CreateEdit" }, 3000);
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
                var url = '/Customers/Delete';
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
            title: 'تنشيط العميل',
            text: 'هل متأكد من تنشيط العميل؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/Customers/ActivePerson';
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
            title: 'ايقاف تنشيط العميل',
            text: 'هل متأكد من ايقاف تنشيط العميل؟',
            icon: 'warning',
            showCancelButton: true,
            animation: true,
            confirmButtonText: 'تأكيد',
            cancelButtonText: 'إلغاء الامر'
        }).then((result) => {
            if (result.value) {
                var url = '/Customers/UnActivePerson';
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




    //#region Customer Responsible
    var initDTCustomerResponsible = function () {
        var table = $('#kt_datatableCustomerResponsible');

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
                url: '/Customers/GetDSCustomerResponsible',
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
							<a href="javascript:;" onclick=Customer_Module.deleteRowCustomerResponsible(\''+ row.Id + '\')  class="btn btn-sm btn-clean btn-icUrln deleteIcon" title="حذف">\
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

    //#region Customer Responsible

    function SubmitFormCustomerResponsible() {
        try {
            var Name = document.getElementById('ResponsName').value;
            var Mob = document.getElementById('ResponsMob').value;
            var Tel = document.getElementById('ResponsJob').value;
            var Job = document.getElementById('ResponsEmail').value;
            var Email = document.getElementById('ResponsTel').value;
            var Transfer = document.getElementById('ResponsTransfer').value;
            var formData = new FormData();
            if (!Name) {
                toastr.error('تأكد من ادخال اسم المسئول', '');
                return false;
            }
            if (!Mob) {
                toastr.error('تأكد من رقم تليفون المسئول', '');
                return false;
            }

            formData.append('ResponsibleName', Name)
            formData.append('ResponsibleMob', Mob)
            formData.append('ResponsibleTel', Tel)
            formData.append('ResponsibleJob', Job)
            formData.append('ResponsibleEmail', Email)
            formData.append('ResponsibleTransfer', Transfer)
            var dataSet = $('#kt_datatableCustomerResponsible').DataTable().rows().data().toArray();
            if (dataSet != null) {
                if (dataSet.length > 0) {
                    formData.append("DT_Datasource", JSON.stringify(dataSet));
                }
            }
            $.ajax({
                type: 'POST',
                url: '/Customers/AddCustomerResponsible',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#kt_datatableCustomerResponsible').DataTable().ajax.reload();
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

    function deleteRowCustomerResponsible(id) {
        $('#kt_datatableCustomerResponsible tbody').on('click', 'a.deleteIcon', function () {
            $('#kt_datatableCustomerResponsible').DataTable().row($(this).parents('tr')).remove().draw();
        })

    };
    //#endregion



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
    function onAreaChange() {
        $.get("/SharedDataSources/onAreaChangeGetRegions", { id: $("#AreaId").val() }, function (data) {
            $("#RegionId").empty();
            $("#RegionId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#RegionId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        })
    };
    function onRegionChange() {
        $.get("/SharedDataSources/onRegionChange", { id: $("#RegionId").val() }, function (data) {
            $("#DistrictId").empty();
            $("#DistrictId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#DistrictId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        })
    };
    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        initCustomerResponsible: function () {
            initDTCustomerResponsible();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
        SubmitFormCustomerResponsible: SubmitFormCustomerResponsible,
        onCountryChange: onCountryChange,
        onCityChange: onCityChange,
        onAreaChange: onAreaChange,
        onRegionChange: onRegionChange,
        deleteRowCustomerResponsible: deleteRowCustomerResponsible,
        ActivePerson: ActivePerson,
        UnActivePerson: UnActivePerson
    };

}();


