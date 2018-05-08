
import {Product} from './product';

export interface IProductAccessory {
    parentProductId: number,
    accessory: Product,
    quantity: number
}

export class ProductAccessory implements IProductAccessory {
    constructor(
        public parentProductId: number,
        public accessory: Product,
        public quantity: number
    ) { }
}