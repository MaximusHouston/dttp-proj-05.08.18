import { Border } from './border.interface';
import { Margin } from './margin.interface';
/**
 * The configuration options for the Gauge area.
 * Represents the entire visible area of the Gauge.
 */
export interface GaugeArea {
    /**
     * The background of the Gauge area. Accepts valid CSS color strings, including hex and rgb.
     */
    background?: string;
    /**
     * The border of the Gauge area.
     */
    border?: Border;
    /**
     * The height of the Gauge area.
     */
    height?: number;
    /**
     * The margin of the Gauge area.
     */
    margin?: number | Margin;
    /**
     * The height of the Gauge area.
     */
    width?: number;
}
