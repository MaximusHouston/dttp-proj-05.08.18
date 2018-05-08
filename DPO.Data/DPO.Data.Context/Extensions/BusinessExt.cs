using DPO.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Data
{
   [DebuggerDisplay("BusinessName={BusinessName} BusinessType={BusinessType}")]
   public partial class Business
   {
       public bool IsWebServiceImport { get; set; }


   }
}
