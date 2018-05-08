import { ConfigurationService } from '../../../common/configuration.service';
import { CategoryAxisTitleComponent } from '../../../chart/category-axis-item/title.component';
/**
 * The title configuration of the navigator category axis.
 */
export declare class NavigatorCategoryAxisTitleComponent extends CategoryAxisTitleComponent {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
