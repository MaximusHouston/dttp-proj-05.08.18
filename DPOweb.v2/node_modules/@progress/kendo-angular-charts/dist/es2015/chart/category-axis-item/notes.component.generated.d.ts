import { drawing } from '@progress/kendo-drawing';
import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { AxisNoteVisualArgs, NoteLine, NotePosition } from '../../common/property-types';
import { CategoryAxisNotesIcon, CategoryAxisNotesLabel, CategoryAxisNotes } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class CategoryAxisNotesComponentGenerated extends SettingsComponent implements CategoryAxisNotes {
    configurationService: ConfigurationService;
    data: any[];
    line: NoteLine;
    position: NotePosition;
    visual: (e: AxisNoteVisualArgs) => drawing.Element;
    icon: CategoryAxisNotesIcon;
    label: CategoryAxisNotesLabel;
    constructor(configurationService: ConfigurationService);
}
