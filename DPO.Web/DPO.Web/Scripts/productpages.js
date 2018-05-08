//basket methods
function initBasketScroll()
{
        var $window = $(window),
            $header = $('#header'),
            headerHeight = $header.offset().top + $header.height(),
            $basket = $('#quoteBasket'),
            $placeholder = $basket.clone();

        $placeholder.attr('id', 'basket-placeholder').css({'visibility':'hidden', 'display':'none'}).insertAfter($basket);

        $window.off('scroll').on('scroll', function (e)
        {
            if($window.scrollTop() > headerHeight && !$basket.hasClass('following'))
            {
                $basket.addClass('following');
                $placeholder.show();
            }
            else if($window.scrollTop() <= headerHeight && $basket.hasClass('following'))
            {
                $basket.removeClass('following');
                $placeholder.hide();
            }
        });

        $window.trigger('scroll');
};

function enableAddQtyToBasket()
{
    //numeric steppers
    //$('#content').on('click', '.numeric-stepper .minus, .numeric-stepper .plus', function () {
    //    var amtsToAdd = false;
    //    $('.numeric-stepper .numbers').each(function () {
    //        if ($(this).val() > 0) {
    //            moreThenOneAmtToAdd = true;
    //            return false;
    //        }
    //    });

    //    (amtsToAdd) ? $('.basket-button').show() : $('.basket-button').hide();
    //});

    //add buttons
    $('#content').on('click', '[data-add]', function ()
    {
        var currentNumericStepper = $(this).parents('.product-qty-picker').find('.numeric-stepper');
        var addParams = {
            ItemId: $(this).attr('data-add'),
            //TODO - make this reference more concrete
            Quantity: $(this).parent().prev().find('input').val()
        };

        $.post('/Userdashboard/BasketUpdateItem', addParams, function (result)
        {
            $('#basketHolder').empty().append(result);
            currentNumericStepper.find('.numbers').val(0);
            initBasketScroll();
        });
    });

    //clear basket button
    $('#content').on('click', '#clearBasketBtn', function ()
    {
        $.post('/Userdashboard/BasketClear', function (result)
        {
            $('#basketHolder').empty().append(result);
            initBasketScroll();
        });
    });
}

//page-level methods
$(function ()
{
    var qtyPickerWidth = $('.product-qty-picker').width();

    if ($('#quoteBasket').length > 0)
    {
        $('.product-ratings').css({ 'margin-right': qtyPickerWidth + 'px' });
        numericStepperHelpers.enableNumericSteppers();
        $('.product-qty-picker').show();
        initBasketScroll();
        enableAddQtyToBasket();
    }

    //$('input[name="Filter"]').attr('placeholder', "Search Products");

    var animationDelay = (Modernizr.csstransitions) ? 650 : 1;

    setTimeout(function () {
        $('.product-ratings .bar-inner').each(function () {
            var thisWidth = (Number($(this).attr('data-actual')) / Number($(this).attr('data-max'))) * 100;
            $(this).css('width', thisWidth + '%');
        });
    }, animationDelay);

    //switch product images (if available)
    $('#content').on('click', '.product-img-btns button', function () {
        var parent = $(this).parents('.product-details');
        var images = parent.find('img').hide();
        parent.find('button').removeClass('active');

        var maxImgHeight = 0;
        images.each(function () {

            var img = $(this);
            var vis = img.is(':visible');

            if (!vis) img.show();
            if (img.height() > maxImgHeight) maxImgHeight = img.height();
            if (!vis) img.hide();
           
        });

        parent.find('a').css('height', maxImgHeight);

        $('#' + $(this).attr('data-img-id')).show();
        $(this).addClass('active');
        return false;
    });

});
