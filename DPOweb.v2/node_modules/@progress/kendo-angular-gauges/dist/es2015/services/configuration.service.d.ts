import { SimpleChange } from '@angular/core';
/**
 * @hidden
 */
export declare class ConfigurationService {
    options: any;
    hasChanges: boolean;
    valueChange: boolean;
    copyChanges(prefix: string, changes: {
        [propertyName: string]: SimpleChange;
    }): void;
    read(): any;
    readValues(): any;
    readValue(): any;
    set(field: string, value: any): void;
    optionContext(field: string): any;
}
