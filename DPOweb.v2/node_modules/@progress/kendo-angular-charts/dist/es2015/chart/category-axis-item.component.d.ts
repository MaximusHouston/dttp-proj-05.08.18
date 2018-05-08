import { CollectionService } from '../common/collection.service';
import { ConfigurationService } from '../common/configuration.service';
import { CategoryAxisItemComponentGenerated } from './category-axis-item.component.generated';
/**
 * The configuration component for a category axis.
 */
export declare class CategoryAxisItemComponent extends CategoryAxisItemComponentGenerated {
    protected configurationService: ConfigurationService;
    protected collectionService: CollectionService;
    constructor(configurationService: ConfigurationService, collectionService: CollectionService);
}
