import { ConfigurationService } from '../../../common/configuration.service';
import { SeriesLabelsComponent } from '../../../chart/series-item/labels.component';
/**
 * The label configuration of the StockChart navigator series.
 */
export declare class NavigatorSeriesLabelsComponent extends SeriesLabelsComponent {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
