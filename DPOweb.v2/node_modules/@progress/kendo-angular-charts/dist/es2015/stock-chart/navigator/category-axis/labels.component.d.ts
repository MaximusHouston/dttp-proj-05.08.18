import { ConfigurationService } from '../../../common/configuration.service';
import { CategoryAxisLabelsComponent } from '../../../chart/category-axis-item/labels.component';
/**
 * The configuration of the axis labels.
 */
export declare class NavigatorCategoryAxisLabelsComponent extends CategoryAxisLabelsComponent {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
