/**
 * @hidden
 */
var CollectionComponent = /** @class */ (function () {
    function CollectionComponent(key, configurationService, collectionChangesService) {
        this.key = key;
        this.configurationService = configurationService;
        this.collectionChangesService = collectionChangesService;
    }
    CollectionComponent.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
        this.configurationService.set(this.key, []);
    };
    CollectionComponent.prototype.ngAfterContentInit = function () {
        var _this = this;
        this.subscription = this.children.changes.subscribe(function () { return _this.collectionChangesService.hasChanges = true; });
    };
    CollectionComponent.prototype.ngAfterContentChecked = function () {
        if (this.collectionChangesService.hasChanges) {
            this.configurationService.set(this.key, this.readItems());
            this.collectionChangesService.hasChanges = false;
        }
    };
    CollectionComponent.prototype.readItems = function () {
        return this.children.map(function (child) { return Object.assign({}, child.configurationService.read()); });
    };
    return CollectionComponent;
}());
export { CollectionComponent };
