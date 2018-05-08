import { BaseEvent } from './base-event';
/**
 * Arguments for the `zoomEnd` event.
 */
export class ZoomEndEvent extends BaseEvent {
    /**
     * @hidden
     */
    constructor(e, sender) {
        super(sender);
        this.axisRanges = e.axisRanges;
        this.originalEvent = e.originalEvent;
    }
}
