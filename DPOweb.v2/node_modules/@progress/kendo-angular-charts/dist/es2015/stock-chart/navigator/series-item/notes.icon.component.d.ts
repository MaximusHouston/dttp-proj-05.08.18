import { ConfigurationService } from '../../../common/configuration.service';
import { SeriesNotesIconComponent } from '../../../chart/series-item/notes.icon.component';
/**
 * The icon of the notes.
 */
export declare class NavigatorSeriesNotesIconComponent extends SeriesNotesIconComponent {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
