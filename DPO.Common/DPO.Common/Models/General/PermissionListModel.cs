//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
 

namespace DPO.Common
{
    public class PermissionListModel
    {
        public bool IsSelected { get; set; }
        public int ReferenceId { get; set; }
        public EntityEnum ReferenceEntityId { get; set; }        
        public string Description { get; set; }
        public string WebDescription { get; set; }
        public string URL { get; set; }
    }
}
