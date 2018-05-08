import { QueryList } from '@angular/core';
import { ConfigurationService } from "../../common/configuration.service";
import { CollectionService, Item } from "../../common/collection.service";
import { TooltipTemplateService } from "../../common/tooltip-template.service";
import { SeriesComponent } from '../../chart/series.component';
/**
 * A collection of one or more navigator series items.
 *
 * @example
 * ```ts
 * import { Component } from '@angular/core';
 *
 * _@Component({
 *   selector: 'my-app',
 *   template: `
 *     <kendo-stockchart>
 *         <kendo-chart-navigator>
 *             <kendo-chart-navigator-series>
 *                 <kendo-chart-navigator-series-item type="area" [data]="data" field="value" categoryField="date">
 *                 </kendo-chart-navigator-series-item>
 *             </kendo-chart-navigator-series>
 *         </kendo-chart-navigator>
 *     </kendo-stockchart>
 *   `
 * })
 * class AppComponent {
 *   public data: any[] = [];
 *
 *   constructor() {
 *      for (let idx = 0; idx < 100; idx++) {
 *          this.data.push({
 *              date: new Date(2017, 0, idx),
 *              value: Math.random() * 100
 *          });
 *      }
 *   }
 * }
 *
 * ```
 */
export declare class NavigatorSeriesComponent extends SeriesComponent {
    protected configurationService: ConfigurationService;
    protected collectionService: CollectionService;
    protected tooltipTemplateService: TooltipTemplateService;
    children: QueryList<Item>;
    constructor(configurationService: ConfigurationService, collectionService: CollectionService, tooltipTemplateService: TooltipTemplateService);
    protected readTooltipTemplates(): void;
}
