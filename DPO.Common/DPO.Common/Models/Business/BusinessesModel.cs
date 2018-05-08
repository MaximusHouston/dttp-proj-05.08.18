//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
 
using System.Collections.Generic;

namespace DPO.Common
{
   public class BusinessesModel : SearchBusiness
   {

      public BusinessesModel()
      {
          Items = new PagedList<BusinessListModel>(new List<BusinessListModel>(), 1, Common.Constants.DEFAULT_USER_DISPLAYSETTINGS_PAGESIZE);
      }

      public PagedList<BusinessListModel> Items { get; set; }

   }
}
