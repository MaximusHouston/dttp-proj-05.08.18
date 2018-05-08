//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
using System.Collections.Generic;
using System.Linq;
 

namespace DPO.Common
{
   public class CheckBoxListModel
   {
      public List<CheckBoxModel> List { get; set; }
      public List<CheckBoxModel> Selected { get; set; }
      public string[] PostedIds { get; set; }
      public EntityEnum EntityReferenceId { get; set; }

      public static List<PermissionListModel> ToPermissionListModel(CheckBoxListModel model)
      {

          if (model == null || model.PostedIds == null) return new List<PermissionListModel>();

          var t = model.PostedIds.Select(p => new PermissionListModel { ReferenceId = int.Parse(p), ReferenceEntityId = model.EntityReferenceId, IsSelected = true }).ToList();
          return t;
      }
   }
}
