import { drawing } from '@progress/kendo-drawing';
import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { AxisTitlePosition, Border, Margin, Padding, TitleVisualArgs } from '../../common/property-types';
import { AxisDefaultsTitle } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class AxisDefaultsTitleComponentGenerated extends SettingsComponent implements AxisDefaultsTitle {
    configurationService: ConfigurationService;
    background: string;
    border: Border;
    color: string;
    font: string;
    margin: Margin | number;
    padding: Padding | number;
    position: AxisTitlePosition;
    rotation: number;
    text: string;
    visible: boolean;
    visual: (e: TitleVisualArgs) => drawing.Element;
    constructor(configurationService: ConfigurationService);
}
