export interface IProductTab {
    isActive: boolean,
    id:number,
    description: string
}
export class ProductTab implements IProductTab {
    constructor(
        public isActive: boolean,
        public id: number,
        public description:string
    ) { }
}