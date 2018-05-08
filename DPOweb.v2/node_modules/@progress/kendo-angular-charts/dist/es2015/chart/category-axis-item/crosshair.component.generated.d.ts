import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { DashType } from '../../common/property-types';
import { CategoryAxisCrosshairTooltip, CategoryAxisCrosshair } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class CategoryAxisCrosshairComponentGenerated extends SettingsComponent implements CategoryAxisCrosshair {
    configurationService: ConfigurationService;
    color: string;
    dashType: DashType;
    opacity: number;
    visible: boolean;
    width: number;
    tooltip: CategoryAxisCrosshairTooltip;
    constructor(configurationService: ConfigurationService);
}
