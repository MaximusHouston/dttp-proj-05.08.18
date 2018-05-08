var tablehelper = (function ()
{
    var _allocateTableColumnIdsForDisplayingOrHidingColumns = function (id)
    {
        var self                = this;
        var tableId             = '#' + id;
        var $table              = $(tableId);
        var $tableOptions       = $table.parents('.tbl-holder').prev('.tbl-opts');
        var $columnOptionsList  = $tableOptions.find('.tbl-column-opts');

        //public array of cells that the user cannot toggle visibility of
        self.alwaysVisibleCells         = ['actions','selections','alerts'];
        var cellIsAllowedToBeHidden     = function(cell)
        {
            if (!cell.attr('class')) return true;

            var classes = cell.attr('class').split(" ");
            for(var i = 0; i < classes.length; i++)
            {
                if ($.inArray(classes[i], self.alwaysVisibleCells) > -1) return false;
            }

            return true;
        };

        //assign a column id to each td (if they are allowed to be hidden)
        $table.find('tbody tr').each(function ()
        {
            $(this).find('td').each(function (tdIndex)
            {
                var currentCell = $(this);
                if (cellIsAllowedToBeHidden(currentCell))
                {
                    currentCell.attr('data-col-id', tdIndex);
                } 
            });
        });

        //assign a column id to each th and add a checkbox to the display list
        //(if they are allowed to be hidden)
        var changeColumnVisibility = function (tableId, columnHeadingLocalStorageKey)
        {
            return function (e)
            {
                var $cb     = $(this);
                var colId   = $cb.attr('data-col-id');
                var column  = $(tableId).find('th[data-col-id="' + colId + '"], td[data-col-id="' + colId + '"]');

                if ($cb.is(':checked'))
                {
                    column.show();
                    localStorage.setItem(columnHeadingLocalStorageKey, 'false');
                }
                else
                {
                    column.hide();
                    localStorage.setItem(columnHeadingLocalStorageKey, 'true');
                }     
            }
        };

        $table.find('th').each(function (index)
        {
            var th = $(this);

            if (cellIsAllowedToBeHidden(th))
            {
                var tableHeaderText  = th.text();
                var newCheckBox      = $('<input type="checkbox" data-col-id="' + index + '" checked />');
                var newLabel         = $('<label>' + tableHeaderText + '</label>');
                var newLi            = $('<li></li>');

                th.attr('data-col-id', index);
                newLi.append(newCheckBox);
                newLi.append(newLabel);

                $columnOptionsList.append(newLi);

                newCheckBox.on('change', changeColumnVisibility(tableId, getDPOLocalStorageKeyName(tableId,tableHeaderText)));

                //hide column now if cell has 'data-hidecol' set to true
                if(columnShouldBeHidden(tableId,tableHeaderText,th.attr('data-hidecol')))
                {
                    newCheckBox.removeAttr('checked').trigger('change');
                }
            }
        });

        return self;

    };

    function getDPOLocalStorageKeyName(tableId, columnHeaderText)
    {
        return 'dpo_' + (tableId + '_' + columnHeaderText).replace(/\s+/g, '').toLowerCase();
    }

    function columnShouldBeHidden(tableId,labelText,dataAttribute)
    {
        var localStorageKeyName = getDPOLocalStorageKeyName(tableId,labelText);
        var localStorageValue = localStorage.getItem(localStorageKeyName);
        //if localstorage is true, hide the column
        if (localStorageValue == 'true') return true;
        //if data attribute says to hide the column but the user has unhidden the column, show the column
        if (dataAttribute == 'true' && localStorageValue == 'false') return false;
        //finally, if the data attribute is true, hide the column
        if (dataAttribute == 'true') return true;

        return false;
    }

    function _toggleOpenTableCells()
    {
        $('td.open').not($(this)).removeClass('open');
        $(this).toggleClass('open');
    };

    function _hideOpenTableCells(e)
    {
        if ($(e.target).hasClass('actions') || $(e.target).parents().hasClass('actions')) return;
        if ($(e.target).hasClass('alerts') || $(e.target).parents().hasClass('alerts')) return;

        $('td.open').removeClass('open');
    };

    function _enableActionsCellDropdowns()
    {
       $('#content').on('click', 'td.actions', _toggleOpenTableCells);
       $('body').on('click', _hideOpenTableCells);
       return self;
    };

    function _enableAlertCellPopups()
    {
        $('#content').on('click', 'td.alerts', _toggleOpenTableCells);
        return self;
    };

    //enable action dropdowns by default
    _enableActionsCellDropdowns();

    //enable hiding table filtering dropdown lists by default,
    //and click-toggling of those lists (if any exist):
    if ($('.tbl-filters .display-btn').length)
    {
        $('body').on('click', function (e)
        {
            if ($(e.target).parents('.tbl-filters').length == 0)
            {
                $('.tbl-filters .display-btn').each(function ()
                {
                    if ($(this).next().is(':visible')) $(this).trigger('click');
                });
            }
        });

        $('.tbl-filters .display-btn').on('click', function (e)
        {
            var listToDisplay = $(this).next();
            listToDisplay.toggle();

            var imgSrc = "/Images/btn-dropdown-arrow-" + (listToDisplay.is(':visible') ? 'up-icon.png' : 'down-icon.png');
            $(this).find('img').attr('src', imgSrc);
        });
    }

    //enable row count changing by default (every table should have one)
    $('input[name="PageSize"]').on('change', function () {
        $(this).parents('form').submit();
    });
  
    /*
    function _enableRowSelectionAndSelectAllFunctionality(id, selectCountChangeCallback)
    {
        var tblId               = '#' + id;
        var selectAllId         = tblId + ' th.selections input[type="checkbox"]';
        var selectChkBoxes      = tblId + ' td.selections input';
        var callBackAfterChange = selectCountChangeCallback || function (count) { return; };

        $(selectAllId).on('change', function () {
            ($(this).is(':checked')) ? $(selectChkBoxes).attr('checked', true).trigger('change') : $(selectChkBoxes).removeAttr('checked').trigger('change');
            callBackAfterChange($(selectChkBoxes + ':checked').length);
        });

        $(selectChkBoxes).on('change', function () {
            ($(this).is(':checked')) ? $(this).parents('tr').addClass('selected') : $(this).parents('tr').removeClass('selected');
            callBackAfterChange($(selectChkBoxes + ':checked').length);
        });

        return this;
    };
    */

    return {
        setColumnIds: _allocateTableColumnIdsForDisplayingOrHidingColumns,
        enableActionsCellDropdowns: _enableActionsCellDropdowns,
        enableAlertCellPopups: _enableAlertCellPopups,
        //enableRowSelection: _enableRowSelectionAndSelectAllFunctionality
    }


}());