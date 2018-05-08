//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using EntityFramework.Extensions;
using DPO.Common;
using System.Reflection;

namespace DPO.Data
{

   public partial class Repository 
   {
     

      public void SystemRoutinesUpdateMembersCountForGroups()
      {
          this.Context.spUpdateMembersCountForGroups();
      }


   }
}