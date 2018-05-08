import { ChartComponent } from '../chart.component';
import { BaseEvent } from './base-event';
import { AxisRange } from '../api-types/axis-range.interface';
/**
 * Arguments for the `zoomEnd` event.
 */
export declare class ZoomEndEvent extends BaseEvent {
    /**
     * A dictionary which contains the range (min and max values) of named axes.
     * The axis name is used as a key.
     *
     * > The dictionary includes only the affected axis ranges.
     * If an axis is locked or its range is not modified, the axis will not be listed.
     */
    axisRanges: {
        [name: string]: AxisRange;
    };
    /**
     * The original user event that triggered the drag action.
     */
    originalEvent: any;
    /**
     * @hidden
     */
    constructor(e: any, sender: ChartComponent);
}
