 

namespace DPO.Common
{
    public class PIMProductReference
    {
        public string ReferenceType { get; set; }
        public string ComponentRequirementTypeID { get; set; }
        public string ParentProductNumber { get; set; }
        public string ProductNumber { get; set; }
        public int Quantity { get; set; }
        public string ParentLinkedId { get; set; }

        public PIMProductReference ShallowCopy()
        {
            return (PIMProductReference)this.MemberwiseClone();
        }
    }
}
