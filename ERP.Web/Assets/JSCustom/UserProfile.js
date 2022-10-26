"use strict";

var UserProfile_Module = function () {
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
    function openModalChangePassword() {
        $('#exampleModalChangePassword').modal('show');
    };
    function onSaveChangePassword(btn) {
        try {
            $.ajax({
                type: 'POST',
                url: "/Home/ChangePassword/",
                data: {
                    oldPassword: $("#oldPassword").val(),
                    newPassword1: $("#newPassword1").val(),
                    newPassword2: $("#newPassword2").val(),
                },
                //contentType: false,
                //processData: false,
                dataType: "json",

                success: function (res) {
                    if (res.isValid) {
                        //$(btn).attr('disabled', 'disabled'); // disabled button after one clicke 
                        $(btn).css('pointer-events', 'none'); // disabled a link after one clicke 
                        toastr.success(res.message, '',)
                        setTimeout(function () { window.location = "/Home/Update" }, 3000);
                    } else {
                        toastr.error(res.message, '');
                    }
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
                        toastr.success(res.message, '',)
                        setTimeout(function () { window.location = "/Home/Update" }, 3000);
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

    return {
        onCountryChange: onCountryChange,
        onCityChange: onCityChange,
        openModalChangePassword: openModalChangePassword,
        onSaveChangePassword: onSaveChangePassword,
        SubmitForm: SubmitForm
    };

}();


