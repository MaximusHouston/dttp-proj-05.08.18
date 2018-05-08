import { ConfigurationService } from '../../../common/configuration.service';
import { SeriesLabelsFromComponent } from '../../../chart/series-item/labels.from.component';
/**
 * The `from` label configuration of the StockChart navigator series.
 */
export declare class NavigatorSeriesLabelsFromComponent extends SeriesLabelsFromComponent {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
