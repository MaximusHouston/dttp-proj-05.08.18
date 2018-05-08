import { Observable } from 'rxjs/Observable';
/**
 * @hidden
 */
export interface Item {
    options: any;
}
/**
 * @hidden
 */
export declare class ItemChange {
    sender: Item;
    options: any;
    constructor(sender: Item, options: any);
}
/**
 * @hidden
 */
export declare class CollectionService {
    onItemChange$: Observable<ItemChange>;
    private source;
    constructor();
    notify(change: ItemChange): void;
}
