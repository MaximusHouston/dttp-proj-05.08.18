import { ConfigurationService } from '../../../common/configuration.service';
import { SeriesNotesComponent } from '../../../chart/series-item/notes.component';
/**
 * The notes configuration of the StockChart navigator series.
 */
export declare class NavigatorSeriesNotesComponent extends SeriesNotesComponent {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
