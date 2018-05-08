//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
 

namespace DPO.Common
{
   public class SearchUser : Search
   {
       public SearchUser(ISearch model):base(model) { }

       public SearchUser() { }

      public long? UserId { get; set; }

      public long? GroupId { get; set; }

      public string Email { get; set; }

      public bool? Enabled { get; set; }

      public bool? Approved { get; set; }

      public bool? Rejected { get; set; }

      public UserTypeEnum? UserTypeId { get; set; }

   }

}