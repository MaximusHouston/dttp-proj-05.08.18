import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { Border, MarkerType } from '../../common/property-types';
import { SeriesExtremes } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class SeriesExtremesComponentGenerated extends SettingsComponent implements SeriesExtremes {
    configurationService: ConfigurationService;
    background: string;
    border: Border;
    rotation: number;
    size: number;
    type: MarkerType;
    constructor(configurationService: ConfigurationService);
}
