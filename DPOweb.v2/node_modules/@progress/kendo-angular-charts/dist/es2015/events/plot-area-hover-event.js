import { BaseEvent } from './base-event';
/**
 * Arguments for the `plotAreaHover` event.
 */
export class PlotAreaHoverEvent extends BaseEvent {
    /**
     * @hidden
     */
    constructor(e, sender) {
        super(sender);
        this.category = e.category;
        this.originalEvent = e.originalEvent;
        this.value = e.value;
        this.x = e.x;
        this.y = e.y;
    }
}
