import { ChartComponent } from '../chart.component';
import { drawing, geometry } from '@progress/kendo-drawing';
/**
 * The context for the point highlight visual function.
 */
export interface AxisNoteVisualArgs {
    /**
     * A function that can be used to get the default visual.
     */
    createVisual: () => drawing.Element;
    /**
     * The note options.
     */
    options: any;
    /**
     * The rectangle that defines the normal position of the visual.
     */
    rect: geometry.Rect;
    /**
     * The instance of the Chart component.
     */
    sender: ChartComponent;
    /**
     * The note value.
     */
    value: string;
}
