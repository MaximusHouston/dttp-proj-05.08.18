import { BaseEvent } from './base-event';
/**
 * Arguments for the `selectEnd` event.
 */
export class SelectEndEvent extends BaseEvent {
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
