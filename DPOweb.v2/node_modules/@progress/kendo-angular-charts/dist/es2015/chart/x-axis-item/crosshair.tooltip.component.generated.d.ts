import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { Border, Padding } from '../../common/property-types';
import { XAxisCrosshairTooltip } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class XAxisCrosshairTooltipComponentGenerated extends SettingsComponent implements XAxisCrosshairTooltip {
    configurationService: ConfigurationService;
    background: string;
    border: Border;
    color: string;
    font: string;
    format: string;
    padding: Padding | number;
    visible: boolean;
    constructor(configurationService: ConfigurationService);
}
