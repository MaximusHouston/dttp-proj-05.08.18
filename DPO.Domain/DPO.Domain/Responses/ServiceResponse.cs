using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPO.Common;
using DPO.Common.Interfaces;
using System.Diagnostics;

namespace DPO.Domain
{
   public class ServiceResponse 
   {
      public ServiceResponse() : base()
      {
         Messages = new Messages();
      }

      public bool HasError { get { return Status != MessageTypeEnum.Success; } }
      public bool IsOK { 
          get { return Status == MessageTypeEnum.Success; }
      }
      public Messages Messages { get; set; }
      public object Model { get; set; }
      public string PropertyReference { get { return Messages.PropertyReference; } set { Messages.PropertyReference = value; } }

      public MessageTypeEnum Status
      {
          get
          {
              if (Messages.HasCriticalErrors) return MessageTypeEnum.Critial;
              if (Messages.HasErrors) return MessageTypeEnum.Error;

              return MessageTypeEnum.Success;
          }
          set
          {
          }
      }

      public void Add(Messages messages)
      {
          this.Messages.Add(messages);
      }

      public void AddCritical(string text)
      {
          this.Messages.Add(MessageTypeEnum.Critial, "", text);
      }

      public void AddError(string text)
      {
          this.Messages.Add(MessageTypeEnum.Error, "", text);
      }

      public void AddError(string property, string text)
      {
          this.Messages.Add(MessageTypeEnum.Error, property, text);
      }

      public void AddSuccess(string text)
      {
          this.Messages.Add(MessageTypeEnum.Success, "", text);
      }

      public List<IMessage> GetMessages(MessageTypeEnum type) { return this.Messages.Items.Where(m => m.Type == type).ToList(); }
   }
}
