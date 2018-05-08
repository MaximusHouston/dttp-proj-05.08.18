//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Common.Interfaces
{
   public interface IMessage
   {
      MessageTypeEnum Type { get; set; }
      string Key { get; set; }
      string Text { get; set; }
   }
}
