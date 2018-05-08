/**
 * The options for the scale ticks.
 */
export interface Ticks {
    /**
     * The color of the ticks. Accepts a valid CSS color string, including hex and rgb.
     */
    color?: string;
    /**
     * The size of the ticks. Represents the length of the line (in pixels) that is drawn to indicate the tick on the scale.
     */
    size?: number;
    /**
     * The visibility of the ticks.
     */
    visible?: boolean;
    /**
     * The ticks width (in pixels).
     */
    width?: number;
}
