import { drawing } from '@progress/kendo-drawing';
import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { AxisNoteVisualArgs, NoteLine, NotePosition } from '../../common/property-types';
import { ValueAxisNotesIcon, ValueAxisNotesLabel, ValueAxisNotes } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class ValueAxisNotesComponentGenerated extends SettingsComponent implements ValueAxisNotes {
    configurationService: ConfigurationService;
    data: any[];
    line: NoteLine;
    position: NotePosition;
    visual: (e: AxisNoteVisualArgs) => drawing.Element;
    icon: ValueAxisNotesIcon;
    label: ValueAxisNotesLabel;
    constructor(configurationService: ConfigurationService);
}
