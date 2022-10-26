"use strict";
//https://htmldom.dev/print-an-image/

var PrintInvoice_Module = function () {
    //#region طباعة فواتير (البيع-مرتجع البيع-التوريد-مرتجع التوريد)
    function Print(invoGud, typ, quantityOnly) {
        var rptTitle, rpSupCustTitle, rptTotalEx, rpSupCustAddress, rpSupCustAddress2, rpSupCustomerCommercialRegisterNo, rpSupCustomerTaxNo = '';
        if (typ === 'purchase') {
            rptTitle = 'فاتورة توريد';
            rpSupCustTitle = 'المورد';
            rptTotalEx = 'اجمالى المصروفات';
        } else if (typ === 'purchaseBack') {
            rptTitle = 'فاتورة مرتجع توريد';
            rpSupCustTitle = 'المورد';
            rptTotalEx = 'اجمالى المصروفات';
        } else if (typ === 'sell') {
            rptTitle = 'فاتورة بيــع';
            rpSupCustTitle = 'العميل';
            rptTotalEx = 'اجمالى الايرادات';
            rpSupCustAddress = 'عنوان العميل';
            rpSupCustAddress2 = 'عنوان التسليم';
            rpSupCustomerCommercialRegisterNo = 'رقم السجل التجاري للعميل';
            rpSupCustomerTaxNo = 'الرقم الضريبي للعميل';
        } else if (typ === 'sellBack') {
            rptTitle = 'فاتورة مرتجع بيــع';
            rpSupCustTitle = 'العميل';
            rptTotalEx = 'اجمالى الايرادات';
        }
        else {
            toastr.error('حدث خطأ اثناء الطباعه', '');
            return false;
        }

        //بيانات الفاتورة 
        $.ajax({
            type: 'GET',
            url: "/SharedDataSources/PrintInvoice",
            data: { id: invoGud, typ: typ },
            datatype: "application/json",
            contentType: "text/plain",
            success: function (res) {
                var data = res.invoice;
                var items = "<table class='table table-bordered' style='width:70mm!text-align:center;direction:rtl!important;font-family:Tahoma!important;color:#000!important;margin:0px!important;width:100%!important;border:solid 1px #000!important;' ><thead><tr>";
                items += "<th>اسم الصنف</th>";
                if (quantityOnly === null) {
                    items += "<th>السعر</th>";
                }
                items += "<th>الكمية</th>";
                if (quantityOnly === null) {
                    items += "<th>القيمة</th>";
                    items += "<th>الخصم</th>";
                }
                items += "</tr></thead>";
                items += "<tbody>";
                //debugger;
                //var tt = data.EntityDataName;
                //var bb = Object.keys(data);
                //var vv = JSON.parse(JSON.stringify(data));
                $.each(data.ItemDetails, function (index, row) {
                    items += "<tr>";
                    items += "<td>" + row.ItemName + "</td>";
                    if (quantityOnly === null) {
                        items += "<td>" + row.Price + "</td>";
                    }
                    items += "<td>" + row.Quantity + "</td>";
                    if (quantityOnly === null) {
                        items += "<td>" + row.Amount + "</td>";
                        items += "<td>" + row.ItemDiscount + "</td>";
                    }
                    items += "</tr>";
                })
                items += "</tbody></table>";
                var mywindow = window.open('', 'PRINT', 'height=400,width=600');

                mywindow.document.write("<html><head><style>");
                mywindow.document.write('body{width:100%!important;text-align:center!important;color:#000000!important;}div{text-align:center!important;color:#000000!important;width:100%!important;}hr{margin:0!important;color:#000000!important;width:100%!important;}h6{margin:0!important;color:#000000!important;}label{font-size:9px!important;color:#000000!important;font-family:Tahoma!important;display:block!}table{width:70mm!text-align:center;direction:rtl!important;font-family:Tahoma!important;color:#000!important;margin:0px!important;width:100%!important;border:solid 1px #000!important;}th{font-size:12px!important;text-align:center;color:#000000!important}td{font-size:12px!important;color:#000000!important;text-align:center;}');
                mywindow.document.write('table,td,th {border-collapse:collapse;border: 1px solid #000000;}');
                mywindow.document.write('</style>');
                mywindow.document.write('</head><body>');
                mywindow.document.write('<img src=' + data.Logo + ' width="150px" height="105px" />');
                mywindow.document.write('<img src="' + data.Logo + '" style="position:absolute; top:25%; left:2%;opacity: 0.2;" />');

                mywindow.document.write('<br><h6><strong>' + data.EntityDataName + '</strong></h6><br>');
                mywindow.document.write('<h6><strong>الرقم التجارى : ' + data.EntityCommercialRegisterNo + '</strong></h6><br>');
                mywindow.document.write('<h6><strong>الرقم الضريبى  : ' + data.EntityDataTaxCardNo + '</strong></h6><br><hr>');

                mywindow.document.write('<h4>' + rptTitle + ' </h4>');
                mywindow.document.write('<label> رقم الفاتورة :  ' + data.InvoiceNumber + '</label><br/>');
                mywindow.document.write('<label> ' + rpSupCustTitle + ' :  ' + data.SuppCustomerName + '</label><br/>');

                if (typ === 'sell') {
                    if (data.SuppCustomerAddress) {
                        mywindow.document.write('<label> ' + rpSupCustAddress + ' :  ' + data.SuppCustomerAddress + '</label><br/>');
                    }
                    if (data.SuppCustomerAddress2) {
                        mywindow.document.write('<label> ' + rpSupCustAddress2 + ' :  ' + data.SuppCustomerAddress2 + '</label><br/>');
                    }
                    if (data.SuppCustomerCommercialRegisterNo) {
                        mywindow.document.write('<label> ' + rpSupCustomerCommercialRegisterNo + ' :  ' + data.SuppCustomerCommercialRegisterNo + '</label><br/>');
                    }
                    if (data.SuppCustomerTaxNo) {
                        mywindow.document.write('<label> ' + rpSupCustomerTaxNo + ' :  ' + data.SuppCustomerTaxNo + '</label><br/>');
                    }

                }


                mywindow.document.write('<label> تاريخ الفاتورة :  ' + data.InvoiceDate + '</label><br/>');
                mywindow.document.write('<label> اجمالى العدد :  ' + data.TotalQuantity + '</label><br/>');
                if (quantityOnly === null) {
                    mywindow.document.write('<label> طريقة الدفع :  ' + data.PaymentTypeName + '</label><br/>');
                }
                mywindow.document.write(items);
                if (quantityOnly === null) {
                    mywindow.document.write('<br/><label> اجمالى الفاتورة :  ' + data.TotalValue + '</label><br/>');
                    mywindow.document.write('<label> ' + rptTotalEx + ' :  ' + data.TotalIncomExpenses + '</label><br/>');
                    mywindow.document.write('<label> اجمالى الخصومات:  ' + data.TotalDiscount + '</label><br/>');
                    mywindow.document.write('<label> ضريبة القيمة المضافة:  ' + data.SalesTax + '</label><br/>');
                    mywindow.document.write('<label> ضريبة ارباح تجارية:  ' + data.ProfitTax + '</label><br/>');
                    mywindow.document.write('<label> صافى الفاتورة:  ' + data.Safy + '</label><br/>');
                    mywindow.document.write('<label> المديونية:  ' + data.PersonBalance + '</label><br/>');
                    var qr = "<div> <img src='data:image/png;base64, " + data.QRCode + "' width='200' /></div>"
                    mywindow.document.write(qr);
                }

                mywindow.document.write('<label style="float:right; padding-right:100px;">  توقيع المستلم</label>');
                mywindow.document.write('<label style="float:left; padding-left:100px; margin-top:10px;"> :الختم </label>');
                mywindow.document.write('</body></html>');
                mywindow.document.close(); // necessary for IE >= 10
                mywindow.focus(); // necessary for IE >= 10*/
                setTimeout(function () { mywindow.print(); }, 600);
                setTimeout(function () { mywindow.close(); }, 600);
                //mywindow.close();


                //mywindow.onload = function () { mywindow.print(); }
                //mywindow.onbeforeunload = setTimeout(function () { mywindow.close(); }, 500);
                //mywindow.onafterprint = setTimeout(function () { mywindow.close(); }, 500);
                //mywindow.onload = function () {
                //    mywindow.document.close(); // necessary for IE >= 10
                //    mywindow.focus(); // necessary for IE >= 10*/
                //    mywindow.print();
                //    mywindow.close();


                //};


                return true;
            },
            error: function (err) {
                toastr.error('حدث خطأ اثناء الطباعه', '');
                console.log(err)
            }
        })
        //to prevent default form submit event
        return false;


    }

    function DownloadInvoice(invoGud, typ) {
        var rptTitle, rpSupCustTitle, rptTotalEx = '';
        if (typ === 'purchase') {
            rptTitle = 'فاتورة توريد';
            rpSupCustTitle = 'المورد';
            rptTotalEx = 'اجمالى المصروفات';
        } else if (typ === 'purchaseBack') {
            rptTitle = 'فاتورة مرتجع توريد';
            rpSupCustTitle = 'المورد';
            rptTotalEx = 'اجمالى المصروفات';
        } else if (typ === 'sell') {
            rptTitle = 'فاتورة بيــع';
            rpSupCustTitle = 'العميل';
            rptTotalEx = 'اجمالى الايرادات';
        } else if (typ === 'sellBack') {
            rptTitle = 'فاتورة مرتجع بيــع';
            rpSupCustTitle = 'العميل';
            rptTotalEx = 'اجمالى الايرادات';
        }
        else {
            toastr.error('حدث خطأ اثناء الطباعه', '');
            return false;
        }
        $.ajax({
            type: 'GET',
            url: "/SharedDataSources/PrintInvoice",
            data: { id: invoGud, typ: typ },
            datatype: "application/json",
            contentType: "text/plain",
            success: function (res) {
                var data = res.invoice;
                var items = "<table class='table table-bordered' style='width:70mm!text-align:center;direction:rtl!important;font-family:Tahoma!important;color:#000!important;margin:0px!important;width:100%!important;border:solid 1px #000!important;' ><thead><tr>";
                items += "<th>اسم الصنف</th>";
                items += "<th>السعر </th>";
                items += "<th>الكمية</th>";
                items += "<th>القيمة</th>";
                items += "<th>الخصم</th>";
                items += "</tr></thead>";
                items += "<tbody>";
                //debugger;
                //var tt = data.EntityDataName;
                //var bb = Object.keys(data);
                //var vv = JSON.parse(JSON.stringify(data));
                $.each(data.ItemDetails, function (index, row) {
                    items += "<tr>";
                    items += "<td>" + row.ItemName + "</td>";
                    items += "<td>" + row.Price + "</td>";
                    items += "<td>" + row.Quantity + "</td>";
                    items += "<td>" + row.Amount + "</td>";
                    items += "<td>" + row.ItemDiscount + "</td>";
                    items += "</tr>";
                })
                items += "</tbody></table>";

                var elem = "<html><head><meta charset='utf-8'><style>";
                //elem += '@page{margin-left:14px;margin-right:18px;margin-top:20px;margin-bottom:4px;}body{width:100%!important;text-align:center!important;color:#000000!important;margin:0px!important;padding:0px!important;}div{text-align:center!important;color:#000000!important;width:100%!important;}hr{margin:0!important;color:#000000!important;width:100%!important;}h6{margin:0!important;color:#000000!important;}label{font-size:20px!important;color:#000000!important;font-family:Tahoma!important;display:block!}table{width:70mm!text-align:center;direction:rtl!important;font-family:Tahoma!important;color:#000!important;margin:0px!important;width:100%!important;border:solid 1px #000!important;}th{font-size:20px!important;text-align:center;color:#000000!important}td{font-size:20px!important;color:#000000!important;text-align:center;}';
                elem += '@page {margin-left: 14px;            margin-right: 18px;            margin-top: 20px;            margin-bottom: 4px;        }        body {            width: 100% !important;            text-align: center !important;            color: #000000 !important;            margin: 0px !important;            padding: 0px !important;        }        div {            text-align: center !important;            color: #000000 !important;            width: 100% !important;        }        hr {           margin: 0 !important;            color: #000000 !important;            width: 100% !important;        }        h6 {            margin: 0 !important;            color: #000000 !important;            font-size: 42px !important;        }        label {            font-size: 40px !important;            color: #000000 !important;            font-family: Tahoma !important;            display: block !important;        }        table {            width: 90mm !important;            text-align:center;            direction: rtl !important;            font-family: Tahoma !important;            color: #000 !important;            margin: 0px !important;            width: 100% !important;            border: solid 1px #000 !important;        }        th {            font-size: 42px !important;            text-align: center;            color: #000000 !important        }        td {            font-size: 42px !important;            color: #000000 !important; text - align: center;        }';
                elem += 'table,td,th {border-collapse:collapse;border: 1px solid #000000;}'
                elem += '</style></head><body>'
                elem += '<img src=' + data.Logo + ' width="150px" height="105px" />';
                elem += '<img src="' + data.Logo + '" style="position:absolute; top:60%; left:30%;opacity: 0.2;" />';

                elem += '<br><h6><strong>' + data.EntityDataName + '</strong></h6><br>';
                elem += '<h6><strong>الرقم التجارى : ' + data.EntityCommercialRegisterNo + '</strong></h6><br>';
                elem += '<h6><strong>الرقم الضريبى  : ' + data.EntityDataTaxCardNo + '</strong></h6><br><hr>';

                elem += '<h4 style="font-size: 60px;">' + rptTitle + ' </h4>';
                elem += '<label> رقم الفاتورة :  ' + data.InvoiceNumber + '</label><br/>';
                elem += '<label> ' + rpSupCustTitle + ' :  ' + data.SuppCustomerName + '</label><br/>';
                elem += '<label> تاريخ الفاتورة :  ' + data.InvoiceDate + '</label><br/>';
                elem += '<label> اجمالى العدد :  ' + data.TotalQuantity + '</label><br/>';
                elem += '<label> طريقة الدفع :  ' + data.PaymentTypeName + '</label><br/>';
                elem += items;
                elem += '<br/><label> اجمالى الفاتورة :  ' + data.TotalValue + '</label><br/>';
                elem += '<label> ' + rptTotalEx + ' :  ' + data.TotalIncomExpenses + '</label><br/>';
                elem += '<label> اجمالى الخصومات:  ' + data.TotalDiscount + '</label><br/>';
                elem += '<label> ضريبة القيمة المضافة:  ' + data.SalesTax + '</label><br/>';
                elem += '<label> ضريبة ارباح تجارية:  ' + data.ProfitTax + '</label><br/>';
                elem += '<label> صافى الفاتورة:  ' + data.Safy + '</label><br/>';
                elem += '<label> المديونية:  ' + data.PersonBalance + '</label><br/>';
                var qr = "<div> <img src='data:image/png;base64, " + data.QRCode + "' width='500' /></div>"
                elem += qr;
                elem += '<label style="float:right; padding-right:100px;">  توقيع المستلم</label>';
                elem += '<label style="float:left; padding-left:100px; margin-top:10px;"> :الختم </label>';
                elem += '</body></html>';
                download(rptTitle, elem);
            },
            error: function (err) {
                toastr.error('حدث خطأ اثناء الطباعه', '');
                console.log(err)
            }
        })


    }

    //#endregion

    //#region طباعة تحصيل قسط)
    function PrintInstallmentSchedule(scheduleId) {
        var rptTitle = 'تحصيل قسط';
        //بيانات الفاتورة 
        $.ajax({
            type: 'GET',
            url: "/SharedDataSources/PrintInstallmentSchedule",
            data: { id: scheduleId },
            datatype: "application/json",
            contentType: "text/plain",
            success: function (data) {
                var items = "<table class='table table-bordered' style='width:70mm!text-align:right;direction:rtl!important;font-family:Tahoma!important;color:#000!important;margin:0px!important;width:100%!important;border:solid 0px #000!important;height:50px!important' >";
                items += "<tbody>";

                items += "<tr>";
                items += "<td>اسم العميل</td>";
                items += "<td>" + data.CustomerName + "</td>";
                items += "</tr>";

                items += "<tr>";
                items += "<td>تاريخ القسط</td>";
                items += "<td>" + data.InstallmentDate + "</td>";
                items += "</tr>";

                items += "<tr>";
                items += "<td>تاريخ التحصيل</td>";
                items += "<td>" + data.PaymentDate + "</td>";
                items += "</tr>";

                items += "<tr>";
                items += "<td>المبلغ</td>";
                items += "<td>" + data.Amount + "</td>";
                items += "</tr>";

                items += "<tr>";
                items += "<td>الاقساط المتبقية</td>";
                items += "<td>" + data.RemindSchedules + "</td>";
                items += "</tr>";

                items += "<tr>";
                items += "<td>مديونية العميل</td>";
                items += "<td>" + data.CustomerDebit + "</td>";
                items += "</tr>";

                items += "</tbody></table>";
                var mywindow = window.open('', 'PRINT', 'height=400,width=600');

                mywindow.document.write("<html><head><style>");
                mywindow.document.write('body{width:100%!important;text-align:center!important;color:#000000!important;}div{text-align:center!important;color:#000000!important;width:100%!important;}hr{margin:0!important;color:#000000!important;width:100%!important;}h6{margin:0!important;color:#000000!important;}label{font-size:9px!important;color:#000000!important;font-family:Tahoma!important;display:block!}table{width:70mm!text-align:center;direction:rtl!important;font-family:Tahoma!important;color:#000!important;margin:0px!important;width:100%!important;border:solid 1px #000!important;}th{font-size:12px!important;text-align:center;color:#000000!important}td{font-size:12px!important;color:#000000!important;text-align:center;}');
                mywindow.document.write('table,td,th {border-collapse:collapse;border: 1px solid #000000;}');
                mywindow.document.write('</style>');
                mywindow.document.write('</head><body>');

                mywindow.document.write('<br><h6><strong>' + data.EntityDataName + '</strong></h6><br>');
                mywindow.document.write('<h6><strong>الرقم التجارى : ' + data.EntityCommercialRegisterNo + '</strong></h6><br>');
                mywindow.document.write('<h6><strong>الرقم الضريبى  : ' + data.EntityDataTaxCardNo + '</strong></h6><br><hr>');

                mywindow.document.write('<h4>' + rptTitle + ' </h4>');
                mywindow.document.write(items);
                mywindow.document.write('<br/><br/><br/>');
                mywindow.document.write('<label style="float:right; padding-right:100px;">  توقيع المستلم</label>');
                mywindow.document.write('<label style="float:left; padding-left:100px; margin-top:10px;"> :الختم </label>');
                mywindow.document.write('</body></html>');
                mywindow.document.close(); // necessary for IE >= 10
                mywindow.focus(); // necessary for IE >= 10*/
                setTimeout(function () { mywindow.print(); }, 600);
                setTimeout(function () { mywindow.close(); }, 600);
                //mywindow.close();


                return true;
            },
            error: function (err) {
                toastr.error('حدث خطأ اثناء الطباعه', '');
                console.log(err)
            }
        })
        //to prevent default form submit event
        return false;


    }
    //#endregion
    //#region طباعة تحصيل نقدى)
    function PrintPayment(paymentId, typ) {
        var rptTitle, rpSupCustTitle, rptTotalEx = '';
        if (typ === 'custPaymentCash') {
            rptTitle = 'تحصيل نقدى من عميل';
            rpSupCustTitle = 'اسم العميل';
        } else if (typ === 'suppPaymentCash') {
            rptTitle = 'صرف لمورد';
            rpSupCustTitle = 'اسم المورد';

        } else if (typ === 'voucherPayment') {
            rptTitle = 'سند صرف';
            rpSupCustTitle = 'من حساب';

        } else if (typ === 'voucherReceipt') {
            rptTitle = 'سند قبض';
            rpSupCustTitle = 'الى حساب';

        }
        else {
            toastr.error('حدث خطأ اثناء الطباعه', '');
            return false;
        }

        //بيانات الفاتورة 
        $.ajax({
            type: 'GET',
            url: "/SharedDataSources/PrintPayment",
            data: { id: paymentId, typ: typ },
            datatype: "application/json",
            contentType: "text/plain",
            success: function (data) {
                var items = "<table class='table table-bordered' style='width:70mm!text-align:right;direction:rtl!important;font-family:Tahoma!important;color:#000!important;margin:0px!important;width:100%!important;border:solid 0px #000!important;height:50px!important' >";
                items += "<tbody>";

                items += "<tr>";
                items += "<td>" + rpSupCustTitle + "</td>";
                items += "<td>" + data.SuppCustomerName + "</td>";
                items += "</tr>";
                items += "<tr>";
                items += "<td>تاريخ العملية</td>";
                items += "<td>" + data.PaymentDate + "</td>";
                items += "</tr>";

                items += "<tr>";
                items += "<td>المبلغ</td>";
                items += "<td>" + data.Amount + "</td>";
                items += "</tr>";

                if (typ != 'voucherPayment' && typ != 'voucherReceipt') {
                    items += "<tr>";
                    items += "<td>المديونية </td>";
                    items += "<td>" + data.PersonDebit + "</td>";
                    items += "</tr>";

                }

                items += "<tr>";
                items += "<td>الخزنة </td>";
                items += "<td>" + data.SafeBankName + "</td>";
                items += "</tr>";

                items += "<tr>";
                items += "<td>ملاحظات </td>";
                items += "<td>" + data.Notes + "</td>";
                items += "</tr>";

                items += "</tbody></table>";
                var mywindow = window.open('', 'PRINT', 'height=400,width=600');

                mywindow.document.write("<html><head><style>");
                mywindow.document.write('body{width:100%!important;text-align:center!important;color:#000000!important;}div{text-align:center!important;color:#000000!important;width:100%!important;}hr{margin:0!important;color:#000000!important;width:100%!important;}h6{margin:0!important;color:#000000!important;}label{font-size:9px!important;color:#000000!important;font-family:Tahoma!important;display:block!}table{width:70mm!text-align:center;direction:rtl!important;font-family:Tahoma!important;color:#000!important;margin:0px!important;width:100%!important;border:solid 1px #000!important;}th{font-size:12px!important;text-align:center;color:#000000!important}td{font-size:12px!important;color:#000000!important;text-align:center;}');
                mywindow.document.write('table,td,th {border-collapse:collapse;border: 1px solid #000000;}');
                mywindow.document.write('</style>');
                mywindow.document.write('</head><body>');

                mywindow.document.write('<br><h6><strong>' + data.EntityDataName + '</strong></h6><br>');
                mywindow.document.write('<h6><strong>الرقم التجارى : ' + data.EntityCommercialRegisterNo + '</strong></h6><br>');
                mywindow.document.write('<h6><strong>الرقم الضريبى  : ' + data.EntityDataTaxCardNo + '</strong></h6><br><hr>');

                mywindow.document.write('<h4>' + rptTitle + ' </h4>');
                mywindow.document.write(items);
                mywindow.document.write('<br/><br/><br/>');
                mywindow.document.write('<label style="float:right; padding-right:100px;">  توقيع المستلم</label>');
                mywindow.document.write('<label style="float:left; padding-left:100px; margin-top:10px;"> :الختم </label>');
                mywindow.document.write('</body></html>');
                mywindow.document.close(); // necessary for IE >= 10
                mywindow.focus(); // necessary for IE >= 10*/
                setTimeout(function () { mywindow.print(); }, 600);
                setTimeout(function () { mywindow.close(); }, 600);



                return true;
            },
            error: function (err) {
                toastr.error('حدث خطأ اثناء الطباعه', '');
                console.log(err)
            }
        })
        //to prevent default form submit event
        return false;


    }

    //#endregion





    function download(filename, text) {
        var element = document.createElement('a');
        element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(text));
        element.setAttribute('download', filename + ".html");

        element.style.display = 'none';
        document.body.appendChild(element);

        element.click();

        document.body.removeChild(element);
    };

    return {
        DownloadInvoice: DownloadInvoice,
        Print: Print,
        PrintInstallmentSchedule: PrintInstallmentSchedule,
        PrintPayment: PrintPayment,
    };

}();


