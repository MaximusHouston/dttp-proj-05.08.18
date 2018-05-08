import { ConfigurationService } from '../../../common/configuration.service';
import { SeriesHighlightComponent } from '../../../chart/series-item/highlight.component';
/**
 * The configuration options of the StockChart series highlight.
 */
export declare class NavigatorSeriesHighlightComponent extends SeriesHighlightComponent {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
