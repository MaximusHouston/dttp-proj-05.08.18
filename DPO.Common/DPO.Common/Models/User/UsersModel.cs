//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
using System;
using System.Collections.Generic;

namespace DPO.Common
{
   public class UsersModel : SearchUser
   {
       public UsersModel() { }

      public UsersModel(ISearch model) : base(model)
      {
         Items = new PagedList<UserListModel>(new List<UserListModel>(), 1, 25);
      }

      public PagedList<UserListModel> Items { get; set; }

   }
}
