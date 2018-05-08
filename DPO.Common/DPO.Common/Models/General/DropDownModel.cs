//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
 
using System.Collections.Generic;

namespace DPO.Common
{
    public class DropDownModel
    {
        public DropDownModel()
        {
        }

        public string AjaxElementId { get; set; }
        public List<SelectListItemExt> Items { get; set; }
    }
}
