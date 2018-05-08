import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { DashType } from '../../common/property-types';
import { AxisDefaultsCrosshairTooltip, AxisDefaultsCrosshair } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class AxisDefaultsCrosshairComponentGenerated extends SettingsComponent implements AxisDefaultsCrosshair {
    configurationService: ConfigurationService;
    color: string;
    dashType: DashType;
    opacity: number;
    visible: boolean;
    width: number;
    tooltip: AxisDefaultsCrosshairTooltip;
    constructor(configurationService: ConfigurationService);
}
