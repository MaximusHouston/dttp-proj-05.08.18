//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
 
using System.Diagnostics;

namespace DPO.Common
{
   [DebuggerDisplay("Name={Name}")]
   public class CheckBoxModel
   {
       public int?   IntId { set { Id = (value.HasValue) ? value.ToString() : null; } }
       public string Id { get; set; }
       public string Name { get; set; }
   }
}
