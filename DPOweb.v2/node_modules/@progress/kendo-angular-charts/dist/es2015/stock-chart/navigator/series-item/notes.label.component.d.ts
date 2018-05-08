import { ConfigurationService } from '../../../common/configuration.service';
import { SeriesNotesLabelComponent } from '../../../chart/series-item/notes.label.component';
/**
 * The label of the notes.
 */
export declare class NavigatorSeriesNotesLabelComponent extends SeriesNotesLabelComponent {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
