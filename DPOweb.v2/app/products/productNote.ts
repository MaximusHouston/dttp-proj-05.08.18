export interface IProductNote {
    productId: number,
    description: string,
    productNoteTypeId: number,
    rank: number
}

export class ProductNote implements IProductNote {
    constructor(
        public productId: number,
        public description: string,
        public productNoteTypeId: number,
        public rank: number
    ) { }
}