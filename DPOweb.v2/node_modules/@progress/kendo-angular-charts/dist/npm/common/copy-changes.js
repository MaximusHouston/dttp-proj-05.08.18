"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/**
 * @hidden
 */
function copyChanges(changes, options) {
    for (var propertyName in changes) {
        if (!changes.hasOwnProperty(propertyName)) {
            continue;
        }
        var value = changes[propertyName].currentValue;
        // We actually care about strong null check
        // tslint:disable-next-line:no-null-keyword
        if (value === null || value === undefined) {
            delete options[propertyName];
        }
        else {
            options[propertyName] = value;
        }
    }
}
exports.copyChanges = copyChanges;
