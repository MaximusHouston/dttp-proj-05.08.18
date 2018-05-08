import { DashType } from './dash-type.interface';
/**
 * The scale line options.
 */
export interface Line {
    /**
     * The color of the lines.
     * Accepts valid CSS color strings, including hex and rgb.
     */
    color?: string;
    /**
     * The dash type of the line.
     */
    dashType?: DashType;
    /**
     * The visibility of the lines.
     */
    visible?: boolean;
    /**
     * The width of the line.
     */
    width?: number;
}
