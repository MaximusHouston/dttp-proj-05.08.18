using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Domain
{
   [Serializable]
   public class DomainServicesException : Exception
   {
      public DomainServicesException()
         : base()
      {
      }

      public DomainServicesException(string message)
         : base(message)
      {
      }

      public DomainServicesException(string message, Exception innerException)
         : base(message, innerException)
      {
      }
   }
}