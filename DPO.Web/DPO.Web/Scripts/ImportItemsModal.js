window.importItemsModal = new function ()
{
    $(function ()
    {
        $('#content').on('click', '#import_items_btn', function () {
            $('#import_items_modal').fadeIn(200);
        });

        $('#content').on('click', '#import_items_cancel, #import_items_modal .close-btn', function () {
            $('#import_items_modal').fadeOut(200);
        });

        $('#content').on('click', "#import_items_confirm",
            PostAfterConfirm($('#import_items_confirm')[0],
                               '#import_items_form', null,
                               'import_items_modal_container',
                                function () { $('#import_items_modal').show(); }));
    });   
};