
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace DPO.Common
{
    [JsonObject(IsReference = false)]
    public class ProductModel : PageModel
    {
        private List<ProductModel> subProducts = new List<ProductModel>();

        public ProductModel()
            : base()
        {
        }

        public List<ProductAccessoryModel> Accessories { get; set; }
        public List<ProductNoteModel> Benefits { get; set; }
        public DocumentModel DimensionalDrawing { get; set; }
        public List<DocumentModel> Documents { get; set; }
        public List<ProductNoteModel> StandardFeatures { get; set; }
        //TODO: Delete after Sep 01, 2017
        //public List<ProductNoteModel> StandardAndFeature { get; set; }
        public List<ProductNoteModel> CabinetFeatures { get; set; }

        public string GetSubmittalSheetTemplateName
        {

            get
            {
                var submittalsheet = "SubmittalTemplate";

                switch (this.SubmittalSheetTypeId)
                {

                    case SubmittalSheetTypeEnum.VRVIndoor:
                    case SubmittalSheetTypeEnum.MultiSplitIndoor:
                        submittalsheet += "_Indoor";
                        break;
                    case SubmittalSheetTypeEnum.MultiSplitOutdoor:
                        submittalsheet += "_Outdoor";
                        break;
                    case SubmittalSheetTypeEnum.SystemCooling:
                    case SubmittalSheetTypeEnum.SystemHP:
                        submittalsheet += "_System";
                        break;
                    case SubmittalSheetTypeEnum.VRVIIIAirCooled:
                        submittalsheet += "_AirCooled";
                        break;
                    case SubmittalSheetTypeEnum.VRVIIIWaterCooled:
                        submittalsheet += "_WaterCooled";
                        break;
                    case SubmittalSheetTypeEnum.Packaged:
                        submittalsheet += "_Packaged";
                        break;
                    case SubmittalSheetTypeEnum.ACAndHP:
                        submittalsheet += "_ACAndHP";
                        break;
                    case SubmittalSheetTypeEnum.CoilsAndAirHandler:
                        submittalsheet += "_CoilsAndAirHandlers";
                        break;
                    case SubmittalSheetTypeEnum.CommercialACAndHP:
                        submittalsheet += "_CommercialACAndHP";
                        break;
                    case SubmittalSheetTypeEnum.CommercialAH:
                        submittalsheet += "_CommercialAH";
                        break;
                    case SubmittalSheetTypeEnum.GasFurnace:
                        submittalsheet += "_GasFurnace";
                        break;
                    case SubmittalSheetTypeEnum.PackagedACAndHP:
                        submittalsheet += "_PackagedACAndHP";
                        break;
                    case SubmittalSheetTypeEnum.PackagedDualFuel:
                        submittalsheet += "_PackagedDF";
                        break;
                    case SubmittalSheetTypeEnum.PackagedGasElectric:
                        submittalsheet += "_PackagedGE";
                        break;

                }

                return submittalsheet;
            }
        }

        public string GetSubmittalSheetTemplateNameV2
        {

            get
            {
                string submittalsheet = "";

                switch (this.SubmittalSheetTypeId)
                {
                    case SubmittalSheetTypeEnum.Controllers:
                    case SubmittalSheetTypeEnum.RTU:
                    case SubmittalSheetTypeEnum.Packaged:
                    //case SubmittalSheetTypeEnum.ACAndHP:
                    //case SubmittalSheetTypeEnum.CoilsAndAirHandler:
                    //case SubmittalSheetTypeEnum.GasFurnace:
                    //case SubmittalSheetTypeEnum.CommercialACAndHP:
                    //case SubmittalSheetTypeEnum.CommercialAH:
                        submittalsheet = "Technical_Specifications";
                        break;
                    case SubmittalSheetTypeEnum.Accessories:
                        submittalsheet = "";
                        break;
                    case SubmittalSheetTypeEnum.Other:
                        submittalsheet = "";
                        break;
                    case SubmittalSheetTypeEnum.SystemHP:
                    case SubmittalSheetTypeEnum.SystemCooling:
                        submittalsheet = "Submittal_Template_System";
                        break;

                    case SubmittalSheetTypeEnum.VRVIndoor:
                    case SubmittalSheetTypeEnum.MultiSplitIndoor:
                        submittalsheet = "Submittal_Template_Indoor";
                        break;
                    case SubmittalSheetTypeEnum.MultiSplitOutdoor:
                        submittalsheet = "Submittal_Template_Outdoor";
                        break;
                    case SubmittalSheetTypeEnum.VRVIIIAirCooled:
                        submittalsheet = "Submittal_Template_AirCooled";
                        break;
                    case SubmittalSheetTypeEnum.VRVIIIWaterCooled:
                        submittalsheet = "Submittal_Template_WaterCooled";
                        break;
                    case SubmittalSheetTypeEnum.ACAndHP:
                        submittalsheet = "Submittal_Template_ACAndHP";
                        break;
                    case SubmittalSheetTypeEnum.CoilsAndAirHandler:
                        submittalsheet = "Submittal_Template_CoilsAndAirHandler";
                        break;
                    case SubmittalSheetTypeEnum.GasFurnace:
                        submittalsheet = "Submittal_Template_GasFurnace";
                        break;
                    case SubmittalSheetTypeEnum.CommercialACAndHP:
                        submittalsheet = "Submittal_Template_CommercialACAndHP";
                        break;
                    case SubmittalSheetTypeEnum.CommercialAH:
                        submittalsheet = "Submittal_Template_CommercialAH";
                        break;
                    case SubmittalSheetTypeEnum.PackagedACAndHP:
                        submittalsheet = "Submittal_Template_PackagedACAndHP";
                        break;
                    case SubmittalSheetTypeEnum.PackagedDualFuel:
                        submittalsheet = "Submittal_Template_PackagedDualFuel";
                        break;
                    case SubmittalSheetTypeEnum.PackagedGasElectric:
                        submittalsheet = "Submittal_Template_PackagedGasElectric";
                        break;
                }

                return submittalsheet;
            }
        }

        public ProductModel GetSystemIndoorUnit
        {
            get
            {
                if (IsSystem)
                {
                    var model = this.SubProducts.Where(u => u.ProductModelTypeId == ProductModelTypeEnum.Indoor).FirstOrDefault();
                    return model;
                }
                return null;
            }
        }

        public ProductModel GetSystemOutdoorUnit
        {
            get
            {
                if (IsSystem)
                {
                    var model = this.SubProducts.Where(u => u.ProductModelTypeId == ProductModelTypeEnum.Outdoor).FirstOrDefault();
                    return model;
                }
                return null;
            }
        }

        public DocumentModel Image { get; set; }
        public ProductModel IndoorUnit { get; set; }
        public bool IsSystem
        {
            get
            {
                return (this.ProductModelTypeId == ProductModelTypeEnum.System);
            }

        }

        public bool IsSystemTemplate
        {
            get
            {
                return (this.SubmittalSheetTypeId == SubmittalSheetTypeEnum.SystemCooling ||
                        this.SubmittalSheetTypeId == SubmittalSheetTypeEnum.SystemCooling);
            }

        }

        public List<DocumentModel> Logos { get; set; }
        public string Name { get; set; }
        public List<ProductNoteModel> Notes { get; set; }
        public ProductModel OutdoorUnit { get; set; }
        public long? ParentProductId { get; set; }
        public List<ProductModel> ParentProducts { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:C}")]
        public decimal Price { get; set; }

        public long? ProductBrandId { get; set; }
        public string ProductBrandName { get; set; }

        public int ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }

        public int? ProductFunctionCategoryId { get; set; }
        public string ProductFunctionCategoryName { get; set; }

        public string ProductClassCode { get; set; }
        public int ProductFamilyId { get; set; }
        public string ProductFamilyName { get; set; }
        public int? ProductSubFamilyId { get; set; }
        public string ProductSubFamilyName { get; set; }
        public int? ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public List<TabModel> ProductFamilyTabs { get; set; }
        public long? ProductId { get; set; }
        public string ProductModelTypeDescription { get; set; }
        public ProductModelTypeEnum ProductModelTypeId { get; set; }
        //public UnitInstallationTypeEnum? UnitInstallationTypeId { get; set; }
        public string ProductNumber { get; set; }
        public List<ProductSpecificationModel> ProductSpecifications { get; set; }
        public decimal? ProductPowerVoltageTypeId { get; set; }
        public decimal? AirFlowRateHighCooling { get; set; }
        public decimal? AirFlowRateHighHeating { get; set; }
        public string ProductPowerVoltageTypeDescription { get; set; }
        public decimal? Tonnage { get; set; }
        public decimal? HeatingCapacityRated { get; set; }
        public decimal? CoolingCapacityRated { get; set; }
        public decimal? CoolingCapacityNominal { get; set; }
        public decimal? EERNonducted { get; set; }
        public decimal? IEERNonDucted { get; set; }
        public decimal? SEERNonducted { get; set; }
        public decimal? COP47Nonducted { get; set; }
        public decimal? HSPFNonducted { get; set; }
        public int? UnitInstallationTypeId { get; set; }
        public string UnitInstallationTypeDescription { get; set; }
        public int? ProductClassPIMId { get; set; }
        public string ProductClassPIMDescription { get; set; }
        public int? ProductHeatExchangerTypeId { get; set; }
        public string ProductHeatExchangerTypeDescription { get; set; }
        public int? ProductCompressorTypeId { get; set; }
        public string ProductCompressorTypeDescription { get; set; }
        public int? ProductGasValveTypeId { get; set; }
        public string ProductGasValveTypeDescription { get; set; }
        public int? ProductMotorSpeedTypeId { get; set; }
        public string ProductMotorSpeedTypeDescription { get; set; }
        public int? ProductInstallationConfigurationTypeId { get; set; }
        public string ProductInstallationConfigurationTypeDescription { get; set; }
        public int? ProductAccessoryTypeId { get; set; }
        public string ProductAccessoryTypeDescription { get; set; }
        public int? ProductEnergySourceTypeId { get; set; }
        public string ProductEnergySourceTypeDescription { get; set; }

        public int? ProductStatusTypeId { get; set; }
        public string ProductStatusTypeDescription { get; set; }

        public int? InventoryStatusId { get; set; }
        public string InventoryStatusDescription { get; set; }

        public DateTime? InvAvailableDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N0}")]
        public decimal Quantity { get; set; }

        public ProductSpecificationsModel Specifications { get; set; }
        public string SubmittalSheetTypeDescription { get; set; }
        public SubmittalSheetTypeEnum SubmittalSheetTypeId { get; set; }
        public List<ProductModel> SubProducts
        {
            get
            {
                return subProducts;
            }
            set
            {
                subProducts = value;

                //As a reference
                this.Specifications.SubProducts = value;
            }
        }
        public string Tags { get; set; }
        //public List<ProductNoteModel> ProductNotes { get; set; }

        public string UnitCombination
        {
            get
            {
                var unitCombination = "";

                foreach (var sub in this.SubProducts)
                {
                    if (sub.ProductModelTypeId != ProductModelTypeEnum.Accessory && sub.ProductModelTypeId != ProductModelTypeEnum.Other)
                    {
                        unitCombination += ((unitCombination != "") ? " + " : "") + sub.ProductNumber;

                        if (sub.Quantity > 1)
                        {
                            unitCombination += "(x" + sub.Quantity.ToString() + ")";
                        }
                    }
                }

                return unitCombination;

            }
        }

        public long? QuoteId { get; set; }

        public int MultiplierTypeId { get; set; }

        public string CodeString { get; set; }
        public byte? LineItemTypeId { get; set; }
    }
}
