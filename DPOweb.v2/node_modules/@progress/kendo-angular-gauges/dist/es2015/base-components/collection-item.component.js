/**
 * @hidden
 */
export class CollectionItemComponent {
    constructor(configurationService, collectionChangesService) {
        this.configurationService = configurationService;
        this.collectionChangesService = collectionChangesService;
    }
    ngOnChanges(changes) {
        this.configurationService.copyChanges('', changes);
        this.collectionChangesService.hasChanges = true;
    }
}
