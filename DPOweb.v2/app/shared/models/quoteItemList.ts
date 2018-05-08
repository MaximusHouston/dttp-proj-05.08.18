import { DocumentModel } from '../../shared/models/document';

export class QuoteItemListModel {
    public accessories: string;
    public airFlowRateHighCooling: string;
    public airFlowRateHighHeating: string;
    public benefits: object[];
    public cabinetFeatures: object[];
    public description: string;
    public documents: DocumentModel[];
    public extendedNet: number;
    public getSubmittalSheetTemplateName: string;
    public getSubmittalSheetTemplateNameV2: string;
    public inventoryStatusDescription: string;
    public inventoryStatusId: number;
    public isCommissionable: boolean;
    public isSystem: boolean;
    public isSystemTemplate: boolean;
    public lineItemTypeId: number;
    public multiplierTypeId: number;
    public priceList: number;
    public priceNet: number;
    public productClassCode: number;
    public productId: string;
    public productNumber: string;
    public productStatusTypeDescription: string;
    public productStatusTypeId: number;
    public productSubFamilyId: number;
    public productTypeId: number;
    public quantity: number;
    public quoteItemId: number;
    public totalListPriceUnitary: number;
    public totalCommissionUnitary: number;
    public quoteId: number; 

    public submittalSheetsDocId: string;
    public installationManual: boolean = false;
    public operationManualDocId: string;
    public installationManualDocId: string;

    public hasSubmittalSheets: boolean = false;
    public hasInstallationManual: boolean = false;
    public hasOperationManual: boolean = false;

    public isSubmittalSheets: boolean = false; 
    public isInstallationManual: boolean = false; 
    public isOperationalManual: boolean = false; 

    public submittalSheetsDocObject: DocumentModel;
    public installationManualDocObject: DocumentModel;
    public operationManualDocObject: DocumentModel;
    
}