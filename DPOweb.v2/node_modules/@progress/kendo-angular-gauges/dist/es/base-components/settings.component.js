/**
 * @hidden
 */
var SettingsComponent = /** @class */ (function () {
    function SettingsComponent(key, configurationService) {
        this.key = key;
        this.configurationService = configurationService;
    }
    SettingsComponent.prototype.ngOnChanges = function (changes) {
        this.configurationService.copyChanges(this.key, changes);
    };
    SettingsComponent.prototype.ngOnDestroy = function () {
        this.configurationService.set(this.key, null);
    };
    return SettingsComponent;
}());
export { SettingsComponent };
