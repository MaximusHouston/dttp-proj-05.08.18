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
   [DebuggerDisplay("Key={Key},Text={Text}")]
   public class Message : IMessage
   {
      public MessageTypeEnum Type { get; set; }
      public string Key { get; set; }
      public string Text { get; set; }
   }

}
