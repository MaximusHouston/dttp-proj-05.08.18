using DPO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DPO.Common
{
    //public class ProjectModel : PageModel, IActiveQuoteInfoBarModel
    public class ProjectModel : PageModel
    {
        public ProjectModel()
        {
            CustomerAddress = new AddressModel();
            EngineerAddress = new AddressModel();
            SellerAddress = new AddressModel();
            ShipToAddress = new AddressModel();
            ActiveQuoteSummary = new QuoteListModel();
            Quotes = new List<QuoteModel>();
        }

        public ProjectModel(UserSessionModel owner)
            : this()
        {
            this.OwnerId = owner.UserId;
        }

        public bool NewRecordAdded { get; set; }

        public long? ProjectId { get; set; }

        public string ProjectIdStr {
            get { return (ProjectId != null) ? ProjectId.ToString() : ""; }
        } 

        public bool HasDAR { get { return false; } }

        public bool Active { get { return true; } set { } }

        public string Name { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? ProjectDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]

        public string ProjectDateDisplay
        {
            get
            {
                return this.ProjectDate != null ? this.ProjectDate.Value.ToString(ResourceUI.DateFormat) : "";
            }
        }

        public DateTime? BidDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? EstimatedClose { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? EstimatedDelivery { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? Expiration { get; set; }

        public bool IsTransferred { get; set; }

        public byte? ConstructionTypeId { get; set; }
        public DropDownModel ConstructionTypes { get; set; }
        public string ConstructionTypeDescription { get; set; }

        public byte? ProjectStatusTypeId { get; set; }
        public DropDownModel ProjectStatusTypes { get; set; }
        public string ProjectStatusDescription { get; set; }

        public byte? ProjectTypeId { get; set; }
        public DropDownModel ProjectTypes { get; set; }
        public string ProjectTypeDescription { get; set; }

        public byte? ProjectOpenStatusTypeId { get; set; }
        public DropDownModel ProjectOpenStatusTypes { get; set; }
        public string ProjectOpenStatusDescription { get; set; }

        public byte? VerticalMarketTypeId { get; set; }
        public DropDownModel VerticalMarketTypes { get; set; }
        public string VerticalMarketDescription { get; set; }

        public string Description { get; set; }

        public string CustomerName { get; set; }
        public string EngineerBusinessName { get; set; }
        public string EngineerName { get; set; }
        public string ShipToName { get; set; }
        public string SellerName { get; set; }
        public string DealerContractorName { get; set; }
        //public string DealerContractorBusinessName { get; set; }// this is CustomerName
        public string BusinessName { get; set; }

        public AddressModel SellerAddress { get; set; }
        public AddressModel EngineerAddress { get; set; }
        public AddressModel CustomerAddress { get; set; }
        public AddressModel ShipToAddress { get; set; }

        public long? OwnerId { get; set; }
        public string OwnerName { get; set; }

        public QuoteListModel ActiveQuoteSummary { get; set; }

        public List<QuoteModel> Quotes { get; set; }

        public bool Deleted { get; set; }

        public string ProjectStatusNotes { get; set; }

        public DropDownModel ProjectLeadStatusTypes { get; set; }
        public ProjectLeadStatusTypeEnum? ProjectLeadStatusTypeId { get; set; }

        public string ProjectLeadStatusTypeDescription { get; set; }

        public string ERPFirstOrderComment { get; set; }
        public string ERPFirstOrderNumber { get; set; }
        public string ERPFirstPONumber { get; set; }
        public DateTime? ERPFirstOrderDate { get; set; }

        public int SquareFootage { get; set; }
        public int NumberOfFloors { get; set; }
        public DateTime ActualCloseDate { get; set; }
        public byte OrderStatus { get; set; }

        public DateTime EstimateReleaseDate { get; set; }
        public DateTime ActualDeliveryDate { get; set; }

        

        public bool? IsCommission { get; set; }
        public string PricingStrategy
        {
            get
            {
                if (IsCommission != null)
                {
                    return IsCommission == true ? "Commission" : "Buy/Sell";
                }
                else return "";
            }
        }

        public bool CommissionConvertNo { get; set; }
        public bool CommissionConvertYes { get; set; }

        public string requestUrl { get; set; }
        public bool HasSuggestionAddress { get; set; }

        public List<string> SuggesstionAddresses { get; set; }

        public Dictionary<string, string> ShippingSuggestionAddress { get; set; }
        public Dictionary<string, string> DealorContractorSuggestionAddress { get; set; }
        public Dictionary<string, string> EngineerSuggestionAddress { get; set; }
        public Dictionary<string, string> SellerSuggestionAddress { get; set; }
    }
}
