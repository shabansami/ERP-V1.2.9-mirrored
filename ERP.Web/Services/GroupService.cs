using ERP.DAL;
using ERP.Web.Utilites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.Services
{
    public static class GroupService
    {
        #region Get Groups Items
        //======== old way recursion
        // public static List<DrawTree> GetItemGroupTree(int? Id)
        // {
        //     IQueryable<Group> parents;
        //     var parentsDTO = new List<DrawTree>();

        //     using (var db=new VTSaleEntities())
        //     {
        //         parents = db.Groups.Where(x => !x.IsDeleted && x.GroupTypeId == Id && x.ParentId == null);
        //         foreach (var item in parents)
        //         {
        //             parentsDTO.Add(new DrawTree()
        //             {
        //                 id = item.Id,
        //                 title = item.Name,
        //                 subs = Recur(item)
        //             });
        //         }
        //     }

        //     return parentsDTO;
        // }
        //static List<DrawTree> Recur(Group group)
        // {
        //     if (group != null && group.Groups1.Count(x => !x.IsDeleted) > 0)
        //         return group.Groups1.Where(x => !x.IsDeleted).AsEnumerable().Select(x => new DrawTree() { id = x.Id, ParentId = x.ParentId, title = x.Name, subs = Recur(x) }).ToList();
        //     else
        //         return new List<DrawTree>();
        // }       
        public static List<DrawTree> GetItemGroupTree(int? Id)
        {
            try
            {
                List<DrawTree> parentsDTO = new List<DrawTree>();
                //List<Group> parents = new List<Group>();

                using (var db = new VTSaleEntities())
                {
                    var parents = db.Groups.Where(x => !x.IsDeleted && x.GroupTypeId == Id).ToList();
                    parentsDTO = ChildrenGroupsOf(parents, null);
                }
                return parentsDTO;


            }
            catch (Exception ex)
            {

                throw;
            }

        }
        static List<DrawTree> ChildrenGroupsOf(List<Group> groups, Guid? parentId)
        {
            try
            {
                var groupsTree = groups.Where(x => x.ParentId == parentId);
                return groupsTree.Select(y => new DrawTree
                {
                    id = y.Id,
                    ParentId = y.ParentId,
                    title = y.Name,
                    subs = ChildrenGroupsOf(groups, y.Id)
                }).ToList();

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        #endregion

    }
}