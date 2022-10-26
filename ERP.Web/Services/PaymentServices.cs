using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.Services
{
    public static class PaymentServices
    {
        //للحصول على قيمة المبالغ التى تم تحصيلها والشيكات المحصلة 
        public static double GetAmount(Guid invoId, Guid? customerId)
        {
            if (invoId != Guid.Empty)
            {
                double totalCollected = 0;
                using (var db=new VTSaleEntities())
                {
                    //اجمالى ما تم تحصيلة من الدفعات النقدية
                    var payments = db.CustomerPayments.Where(x => !x.IsDeleted).ToList();
                    var cheques = db.Cheques.Where(x => !x.IsDeleted).ToList();
                    try
                    {
                        if (customerId != null)
                        {
                            totalCollected = payments.Where(x => x.CustomerId == customerId && x.SellInvoiceId == invoId && x.IsApproval).Sum(x => x.Amount);
                            totalCollected += cheques.Where(x => x.CustomerId == customerId && x.InvoiceId == invoId && x.IsCollected).Sum(x => x.Amount);
                            return totalCollected;
                        }
                        else
                            return 0;
                    }
                    catch (Exception ex)
                    {
                        return 0;
                    }

                }
            }
            else
                return 0;
        }
    }
}