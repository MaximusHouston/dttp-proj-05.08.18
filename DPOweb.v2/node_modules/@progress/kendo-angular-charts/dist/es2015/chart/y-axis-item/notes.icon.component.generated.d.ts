import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { Border, MarkerType } from '../../common/property-types';
import { YAxisNotesIcon } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class YAxisNotesIconComponentGenerated extends SettingsComponent implements YAxisNotesIcon {
    configurationService: ConfigurationService;
    background: string;
    border: Border;
    size: number;
    type: MarkerType;
    visible: boolean;
    constructor(configurationService: ConfigurationService);
}
