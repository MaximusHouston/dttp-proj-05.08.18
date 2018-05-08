using System;
using DPO.Common;
using System.ComponentModel.DataAnnotations;

namespace DPO.Model.Light
{
    public class ProjectViewModel : PageModel
    {
        public bool Alert { get; set; }
        public string AlertText { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime BidDate { get; set; }
        public string BusinessName { get; set; }
        public string CustomerName { get; set; }
        public bool Deleted { get; set; }
        public string EngineerName { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime EstimatedClose { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime EstimatedDelivery { get; set; }

        public DateTime Expiration { get; set; }

        public string ERPFirstOrderComment { get; set; }
        public string ERPFirstOrderNumber { get; set; }
        public string ERPFirstPONumber { get; set; }
        public DateTime? ERPFirstOrderDate { get; set; }

        public bool IsEditable
        {
            get
            {
                if (this.Deleted)
                {
                    return false;
                }

                if (this.IsTransferred)
                {
                    return false;
                }

                bool result = false;

                if (this.ProjectStatusId != null)
                {
                    ProjectStatusTypeEnum pt = (ProjectStatusTypeEnum)this.ProjectStatusId;

                    switch (pt)
                    {
                        case ProjectStatusTypeEnum.Open:
                            result = true;
                            break;
                        default:
                            result = false;
                            break;
                    }
                }

                return result;
            }
        }

        public string ProjectLeadStatus { get; set; }

        public ProjectLeadStatusTypeEnum? ProjectLeadStatusId { get; set; }

        public bool IsTransferred { get; set; }

        public Messages Messages { get; set; }
        public int? ModelSaveState { get; set; }
        public string Name { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime ProjectDate { get; set; }

        public long? ProjectId { get; set; }

        public string ProjectIdStr {
            get { return (ProjectId != null) ? ProjectId.ToString() : ""; }
            set { if(ProjectIdStr != null) this.ProjectId = Int64.Parse(this.ProjectIdStr); }
        }

        public string ProjectOpenStatus { get; set; }

        public int ProjectOpenStatusId { get; set; }

        public string ProjectOwner { get; set; }

        public string ProjectStatus { get; set; }

        public int? ProjectStatusId { get; set; }

        public string ProjectType { get; set; }

        public int ProjectTypeId { get; set; }

        public string ShipToName { get; set; }

        //=== Active Quote Info ====

        public long? ActiveQuoteId { get; set; }

        public string ActiveQuoteTitle { get; set; }

        public bool? IsCommission { get; set; }

        public string PricingStrategy {
            get {
                //return (IsCommission != null) ? ProjectId.ToString() : "";
                if (IsCommission != null) {
                    return IsCommission == true ? "Commission" : "Buy/Sell";
                }
                else return "";
            }
        }

        public bool RecalculationRequired { get; set; }

        public decimal? TotalList { get; set; }

        public decimal? TotalSell { get; set; }

        public decimal? TotalNet { get; set; }

        public decimal? TotalCountVRVOutDoor { get; set; }

        public decimal? TotalCountSplitOutDoor { get; set; }

        public long? DiscountRequestId { get; set; }

        public long? CommissionRequestId { get; set; }

        public string DarComStatus { get; set; }
        //=============================

        //==============Remove after Project Grid Modification is completed ========
        //public long ProjectId { get; set; }
        //public string ProjectIdStr
        //{
        //    get { return (ProjectId != null) ? ProjectId.ToString() : ""; }
        //}

        //public ProjectViewModel() { }

        //public ProjectViewModel(DPO.Common.UserSessionModel owner)
        //    : this()
        //{
        //    this.OwnerId = owner.UserId;
        //}

        // public long? OwnerId { get; set; }
        //public string OwnerName { get; set; }

        //public string Name { get; set; }
        //public DateTime ProjectDate { get; set; }
        //public DateTime BidDate { get; set; }
        //public DateTime EstimatedClose { get; set; }
        //public DateTime EstimatedDelivery { get; set; }

        //public ConstructionTypeEnum ConstructionTypeId { get; set; }
        //public string ConstructionTypeDescription { get { return ConstructionTypeId.GetDescription();} }

        //public ProjectStatusTypeEnum ProjectStatusTypeId { get; set; }
        //public string ProjectStatusTypeDescription { get { return ProjectStatusTypeId.GetDescription(); } }

        //public ProjectTypeEnum ProjectTypeId { get; set; }
        //public string ProjectTypeDescription { get { return ProjectTypeId.GetDescription(); } }

        //public ProjectOpenStatusTypeEnum ProjectOpenStatusTypeId { get; set; }
        //public string ProjectOpenStatusTypeDescription { get { return ProjectOpenStatusTypeId.GetDescription(); } }

        //public VerticalMarketTypeEnum VerticalMarketTypeId { get; set; }
        //public string VerticalMarketTypeDescription { get { return VerticalMarketTypeId.GetDescription(); } }

        //public String Description { get; set; }
        //public Byte? PricingTypeId { get; set; }
        //public String PricingTypeDescription {
        //    get { return (PricingTypeId != null && PricingTypeId == 1) ? "Buy/Sell" : "Commission"; } // need to be updated when we have more pricing types
        //}

        //==================================================================================
    }
}
