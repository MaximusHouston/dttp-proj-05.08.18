import { ConfigurationService } from '../../../common/configuration.service';
import { SeriesOutliersComponent } from '../../../chart/series-item/outliers.component';
/**
 * The outliers configuration of the StockChart navigator series. Applies to mild outliers.
 */
export declare class NavigatorSeriesOutliersComponent extends SeriesOutliersComponent {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
