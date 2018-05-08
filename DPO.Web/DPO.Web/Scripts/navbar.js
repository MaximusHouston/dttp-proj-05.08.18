$(function () {
    //add click listener to locations list
    if ($('#city_locations_list').length) {
        $('#city_locations_list').on('click', function () {
            $(this).toggleClass('open');
        });

        $('body').on('click', function (e) {
            if ($('#city_locations_list').hasClass('open')) {
                if ($(e.target).attr('id') !== "city_locations_list" && $(e.target).parents('#city_locations_list').length == 0) {
                    $('#city_locations_list').removeClass('open');
                }
            }

        });
    }
    //add click listener to user login bar (if signed in)
    if ($('#user_account_options').length) {
        $('#user_account_options').on('click', function () {
            $(this).toggleClass('open');
        });

        //add click listener to body to hide top nav user opts
        $('body').on('click', function (e) {
            if ($('#user_account_options').hasClass('open')) {
                if ($(e.target).attr('id') !== "user_account_options" && $(e.target).parents('#user_account_options').length == 0) {
                    $('#user_account_options').removeClass('open');
                }
            }

        });
    }

   
});

function GoToLibrary() {
    $('#building_3')[0].click();
}