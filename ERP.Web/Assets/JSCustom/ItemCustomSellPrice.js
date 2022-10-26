"use strict";

var ItemCustomSellPrice_Module = function () {
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

                        var div="<div style='color:black;' >تحديد سعر بيع الاصناف</div>"
                        return div;
                    },
                    customize: function (win) {
                    $(win.document.body)
                        //.css('font-size', '20pt')
                        .prepend(
                            '<img src=' + localStorage.getItem("logo")+' style="position:absolute; top:25%; right:10%;opacity: 0.2;" />'
                            //'<img src="data:image/png;base64,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" style="position:absolute; top:0; right:0;" />'
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
                    "filename": "تحديد سعر بيع الاصناف",
                    "title": "تحديد سعر بيع الاصناف",
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
                url: '/ItemCustomSellPrices/GetAll',
                type: 'GET',

                    },
        columns: [
            { data: 'Num', responsivePriority: 0 },
            { data: 'BranchName', title: 'الفرع' },
            { data: 'GroupBasicName', title: 'المجموعة' },
            { data: 'ItemName', title: 'الصنف'},
            { data: 'ProfitPercentage', title: 'نسبة الربح واى مصاريف '},
            { data: 'Actions', responsivePriority: -1,className:'actions'},

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
                                return `
							<div class="btn-group">
                                <a href="/ItemCustomSellPrices/Edit/`+ row.Id + `" class="btn btn-sm btn-clean btn-icon" title="تعديل">\
								<i class="la la-edit"></i>\
							</a>
							<a href="javascript:;" onclick=ItemCustomSellPrice_Module.deleteRow('`+ row.Id + `') class="btn btn-sm btn-clean btn-icUrln" title="حذف">\
								<i class="la la-trash"></i>
							</a></div>\
						`;
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
                        toastr.success(res.message, '')
                        if (!res.isInsert) {
                            setTimeout(function () { window.location = "/ItemCustomSellPrices/Index" }, 3000);
                        }else
                            setTimeout(function () { window.location = "/ItemCustomSellPrices/CreateEdit" } , 3000);
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
                var url = '/ItemCustomSellPrices/Delete';
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

    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        SubmitForm: SubmitForm,
        deleteRow: deleteRow,
    };

}();


