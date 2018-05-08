import { BaseEvent } from './base-event';
/**
 * Arguments for the `seriesClick` event.
 */
export class SeriesClickEvent extends BaseEvent {
    /**
     * @hidden
     */
    constructor(e, sender) {
        super(sender);
        this.category = e.category;
        this.dataItem = e.dataItem;
        this.originalEvent = e.originalEvent;
        this.percentage = e.percentage;
        this.point = e.point;
        this.series = e.series;
        this.stackValue = e.stackValue;
        this.value = e.value;
    }
}
