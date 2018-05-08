/**
 * Specifies the position of a note.
 *
 * @example
 * ```ts
 * import { Component } from '@angular/core';
 * import { NotePosition } from '@progress/kendo-angular-charts';
 *
 * _@Component({
 *   selector: 'my-app',
 *   template: `
 *     <kendo-chart>
 *       <kendo-chart-value-axis>
 *         <kendo-chart-value-axis-item>
 *           <kendo-chart-value-axis-item-notes [position]="position" [data]="[{ value: 1 }]">
 *           </kendo-chart-value-axis-item-notes>
 *         </kendo-chart-value-axis-item>
 *       </kendo-chart-value-axis>
 *     </kendo-chart>
 *   `
 * })
 * class AppComponent {
 *   public position: NotePosition = "right";
 * }
 *
 * ```
 */
export declare type NotePosition = 'top' | 'bottom' | 'left' | 'right';
