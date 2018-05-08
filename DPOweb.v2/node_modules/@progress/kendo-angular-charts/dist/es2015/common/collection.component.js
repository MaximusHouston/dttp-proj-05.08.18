import { Change } from './configuration.service';
/**
 * @hidden
 */
export class CollectionComponent {
    constructor(configKey, configurationService, collectionService) {
        this.configKey = configKey;
        this.configurationService = configurationService;
        this.collectionService = collectionService;
        this.items = [];
        this.subscription = collectionService.onItemChange$.subscribe(changes => this.processChanges(changes));
    }
    ngOnDestroy() {
        this.subscription.unsubscribe();
    }
    ngAfterContentInit() {
        this.readItems();
        this.children.changes.subscribe(() => this.readItems());
    }
    processChanges(changes) {
        if (!this.children) {
            return;
        }
        const index = this.children.toArray().indexOf(changes.sender);
        if (index < 0) {
            return;
        }
        this.items[index] = changes.options;
        this.change();
    }
    readItems() {
        this.items = this.children.map(s => s.options);
        this.change();
    }
    change() {
        this.configurationService.notify(new Change(this.configKey, this.items));
    }
}
