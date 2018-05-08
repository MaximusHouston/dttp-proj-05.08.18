import { Labels } from './labels.interface';
import { RadialLabelPosition } from './radial-label-position';
export interface RadialLabels extends Labels {
    /**
     * The position of the labels.
     */
    position?: RadialLabelPosition;
}
