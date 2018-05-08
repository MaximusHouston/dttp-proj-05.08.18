 

namespace DPO.Common
{
    public class ProductListModel : SearchProduct
    {
        public ProductListModel() : base() {
            Product = new ProductModel();
        }

        public ProductListModel(ISearch search) : base(search) {}

        public ProductModel Product { get; set; }

        public new string ProductClassCode { get; set; }        
    }
}
