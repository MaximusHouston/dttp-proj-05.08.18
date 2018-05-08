namespace DPO.Common
{
    public class SearchProduct : Search
    {
        public SearchProduct()
            : base()
        {
        }

        public SearchProduct(ISearch search)
            : base(search)
        {
        }

        public int? BrandId { get; set; }
        public decimal? CoolingCapacityRatedMax { get; set; }
        public decimal? CoolingCapacityRatedMin { get; set; }
        public decimal? CoolingCapacityRatedValue { get; set; }
        public decimal? CoolingCapacityNominalValue { get; set; }
        public decimal? HeatingCapacityRatedValue { get; set; }
        public decimal? AirFlowRateHighValue { get; set; }
        public decimal? COP47NonDuctedMax { get; set; }
        public decimal? COP47NonDuctedMin { get; set; }
        public DropDownModel DropDownCoolingCapacityRated { get; set; }
        public DropDownModel DropDownCoolingCapacityNominal { get; set; }
        public DropDownModel DropDownHeatingCapacityRated { get; set; }
        public DropDownModel DropDownHeatExchangerType { get; set; }
        public DropDownModel DropDownPowerVoltage { get; set; }
        public DropDownModel DropDownProductCategory { get; set; }
        public DropDownModel DropDownSortBy { get; set; }
        public DropDownModel DropDownUnitInstallationType { get; set; }
        public DropDownModel DropDownCompressorType { get; set; }
        public DropDownModel DropDownGasVavleType { get; set; }
        public DropDownModel DropDownMotorType { get; set; }
        public DropDownModel DropDownInstallationConfigurationType { get; set; }
        public DropDownModel DropDownAirFlowRateHigh { get; set; }
        public decimal? EERNonDuctedMax { get; set; }
        public decimal? EERNonDuctedMin { get; set; }
        public bool FormSubmittedPreviously { get; set; }
        public int? ProductHeatExchangerTypeId { get; set; }
        public decimal? HeatingCapacityRatedMax { get; set; }
        public decimal? HeatingCapacityRatedMin { get; set; }
        public decimal? HSPFNonDuctedMax { get; set; }
        public decimal? HSPFNonDuctedMin { get; set; }
        public decimal? IEERNonDuctedMax { get; set; }
        public decimal? IEERNonDuctedMin { get; set; }
        public int? ProductPowerVoltageTypeId { get; set; }
        public int? ProductCategoryId { get; set; }
        public string ProductClassCode { get; set; }
        public int? ProductFamilyId { get; set; }
        public int? ProductSubFamilyId { get; set; }
        public int? ProductTypeId { get; set; }
        public ProductStatusTypeEnum? ProductStatusTypeId { get; set; }
        public long? ProductId { get; set; }
        public ProductModelTypeEnum? ProductModelTypeId { get; set; }
        public UnitInstallationTypeEnum? UnitInstallationTypeId { get; set; }
        public ProductClassPIMEnum? ProductClassPIMId { get; set; }
        public CompressorTypeEnum? ProductCompressorStageId { get; set; }
        public GasValveTypeEnum? ProductGasValveTypeId { get; set; }
        public MotorTypeEnum? ProductMotorSpeedTypeId { get; set; }
        public InstallationConfigurationTypeEnum? ProductInstallationConfigurationTypeId { get; set; }
        public string ProductNumber { get; set; }
        public decimal? SEERNonDuctedMax { get; set; }
        public decimal? SEERNonDuctedMin { get; set; }
        //public int? UnitInstallationTypeId { get; set; }
        public bool IsSearch { get; set; }
    }
}