import { RadialLabels } from './radial-labels.interface';
import { Scale } from './scale.interface';
import { Range } from './range.interface';
/**
 * The scale options of the Gauge.
 */
export interface RadialScale extends Scale {
    /**
     * Configures the scale labels.
     */
    labels?: RadialLabels;
    /**
     * The distance between the range indicators and the ticks.
     */
    rangeDistance?: number;
    /**
     * The ranges of the scale.
     */
    ranges?: Range[];
    /**
     * The starting angle of the Gauge.
     * The Gauge is rendered clockwise (0 degrees equal 180 degrees in the polar coordinate system).
     */
    startAngle?: number;
    /**
     * The ending angle of the Gauge.
     * The Gauge is rendered clockwise (0 degrees equal to 180 degrees in the polar coordinate system).
     */
    endAngle?: number;
}
