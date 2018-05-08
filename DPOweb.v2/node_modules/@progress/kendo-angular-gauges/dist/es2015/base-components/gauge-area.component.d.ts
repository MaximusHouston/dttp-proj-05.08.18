import { SettingsComponent } from './settings.component';
import { GaugeArea, Border, Margin } from '../types';
/**
 * @hidden
 */
export declare class GaugeAreaComponent extends SettingsComponent implements GaugeArea {
    background?: string;
    border?: Border;
    height?: number;
    margin?: number | Margin;
    width?: number;
}
