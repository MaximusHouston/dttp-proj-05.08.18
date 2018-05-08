import { ConfigurationService } from '../../../common/configuration.service';
import { CategoryAxisNotesLabelComponent } from '../../../chart/category-axis-item/notes.label.component';
/**
 * The label of the notes.
 */
export declare class NavigatorCategoryAxisNotesLabelComponent extends CategoryAxisNotesLabelComponent {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
