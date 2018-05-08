import { ConfigurationService } from '../../common/configuration.service';
import { SeriesDefaultsLabelsFromComponentGenerated } from '../series-defaults/labels.from.component.generated';
/**
 * The `from` label configuration of the Chart series.
 */
export declare class SeriesDefaultsLabelsFromComponent extends SeriesDefaultsLabelsFromComponentGenerated {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
