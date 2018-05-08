using Newtonsoft.Json;
using System;

namespace DPO.Common
{
    [JsonObject(IsReference = false)]
    public class DocumentModel
    {
        private long productId;
        public long ProductId { get { return productId; } set { productId = value; } }

        public string ProductNumber { get; set; }

        public Guid? DocumentId { get; set; }
        private string description;

        public string Description
        {
            get
            {
                return description ?? Type;
            }
            set { description = value; }
        }

        public string Type { get; set; }

        public bool HasImage { get { return (this.FileName != null); } }

        public string URL
        {
            get
            {
                var url = "";

                if (this.FileName == null)
                {
                    url = "/Images/NoImage.png";
                }
                else
                {
                    if (this.DocumentTypeId == (int)DocumentTypeEnum.ProductImageLowRes ||
                        this.DocumentTypeId == (int)DocumentTypeEnum.ProductImageHighRes ||
                        this.DocumentTypeId == (int)DocumentTypeEnum.DimensionalDrawing ||
                        this.DocumentTypeId == (int)DocumentTypeEnum.ProductLogos
                        )
                        url = "/image/" + Uri.EscapeUriString(this.Type) + "/" + Uri.EscapeUriString(this.FileName);
                    else
                    if (this.DocumentTypeId == (int)DocumentTypeEnum.QuotePackageAttachedFile)
                        url = "";
                    //else if (isLCSTSubmittal)
                    //{
                    //    url = LCSTSubmittalURL;
                    //}
                    else
                        url = "/document/" + Uri.EscapeUriString(this.Type) + "/" + Uri.EscapeUriString(this.FileName);
                }

                return url;
            }
            set {

            }
        }

        //public bool isLCSTSubmittal { get; set; }
        //public string LCSTSubmittalURL {get;set;}

        public string SiteFinityUrl
        {
            get
            {
                if (this.DocumentTypeId == (int)DocumentTypeEnum.ProductImageLowRes ||
                      this.DocumentTypeId == (int)DocumentTypeEnum.ProductImageHighRes)
                {
                    return Utilities.Config("sitefinity.documents.location") + "image/" + this.FileName + ".png";
                }
                else
                {
                    return getSiteFinityURL();
                }
            }
            set { }
        }

        public string AbsoultePath
        {
            get
            {
                var location = Utilities.GetDocumentLocation(this.Description, this.FileName);
                return location;
            }
        }

        public string FileName { get; set; }
        public string Name { get; set; }

        public int? DocumentTypeId { get; set; }

        public short Rank { get; set; }
        public string FileExtension { get; set; }

        public string getSiteFinityURL() {
            if (this.Type != null)
            {
                string documentType = this.Type.ToLower();
                documentType = documentType.Replace(' ', '-');
                string url = Utilities.Config("sitefinity.documents.location") + documentType + "/" + this.FileName + "." + this.FileExtension;
                return url;
            }
            else {
                return "";
            }
        }
    }
}
