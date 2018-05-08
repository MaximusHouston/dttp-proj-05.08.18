import { ConfigurationService } from '../../common/configuration.service';
import { SeriesExtremesComponentGenerated } from '../series-item/extremes.component.generated';
/**
 * The configuration of the Chart series extremes.
 * Applies to extreme outliers.
 * For more information, refer to [`series.outliers`]({% slug api_charts_seriesitemcomponent %}#toc-outliers).
 */
export declare class SeriesExtremesComponent extends SeriesExtremesComponentGenerated {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
