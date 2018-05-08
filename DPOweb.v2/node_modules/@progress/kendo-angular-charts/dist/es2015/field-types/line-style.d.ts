/**
 * The line style of the series.
 *
 * @example
 * ```ts
 * import { Component } from '@angular/core';
 * import { LineStyle } from '@progress/kendo-angular-charts';
 *
 * _@Component({
 *   selector: 'my-app',
 *   template: `
 *     <kendo-chart>
 *       <kendo-chart-series>
 *         <kendo-chart-series-item type="line" [data]="[1, 2, 1]" [style]="style">
 *         </kendo-chart-series-item>
 *       </kendo-chart-series>
 *     </kendo-chart>
 *   `
 * })
 * export class AppComponent {
 *   public style: LineStyle = "smooth";
 * }
 *
 * ```
 */
export declare type LineStyle = 'normal' | 'step' | 'smooth';
