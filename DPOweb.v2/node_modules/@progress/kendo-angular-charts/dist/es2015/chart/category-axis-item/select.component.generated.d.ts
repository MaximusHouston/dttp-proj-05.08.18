import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { MousewheelSelect } from '../../common/property-types';
import { CategoryAxisSelect } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class CategoryAxisSelectComponentGenerated extends SettingsComponent implements CategoryAxisSelect {
    configurationService: ConfigurationService;
    from: any;
    max: any;
    min: any;
    mousewheel: boolean | MousewheelSelect;
    to: any;
    constructor(configurationService: ConfigurationService);
}
