using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DPO.Common
{
    public class BuildingLinkModel: IValidatableObject
    {
        public int id { get; set; }
        public int? floorId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public bool enabled { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (enabled)
            {
                if (String.IsNullOrEmpty(title)) yield return new ValidationResult(Resources.ResourceUI.PleaseEnterTitle, new[] { "title" });
                if (String.IsNullOrEmpty(description)) yield return new ValidationResult(Resources.ResourceUI.PleaseEnterDescription, new[] { "description" });
                if (String.IsNullOrEmpty(url)) yield return new ValidationResult(Resources.ResourceUI.PleaseEnterURL, new[] { "url" });
            }
        }
    }
}
