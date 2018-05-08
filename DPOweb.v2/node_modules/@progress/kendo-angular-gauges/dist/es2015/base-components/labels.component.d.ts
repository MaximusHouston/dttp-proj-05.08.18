import { SettingsComponent } from './settings.component';
import { Border, Margin, Padding, Labels } from '../types';
/**
 * @hidden
 */
export declare class LabelsComponent extends SettingsComponent implements Labels {
    background?: string;
    border?: Border;
    color?: string;
    font?: string;
    format?: string;
    margin?: number | Margin;
    padding?: number | Padding;
    content?: (e: any) => string;
    visible?: boolean;
}
