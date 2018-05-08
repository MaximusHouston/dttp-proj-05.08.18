/**
 * The position of an axis title.
 *
 * @example
 * ```ts
 * import { Component } from '@angular/core';
 * import { AxisTitlePosition } from '@progress/kendo-angular-charts';
 *
 * _@Component({
 *   selector: 'my-app',
 *   template: `
 *     <kendo-chart>
 *       <kendo-chart-value-axis>
 *         <kendo-chart-value-axis-item>
 *           <kendo-chart-value-axis-item-title [position]="position" text="Title">
 *           </kendo-chart-value-axis-item-title>
 *         </kendo-chart-value-axis-item>
 *       </kendo-chart-value-axis>
 *     </kendo-chart>
 *   `
 * })
 * class AppComponent {
 *   public position: AxisTitlePosition = "top";
 * }
 *
 * ```
 */
export declare type AxisTitlePosition = 'top' | 'bottom' | 'left' | 'right' | 'center';
