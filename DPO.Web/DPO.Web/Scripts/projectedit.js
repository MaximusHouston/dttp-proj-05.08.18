$(function () {
    //tab navigation within page
    $('#content').on('click', '.sub-tab-bar li', function ()
    {
        $('.sub-tab-view').hide();
        $('.sub-tab-bar li').removeClass('active-tab');

        var viewToShow = $(this).attr('data-view');
        $('#' + viewToShow).show();

        $(this).addClass('active-tab');
    });

    var projectStatusValue = parseInt($('#ProjectStatusTypeId').val(), 10);

    if (projectStatusValue !== 1) disableEditing();

    function disableEditing()
    {
        var form = $('#ProjectEditForm');

        form.find('input, select').not('#ProjectStatusTypeId').attr('disabled', true);
        form.find('label').not('[for="Description"],[for="ProjectStatusTypeId"]').addClass('disabled');
        form.find('.ui-datepicker-trigger').hide();

        $('#ProjectStatusTypeId').on('change', function ()
        {
            ($(this).val() === '1') ? $('#EditingDisabledMessage').slideDown(100) : $('#EditingDisabledMessage').slideUp(100);
        });
    }


    //delay project edit submission until after fields have been re-enabled(?)
    $('#content').on('click', '#ProjectEditFormSubmitBtn, #EditingDisabledMessage', function ()
    {
        //form sends all fields back, need them all to be enabled
        $('#ProjectEditForm input, #ProjectEditForm select').removeAttr('disabled');
        $('.sub-tab-view').addClass('disabled');

        PostAfterConfirm(this, '#ProjectEditForm', null, null, function (data)
        {
            if( $('.input-validation-error').length)
            {
                var sectionWithError = $('.input-validation-error').eq(0).parents('.sub-tab-view');
                if (!sectionWithError.is(':visible'))
                {
                    $('.sub-tab-bar li[data-view="' + sectionWithError.attr('id') + '"]').click();
                }
            }

        })();
    });

   
    

    $('#shipToAddress #ShipToAddress_CountryCode').on('change', function (e) {
        var value = $('#shipToAddress #ShipToAddress_CountryCode').val();
        $('#projectDetails #ShipToAddress_CountryCode').val(value);

    });

    $('#projectDetails #ShipToAddress_CountryCode').on('change', function (e) {
        var value = $('#projectDetails #ShipToAddress_CountryCode').val();
        $('#shipToAddress #ShipToAddress_CountryCode').val(value);
    });

   
    $('#shipToAddress #ShipToAddress_StateId').on('change', function (e) {
        var value = $('#shipToAddress #ShipToAddress_StateId').val();
        $('#projectDetails #ShipToAddress_StateId').val(value);
        //$('#projectDetails #ShipToAddress_StateId')[0].value = value;

    });

    $('#projectDetails #ShipToAddress_StateId').on('change', function (e) {
        var value = $('#projectDetails #ShipToAddress_StateId').val();
        $('#shipToAddress #ShipToAddress_StateId').val(value);
        //$('#shipToAddress #ShipToAddress_StateId')[0].value = value;
    });
});