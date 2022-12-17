using ERP.Web.DataTablesDS;
using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERP.DAL.Models;
using static ERP.Web.Utilites.Lookups;
using System.Data.Entity;
using ERP.Web.ViewModels;

namespace ERP.Web.Services
{
    public static class ProductionOrderService
    {
        #region Production Order
        //احتساب تكلفة اجمالى التكلفة على امر الانتاج
        public static ResultMethods GetProductionOrderCosts(Guid? productionOrderId, List<ItemsMaterialDT> itemMaterials, List<InvoiceExpensesDT> orderExpenses, double orderQuantity)
        {
            //productionOrderId (=null) insert new production order
            //productionOrderId (has value) Register Damages production order
            ProductionOrder productionOrder;
            double? materialItemCost = 0;
            double? damagesCost = 0;
            double orderExpenseCost = 0;
            double? totalCost = 0;
            double? finalItemCost = 0;

            if (productionOrderId == null)
            {
                //اجمالى تكلفة المواد الخام 
                materialItemCost = itemMaterials.Select(x => x.Quantity * x.ItemCost).DefaultIfEmpty(0).Sum();
                //اجمالى تكلفة التوالف والهوالك 
                //damagesCost = itemMaterials.Select(x => x.Quantitydamage * x.ItemCost).DefaultIfEmpty(0).Sum();
                //اجمالى التكالبف والمصروفات

                 orderExpenseCost = orderExpenses.Sum(x => x.ExpenseAmount);
                //اجمالى تكلفة امر الانتاج
                totalCost =Math.Round(materialItemCost + damagesCost + orderExpenseCost??0,2);
                //تكلفة الوحدة الواحدة
                finalItemCost =Math.Round(totalCost / orderQuantity??0,2);
                return new ResultMethods { IsValid = true, MaterialItemCost = materialItemCost ?? 0, DamagesCost = damagesCost ?? 0, OrderExpenseCost = orderExpenseCost, TotalCost = totalCost ?? 0, FinalItemCost = finalItemCost ?? 0 };
            }
            else
            {
                using (var db = new VTSaleEntities())
                {
                    productionOrder = db.ProductionOrders.Where(x => x.Id == productionOrderId).FirstOrDefault();
                    if (productionOrder == null)
                        return new ResultMethods { IsValid = false };
                    //اجمالى تكلفة المواد الخام 
                    materialItemCost = productionOrder.ProductionOrderDetails.Where(x => !x.IsDeleted).Select(x => x.Quantity * x.ItemCost).DefaultIfEmpty(0).Sum();
                    //اجمالى تكلفة التوالف والهوالك 
                    damagesCost = itemMaterials.Select(x => x.Quantitydamage * x.ItemCost).DefaultIfEmpty(0).Sum();
                    //اجمالى التكالبف والمصروفات

                    orderExpenseCost = orderExpenses.Sum(x => x.ExpenseAmount);
                    //اجمالى تكلفة امر الانتاج
                    totalCost = materialItemCost + damagesCost + orderExpenseCost;
                    //تكلفة الوحدة الواحدة
                    //finalItemCost = totalCost / productionOrder.OrderQuantity;
                    return new ResultMethods { IsValid = true, MaterialItemCost = materialItemCost ?? 0, DamagesCost = damagesCost ?? 0, OrderExpenseCost = orderExpenseCost, TotalCost = totalCost ?? 0, FinalItemCost = finalItemCost ?? 0 };

                }
            }
        }
        public static ResultMethods GetProductionOrderCost(Guid? productionOrderId, List<ProductionOrderDetail> allItems, List<InvoiceExpensesDT> orderExpenses)
        {
            ProductionOrder productionOrder;
            double? itemInCost = 0;
            double? damagesCost = 0;
            double orderExpenseCost = 0;
            double? totalCost = 0;
            double? finalItemCost = 0;
            var orderQuantity = allItems.Where(x => x.ProductionTypeId == (int)ProductionTypeCl.Out).Sum(x => (double?)x.Quantity??0);
            if (productionOrderId == null)
            {
                //اجمالى تكلفة المواد الداخلة 
                itemInCost = allItems.Where(x=>x.ProductionTypeId==(int)ProductionTypeCl.In).Select(x => x.Quantity * x.ItemCost).DefaultIfEmpty(0).Sum();
                //اجمالى تكلفة التوالف والهوالك 
                damagesCost = allItems.Where(x => x.ProductionTypeId == (int)ProductionTypeCl.In).Select(x => x.Quantitydamage * x.ItemCost).DefaultIfEmpty(0).Sum();
                //اجمالى التكالبف والمصروفات

                 orderExpenseCost = orderExpenses.Sum(x => x.ExpenseAmount);
                //اجمالى تكلفة امر الانتاج
                totalCost =Math.Round(itemInCost + damagesCost + orderExpenseCost??0,2);
                //تكلفة الوحدة الواحدة
                finalItemCost =Math.Round(totalCost / orderQuantity??0,2);
                return new ResultMethods { IsValid = true, MaterialItemCost = itemInCost ?? 0, DamagesCost = damagesCost ?? 0, OrderExpenseCost = orderExpenseCost, TotalCost = totalCost ?? 0, FinalItemCost = finalItemCost ?? 0 };
            }
            else
            {
                using (var db = new VTSaleEntities())
                {
                    productionOrder = db.ProductionOrders.Where(x => x.Id == productionOrderId).FirstOrDefault();
                    if (productionOrder == null)
                        return new ResultMethods { IsValid = false };
                    //اجمالى تكلفة المواد الخام 
                    itemInCost = productionOrder.ProductionOrderDetails.Where(x => !x.IsDeleted&&x.ProductionTypeId==(int)ProductionTypeCl.In).Select(x => x.Quantity * x.ItemCost).DefaultIfEmpty(0).Sum();
                    //اجمالى تكلفة التوالف والهوالك 
                    damagesCost = allItems.Where(x => x.ProductionTypeId == (int)ProductionTypeCl.In).Select(x => x.Quantitydamage * x.ItemCost).DefaultIfEmpty(0).Sum();
                    //اجمالى التكالبف والمصروفات

                    orderExpenseCost = orderExpenses.Sum(x => x.ExpenseAmount);
                    //اجمالى تكلفة امر الانتاج
                    totalCost = itemInCost + damagesCost + orderExpenseCost;
                    //تكلفة الوحدة الواحدة
                    //finalItemCost = totalCost / productionOrder.OrderQuantity;
                    return new ResultMethods { IsValid = true, MaterialItemCost = itemInCost ?? 0, DamagesCost = damagesCost ?? 0, OrderExpenseCost = orderExpenseCost, TotalCost = totalCost ?? 0, FinalItemCost = finalItemCost ?? 0 };

                }
            }
        }
        public class ResultMethods
        {
            public bool IsValid { get; set; }
            public double MaterialItemCost { get; set; }
            public double DamagesCost { get; set; }
            public double OrderExpenseCost { get; set; }
            public double TotalCost { get; set; }
            public double FinalItemCost { get; set; }
        }
        #endregion

