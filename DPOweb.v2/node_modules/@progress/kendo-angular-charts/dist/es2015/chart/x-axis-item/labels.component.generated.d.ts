import { drawing } from '@progress/kendo-drawing';
import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { AxisLabelVisualArgs, Border, DateFormats, LabelRotation } from '../../common/property-types';
import { Margin, Padding } from '../../common/property-types';
import { XAxisLabels } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class XAxisLabelsComponentGenerated extends SettingsComponent implements XAxisLabels {
    configurationService: ConfigurationService;
    background: string;
    border: Border;
    color: string;
    content: (e: any) => string;
    culture: string;
    dateFormats: DateFormats;
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
