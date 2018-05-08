/**
 * @hidden
 */
var CollectionItemComponent = /** @class */ (function () {
    function CollectionItemComponent(configurationService, collectionChangesService) {
        this.configurationService = configurationService;
        this.collectionChangesService = collectionChangesService;
    }
    CollectionItemComponent.prototype.ngOnChanges = function (changes) {
        this.configurationService.copyChanges('', changes);
        this.collectionChangesService.hasChanges = true;
    };
    return CollectionItemComponent;
}());
export { CollectionItemComponent };
