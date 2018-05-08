import { PreventableEvent } from './preventable-event';
/**
 * Arguments for the `select` event.
 */
export class SelectEvent extends PreventableEvent {
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
