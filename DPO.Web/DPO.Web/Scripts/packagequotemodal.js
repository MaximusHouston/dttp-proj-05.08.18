window.quotePackageModal = new function () {
    $(function () {
        $('#content').on('click', '#attach_new_file_btn', function ()
        {
            $('#quote_package_modal').fadeIn(200);
        });

        $('#content').on('click', '#quote_package_cancel, #quote_package_modal .close-btn', function ()
        {
            $('#quote_package_modal').fadeOut(200);
        });

        $('#content').on('click', "#quote_package_confirm", PostAfterConfirm($('#quote_package_confirm')[0], '#quote_package_form', null, 'quote_package_modal_container', function () { $('#quote_package_modal').show(); }));
    });
};