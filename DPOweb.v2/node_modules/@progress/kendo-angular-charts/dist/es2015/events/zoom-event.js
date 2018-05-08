import { PreventableEvent } from './preventable-event';
/**
 * Arguments for the `zoom` event.
 */
export class ZoomEvent extends PreventableEvent {
    /**
     * @hidden
     */
    constructor(e, sender) {
        super(sender);
        this.axisRanges = e.axisRanges;
        this.delta = e.delta;
        this.originalEvent = e.originalEvent;
    }
}
