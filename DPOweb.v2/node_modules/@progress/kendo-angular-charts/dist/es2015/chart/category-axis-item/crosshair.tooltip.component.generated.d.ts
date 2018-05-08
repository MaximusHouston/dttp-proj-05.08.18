import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { Border, Padding } from '../../common/property-types';
import { CategoryAxisCrosshairTooltip } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class CategoryAxisCrosshairTooltipComponentGenerated extends SettingsComponent implements CategoryAxisCrosshairTooltip {
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
