/**
 * @hidden
 */
export function copyChanges(changes, options) {
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
