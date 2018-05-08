//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Common
{
    public class ValidationMessages
    {
        public static string Must_Have_Value(string desc)
        {
            return string.Format("{0} is required.", desc);
        }

        public static string Too_Many_Characters(string desc,int length )
        {
            return string.Format("{0} has too many characters, maximum allowed is {1}.",desc, length);
        }


        public static string Too_Few_Characters(string desc, int length)
        {
            return string.Format("{0} needs to have at least {1} characters.", desc, length);
        }
        
        public static string Phone_Number_Invalid_Format(string desc)
        {
            return string.Format("{0} is not valid. {1} are examples of valid formats.", desc, Resources.ResourceUI.PhoneNumberExamples);
        }

        public static string Postalcode_Invalid_Format(string desc)
        {

            return string.Format("{0} is not valid code. {1} are examples of valid formats.", Resources.ResourceUI.ZipCode, Resources.ResourceUI.ZipCodeExamples);
        }

        public static string Email_Invalid_Format(string desc)
        {
            return string.Format("{0} needs to be in a recognised format. 'joeblogs@helpme.com','joebloggs@helpme.co.uk' are examples of valid formats.", desc);
        }

        public static string URL_Invalid_Format(string desc)
        {
            return string.Format("{0} needs to be in a recognised format. 'www.google.com','www.cissportal.net' are examples of valid formats.", desc);
        }

        public static string Concurrency_Failure
        {
            get { return "Unable to update. The information that was shown has subsequently changed please refresh and try again."; }
        }

        

        public static string Date_Invalid_Format(string desc)
        {
            return string.Format("{0} format invalid. Format required is dd/mm/yyyy.", desc);
        }

        public static string Aleady_Exists(string desc)
        {
            return string.Format("{0} already exists.",desc);
        }

        public static string Date_Out_Of_Range(string desc, DateTime from,DateTime to)
        {
             return string.Format("{0} needs to be between {1} and {2}.",desc,from.ToString("dd/MM/yyyy"),to.ToString("dd/MM/yyyy")) ; 
        }

        public static string Number_Out_Of_Range(string desc, decimal from, decimal to)
        {
            return string.Format("{0} needs to be between {1} and {2}.",desc, from, to);
        }

        public static string Date_Invalid_Format(string desc, DateTime from, DateTime to)
        {
            return string.Format("{0} needs to be between {1} and {2}.",desc, from.ToString("dd/MM/yyyy"), to.ToString("dd/MM/yyyy"));
        }

        public static string Number_Invalid_Format(string desc)
        {
            return string.Format("{0} needs to be a number.", desc);
        }

        public static string Cannot_Contain_Commas(string desc)
        {
            return string.Format("{0} cannot contain commas.", desc);
        }

        public static string No_Data_Entered
        {
            get { return "Please enter the required information."; }
        }

        public static string Please_Enter_More_Information(string desc)
        {
            return string.Format("Please enter a least one of the highlighted fields in the {0} section.", desc);
        }

        public static string File_Not_An_Image
        {
            get { return "File is not an image file."; }
        }

        public static string File_Not_Uploaded
        {
            get { return "No file has been uploaded. Please upload a file."; }
        }

        public static string File_Wrong_Type(string allowedTypes)
        {
            return string.Format("File type is not in the correct format. File types allowed are {0} .", allowedTypes);
        }

        public static string File_Too_Big(int maxSizeBytes)
        {
            return string.Format("File is too big. Maximum size allowed is {0} bytes (~{1}k).", maxSizeBytes, maxSizeBytes / 1024);
        }

   }
}
