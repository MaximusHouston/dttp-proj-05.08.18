
export interface ITabModel {
    isActive: boolean,
    id: number,
    description: string
}

export class TabModel implements ITabModel {
    constructor(
        public isActive: boolean,
        public id: number,
        public description: string   
    ) { }
}