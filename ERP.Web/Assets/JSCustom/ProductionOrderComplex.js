"use strict";
var ProductionOrderComplex_Module = function () {

    //#region ======== Save Production Order ==================
    var initDT = function () {
        var table = $('.kt_datatableItems');

        table.DataTable({
            responsive: true,
            searchDelay: 500,
            processing: true,
            serverSide: false,
            paging: false,
            info: false,
            lengthChange: false,
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



            //"order": [[0, "desc"]]
            //"order": [[0, "desc"]]

        });



    };

    function getStoresBranchChanged() {  // تحديد محزن الصيانة
        $.get("/SharedDataSources/getStoresOnBranchChanged", { id: $("#BranchId").val(), isDamage: false, userId: $("#Hdf_userId").val() }, function (data) {
            $("#ProductionUnderStoreId").empty();
            $("#ProductionStoreId").empty();
            $("#ProductionUnderStoreId").append("<option value=>اختر عنصر من القائمة </option>");
            $("#ProductionStoreId").append("<option value=>اختر عنصر من القائمة </option>");
            $.each(data, function (index, row) {
                $("#ProductionUnderStoreId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
                $("#ProductionStoreId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            })
        });
    };

    //#endregion ============== end ==============




    return {
        //main function to initiate the module
        init: function () {
            initDT();
        },
        getStoresBranchChanged: getStoresBranchChanged
    };

}();

