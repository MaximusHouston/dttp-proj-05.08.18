import { QueryList } from '@angular/core';
import { ConfigurationService } from '../common/configuration.service';
import { CollectionService, Item } from '../common/collection.service';
import { CollectionComponent } from '../common/collection.component';
/**
 * @hidden
 */
export declare abstract class XAxisComponentGenerated extends CollectionComponent {
    protected configurationService: ConfigurationService;
    protected collectionService: CollectionService;
    children: QueryList<Item>;
    constructor(configurationService: ConfigurationService, collectionService: CollectionService);
}
