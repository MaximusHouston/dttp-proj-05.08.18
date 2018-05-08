
import { TabModel } from './tab';
import { CategoryListModel } from './categoryList';

export interface IProductFamily {

    productFamilyTabs: Array<TabModel>,
    items: Array<CategoryListModel> 
}

export class ProductFamily implements IProductFamily {
    constructor(
        public productFamilyTabs: Array<TabModel>,
        public items: Array<CategoryListModel> 
    ) { }
}