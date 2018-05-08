//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Common
{
   
   public enum DocumentTypeEnum : int
   {
        QuotePackageAttachedFile = 5,

        ProductFlyer = 100000000,
        ProductBrochure = 100000001,
        InstallationManual = 100000002,
        OperationManual = 100000003,
        EngineeringManual = 100000004,
        ServiceManual = 100000005,
        ProductImageLowRes = 100000006,
        ProductImageHighRes = 100000007,
        SubmittalData = 100000008,
        WrittenGuideSpec = 100000009,
        DimensionalDrawing = 100000010,
        CADDrawing = 100000011,
        RevitDrawing = 100000012,
        Other = 100000013,
        ProductLogos = 100000014,
        PartsManual = 100000021,
        ApplicationGuide = 100000022,
        LineCard = 100000015,
        ProductCatalog = 100000019
    }

}
