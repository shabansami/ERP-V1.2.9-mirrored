using ERP.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class ItemProductionVM
    {
        public ItemProductionVM()
        {
            this.ItemIds = new List<Guid>();
        }

        public string Name { get; set; }
        public List<Guid> ItemIds { get; set; } // الاصناف النهائية فى حالة التصنيع والاوليه فى حالة التقطيع 
        public int? ItemProductionTypeId { get; set; }
        public string DT_DatasourceItems { get; set; } //الاصناف التى بها مقاسات واوازان 
        public string DT_DatasourceDetails { get; set; } //الاصناف التى بها مقاسات واوازان 

    }
}