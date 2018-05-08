import { ConfigurationService } from '../../common/configuration.service';
import { LegendItemComponentGenerated } from '../legend/item.component.generated';
/**
 * The configuration of the Chart legend item.
 */
export declare class LegendItemComponent extends LegendItemComponentGenerated {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
