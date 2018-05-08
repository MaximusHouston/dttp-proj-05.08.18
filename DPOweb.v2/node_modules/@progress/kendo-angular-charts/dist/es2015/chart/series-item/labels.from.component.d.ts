import { ConfigurationService } from '../../common/configuration.service';
import { SeriesLabelsFromComponentGenerated } from '../series-item/labels.from.component.generated';
/**
 * The `from` label configuration of the Chart series.
 */
export declare class SeriesLabelsFromComponent extends SeriesLabelsFromComponentGenerated {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
