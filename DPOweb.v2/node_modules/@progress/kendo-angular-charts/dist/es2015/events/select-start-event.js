import { PreventableEvent } from './preventable-event';
/**
 * Arguments for the `selectStart` event.
 */
export class SelectStartEvent extends PreventableEvent {
    /**
     * @hidden
     */
    constructor(e, sender) {
        super(sender);
        this.axis = e.axis;
        this.from = e.from;
        this.to = e.to;
    }
}
