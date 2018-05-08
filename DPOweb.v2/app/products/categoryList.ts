export interface ICategoryListModel {
    id: number,
    name: string,
    description: string
}

export class CategoryListModel implements ICategoryListModel {
    constructor(
        public id: number,
        public name: string,
        public description: string
    ) { }
}