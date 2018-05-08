import { ConfigurationService } from '../../../common/configuration.service';
import { SeriesErrorBarsComponent } from '../../../chart/series-item/error-bars.component';
/**
 * The error bars of the StockChart navigator series.
 */
export declare class NavigatorSeriesErrorBarsComponent extends SeriesErrorBarsComponent {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
