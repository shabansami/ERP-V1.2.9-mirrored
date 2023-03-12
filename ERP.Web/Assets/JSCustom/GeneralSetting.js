"use strict";
var GeneralSetting_Module = function () {
    let editorAr;
    let editorEn;

    ClassicEditor.create(document.querySelector("#PrintSetting_QuotationNoteAr"), { language: { ui: 'en', content: 'ar' } }).then(function (e) { e.ui.view.editable.element.style.height = "200px"; editorAr = e; }).catch(function (e) { console.error(e) });
    ClassicEditor.create(document.querySelector("#PrintSetting_QuotationNoteEn"), { language: { ui: 'en', content: 'en' } }).then(function (e) { e.ui.view.editable.element.style.height = "200px"; editorEn = e; }).catch(function (e) { console.error(e) });

    //#region الاعدادات العامة
    function SubmitForm(btn, formId, tab) {
        try {
            var form = document.getElementById(formId);
            var formData = new FormData(form);
            formData.append("tabNum", tab);
            formData.append("editorNotesAr", editorAr.getData())
            formData.append("editorNotesEn", editorEn.getData())
            $.ajax({
                type: 'POST',
                url: form.action,
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        //$(btn).attr('disabled', 'disabled'); // disabled button after one clicke 
                        //$(btn).css('pointer-events', 'none'); // disabled a link after one clicke 
                        toastr.success(res.message, '',)
                        $('#imgPath').attr('src', '../Files/printLogo.jpg')
                        //setTimeout(function () { window.location = "/GeneralSettings/CreateEdit" }, 3000);
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
    function getStoresBranchChanged(branchId, storeId) {  // تحديد محزن الصيانة
        $.get("/SharedDataSources/getStoresOnBranchChanged", { id: $(branchId).val(), isDamage: false, userId: $("#Hdf_userId").val() }, function (data) {
            var store = $('#' + storeId);
            $(store).empty();
            $(store).append("<option value=>اختر عنصر من القائمة </option>");
            $.each(data, function (index, row) {
                $(store).append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            })
        });
    };
    function getStoreDamagesBranchChanged() {  // تحديد محزن التوالف
        $.get("/SharedDataSources/getStoresOnBranchChanged", { id: $('#BranchMaintenanceDamageId').val(), isDamage: true, userId: $("#Hdf_userId").val() }, function (data) {
            $('#MaintenanceDamageStoreId').empty();
            $('#MaintenanceDamageStoreId').append("<option value=>اختر عنصر من القائمة </option>");
            $.each(data, function (index, row) {
                $('#MaintenanceDamageStoreId').append("<option value='" + row.Id + "'>" + row.Name + "</option>");
            })
        });
    };

    //#endregion


    //function getStoresInBranchChanged() {  // get internal stores by branchId
    //    $.get("/SharedDataSources/getStoresOnBranchChanged", { id: $("#BranchInId").val() }, function (data) {
    //        $("#ProductionInStoreId").empty();
    //        $("#ProductionInStoreId").append("<option value=>اختر عنصر من القائمة </option>");
    //        $.each(data, function (index, row) {
    //            $("#ProductionInStoreId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
    //        })
    //    });
    //};
    //function getStoresExBranchChanged() {  // get external stores by branchId
    //    $.get("/SharedDataSources/getStoresOnBranchChanged", { id: $("#BranchExId").val() }, function (data) {
    //        $("#ProductionExStoreId").empty();
    //        $("#ProductionExStoreId").append("<option value=>اختر عنصر من القائمة </option>");
    //        $.each(data, function (index, row) {
    //            $("#ProductionExStoreId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
    //        })
    //    });
    //};
    //function getStoresUnderBranchChanged() {  // تحديد محزن تحت التصنيع
    //    $.get("/SharedDataSources/getStoresOnBranchChanged", { id: $("#BranchUnderId").val() }, function (data) {
    //        $("#ProductionUnderStoreId").empty();
    //        $("#ProductionUnderStoreId").append("<option value=>اختر عنصر من القائمة </option>");
    //        $.each(data, function (index, row) {
    //            $("#ProductionUnderStoreId").append("<option value='" + row.Id + "'>" + row.Name + "</option>");
    //        })
    //    });
    //};

    return {
        SubmitForm: SubmitForm,
        onCountryChange: onCountryChange,
        onCityChange: onCityChange,
        getStoresBranchChanged: getStoresBranchChanged,
        getStoreDamagesBranchChanged: getStoreDamagesBranchChanged,
    };

}();


