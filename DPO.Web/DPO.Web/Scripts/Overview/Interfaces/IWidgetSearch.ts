
module Overview {
    export interface IWidgetSearch {
        businessId: string;
        userId: string;
        dateTypeId: string;
        expirationDays: number;
        onlyAlertedProjects: Boolean;
        projectId: string;
        projectOpenStatusTypeId: string;
        projectStartDate: Date;
        projectStartEnd: Date;
        projectStatusTypeId: string;
        showDeletedProjects: Boolean;
        year: number;
        filter: string;
        previousFilter: string;
    }
} 