//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
 
namespace DPO.Common
{
  public class Helpers
  {
      public static string DisplayName(IUser user)
      {
         return DisplayName(user.FirstName, user.MiddleName, user.LastName);
      }

      public static string DisplayName(string firstName, string middleName, string lastName)
      {
          string result = string.Empty;
          result += firstName + (!string.IsNullOrEmpty(firstName) ? " " : string.Empty);
          result += middleName + (!string.IsNullOrEmpty(middleName) ? " " : string.Empty);
          result += lastName + (!string.IsNullOrEmpty(lastName) ? " " : string.Empty);
          return result.Trim();
      }
  }
}
