 
using System.Collections.Generic; 
using System.Web.Mvc;

namespace DPO.Common.Models.Project
{
    public class ProjectDarComTypesModel
    {
        public ProjectDarComTypesModel()
        {
            ProjectDarComTypesList = new List<SelectListItem>();
        }
       
        public int ProjectDarComId { get; set; }
        public IEnumerable<SelectListItem> ProjectDarComTypesList { get; set; }  
    }
}
