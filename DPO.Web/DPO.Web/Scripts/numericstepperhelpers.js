var numericStepperHelpers = (function () {

   var addClickEventsToAllNumericSteppers = function (options)
    {
        var config = {
            step: 1,
            decimalPoints: 0,
            trailingChars : ''
        };

        $.extend(config, options);

        function updateStepper(isIncrement)
        {
            var stepper = $(this).parents('.numeric-stepper');
            var input = stepper.find('.numbers');
            var _trailingChars = stepper.attr('data-trail') || config.trailingChars;
            var _step = stepper.attr('data-step') || config.step;
            var _decimalPoints = stepper.attr('data-decimal') || config.decimalPoints;

            var inputVal = input.val().split(_trailingChars).join('');

            if (!isIncrement)
            {
                if (inputVal >= _step) input.val(Number(inputVal - _step).toFixed(_decimalPoints) +_trailingChars);
                else (input.val((0).toFixed(_decimalPoints) + _trailingChars));
                return;
            }

            var newVal = (Number(inputVal) + Number(_step)).toFixed(_decimalPoints);
            input.val(newVal + _trailingChars); 
        }

        $('.numeric-stepper .numbers').on('keyup', function ()
        {
            var stepper = $(this).parents('.numeric-stepper');
            var _trailingChars = stepper.attr('data-trail') || config.trailingChars;

            var val = parseInt($(this).val().split(_trailingChars).join(''));

            if (val < 0 || isNaN(val)) $(this).val(0);
        });

        $('.numeric-stepper .minus').on('click', function ()
        {
            updateStepper.call(this, false);  
        });

        $('.numeric-stepper .plus').on('click', function ()
        {
            updateStepper.call(this, true);
        });

        return this;
   };

    return {
        enableNumericSteppers: addClickEventsToAllNumericSteppers
    };
}());