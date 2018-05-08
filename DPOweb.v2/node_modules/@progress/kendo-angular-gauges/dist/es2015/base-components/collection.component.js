/**
 * @hidden
 */
export class CollectionComponent {
    constructor(key, configurationService, collectionChangesService) {
        this.key = key;
        this.configurationService = configurationService;
        this.collectionChangesService = collectionChangesService;
    }
    ngOnDestroy() {
        this.subscription.unsubscribe();
        this.configurationService.set(this.key, []);
    }
    ngAfterContentInit() {
        this.subscription = this.children.changes.subscribe(() => this.collectionChangesService.hasChanges = true);
    }
    ngAfterContentChecked() {
        if (this.collectionChangesService.hasChanges) {
            this.configurationService.set(this.key, this.readItems());
            this.collectionChangesService.hasChanges = false;
        }
    }
    readItems() {
        return this.children.map(child => Object.assign({}, child.configurationService.read()));
    }
}
