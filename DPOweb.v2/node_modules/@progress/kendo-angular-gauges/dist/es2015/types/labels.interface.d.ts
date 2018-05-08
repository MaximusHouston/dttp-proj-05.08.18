import { Border } from './border.interface';
import { Margin } from './margin.interface';
import { Padding } from './padding.interface';
/**
 * The configuration of the scale labels.
 */
export interface Labels {
    /**
     * The background of the labels.
     * Accepts valid CSS color strings, including hex and rgb.
     */
    background?: string;
    /**
     * The border of the labels.
     */
    border?: Border;
    /**
     * The color of the labels.
     * Accepts valid CSS color strings, including hex and rgb.
     */
    color?: string;
    /**
     * The font of the labels.
     */
    font?: string;
    /**
     * The format that is used to display the labels.
     * Uses the IntlService [`format`]({% slug api_intl_intlservice %}#toc-format) method.
     */
    format?: string;
    /**
     * The margin of the labels.
     */
    margin?: number | Margin;
    /**
     * The padding of the labels.
     */
    padding?: number | Padding;
    /**
     * The function which returns the label content.
     *
     * The available fields in the function argument are:
     *
     * - `value`&mdash;The value of the label.
     */
    content?: (e: any) => string;
    /**
     * The visibility of the labels.
     */
    visible?: boolean;
}
