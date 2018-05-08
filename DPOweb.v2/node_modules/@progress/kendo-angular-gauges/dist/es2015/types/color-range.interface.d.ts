/**
 * The color range configuration.
 */
export interface ColorRange {
    /**
     * The color of the range. Accepts valid CSS color strings, including hex and rgb.
     */
    color?: string;
    /**
     * The opacity of the range.
     */
    opacity?: number;
    /**
     * The range start value.
     */
    from?: number;
    /**
     * The range end value.
     */
    to?: number;
}
