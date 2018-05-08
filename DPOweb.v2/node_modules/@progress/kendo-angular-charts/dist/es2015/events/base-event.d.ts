import { ChartComponent } from '../chart.component';
/**
 * @hidden
 */
export declare abstract class BaseEvent {
    /**
     * The `ChartComponent` that triggered the event.
     */
    sender: ChartComponent;
    /**
     * @hidden
     */
    constructor(sender: ChartComponent);
}
