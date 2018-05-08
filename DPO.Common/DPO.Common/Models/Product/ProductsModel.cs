//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
//
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
using System.Collections.Generic;

namespace DPO.Common
{
    public class ProductsModel : SearchProduct
    {
        //enum to change the View of Products
        public enum ProductViewOption
        {
            List,
            Table,
            Grid
        }

        public ProductsModel(SearchProduct search)
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

            this.DropDownCoolingCapacityRated = search.DropDownCoolingCapacityRated;
            this.DropDownCoolingCapacityNominal = search.DropDownCoolingCapacityNominal;
            this.DropDownHeatingCapacityRated = search.DropDownHeatingCapacityRated;

            this.DropDownHeatExchangerType = search.DropDownHeatExchangerType;
            this.DropDownPowerVoltage = search.DropDownPowerVoltage;
            this.DropDownProductCategory = search.DropDownProductCategory;
            this.DropDownSortBy = search.DropDownSortBy;
            this.DropDownUnitInstallationType = search.DropDownUnitInstallationType;
            this.DropDownCompressorType = search.DropDownCompressorType;
            this.DropDownGasVavleType = search.DropDownGasVavleType;
            this.DropDownMotorType = search.DropDownMotorType;
            this.DropDownInstallationConfigurationType = search.DropDownInstallationConfigurationType;
            this.DropDownAirFlowRateHigh = search.DropDownAirFlowRateHigh;
            this.FormSubmittedPreviously = search.FormSubmittedPreviously;
            this.IsSearch = search.IsSearch;
        }

        public ProductsModel()
        {
            Products = new PagedList<ProductListModel>(new List<ProductListModel>(), 1, 25);
        }

        public bool IsFirstPost { get; set; }
        //public bool IsSearch { get; set; }
        public string ProductCategoryName { get; set; }
        public string ProductFamilyName { get; set; }
        public List<TabModel> ProductFamilyTabs { get; set; }
        public List<TabModel> UnitInstallationTypeTabs { get; set; }
        public List<TabModel> ProductClassPIMTabs { get; set; }
        public List<ProductListModel> Products { get; set; }
        public ProductViewOption? ViewOption { get; set; }
    }
}