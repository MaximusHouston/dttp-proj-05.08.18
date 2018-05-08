using DPO.Common;
using DPO.Common.Models.Project;
using System;
using System.Collections.Generic;


namespace DPO.Model.Light
{
    [Serializable]
    public class ProjectsGridViewModel: SearchProject
    {
        public ProjectsGridViewModel()
        {
            Items = new PagedList<ProjectViewModel>(new List<ProjectViewModel>(), 1, 25);
        }

        public DropDownModel BusinessesInGroup { get; set; }
        public PagedList<ProjectViewModel> Items { get; set; }
        public ModelModeEnum ModelMode { get; set; }
        public DropDownModel ProjectDateTypes { get; set; }
        public ProjectExportTypeEnum? ProjectExportType { get; set; }
        public DropDownModel ProjectExportTypes { get; set; }
        public DropDownModel ProjectOpenStatusTypes { get; set; }
        public DropDownModel ProjectLeadStatusTypes { get; set; }
        public DropDownModel ProjectStatusTypes { get; set; }
        public DropDownModel ProjectTypes { get; set; }
        public decimal TotalList { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalSell { get; set; }
        public decimal TotalSplitCount { get; set; }
        public decimal TotalVRVOutdoorCount { get; set; }
        public DropDownModel UsersInGroup { get; set; }

        public List<ProjectModel> DeleteProjects { get; set; }

        //new property for DAR/COM
        public DropDownModel ProjectDarComTypes { get; set; }


    }
}
