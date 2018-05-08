import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { Border, Padding } from '../../common/property-types';
import { SeriesDefaultsTooltip } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class SeriesDefaultsTooltipComponentGenerated extends SettingsComponent implements SeriesDefaultsTooltip {
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
