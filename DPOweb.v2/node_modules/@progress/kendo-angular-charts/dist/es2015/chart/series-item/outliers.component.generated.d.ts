import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { Border, MarkerType } from '../../common/property-types';
import { SeriesOutliers } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class SeriesOutliersComponentGenerated extends SettingsComponent implements SeriesOutliers {
    configurationService: ConfigurationService;
    background: string;
    border: Border;
    rotation: number;
    size: number;
    type: MarkerType;
    constructor(configurationService: ConfigurationService);
}
