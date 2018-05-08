import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { Border, MarkerType } from '../../common/property-types';
import { SeriesNotesIcon } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class SeriesNotesIconComponentGenerated extends SettingsComponent implements SeriesNotesIcon {
    configurationService: ConfigurationService;
    background: string;
    border: Border;
    size: number;
    type: MarkerType;
    visible: boolean;
    constructor(configurationService: ConfigurationService);
}
