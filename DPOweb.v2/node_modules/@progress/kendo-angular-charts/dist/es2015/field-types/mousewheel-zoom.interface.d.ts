import { LockAxis } from './lock-axis';
/**
 * The configuration of the mousewheel action for zooming.
 */
export interface MousewheelZoom {
    /**
     * Specifies an axis that should not be zoomed.
     */
    lock?: LockAxis;
}
