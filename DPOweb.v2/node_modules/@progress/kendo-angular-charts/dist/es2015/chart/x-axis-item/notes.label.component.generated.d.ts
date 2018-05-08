import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { Border, NoteLabelPosition } from '../../common/property-types';
import { XAxisNotesLabel } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class XAxisNotesLabelComponentGenerated extends SettingsComponent implements XAxisNotesLabel {
    configurationService: ConfigurationService;
    background: string;
    border: Border;
    color: string;
    content: (e: any) => string;
    font: string;
    format: string;
    position: NoteLabelPosition;
    rotation: number;
    visible: boolean;
    constructor(configurationService: ConfigurationService);
}
