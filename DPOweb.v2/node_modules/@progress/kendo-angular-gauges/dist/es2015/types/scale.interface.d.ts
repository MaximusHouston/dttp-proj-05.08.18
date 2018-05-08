import { Labels } from './labels.interface';
import { Ticks } from './ticks.interface';
/**
 * The scale options of the Gauge.
 */
export interface Scale {
    /**
     * Configures the scale labels.
     */
    labels?: Labels;
    /**
     * Configures the major scale ticks.
     */
    majorTicks?: Ticks;
    /**
     * Configures the minor scale ticks.
     */
    minorTicks?: Ticks;
    /**
     * The minimum value of the scale.
     */
    min?: number;
    /**
     * The maximum value of the scale.
     */
    max?: number;
    /**
     * The interval between minor divisions.
     */
    minorUnit?: number;
    /**
     * The interval between major divisions.
     */
    majorUnit?: number;
    /**
     * Reverses the scale direction.
     */
    reverse?: boolean;
    /**
     * The width of the range indicators.
     */
    rangeSize?: number;
    /**
     * The default color of the ranges.
     */
    rangePlaceholderColor?: string;
}
