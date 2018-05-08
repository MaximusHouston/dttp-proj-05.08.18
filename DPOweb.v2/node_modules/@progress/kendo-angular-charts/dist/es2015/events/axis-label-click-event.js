import { BaseEvent } from './base-event';
/**
 * Arguments for the `axisLabelClick` event.
 */
export class AxisLabelClickEvent extends BaseEvent {
    /**
     * @hidden
     */
    constructor(e, sender) {
        super(sender);
        this.axis = e.axis;
        this.dataItem = e.dataItem;
        this.index = e.index;
        this.text = e.text;
        this.value = e.value;
    }
}
