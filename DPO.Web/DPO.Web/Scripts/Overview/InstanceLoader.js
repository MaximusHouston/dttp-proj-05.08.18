// https://www.stevefenton.co.uk/2014/07/creating-typescript-classes-dynamically/
var Overview;
(function (Overview) {
    var InstanceLoader = /** @class */ (function () {
        function InstanceLoader() {
        }
        InstanceLoader.getInstance = function (context, name) {
            var args = [];
            for (var _i = 2; _i < arguments.length; _i++) {
                args[_i - 2] = arguments[_i];
            }
            var instance = Object.create(context[name].prototype);
            instance.constructor.apply(instance, args);
            return instance;
        };
        return InstanceLoader;
    }());
    Overview.InstanceLoader = InstanceLoader;
})(Overview || (Overview = {}));
//# sourceMappingURL=InstanceLoader.js.map