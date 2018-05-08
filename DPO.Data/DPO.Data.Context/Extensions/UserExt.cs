using DPO.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Data
{
   [DebuggerDisplay("Email={Email} UserType = {UserType}")]
   public partial class User : IUser
   {
       public bool IsRegistering { get; set; }

       public bool IsNewBusiness { get; set; }
   }
}
