 
using System.Collections.Generic;

namespace DPO.Common
{
    public class ProductSpecificationModel
    {
        public short Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public long? ProductId { get; set; }
        public string Value { get; set; }
    }

    public class ProductSpecificationsModel
    {
        public ProductSpecificationsModel()
            : base()
        {
        }

        public Dictionary<string, ProductSpecificationModel> All { get; set; }
        public string Name { get; set; }
        public bool NoRecords
        {
            get
            {
                return (All == null || All.Count == 0);
            }
        }

        public List<ProductModel> SubProducts { get; set; }
        public string GetDecimal(string name, string format)
        {
            var result = GetDecimal(name);

            if (result == null) return null;

            return string.Format("{0:" + format + "}", result);
        }

        public decimal? GetDecimal(string name)
        {
            string value = GetString(name);

            decimal result;

            if (decimal.TryParse(value, out result))
            {
                return (decimal?)result;
            }

            return null;
        }

        public short GetId(string name)
        {
            var result = GetMatchingSpecification(name);

            return result != null ? result.Id : (short)-1;
        }

        public string GetKey(string name)
        {
            var result = GetMatchingSpecification(name);

            return result != null ? result.Key : null;
        }

        public string GetString(string name)
        {
            var result = GetMatchingSpecification(name);

            return result != null ? result.Value : null;
        }

        public string GetValue(string name)
        {
            var result = GetMatchingSpecification(name);

            return result != null ? result.Value : null;
        }

        private ProductSpecificationModel GetMatchingSpecification(string name)
        {
            ProductSpecificationModel result = null;

            if (All != null)
            {
                if (All.TryGetValue(name, out result))
                {
                    return result;
                }
            }

            // If main not found then look at sub components (if any)
            var i = 0;
            if (SubProducts != null)
            {
                while (i < SubProducts.Count && result == null && SubProducts[i].Specifications != null)
                {
                    result = SubProducts[i].Specifications.GetMatchingSpecification(name);
                    i++;
                }
            }

            return result;
        }
    }
}
