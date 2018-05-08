import { QuoteItemListModel } from '../../shared/models/quoteItemList';
import { DocumentModel } from '../../shared/models/document';
import { ProductAndDocumentModel } from './product-and-document';

export class SubmittalPackageModel {
    public tooId: number;
    public name: string = "";
    public fileName: string = "";
    public description: string = "";
    public order: string = "";
    public addToQuote: boolean = false;
    public accessUrl: string = "";
    public clickable: boolean = false;
    public downloadable: boolean = false;

    public projectName: string = ""; 
    public projectStatusTypeId: number;
    public isTransferred: boolean; 
    public withCostPrice: boolean;
   
    public projectOwnerCommissionSchemeAllowed: boolean;
    public items: QuoteItemListModel[];
    public orderId: string;
    public orderStatusTypeId: number;
    public pageSizes: object[];

    public projectId: string;
    public projectIdStr: string;
  
    public quoteId: string;
    public quoteIdStr: string;
    public quotePackage: DocumentModel[];
 
    public quotePackageAttachedFiles: DocumentModel[];
    public awaitingDiscountRequest: boolean;
   
    public hasDAR: boolean;
    public hasConfiguredItem: boolean;
    public isCommissionRequest: boolean;
    public awaitingCommissionRequest: boolean;
    public hasCOM: boolean;
    public hasOrder: boolean;
    public isCommission: boolean;
    public commissionConvertNo: boolean;
    public commissionConvertYes: boolean;
    public showCommissionConvertPopup: boolean;
    public commissionRequestStatusTypeId: number;
    public discountRequestStatusTypeId: number;   
    public commissionRequestAvailable: boolean;
 
    public hasUnavailableProduct: boolean;
    public hasObsoleteProduct: boolean;
    public hasObsoleteAndUnavailableProduct: boolean; 

    public submittalCheckBox: string;
    
    public productsAndDocuments: ProductAndDocumentModel[];
}

  