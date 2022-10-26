using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DAL.Models;

namespace ERP.DAL
{
    public partial class VTSaleEntities
    {
        public int SaveChanges(Guid? userId)
        {
            try
            {
                //when inserted
                var insertEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Added).ToList();
                // Run through entities and look for a property named UpdateTime
                insertEntities.ForEach(e =>
                {
                    // Tro to get property "UpdateTime"
                    var IdProperty = e.Entity.GetType().GetProperty(nameof(BaseModel.Id));
                    // If the entity has this property then set it
                    if (IdProperty != null && IdProperty.PropertyType == typeof(Guid))
                        // Set to UTC now
                        IdProperty.SetValue(e.Entity, Guid.NewGuid());


                    // Tro to get property "UpdateTime"
                    var updTimeProperty = e.Entity.GetType().GetProperty(nameof(Base.CreatedOn));
                    // If the entity has this property then set it
                    if (updTimeProperty != null)
                        // Set to UTC now
                        updTimeProperty.SetValue(e.Entity, DALUtility.GetDateTime());

                    var userProperty = e.Entity.GetType().GetProperty(nameof(Base.CreatedBy));
                    // If the entity has this property then set it
                    if (userProperty != null)
                        // Set to current user id 
                        userProperty.SetValue(e.Entity, userId);
                });

                var updateEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified).ToList();
                // Run through entities and look for a property named UpdateTime
                updateEntities.ForEach(e =>
                {
                    var isDeleted = (bool)e.Entity.GetType().GetProperty(nameof(Base.IsDeleted)).GetValue(e.Entity);
                    string propertyNameOn = "";
                    string propertyNameBy = "";
                    if (isDeleted)
                    {
                        propertyNameOn = nameof(Base.DeletedOn);
                        propertyNameBy = nameof(Base.DeletedBy);
                    }
                    else
                    {
                        propertyNameOn = nameof(Base.ModifiedOn);
                        propertyNameBy = nameof(Base.ModifiedBy);
                    }
                    // Tro to get property "UpdateTime"
                    var updTimeProperty = e.Entity.GetType().GetProperty(propertyNameOn);
                    // If the entity has this property then set it
                    if (updTimeProperty != null)
                        // Set to UTC now
                        updTimeProperty.SetValue(e.Entity, DALUtility.GetDateTime());

                    var userProperty = e.Entity.GetType().GetProperty(propertyNameBy);
                    // If the entity has this property then set it
                    if (userProperty != null)
                        // Set to current user id 
                        userProperty.SetValue(e.Entity, userId);
                });

                var deleteEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted).ToList();
                // Run through entities and look for a property named UpdateTime
                deleteEntities.ForEach(e =>
                {
                    // Tro to get property "UpdateTime"
                    var updTimeProperty = e.Entity.GetType().GetProperty(nameof(Base.DeletedOn));
                    // If the entity has this property then set it
                    if (updTimeProperty != null)
                        // Set to UTC now
                        updTimeProperty.SetValue(e.Entity, DALUtility.GetDateTime());

                    var userProperty = e.Entity.GetType().GetProperty(nameof(Base.DeletedBy));
                    // If the entity has this property then set it
                    if (userProperty != null)
                        // Set to current user id 
                        userProperty.SetValue(e.Entity, userId);

                });
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }


        }
        public bool IsDisposed { get; set; }
        public new void Dispose()
        {
            IsDisposed = true;
            base.Dispose();
        }
        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
            base.Dispose(disposing);
        }

    }
}
