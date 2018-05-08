import { ConfigurationService } from '../../common/configuration.service';
import { SeriesDefaultsLabelsToComponentGenerated } from '../series-defaults/labels.to.component.generated';
/**
 * The `to` label configuration of the Chart series.
 */
export declare class SeriesDefaultsLabelsToComponent extends SeriesDefaultsLabelsToComponentGenerated {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
