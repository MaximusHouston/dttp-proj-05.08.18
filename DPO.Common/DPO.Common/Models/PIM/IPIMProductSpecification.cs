
namespace DPO.Common
{
    public interface IPIMProductSpecification
    {
        string AttributeID { get; set; }

        object Data { get; set; }

        object RawData { get; set; }

        bool UseValueID { get; set; }

        string IDPattern { get; set; }
    }
}