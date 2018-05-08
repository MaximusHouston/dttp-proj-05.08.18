import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { LegendLabels } from '../../common/property-types';
import { LegendInactiveItems } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class LegendInactiveItemsComponentGenerated extends SettingsComponent implements LegendInactiveItems {
    configurationService: ConfigurationService;
    labels: LegendLabels;
    constructor(configurationService: ConfigurationService);
}
