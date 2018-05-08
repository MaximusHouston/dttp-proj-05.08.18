import { drawing } from '@progress/kendo-drawing';
import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { AxisLabelVisualArgs, LabelRotation, Margin, Padding } from '../../common/property-types';
import { AxisDefaultsLabels } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class AxisDefaultsLabelsComponentGenerated extends SettingsComponent implements AxisDefaultsLabels {
    configurationService: ConfigurationService;
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
