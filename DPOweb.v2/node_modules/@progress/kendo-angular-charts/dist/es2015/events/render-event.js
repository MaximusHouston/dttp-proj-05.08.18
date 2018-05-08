import { BaseEvent } from './base-event';
/**
 * Arguments for the `render` event.
 */
export class RenderEvent extends BaseEvent {
    /**
     * @hidden
     */
    constructor(_e, sender) {
        super(sender);
    }
}
