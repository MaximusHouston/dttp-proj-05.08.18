/**
 * The position of a note label.
 *
 * @example
 * ```ts
 * import { Component } from '@angular/core';
 * import { NoteLabelPosition } from '@progress/kendo-angular-charts';
 *
 * _@Component({
 *   selector: 'my-app',
 *   template: `
 *     <kendo-chart>
 *       <kendo-chart-value-axis>
 *         <kendo-chart-value-axis-item>
 *             <kendo-chart-value-axis-item-notes [data]="[{value: 1, label: { text: 'Foo' }}]">
 *               <kendo-chart-value-axis-item-notes-label [position]="position">
 *               </kendo-chart-value-axis-item-notes-label>
 *             </kendo-chart-value-axis-item-notes>
 *         </kendo-chart-value-axis-item>
 *       </kendo-chart-value-axis>
 *     </kendo-chart>
 *   `
 * })
 * class AppComponent {
 *   public position: NoteLabelPosition = "outside";
 * }
 *
 * ```
 */
export declare type NoteLabelPosition = 'inside' | 'outside';
