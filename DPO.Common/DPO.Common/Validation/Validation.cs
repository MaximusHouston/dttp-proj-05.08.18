using DPO.Resources;
//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DPO.Common
{
   public class Validation
   {

      static public bool IsPasswordConfirmed(Messages messages,string password,string confirmpassword, string propertyName)
      {


         string result = IsPasswordConfirmed(password, confirmpassword);
         if (result != null)
         {
            messages.AddError(propertyName, result);
            return false;
         }
         return true;
      }

      static public string IsPasswordConfirmed(string password,string confirmpassword)
      {
          password = (password + "").Trim();
          confirmpassword = (confirmpassword + "").Trim();
            if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(confirmpassword) && string.Compare(password, confirmpassword) != 0)
            {
                return ResourceModelUser.MU010;
            }
         return null;
      }

      static public bool IsText(Messages messages, string value, string property, int maxLength, bool required)
      {
         return IsText(messages, value, property, property, maxLength, required);
      }

      static public bool IsText(Messages messages, string value, string propertyName, string desc, int maxLength, bool required)
      {
         string result = IsText(value, desc, 0, maxLength, required);

         if (result != null)
         {
            messages.Add(MessageTypeEnum.Error, propertyName, result);
            return false;
         }
         return true;
      }

      static public bool IsText(Messages messages, string value, string propertyName, string desc, int minLength, int maxLength, bool required)
      {
         string result = IsText(value, desc, minLength, maxLength, required);
         if (result != null)
         {
            messages.AddError(propertyName, result);
            return false;
         }
         return true;
      }

      static public string IsText(string value, string desc, int minLength, int maxLength, bool required)
      {
          value = (value + "").Trim();

         if (string.IsNullOrEmpty(value))
         {
            if (required)
            {
               return ValidationMessages.Must_Have_Value(desc);
            }
            else
            {
               return null;
            }
         }

         if (value.Length > maxLength)
         {
            return ValidationMessages.Too_Many_Characters(desc, maxLength);
         }

         if (value.Length < minLength)
         {
            return ValidationMessages.Too_Few_Characters(desc, minLength);
         }

         return null;
      }

      static public bool IsInDateRange(Messages messages, DateTime? value, string propertyName, string desc, DateTime from, DateTime to, bool required)
      {
         if (!value.HasValue)
         {
            if (required)
            {
               messages.AddError(propertyName, ValidationMessages.Must_Have_Value(desc));
               return false;
            }
            else
            {
               return true;
            }
         }

         if (value.HasValue && (value.Value < from || value.Value > to))
         {
            messages.AddError(propertyName, ValidationMessages.Date_Out_Of_Range(desc, from, to));
            return false;
         }

         return true;
      }

      static public bool IsDateSet(Messages messages, DateTime? value, string propertyName, string desc)
      {
          if (!value.HasValue)
          {
              messages.AddError(propertyName, ValidationMessages.Must_Have_Value(desc));
              return false;
          }
          else
          {
              return true;
          }
      }

      static public bool IsDropDownSet(Messages messages, int? value, string propertyName, string desc)
      {
          if (!value.HasValue || value.Value <= 0)
          {
              messages.AddError(propertyName, ValidationMessages.Must_Have_Value(desc));
              return false;
          }
          else
          {
              return true;
          }
      }

      static public bool IsWholeNumber(string value)
      {
         int iValue;

         if (int.TryParse(value, out iValue))
         {
            return true;
         };

         return (iValue <= 0);

      }

      static public bool IsNumber(string value)
      {
         int iValue;

         return int.TryParse(value, out iValue);
      }

      static public bool IsDecimal(Messages messages, string value, string propertyName, string desc, bool required)
      {
          value = (value + "").Trim();

         if (string.IsNullOrEmpty(value))
         {
            if (required)
            {
               messages.AddError(propertyName, ValidationMessages.Must_Have_Value(desc));
               return false;
            }
            else
            {
               return true;
            }
         }

         if (!IsDecimal(value))
         {
            messages.AddError(propertyName, ValidationMessages.Number_Invalid_Format(desc));
            return false;
         }

         return true;
      }

      static public bool IsDecimal(string value)
      {
         decimal iValue;

         return decimal.TryParse(value, out iValue);
      }

       static public bool IsWholeNumber(Messages messages, string value, string propertyName, string desc, bool required)
      {
         if (string.IsNullOrEmpty(value))
         {
            if (required)
            {
               messages.AddError(propertyName, ValidationMessages.Must_Have_Value(desc));
               return false;
            }
            else
            {
               return true;
            }
         }

         if (!IsWholeNumber(value))
         {
            messages.AddError(propertyName, ValidationMessages.Number_Invalid_Format(desc));
            return false;
         }

         return true;
      }

       static public bool IsPhoneNumber(Messages messages, string value, string propertyName, string desc, bool required, string country)
      {
          string result = IsPhoneNumber(value, desc, required,country);
         if (result != null)
         {
            messages.AddError(propertyName, result);
            return false;
         }
         return true;
      }

       static public string IsPhoneNumber(string value, string desc, bool required, string country)
       {
           value = (value + "").Trim();
           var saveCulture = Thread.CurrentThread.CurrentCulture;

           if (string.IsNullOrEmpty(value))
           {
               if (required)
               {
                   return ValidationMessages.Must_Have_Value(desc);
               }
               return null;
           }

           if (string.IsNullOrEmpty(country)) country = "US";
           Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-" + country);
           try
           {


               if (value.Length > 25)
               {
                   return ValidationMessages.Phone_Number_Invalid_Format(desc);
               }

               Regex pattern = new Regex(Resources.ResourceUI.TelephoneRegEx);

               if (!pattern.IsMatch(value.Trim()))
               {
                   return ValidationMessages.Phone_Number_Invalid_Format(desc);

               }
               return null;
           }
           finally
           {
               Thread.CurrentThread.CurrentCulture = saveCulture;
           }
       }

      static public bool IsPostalCode(Messages messages, string value, string propertyName,bool required, string country)
      {
         string result = IsPostalCode(value, required,country);
         if (result != null)
         {
            messages.AddError(propertyName, result);
            return false;
         }
         return true;
      }
      static public string IsPostalCode(string value,bool required, string country)
      {
          value = (value + "").Trim();

          var saveCulture = Thread.CurrentThread.CurrentCulture;



          if (string.IsNullOrEmpty(country)) country = "US";
          Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-" + country);
          try
          {
              if (string.IsNullOrEmpty(value))
              {
                  if (required)
                  {
                      return ValidationMessages.Must_Have_Value(Resources.ResourceUI.ZipCode);
                  }
                  return null;
              }

              if (value.Length > 10)
              {
                  return ValidationMessages.Postalcode_Invalid_Format(Resources.ResourceUI.ZipCode);
              }

              Regex pattern = new Regex(Resources.ResourceUI.ZipCodeRegEx);

              if (!pattern.IsMatch(value.Trim()))
              {
                  return ValidationMessages.Postalcode_Invalid_Format(Resources.ResourceUI.ZipCode);
              }

              return null;
          }
          finally
          {
              Thread.CurrentThread.CurrentCulture = saveCulture;
          }
      }

      static public bool IsEmail(Messages messages, string value, string propertyName, string desc, int length, bool required)
      {
         string result = IsEmail(value, desc,length, required);
         if (result != null)
         {
            messages.AddError(propertyName, result);
            return false;
         }
         return true;
      }

      static public string IsEmail(string value, string desc, int length, bool required)
      {

          value = (value + "").Trim();

         if (string.IsNullOrEmpty(value))
         {
            if (required)
            {
               return ValidationMessages.Must_Have_Value(desc);
            }
            return null;
         }

         if (value.Length > length)
         {
            return ValidationMessages.Too_Many_Characters(desc, length);
         }

         Regex email = new Regex(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$");

         if (!email.IsMatch(value.Trim()))
         {
            return ValidationMessages.Email_Invalid_Format(desc);
         }

         return null;
      }

      static public bool IsURL(Messages messages, string value, string propertyName, string desc, int length, bool required)
      {
         string result = IsURL(value, desc, length, required);
         if (result != null)
         {
            messages.AddError(propertyName, result);
            return false;
         }
         return true;

      }

      static public string IsURL(string value, string desc,int length, bool required)
      {
          value = (value + "").Trim();

         if (string.IsNullOrEmpty(value))
         {
            if (required)
            {
               return ValidationMessages.Must_Have_Value(desc);
            }
            return null;
         }

         if (value.Length > length)
         {
            return ValidationMessages.Too_Many_Characters(desc, length);
         }

         Regex url = new Regex(@"(([\w]+:)?//)?(([\d\w]|%[a-fA-f\d]{2,2})+(:([\d\w]|%[a-fA-f\d]{2,2})+)?@)?([\d\w][-\d\w]{0,253}[\d\w]\.)+[\w]{2,4}(:[\d]+)?(/([-+_~.\d\w]|%[a-fA-f\d]{2,2})*)*(\?(&?([-+_~.\d\w]|%[a-fA-f\d]{2,2})=?)*)?(#([-+_~.\d\w]|%[a-fA-f\d]{2,2})*)?");

         if (!url.IsMatch(value.Trim()))
         {
            return ValidationMessages.URL_Invalid_Format(desc);
         }

         return null;

      }




 


   }
}
