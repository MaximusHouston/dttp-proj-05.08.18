import { TemplateRef } from '@angular/core';
/**
 * @hidden
 */
export declare class TooltipTemplateService {
    private seriesTemplates;
    private template;
    private sharedTemplate;
    setTemplate(template: TemplateRef<any>): void;
    getTemplate(seriesIndex: number): TemplateRef<any>;
    setSeriesTemplates(seriesTemplates: Array<TemplateRef<any>>): void;
    setSharedTemplate(sharedTemplate: TemplateRef<any>): void;
    getSharedTemplate(): TemplateRef<any>;
}
