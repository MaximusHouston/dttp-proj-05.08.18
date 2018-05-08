import { PreventableEvent } from './preventable-event';
/**
 * Arguments for the `drag` event.
 */
export class DragEvent extends PreventableEvent {
    /**
     * @hidden
     */
    constructor(e, sender) {
        super(sender);
        this.axisRanges = e.axisRanges;
        this.originalEvent = e.originalEvent;
    }
}
