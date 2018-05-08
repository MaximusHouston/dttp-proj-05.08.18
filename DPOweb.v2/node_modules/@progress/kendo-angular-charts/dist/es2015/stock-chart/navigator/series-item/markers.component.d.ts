import { ConfigurationService } from '../../../common/configuration.service';
import { SeriesMarkersComponent } from '../../../chart/series-item/markers.component';
/**
 * The marker configuration of the StockChart navigator series.
 */
export declare class NavigatorSeriesMarkersComponent extends SeriesMarkersComponent {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
