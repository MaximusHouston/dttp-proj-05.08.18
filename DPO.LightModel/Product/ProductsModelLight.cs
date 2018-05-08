using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPO.Common;

namespace DPO.Model.Light
{
    public class ProductsModelLight : SearchProduct
    {
        public ProductsModelLight(SearchProduct search)
            : base(search)
        {
            this.ProductFamilyId = search.ProductFamilyId;

            this.ProductSubFamilyId = search.ProductSubFamilyId;

            this.ProductTypeId = search.ProductTypeId;

            this.ProductModelTypeId = search.ProductModelTypeId;

            this.ProductPowerVoltageTypeId = search.ProductPowerVoltageTypeId;

            this.CoolingCapacityRatedMax = search.CoolingCapacityRatedMax;
            this.CoolingCapacityRatedMin = search.CoolingCapacityRatedMin;

            this.HeatingCapacityRatedMax = search.HeatingCapacityRatedMax;
            this.HeatingCapacityRatedMin = search.HeatingCapacityRatedMin;

            this.SEERNonDuctedMax = search.SEERNonDuctedMax;
            this.SEERNonDuctedMin = search.SEERNonDuctedMin;

            this.IEERNonDuctedMax = search.IEERNonDuctedMax;
            this.IEERNonDuctedMin = search.IEERNonDuctedMin;

            this.EERNonDuctedMax = search.EERNonDuctedMax;
            this.EERNonDuctedMin = search.EERNonDuctedMin;

            this.HSPFNonDuctedMax = search.HSPFNonDuctedMax;
            this.HSPFNonDuctedMin = search.HSPFNonDuctedMin;

            this.COP47NonDuctedMax = search.COP47NonDuctedMax;
            this.COP47NonDuctedMin = search.COP47NonDuctedMin;


            this.ProductHeatExchangerTypeId = search.ProductHeatExchangerTypeId;

            this.UnitInstallationTypeId = search.UnitInstallationTypeId;

            this.ProductClassPIMId = search.ProductClassPIMId;

            this.ProductCategoryId = search.ProductCategoryId;

            this.CoolingCapacityRatedValue = search.CoolingCapacityRatedValue;
            this.CoolingCapacityNominalValue = search.CoolingCapacityNominalValue;
            this.HeatingCapacityRatedValue = search.HeatingCapacityRatedValue;
            this.AirFlowRateHighValue = search.AirFlowRateHighValue;


        }

        public ProductsModelLight()
        {
            Products = new PagedList<ProductListModel>(new List<ProductListModel>(), 1, 25);
        }

        public bool IsSearch { get; set; }
        public List<ProductListModel> Products { get; set; }
    }
}