"use strict";



var PricesChange_Module = function () {

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
                url: '/PricesChanges/GetPricesPurchaseChanges',
                type: 'GET',
                data(d) {
                    d.itemId = $("#ItemId").val();
                   
                }

                    },
        columns: [
            { data: 'Id', title: 'م', visible: false },
            { data: 'ItemId', visible: false },
            { data: 'Price', title: 'سعر الشراء' },
            { data: 'Notes', title: 'ملاحظات' },

        ],
            columnDefs: [
                {
                    targets: 1,
                    title: 'م',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return  meta.row + meta.settings._iDisplayStart + 1;
                    },
                },
                    ],

"order": [[0, "asc"]]
//"order": [[0, "desc"]] 

                });
    };


    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
    };

}();

