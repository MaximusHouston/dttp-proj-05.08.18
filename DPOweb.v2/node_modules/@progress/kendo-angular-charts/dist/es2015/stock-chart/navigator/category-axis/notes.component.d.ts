import { ConfigurationService } from '../../../common/configuration.service';
import { CategoryAxisNotesComponent } from '../../../chart/category-axis-item/notes.component';
/**
 * The configuration of the category axis notes.
 */
export declare class NavigatorCategoryAxisNotesComponent extends CategoryAxisNotesComponent {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
