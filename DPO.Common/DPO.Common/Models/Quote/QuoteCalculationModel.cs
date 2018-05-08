
using System.ComponentModel.DataAnnotations;

namespace DPO.Common
{
    public class QuoteCalculationModel
    {
        public bool IsGrossMargin { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal NetMaterialValue { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal NetMaterialValueSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal NetMaterialValueVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal NetMaterialValueUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal NetMaterialValueLCPackage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N3}")]
        public decimal NetMultiplier { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N3}")]
        public decimal NetMultiplierSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N3}")]
        public decimal NetMultiplierVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N3}")]
        public decimal NetMultiplierUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N3}")]
        public decimal NetMultiplierLCPackage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalCommissionAmount { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N3}")]
        public decimal TotalCommissionPercentage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalDiscountAmount { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalDiscountAmountSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalCommissionAmountSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalDiscountAmountVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalCommissionAmountVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalDiscountAmountUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalDiscountAmountLCPackage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalCommissionAmountUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N3}")]
        public decimal TotalDiscountPercentage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N3}")]
        public decimal TotalDiscountPercentageSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N3}")]
        public decimal TotalDiscountPercentageVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N3}")]
        public decimal TotalDiscountPercentageUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N3}")]
        public decimal TotalDiscountPercentageLCPackage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalList { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalListSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalListVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalListUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalListLCPackage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalNet { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalNetSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalNetVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalNetUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalNetLCPackage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalSell { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalSellSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalSellVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalSellUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalSellLCPackage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalLCPackage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalPriceAfterDiscount { get; set; }
    }
}
