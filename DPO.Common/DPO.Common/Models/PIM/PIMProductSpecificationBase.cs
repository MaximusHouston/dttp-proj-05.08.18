 

namespace DPO.Common
{
    public class PIMProductSpecificationBase<T> : IPIMProductSpecification
    {
        public string AttributeID { get; set; }

        public bool UseValueID { get; set; }

        public string IDPattern { get; set; }

        public string Text { get; set; }

        public T Value { get; set; }

        public T RawValue { get; set; }

        public object Data
        {
            get
            {
                return Value;
            }
            set
            {
                Value = (T)value;
            }
        }

        public object RawData
        {
            get
            {
                return RawValue;
            }
            set
            {
                RawValue = (T)value;
            }
        }

       
    }
}
