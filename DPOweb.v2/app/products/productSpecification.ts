export interface IProductSpecification
{
    id: number,
    key: number,
    name: string,
    productId: number,
    value:string
}

export class ProductSpecification implements IProductSpecification {
    constructor(
        public id: number,
        public key: number,
        public name: string,
        public productId: number,
        public value: string

    ) { }
}