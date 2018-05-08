using DPO.Common.Models.Project;

//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
//
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
using System.Collections.Generic;

namespace DPO.Common
{
    public class ProjectsModel : SearchProject
    {
        public ProjectsModel()
        {
            Items = new PagedList<ProjectListModel>(new List<ProjectListModel>(), 1, 25);
        }

        public DropDownModel BusinessesInGroup { get; set; }
        public PagedList<ProjectListModel> Items { get; set; }
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
        public int TotalSplitCount { get; set; }
        public int TotalVRVOutdoorCount { get; set; }
        public DropDownModel UsersInGroup { get; set; }

        public List<ProjectModel> DeleteProjects { get; set; }

        //adding new property for DAR/COM
        public DropDownModel ProjectDarComTypes { get; set; }

        
    }
}