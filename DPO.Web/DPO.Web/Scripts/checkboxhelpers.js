var checkBoxHelper = (function ()
{
    var convertCheckboxListHelperOutputIntoUnorderedList = function ()
    {
        $('.chkbox-hlpr').each(function ()
        {
            var goodHelper = $('<ul></ul>');

            //break content at points defined in data-attribute
            var contentBreakPoints = $(this).attr('data-break-content-at-labels');
            var contentLabelPrefix = $(this).attr('data-label-prefix') || "";

            if (typeof contentBreakPoints === "string")
            {
                contentBreakPoints = contentBreakPoints.split(",");
                for (var i = 0; i < contentBreakPoints.length; i++)
                {
                    contentBreakPoints[i] = contentBreakPoints[i].toLowerCase();
                }
            }
            else
            {
                contentBreakPoints = [];
            }

            $(this).find('input').each(function ()
            {
                var li = $('<li></li>');
                var label = $(this).next();

                if (contentBreakPoints.length)
                {
                    if (label.text().toLowerCase().indexOf(contentBreakPoints[0]) > -1)
                    {
                        goodHelper.append('<li class="fake-separator"><h5>' + contentBreakPoints[0] + ' ' + contentLabelPrefix + '</h5></li>');
                        contentBreakPoints.shift();
                    }
                }

                li.append($(this));
                li.append(label);

                goodHelper.append(li);

            });

            $(this).empty();
            $(this).append(goodHelper);
        });

        return this;
    }

    var setUpDirtyFlagForListsOfCheckboxes = function () {
        $('.chkbox-hlpr-select-all input').on('change', function (e) {
            var allCbs = $(this).parent().next('.chkbox-hlpr').find('input[type="checkbox"]');
            ($(this).is(':checked')) ? allCbs.attr('checked', true) : allCbs.removeAttr('checked');
        });

        return this;
    }

    var disableCheckboxesBasedOnSecurityPriviledges = function(checkBoxList, userCanChangeCBs)
    {
        if(userCanChangeCBs == "False")
        {
            $(checkBoxList + " label").addClass('disabled');
            $(checkBoxList + ' input[type="checkbox"]').attr('disabled', true);
        }
    }

    return {
        createListsOfCheckboxes: convertCheckboxListHelperOutputIntoUnorderedList,
        enableSelectAllOnCheckboxLists: setUpDirtyFlagForListsOfCheckboxes,
        disableCheckboxesBasedOnSecurityPriviledges: disableCheckboxesBasedOnSecurityPriviledges
    }
}());