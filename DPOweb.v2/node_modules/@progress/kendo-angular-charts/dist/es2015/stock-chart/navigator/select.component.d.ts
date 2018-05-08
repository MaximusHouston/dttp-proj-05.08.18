import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { MousewheelSelect } from '../../common/property-types';
import { NavigatorSelect } from '../option-types';
/**
 * Specifies the initially selected range.
 * If no range is specified, the full range of values is rendered.
 */
export declare class NavigatorSelectComponent extends SettingsComponent implements NavigatorSelect {
    configurationService: ConfigurationService;
    from: Date;
    to: Date;
    mousewheel: boolean | MousewheelSelect;
    constructor(configurationService: ConfigurationService);
}
