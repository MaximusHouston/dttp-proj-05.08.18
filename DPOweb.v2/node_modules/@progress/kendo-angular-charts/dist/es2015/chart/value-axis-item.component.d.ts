import { CollectionService } from '../common/collection.service';
import { ConfigurationService } from '../common/configuration.service';
import { ValueAxisItemComponentGenerated } from './value-axis-item.component.generated';
/**
 * The configuration component for a value axis.
 */
export declare class ValueAxisItemComponent extends ValueAxisItemComponentGenerated {
    protected configurationService: ConfigurationService;
    protected collectionService: CollectionService;
    constructor(configurationService: ConfigurationService, collectionService: CollectionService);
}
