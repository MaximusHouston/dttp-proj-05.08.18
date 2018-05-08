import { Document } from './document';
import { ProductAccessory } from './productAccessory';
import { ProductNote } from './productNote';
import { ProductTab} from './productTab';
import { ProductTypeEnum } from './productTypeEnum';
import { UnitInstallationTypeEnum } from './unitInstallationtypeEnum';
import { ProductSpecification } from './productSpecification';
import { SubmittalSheetTypeEnum } from './submittalSheetTypeEnum';

export interface IProduct {

    accessories: Array<ProductAccessory>,
    benefits: Array<ProductNote>,
    dimensionalDrawing: Document,
    documents: Array<Document>,
    features: Array<ProductNote>,
    getSubmittalSheetTemplateName: string,
    getSystemIndoorUnit: Product,
    getSystemOutdoorUnit: Product,
    productImage: Document,
    indoorUnit: IProduct,
    isSystem: boolean,
    isSystemTemplte: boolean,
    logos: Array<Document>,
    name: string,
    notes: Array<ProductNote>,
    outDoorUnit: Product,
    parentProductId: number,
    parentProducts: Array<Product>,
    price: number,
    productBrandId: number,
    productBrandName: string,
    productCategoryName: string,
    productClassCode: string,
    productFamilyId: number,
    productFamilyName: string,
    productFamilyTabs: Array<ProductTab>,
    productId: number,
    productTypeDescription: string,
    productTypeId: ProductTypeEnum,
    unitInstallationTypeId: UnitInstallationTypeEnum,
    productNumber: string,
    productSpecifications: Array<ProductSpecification>,
    quantity: number,
    specifications: ProductSpecification,
    submittalSheetTypeDescription: string,
    submittalSheetTypeId: SubmittalSheetTypeEnum,
    subProducts: Array<Product>,
    tabs: string,
    unitCombination: string,
    quoteId: number
}

export class Product implements IProduct {
    constructor(
        public accessories: Array<ProductAccessory>,
        public benefits: Array<ProductNote>,
        public dimensionalDrawing: Document,
        public documents: Array<Document>,
        public features: Array<ProductNote>,
        public getSubmittalSheetTemplateName: string,
        public getSystemIndoorUnit: Product,
        public getSystemOutdoorUnit: Product,
        public productImage: Document,
        public indoorUnit: Product,
        public isSystem: boolean,
        public isSystemTemplte: boolean,
        public logos: Array<Document>,
        public name: string,
        public notes: Array<ProductNote>,
        public outDoorUnit: Product,
        public parentProductId: number,
        public parentProducts: Array<Product>,
        public price: number,
        public productBrandId: number,
        public productBrandName: string,
        public productCategoryName: string,
        public productClassCode: string,
        public productFamilyId: number,
        public productFamilyName: string,
        public productFamilyTabs: Array<ProductTab>,
        public productId: number,
        public productTypeDescription: string,
        public productTypeId: ProductTypeEnum,
        public unitInstallationTypeId: UnitInstallationTypeEnum,
        public productNumber: string,
        public productSpecifications: Array<ProductSpecification>,
        public quantity: number,
        public specifications: ProductSpecification,
        public submittalSheetTypeDescription: string,
        public submittalSheetTypeId: SubmittalSheetTypeEnum,
        public subProducts: Array<Product>,
        public tabs: string,
        public unitCombination: string,
        public quoteId: number
    ) { }
}