/**
 * The dash line type.
 *
 * @example
 * ```ts
 * import { Component } from '@angular/core';
 * import { DashType } from '@progress/kendo-angular-charts';
 *
 * _@Component({
 *   selector: 'my-app',
 *   template: `
 *     <kendo-chart>
 *       <kendo-chart-series>
 *         <kendo-chart-series-item type="line" [data]="[1, 2, 3]" [dashType]="dashType">
 *         </kendo-chart-series-item>
 *       </kendo-chart-series>
 *     </kendo-chart>
 *   `
 * })
 * class AppComponent {
 *   public dashType: DashType = "dot";
 * }
 *
 * ```
 */
export declare type DashType = 'dash' | 'dashDot' | 'dot' | 'longDash' | 'longDashDot' | 'longDashDotDot' | 'solid';
