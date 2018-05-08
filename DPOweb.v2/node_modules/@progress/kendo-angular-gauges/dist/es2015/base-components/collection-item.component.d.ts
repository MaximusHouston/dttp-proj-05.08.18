import { OnChanges, SimpleChange } from '@angular/core';
import { CollectionChangesService, ConfigurationService } from '../services';
/**
 * @hidden
 */
export declare class CollectionItemComponent implements OnChanges {
    configurationService: ConfigurationService;
    collectionChangesService: CollectionChangesService;
    constructor(configurationService: ConfigurationService, collectionChangesService: CollectionChangesService);
    ngOnChanges(changes: {
        [propertyName: string]: SimpleChange;
    }): void;
}
