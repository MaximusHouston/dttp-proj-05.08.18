import { ConfigurationService } from '../../../common/configuration.service';
import { CategoryAxisCrosshairComponent } from '../../../chart/category-axis-item/crosshair.component';
/**
 * The configuration options of the crosshair.
 */
export declare class NavigatorCategoryAxisCrosshairComponent extends CategoryAxisCrosshairComponent {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
