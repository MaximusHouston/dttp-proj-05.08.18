import { PreventableEvent } from './preventable-event';
/**
 * Arguments for the `dragStart` event.
 */
export class DragStartEvent extends PreventableEvent {
    /**
     * @hidden
     */
    constructor(e, sender) {
        super(sender);
        this.axisRanges = e.axisRanges;
        this.originalEvent = e.originalEvent;
    }
}
