using ERP.Desktop.DTOs;
using ERP.DAL;
using ERP.Desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ERP.Desktop.Services.Employees;

namespace ERP.Desktop.Services.Settings
{
    internal class ShiftServices
    {
        VTSaleEntities dbContext = DBContext.UnitDbContext;
        public List<ShiftVM> GetAllNotClosedShifts()
        {
            var branches = EmployeeService.GetBranchesByUser(UserServices.UserInfo);
            var shifts = dbContext.ShiftsOfflines.Where(x => !x.IsDeleted && !x.IsClosed).ToList().Where(x=>branches.Any(b=>b.ID==x.PointOfSale.BrunchId)).ToList();

            return shifts.Select(x => new ShiftVM()
            {
                ID = x.Id,
                Date = x.Date,
                ShiftNumber = x.ShiftNumber,
                EmpID = x.EmployeeID,
                EmpName = x.Employee.Person.Name,
                POSID = x.PointOfSaleID,
                POSName = x.PointOfSale.Name
            }).ToList();
        }
        public List<ShiftVM> GetAllClosedShifts(DateTime dtFrom, DateTime dtTo)
        {
            var dbContext = DBContext.UnitDbContext;
            var branches = EmployeeService.GetBranchesByUser(UserServices.UserInfo);
            var shifts = dbContext.ShiftsOfflines.Where(x => !x.IsDeleted && x.IsClosed && DbFunctions.TruncateTime(x.ClosedOn) >= dtFrom && DbFunctions.TruncateTime(x.ClosedOn) <= dtTo).ToList().Where(x => branches.Any(b => b.ID == x.PointOfSale.BrunchId)).ToList();


            return shifts.Select(x => new ShiftVM()
            {
                ID = x.Id,
                ShiftNumber = x.ShiftNumber,
                Date = x.Date,
                EmpID = x.EmployeeID,
                EmpName = x.Employee.Person.Name,
                POSID = x.PointOfSaleID,
                POSName = x.PointOfSale.Name
            }).ToList();
        }
        public DtoResultObj<ShiftsOffline> AddNewShift(ShiftsOffline shift)
        {
            try
            {
                shift.ShiftNumber = Properties.Settings.Default.CodePrefix + (dbContext.ShiftsOfflines.Count() + 1);
                dbContext.ShiftsOfflines.Add(shift);
                dbContext.SaveChanges(UserServices.UserInfo.UserId);
                return new DtoResultObj<ShiftsOffline>() { IsSuccessed = true , Object = shift };
            }
            catch (Exception ex)
            {
                return new DtoResultObj<ShiftsOffline>() { IsSuccessed = false, Message = ex.Message };
            }
        }

        public DtoResult CloseShift(Guid shiftID)
        {
            try
            {
                var shift = dbContext.ShiftsOfflines.FirstOrDefault(x => x.Id == shiftID && !x.IsDeleted && !x.IsClosed);
                if (shift != null)
                {
                    shift.IsClosed = true;
                    dbContext.SaveChanges(UserServices.UserInfo.UserId);
                    return new DtoResult() { IsSuccessed = true };
                }
                return new DtoResult() { IsSuccessed = false, Message = "Shift Not Found or already closed" };
            }
            catch (Exception ex)
            {
                return new DtoResult() { IsSuccessed = false, Message = ex.Message };
            }
        }
    }
}
