using DPO.Common;
using DPO.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Domain.Services
{
    public class ProductComponentCalculator : BaseServices
    {
        public ProductComponentCalculator() : base() { }
        public ProductComponentCalculator(DPOContext context)
            : base(context)
        {
        }

        public decimal CalculateProductsComponent(QuoteItem quoteItem, IList<int> modelTypes)
        {
            var listQuery = this.Context.ProductAccessories
                .Where(a => a.ParentProductId == quoteItem.ProductId
                        && a.RequirementTypeId == (int)RequirementTypeEnums.Standard);

            if (modelTypes != null)
            {
                listQuery = listQuery.Where(a => modelTypes.Contains(a.Product.ProductModelTypeId));
            }

            var listCount = listQuery.ToList();

            return listCount.Count > 0 ? listCount.Sum(i => i.Quantity * quoteItem.Quantity) : (quoteItem == null) ? 0 : quoteItem.Quantity;
        }

        private bool LoadProductForQuoteItem(QuoteItem item)
        {
            if (item == null)
            {
                return false;
            }

            if (item.Product == null)
            {
                item.Product = Db.Context.Products
                    .Where(w => w.ProductId == item.ProductId)
                    .FirstOrDefault();
            }

            return item.Product == null ? false : true;
        }

        public int CalculateVRVOutdoor(QuoteItem item)
        {
            if (!LoadProductForQuoteItem(item))
            {
                return 0;
            }

            var prod = item.Product;

            if (prod.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                && prod.ProductFamilyId == (int)ProductFamilyEnum.VRV)
            {
                return CalculateProductsComponent(new QuoteItem[] { item }, new int[] { (int)ProductModelTypeEnum.Outdoor });
            }

            return 0;
        }

        public int CalculateVRVIndoor(QuoteItem item)
        {
            if (!LoadProductForQuoteItem(item))
            {
                return 0;
            }

            var prod = item.Product;

            if (prod.ProductModelTypeId == (int)ProductModelTypeEnum.Indoor
                && prod.ProductFamilyId == (int)ProductFamilyEnum.VRV)
            {
                return CalculateProductsComponent(new QuoteItem[] { item }, new int[] { (int)ProductModelTypeEnum.Outdoor });
            }

            return 0;
        }

        public int CalculateSplitOutdoor(QuoteItem item)
        {
            if (!LoadProductForQuoteItem(item))
            {
                return 0;
            }

            var prod = item.Product;

            if ((prod.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                    || prod.ProductModelTypeId == (int)ProductModelTypeEnum.System)
                && (prod.ProductFamilyId == (int)ProductFamilyEnum.MiniSplit ||
                        prod.ProductFamilyId == (int)ProductFamilyEnum.AlthermaSplit ||
                        prod.ProductFamilyId == (int)ProductFamilyEnum.SkyAir ||
                        prod.ProductFamilyId == (int)ProductFamilyEnum.MultiSplit))
            {
                return CalculateProductsComponent(new QuoteItem[] { item }, new int[] { (int)ProductModelTypeEnum.Outdoor });
            }

            return 0;
        }

        public int CalculateRTU(QuoteItem item)
        {
            if (!LoadProductForQuoteItem(item))
            {
                return 0;
            }

            var prod = item.Product;

            if (prod.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                && (prod.ProductFamilyId == (int)ProductFamilyEnum.LightCommercialSplitSystem || prod.ProductFamilyId == (int)ProductFamilyEnum.LightCommercialPackagedSystem))
            {
                return CalculateProductsComponent(new QuoteItem[] { item });
            }

            return 0;
        }

        public int CalculateProductsComponent(IList<QuoteItem> quoteItems, IList<int> modelTypes = null)
        {
            var results = 0;

            foreach(var quoteItem in quoteItems)
            {
                results += (int)CalculateProductsComponent(quoteItem, modelTypes);
            }

            return results;
        }
    }
}
