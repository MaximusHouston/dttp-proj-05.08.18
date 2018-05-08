using DPO.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPO.Data
{
    [Table("DocumentProductLinks")]
    public partial class DocumentProductLink
   {
        private string importProductNumber;
        public string ImportProductNumber 
        {
            get
            {
                if (importProductNumber != null) return importProductNumber;

                if (this.Product != null) return this.Product.ProductNumber;

                return null;

            }
            set
            {
                importProductNumber = value;
            }
        }
   }
}
