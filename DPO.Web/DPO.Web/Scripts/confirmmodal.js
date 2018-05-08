window.confirmModal = new function ()
{
    this.showConfirmMsg = function (title, msg, callback) {
        var elem = $('#confirm_modal');
        elem.find('h1').text(title);
        elem.find('p').text(msg);
        $('#confirm_modal_yes').off('click').on('click', callback);
        elem.fadeIn(200);
    };

    this.showConfirmMsgForElement = function (el, title, msg, callback) {
        var elem = $('#'+el);
        elem.find('h1').text(title);
        elem.find('p').text(msg);
        elem.find('#confirm_modal_yes').off('click').on('click', callback);
        elem.find('.cancel-btn').off('click').on('click', function (e) {
            $(this).parents('.modal').fadeOut(200);
            return false;
        });
        elem.fadeIn(200);
    };
};
