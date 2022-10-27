"use strict";

var RptProductionOrder_Module = function () {
    //#region ============== اوامر الانتاج
    var initProduction = function () {
        var table = $('#kt_datatable').DataTable({
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
                        return 'اوامر الانتاج';
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
                    filename: "اوامر الانتاج",
                    title: "اوامر الانتاج",
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
                url: '/RptProductionOrders/GetProductionOrders',
                type: 'GET',
                data(d) {
                    d.branchId = $("#BranchId").val();
                    d.colorId = $("#ColorId").val();
                    d.dFrom = $("#dtFrom").val();
                    d.dTo = $("#dtTo").val();
                    d.OrderNumber = $("#OrderNumber").val();
                    d.isDone = $("#IsDone").is(":checked");
                    d.itemId = $("#ItemId").val();
                }


            },
            columns: [
                {

                    class: "details-control",
                    orderable: false,
                    data: null,
                    defaultContent: ""
                },
                { data: 'OrderNumber', title: 'رقم امر الانتاج' },
                { data: 'FinalItemName', title: 'الصنف النهائى' },
                { data: 'ProductionOrderDate', title: 'تاريخ الانتاج' },
                { data: 'OrderQuantity', title: 'عدد الانتاج' },
                { data: 'FinalItemCost', title: 'تكلفة المنتج النهائى' },
                { data: 'MaterialItemCost', title: 'تكلفة المواد الخام' },
                { data: 'TotalCost', title: 'تكلفة امر الانتاج' },
                //{ data: 'Actions', responsivePriority: -1 },

            ], columnDefs: [

      //          {
      //              targets: -1,
      //              title: 'عمليات',
      //              orderable: false,
      //              render: function (data, type, row, meta) {
      //                  if (row.IsDone) {
      //                      return '\
						//	<div class="btn-group">\
						//	<a href="/ProductionOrders/ShowProductionOrder/?ordrGud=' +
      //                          row.Id +
      //                          '" class="btn btn-sm btn-clean btn-icon" title="عرض ملخص أمر اللإنتاج">\
						//		<i class="fa fa-search"></i>\
						//	</a>\</div>\
						//';
      //                  } 


      //              },
      //          }

            ],
            drawCallback: function () {
                var api = this.api();
                $("#totalQuantity").text(api.column(4).data().sum());
                $("#materialItemCost").text(api.column(6).data().sum());
                $("#totalCost").text(api.column(7).data().sum());
            },

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

        // Array to track the ids of the details displayed rows
        var detailRows = [];

        $('#kt_datatable tbody').on('click', 'tr td.details-control', function () {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var idx = $.inArray(tr.attr('id'), detailRows);
            if (row.child.isShown()) {
                tr.removeClass('details');
                row.child.hide();

                // Remove from the 'open' array
                detailRows.splice(idx, 1);
            }
            else {
                tr.addClass('details');
                row.child(format(row.data())).show();

                // Add to the 'open' array
                if (idx === -1) {
                    detailRows.push(tr.attr('id'));
                }
            }
        });

        // On each draw, loop over the `detailRows` array and show any child rows
        table.on('draw', function () {
            $.each(detailRows, function (i, id) {
                $('#' + id + ' td.details-control').trigger('click');
            });
        });

    };

    function format(d) {
        var tr = '';
        $.each(d.MaterialItems, function (index, data) {
            tr += ' <tr>' +
                ' <th scope="row">' + (index + 1) + '</th>' +
                ' <td>' + data.ItemName + '</td>' +
                '<td>' + data.Quantity + '</td>' +
                '<td>' + data.ItemCost + '</td>' +
                '<td>' + data.Quantitydamage + '</td>' +
                '</tr>';


        });
        return '<table class="table table-primary mb-6"><thead>' +
            '  <tr>' +
            ' <th scope="col">م</th>' +
            '  <th scope="col">الصنف الخام</th>' +
            '  <th scope="col">الكمية</th>' +
            '  <th scope="col">تكلفة المنتج الخام</th>' +
            '  <th scope="col">عدد التوالف</th>' +
            ' </tr>' +
            '</thead > ' +
            '<tbody>' +
            tr
        ' </tbody>' +
            '</table >';

    };
    //#endregion

    //العملاء بدلالة فئة العملاء
    function getCustomerOnCategoryChange() {
        var selMen = $("#EmployeeId").val();
        $.get("/SharedDataSources/GetCustomerOnCategoryChange/", { id: $("#PersonCategoryId").val(), saleMenId: selMen }, function (data) {
            $("#CustomerId").empty();
            $("#CustomerId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#CustomerId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });
        });
    };
    function getSaleMenDepartmentChange() {
        $.get("/SharedDataSources/GetSaleMenByDepartment", { id: $("#DepartmentId").val() }, function (data) {
            $("#EmployeeId").empty();
            $("#EmployeeId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#EmployeeId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });

        });
    };
    function onItemTypeChange() {
        $.get("/SharedDataSources/OnItemTypeChange", { id: $("#ItemtypeId").val() }, function (data) {
            $("#ItemId").empty();
            $("#ItemId").append("<option value=>اختر عنصر من القائمة</option>");
            $.each(data, function (index, row) {
                $("#ItemId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            });
        });
    };
    return {
        //main function to initiate the module
        initProduction: initProduction,

        getCustomerOnCategoryChange: getCustomerOnCategoryChange,
        getSaleMenDepartmentChange: getSaleMenDepartmentChange,
        onItemTypeChange: onItemTypeChange,
    };

}();


