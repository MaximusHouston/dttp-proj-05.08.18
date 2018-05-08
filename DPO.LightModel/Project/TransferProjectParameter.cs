using DPO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Model.Light
{
    [Serializable]
    public class TransferProjectParameter
    {
        public TransferProjectParameter()
        {

        }

        public long ProjectId { get; set; }
        public string Email { get; set; }
    }
}
