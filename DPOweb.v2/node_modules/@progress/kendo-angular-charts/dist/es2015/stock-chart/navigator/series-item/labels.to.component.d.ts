import { ConfigurationService } from '../../../common/configuration.service';
import { SeriesLabelsToComponent } from '../../../chart/series-item/labels.to.component';
/**
 * The `to` label configuration of the StockChart navigator series.
 */
export declare class NavigatorSeriesLabelsToComponent extends SeriesLabelsToComponent {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
