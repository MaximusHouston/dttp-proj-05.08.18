import { ConfigurationService } from '../../common/configuration.service';
import { SeriesOutliersComponentGenerated } from '../series-item/outliers.component.generated';
/**
 * The configuration of the Chart series outliers.
 * Applies to mild outliers.
 * For more information, refer to the [`series.extremes`]({% slug api_charts_seriesitemcomponent %}#toc-extremes) option.
 */
export declare class SeriesOutliersComponent extends SeriesOutliersComponentGenerated {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
