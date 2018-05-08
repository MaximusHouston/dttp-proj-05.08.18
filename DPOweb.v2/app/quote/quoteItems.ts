export class QuoteItems{
    public items: any = [];
}

export class QuoteItem {
    public quoteId: string = "";
    public productId: string = "";
    public priceNet: number;
    public priceList: number;
    public isCommissionable: boolean = false;
    public quantity: number;
    public productNumber: string = "";
    public description: string = "";
    public quoteItemId: string = "";
    public productClassCode : string = "";
    public submittalSheetTypeId: number;
    public tags: string = "";
}