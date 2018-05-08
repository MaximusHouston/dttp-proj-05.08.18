import { PreventableEvent } from './preventable-event';
/**
 * Arguments for the `zoomStart` event.
 */
export class ZoomStartEvent extends PreventableEvent {
    /**
     * @hidden
     */
    constructor(e, sender) {
        super(sender);
        this.axisRanges = e.axisRanges;
        this.originalEvent = e.originalEvent;
    }
}
