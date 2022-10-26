using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class NotificationVM
    {
        public NotificationVM()
        {
            ExpenesePeriodic = new List<NotificationDto>();
            IncomePeriodic = new List<NotificationDto>();
            SellInvoiceDueDateClient = new List<NotificationDto>();
            PurchaseInvoiceDueDateSupplier = new List<NotificationDto>();
            ChequesClients = new List<NotificationDto>();
            ChequesSuppliers = new List<NotificationDto>();
            SellInvoicePayments = new List<NotificationDto>();
            SellInvoiceInstallments = new List<NotificationDto>();
            TaskEmployees = new List<NotificationDto>();
        }
        public List<NotificationDto> ExpenesePeriodic { get; set; }
        public List<NotificationDto> IncomePeriodic { get; set; }
        public List<NotificationDto> SellInvoiceDueDateClient { get; set; }
        public List<NotificationDto> PurchaseInvoiceDueDateSupplier { get; set; }
        public List<NotificationDto> ChequesClients { get; set; }
        public List<NotificationDto> ChequesSuppliers { get; set; }
        public List<NotificationDto> SellInvoicePayments { get; set; }
        public List<NotificationDto> SellInvoiceInstallments { get; set; }
        public List<NotificationDto> TaskEmployees { get; set; }
    }

    public class NotificationDto
    {
        public NotificationDto()
        {
            Employees = new List<string>();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public DateTime? DueDate { get; set; }
        public List<string> Employees { get; set; }
    }
}