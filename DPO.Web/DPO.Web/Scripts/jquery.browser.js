//http://stackoverflow.com/questions/14923301/uncaught-typeerror-cannot-read-property-msie-of-undefined-jquery-tools
// TODO: HACK: Remove any reference to $browser.  It looks like it might be a part of the date picker plug-in that is causing the problem.
jQuery.browser = {};
(function () {
    jQuery.browser.msie = false;
    jQuery.browser.version = 0;
    if (navigator.userAgent.match(/MSIE ([0-9]+)\./)) {
        jQuery.browser.msie = true;
        jQuery.browser.version = RegExp.$1;
    }
})();