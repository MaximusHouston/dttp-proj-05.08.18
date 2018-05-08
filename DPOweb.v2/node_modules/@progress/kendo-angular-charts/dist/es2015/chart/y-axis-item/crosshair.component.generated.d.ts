import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { YAxisCrosshairTooltip, YAxisCrosshair } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class YAxisCrosshairComponentGenerated extends SettingsComponent implements YAxisCrosshair {
    configurationService: ConfigurationService;
    color: string;
    opacity: number;
    visible: boolean;
    width: number;
    tooltip: YAxisCrosshairTooltip;
    constructor(configurationService: ConfigurationService);
}