        #region تقرير موظفى خطوط الانتاج
        public static List<RptProductionLineDto> GetProductionOrderByEmployee(Guid? branchId, Guid? employeeId,DateTime? dtFrom,DateTime?dtTo)
        {
            using (var db=new VTSaleEntities())
            {
                IQueryable<ProductionOrder> productionOrders = null;
                if (employeeId != null&&branchId!=null)
                {
                    productionOrders = db.ProductionOrders.Where(x => !x.IsDeleted && x.ProductionLineId != null&&x.BranchId==branchId);
                    if (dtFrom!=null&&dtTo!=null)
                        productionOrders = productionOrders.Where(x => DbFunctions.TruncateTime(x.ProductionOrderDate) >= dtFrom && DbFunctions.TruncateTime(x.ProductionOrderDate) <= dtTo);
                    var t = productionOrders.ToList();
                    return productionOrders.ToList().Select(x => new RptProductionLineDto
                    {
                        ProductionId=x.Id,
                        ProductionOrderDate=x.ProductionOrderDate.ToString(),
                        ProductionNumber=x.OrderNumber,
                        ProductionLineName=x.ProductionLine.Name,
                        ProductionLineId=x.ProductionLineId,
                        EmployeeCost =x.ProductionLine.ProductionLineEmployees.Where(e=>!e.IsDeleted&&e.EmployeeId==employeeId).Sum(e=>(double?)e.HourlyWage??0 ) * x.ProductionOrderHours
                    }).ToList();
                }
                else
                    return new List<RptProductionLineDto>();
            }
        }
        #endregion
        #region تقرير خطوط الانتاج
        public static List<RptProductionLineDto> GetProductionOrderByProLine(Guid? branchId, Guid? productionLineId, DateTime? dtFrom,DateTime?dtTo)
        {
            using (var db=new VTSaleEntities())
            {
                IQueryable<ProductionOrder> productionOrders = null;
                if (productionLineId != null&&branchId!=null)
                {
                    productionOrders = db.ProductionOrders.Where(x => !x.IsDeleted && x.ProductionLineId != null&&x.BranchId==branchId);
                    if (dtFrom!=null&&dtTo!=null)
                        productionOrders = productionOrders.Where(x => DbFunctions.TruncateTime(x.ProductionOrderDate) >= dtFrom && DbFunctions.TruncateTime(x.ProductionOrderDate) <= dtTo);
                    return productionOrders.Select(x => new RptProductionLineDto
                    {
                        ProductionId=x.Id,
                        ProductionOrderDate=x.ProductionOrderDate.ToString(),
                        ProductionNumber=x.OrderNumber,
                        ProductionLineName=x.ProductionLine.Name,
                        ProductionLineId=x.ProductionLineId,
                        EmployeeCost =x.ProductionLine.ProductionLineEmployees.Where(e=>!e.IsDeleted).Sum(e=>(double?)e.HourlyWage??0 ) * x.ProductionOrderHours
                    }).ToList();
                }
                else
                    return new List<RptProductionLineDto>();
            }
        }
        #endregion
    }
}