import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { XAxisCrosshairTooltip, XAxisCrosshair } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class XAxisCrosshairComponentGenerated extends SettingsComponent implements XAxisCrosshair {
    configurationService: ConfigurationService;
    color: string;
    opacity: number;
    visible: boolean;
    width: number;
    tooltip: XAxisCrosshairTooltip;
    constructor(configurationService: ConfigurationService);
}
