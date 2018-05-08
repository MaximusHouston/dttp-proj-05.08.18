import { SettingsComponent } from './settings.component';
import { Scale, Labels, Ticks } from '../types';
/**
 * @hidden
 */
export declare class ScaleComponent extends SettingsComponent implements Scale {
    labels?: Labels;
    majorTicks?: Ticks;
    minorTicks?: Ticks;
    min?: number;
    max?: number;
    minorUnit?: number;
    majorUnit?: number;
    reverse?: boolean;
    rangeSize?: number;
    rangePlaceholderColor?: string;
}
