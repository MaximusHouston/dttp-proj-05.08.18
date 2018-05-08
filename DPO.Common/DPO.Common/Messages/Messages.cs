//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
using System;
using System.Data.Entity.Core.Objects;
using System.Data;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DPO.Common.Interfaces;
using System.Diagnostics;

namespace DPO.Common
{

   public class Messages : IMessages
   {
      public Messages()
      {
         Items = new List<IMessage>();
      }

      public  List<IMessage> Items { get; set; }

      public bool HasCriticalErrors { get; set; }
      public bool HasErrors { get; set; }

      private string propertyReference;
      public string PropertyReference { get { return propertyReference; } set { propertyReference = (string.IsNullOrEmpty(value) ? "" : (value + ".")); } }

      public void Add(IMessages messages)
      {
         foreach(var msg in messages.Items.ToList())
         {
            this.Add(msg); // must call add and not add range in
         }
      }

      public void Add(MessageTypeEnum type, string key, string text)
      {
         this.Add(new Message
         {
            Type = type,
            Key = (string.IsNullOrEmpty(key) ? "" : PropertyReference) + key,
            Text = text
         });
      }

      public void Add(IMessage message)
      {
         if (message.Type == MessageTypeEnum.Error) this.HasErrors = true;
         if (message.Type == MessageTypeEnum.Critial) this.HasCriticalErrors = true;

         this.Items.Add(message);
        
      }

      public void Clear()
      {
         this.Items.Clear();
         this.HasErrors = false;
         this.HasCriticalErrors = false;
      }

      public void AddError(string text)
      {
         Add(MessageTypeEnum.Error, null, text);
      }

      public void AddWarning(string text)
      {
         Add(MessageTypeEnum.Warning, null, text);
      }

      public void AddInformation(string text)
      {
         Add(MessageTypeEnum.Information, null, text);
      }

      public void AddError(string entityproperty, string text)
      {
         Add(MessageTypeEnum.Error, entityproperty, text);
      }

      public void AddWarning(string entityproperty, string text)
      {
         Add(MessageTypeEnum.Warning, entityproperty, text);
      }

      public void AddInformation(string entityproperty, string text)
      {
         Add(MessageTypeEnum.Error, entityproperty, text);
      }

      public void AddAudit(string text)
      {
         Add(MessageTypeEnum.Audit,null, text);
      }

      public void AddAudit(Exception e)
      {
         //string message = e.Message + ((e.InnerException != null) ? " " +e.InnerException.Message : "");
         //Add(MessageTypeEnum.Audit, null, message);

         //string stack = string.Concat(e.StackTrace);
         //Add(MessageTypeEnum.Audit, null, stack);

         Utilities.ErrorLog(e, null);
      }

      public void AddAudit(string key, string text)
      {
         Add(MessageTypeEnum.Audit, key, text);
      }


   }
}
