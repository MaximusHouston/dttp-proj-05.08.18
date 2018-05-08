import { drawing } from '@progress/kendo-drawing';
import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { AxisLabelVisualArgs, Border, LabelRotation, Margin } from '../../common/property-types';
import { Padding } from '../../common/property-types';
import { ValueAxisLabels } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class ValueAxisLabelsComponentGenerated extends SettingsComponent implements ValueAxisLabels {
    configurationService: ConfigurationService;
    background: string;
    border: Border;
    color: string;
    content: (e: any) => string;
    font: string;
    format: string;
    margin: Margin | number;
    mirror: boolean;
    padding: Padding | number;
    rotation: LabelRotation | number | 'auto';
    skip: number;
    step: number;
    visible: boolean;
    visual: (e: AxisLabelVisualArgs) => drawing.Element;
    constructor(configurationService: ConfigurationService);
}
