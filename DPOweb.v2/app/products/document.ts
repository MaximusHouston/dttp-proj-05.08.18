export interface IDocument {
    productId: number,
    productNumber: string,
    documentId: string,
    description: string,
    type: string,
    hasImage: boolean,
    documentUrl: string,
    absolutePath: string,
    fileName: string,
    documentTypeId: number,
    rank: number
}

export class Document implements IDocument
{
    constructor(
        public productId: number,
        public productNumber: string,
        public documentId: string,
        public description: string,
        public type: string,
        public hasImage: boolean,
        public documentUrl: string,
        public absolutePath: string,
        public fileName: string,
        public documentTypeId: number,
        public rank: number
    ) { }
}