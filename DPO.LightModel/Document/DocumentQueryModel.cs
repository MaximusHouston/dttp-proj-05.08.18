using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPO.Common;

namespace DPO.Model.Light
{
    public class DocumentQueryModel : SearchDocument
    {
        public DocumentQueryModel() {
            Documents = new List<DocumentModel>();
        }
        public List<DocumentModel> Documents;
    }
}
