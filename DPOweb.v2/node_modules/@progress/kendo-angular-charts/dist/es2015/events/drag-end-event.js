import { BaseEvent } from './base-event';
/**
 * Arguments for the `dragEnd` event.
 */
export class DragEndEvent extends BaseEvent {
    /**
     * @hidden
     */
    constructor(e, sender) {
        super(sender);
        this.axisRanges = e.axisRanges;
        this.originalEvent = e.originalEvent;
    }
}
