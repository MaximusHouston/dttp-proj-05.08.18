/**
 * @hidden
 */
export class SettingsComponent {
    constructor(key, configurationService) {
        this.key = key;
        this.configurationService = configurationService;
    }
    ngOnChanges(changes) {
        this.configurationService.copyChanges(this.key, changes);
    }
    ngOnDestroy() {
        this.configurationService.set(this.key, null);
    }
}
