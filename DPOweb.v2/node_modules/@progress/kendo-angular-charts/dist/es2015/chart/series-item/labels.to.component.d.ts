import { ConfigurationService } from '../../common/configuration.service';
import { SeriesLabelsToComponentGenerated } from '../series-item/labels.to.component.generated';
/**
 * The `to` label configuration of the Chart series.
 */
export declare class SeriesLabelsToComponent extends SeriesLabelsToComponentGenerated {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
