import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { Border, MarkerType } from '../../common/property-types';
import { XAxisNotesIcon } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class XAxisNotesIconComponentGenerated extends SettingsComponent implements XAxisNotesIcon {
    configurationService: ConfigurationService;
    background: string;
    border: Border;
    size: number;
    type: MarkerType;
    visible: boolean;
    constructor(configurationService: ConfigurationService);
}
