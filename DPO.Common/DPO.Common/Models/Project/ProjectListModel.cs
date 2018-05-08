using System;
using System.ComponentModel.DataAnnotations;

namespace DPO.Common
{
    public class ProjectListModel : PageModel
    {
        public DiscountRequestModel ActiveDiscountRequestSummary { get; set; }
        public QuoteListModel ActiveQuoteSummary { get; set; }
        public CommissionRequestModel ActiveCommissionRequestSummary { get; set; }

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

        public string ProjectIdStr { get { return (ProjectId != null) ? ProjectId.ToString() : ""; } }

        public string ProjectOpenStatus { get; set; }

        public int ProjectOpenStatusId { get; set; }

        public string ProjectOwner { get; set; }

        public string ProjectStatus { get; set; }

        public int? ProjectStatusId { get; set; }

        public string ProjectType { get; set; }

        public int ProjectTypeId { get; set; }

        public string ShipToName { get; set; }

        public decimal? TotalNet { get; set; }

    }
}
