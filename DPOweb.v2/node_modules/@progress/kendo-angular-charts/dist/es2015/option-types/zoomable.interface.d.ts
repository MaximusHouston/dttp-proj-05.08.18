import { DragAction, MousewheelZoom } from '../common/property-types';
/**
 * The zoomable options.
 */
export interface Zoomable {
    /**
     * Specifies if the Chart can be zoomed by using the mousewheel.
     */
    mousewheel?: boolean | MousewheelZoom;
    /**
     * Specifies if the Chart can be zoomed by using selection.
     */
    selection?: boolean | DragAction;
}
