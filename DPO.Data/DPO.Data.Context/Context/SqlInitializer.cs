//===================================================================================
// Microsoft patterns & practices
// Silk : Web Client Guidance
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using DPO.Common;

namespace DPO.Data
{
    /// <summary>
    /// A base class for initializing SQL CE databases.
    /// </summary>
    /// <typeparam name="T">The concrete DbContext to use.</typeparam>
    internal abstract class SqlInitializer<T> : IDatabaseInitializer<T> where T : DbContext
    {
        public abstract void InitializeDatabase(T context);

        #region Helpers

        private static string ReplaceDataDirectory(string inputString)
        {
            string str = inputString.Trim();
            if (string.IsNullOrEmpty(inputString) ||
                !inputString.StartsWith("|DataDirectory|", StringComparison.InvariantCultureIgnoreCase))
            {
                return str;
            }
            string data = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            if (string.IsNullOrEmpty(data))
            {
                data = AppDomain.CurrentDomain.BaseDirectory ?? Environment.CurrentDirectory;
            }
            if (string.IsNullOrEmpty(data))
            {
                data = string.Empty;
            }
            int length = "|DataDirectory|".Length;
            if ((inputString.Length > "|DataDirectory|".Length) && ('\\' == inputString["|DataDirectory|".Length]))
            {
                length++;
            }
            return Path.Combine(data, inputString.Substring(length));
        }

        #endregion
    }
}