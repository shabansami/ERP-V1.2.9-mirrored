using ERP.DAL;
using ERP.Web.Utilites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.Services
{
    public static class UploadCenterService
    {
        public static bool selected { get; set; } = true;

        #region Upload Center Folder
        public static List<DrawTree> GetUploadCenter(bool selectedLevel = false)
        {
            selected = selectedLevel;
            List<DrawTree> uploadCenters = new List<DrawTree>();
            using (var db = new VTSaleEntities())
            {
                //-================== 4 ============================
                //if (onlyFolder)
                //    parentss = db.UploadCenters.Where(x => !x.IsDeleted && x.IsFolder).ToList();
                //else
                //    parentss = db.UploadCenters.Where(x => !x.IsDeleted).ToList();
                var parentss = db.UploadCenters.Where(x => !x.IsDeleted && x.IsFolder).ToList();
                uploadCenters = ChildrenOfLevel(parentss, null);
            }
            return uploadCenters;
        }
        public static List<DrawTree> ChildrenOfLevel(List<UploadCenter> uploadCenters, Guid? parentId)
        {
            var levelChildrens = uploadCenters.Where(i => i.ParentId == parentId && !i.IsDeleted);
            if (selected)
            {
                return levelChildrens.Select(i => new DrawTree
                {
                    id = i.Id,
                    title = i.Name,
                    ParentId = i.ParentId,
                    SelectedTree = i.IsFolder
                    ,
                    subs = ChildrenOfLevel(uploadCenters, i.Id)
                })
                .ToList();
            }
            else
            {
                return levelChildrens.Select(i => new DrawTree
                {
                    id = i.Id,
                    title = i.Name,
                    ParentId = i.ParentId,
                    SelectedTree = true,
                    subs = ChildrenOfLevel(uploadCenters, i.Id)
                })
                .ToList();
            }

        }


        #endregion

    }
}