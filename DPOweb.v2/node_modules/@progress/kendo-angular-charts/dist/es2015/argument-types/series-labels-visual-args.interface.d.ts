import { ChartComponent } from '../chart.component';
import { drawing, geometry } from '@progress/kendo-drawing';
/**
 * The context for the series label visual function.
 */
export interface SeriesLabelsVisualArgs {
    /**
     * A function that can be used to get the default visual.
     */
    createVisual: () => drawing.Element;
    /**
     * The label options.
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
     * The label text.
     */
    text: string;
}
