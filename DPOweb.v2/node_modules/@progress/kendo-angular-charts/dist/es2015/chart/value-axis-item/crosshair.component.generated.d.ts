import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { ValueAxisCrosshairTooltip, ValueAxisCrosshair } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class ValueAxisCrosshairComponentGenerated extends SettingsComponent implements ValueAxisCrosshair {
    configurationService: ConfigurationService;
    color: string;
    opacity: number;
    visible: boolean;
    width: number;
    tooltip: ValueAxisCrosshairTooltip;
    constructor(configurationService: ConfigurationService);
}
