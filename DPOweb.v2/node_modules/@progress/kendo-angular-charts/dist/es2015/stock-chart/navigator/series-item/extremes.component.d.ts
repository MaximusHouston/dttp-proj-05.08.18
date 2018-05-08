import { ConfigurationService } from '../../../common/configuration.service';
import { SeriesExtremesComponent } from '../../../chart/series-item/extremes.component';
/**
 * The extremes configuration of the StockChart navigator series. Applies to extreme outliers.
 */
export declare class NavigatorSeriesExtremesComponent extends SeriesExtremesComponent {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
