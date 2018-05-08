//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using System;

namespace DPO.Common
{
   public class UserResetPasswordModel
   {
      // User info
      public string SecurityKey { get; set; }
      public string Email { get; set; }
      public long   SecurityTicks { get; set; }
      public string NewPassword { get; set; }
      public string ConfirmPassword { get; set; }

      public bool GenerateSecurityKey()
      {
         return GenerateSecurityKey(DateTime.UtcNow);
      }

      public bool GenerateSecurityKey(DateTime time)
      {
         if (Validation.IsEmail(this.Email,"Email",255,true) != null)
         {
            return false;
         }

         this.SecurityKey = Crypto.Encrypt(string.Format("{0}&{1}", Email, time.Ticks));

         return true;
      }

      public bool DecryptSecurityKey()
      {
         if (string.IsNullOrEmpty(SecurityKey))
         {
            return false;
         }

         string[] keyParts = Crypto.Decrypt(SecurityKey).Split('&');

         if (keyParts.Length != 2)
         {
            return false;
         }

         this.Email = keyParts[0];

         long parse;

         if (!long.TryParse(keyParts[1], out parse))
         {
            return false;
         }

         this.SecurityTicks = parse;

         return true;
      }
   }
}
