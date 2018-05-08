 
using System.Collections.Generic;

namespace DPO.Common
{
    public class SystemMatchupModel
    {
        public SystemMatchupModel()
            : base()
        {
        }

        public List<string> ProductNumbers = new List<string>();
        public int Quantity { get; set; }

        public string AHRI { get; set; }

        public double SEER { get; set; }
        public double EER { get; set; }
        public double Cooling { get; set; }
        public double Fit { get; set; }
        public double AFUE { get; set; }
        public bool ContinueAdding { get; set; }

        public List<string> ValidProducts = new List<string>();
        public List<string> InValidProducts = new List<string>();

        public string Tags
        {
            get
            {
                string validProductNumbers = string.Join(", ", this.ValidProducts);
                return "|| AHRI " + this.AHRI + ": Products [ " + validProductNumbers + " ]" + ", SEER " + this.SEER + ", EER " + this.EER + ", Cooling " + this.Cooling + ", Fit " + this.Fit + ", AFUE " + this.AFUE;
            }
        }
    }
}
