import { AfterContentChecked } from '@angular/core';
import { ConfigurationService } from '../common/configuration.service';
import { CollectionService } from '../common/collection.service';
import { TooltipTemplateService } from '../common/tooltip-template.service';
import { SeriesComponentGenerated } from './series.component.generated';
/**
 * A collection of one or more series items.
 *
 * @example
 * ```ts
 * import { Component } from '@angular/core';
 *
 * _@Component({
 *   selector: 'my-app',
 *   template: `
 *     <kendo-chart>
 *       <kendo-chart-series>
 *         <kendo-chart-series-item type="line" [data]="[1, 2, 3]">
 *         </kendo-chart-series-item>
 *       </kendo-chart-series>
 *     </kendo-chart>
 *   `
 * })
 * class AppComponent {
 * }
 *
 * ```
 */
export declare class SeriesComponent extends SeriesComponentGenerated implements AfterContentChecked {
    protected configurationService: ConfigurationService;
    protected collectionService: CollectionService;
    protected tooltipTemplateService: TooltipTemplateService;
    constructor(configurationService: ConfigurationService, collectionService: CollectionService, tooltipTemplateService: TooltipTemplateService);
    ngAfterContentChecked(): void;
    protected readTooltipTemplates(): void;
}
