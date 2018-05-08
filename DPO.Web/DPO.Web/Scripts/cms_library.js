$(function ()
{
    /* 
    * repopulate main directory list
    */

    function updateDirectoryList(callback)
    {
        $.get("LibraryDirectories", function (result)
        {
            $('#GroupsList').empty().append(result);

            if(typeof callback === "function")
            {
                callback();
            }
        });
    }

    /* 
    * repopulate main document list
    */
    function updateDocumentList(directoryId, newTitle, isLibraryHome)
    {
        isLibraryHome = isLibraryHome || false;
        newTitle = newTitle || $('#document_set_title').text();

        $.get("LibraryDirectoryDocuments", { dirId: directoryId }, function (result)
        {
            $('#DeleteMultipleDocBtn').hide();

            $('#UserList').empty().append(result);

            $('#document_set_title').text(newTitle);
                
            $('#current_directory_id').val(directoryId);

            var docAmt = $('#UserList tbody tr').not('.nodocs').length;

            $('#document_set_amount').text(docAmt + ((docAmt == 1) ? " document" : " documents"));

            (isLibraryHome) ? $('.addBtn').hide() : $('.addBtn').show();

            if($('#library_document_modal').is(':visible'))  $('#library_document_modal').fadeOut(200);

        });
    }

    /* 
    * move document from one directory to another
    */
    function moveDocument(oldDirId, dirId, docId, callback)
    {
        var params = {
            OldDirId: oldDirId,
            DirId: dirId,
            DocId: docId
        };

        $.post("MoveLibraryDocument", params, function (result)
        {
            if (result)
            {
                updateDocumentList(oldDirId);

                if(typeof callback === "function")
                {
                    callback();
                }
            }
        });
    }

    /* 
    * copy document from one directory to another
    */
    function copyDocument(dirId, docId, callback)
    {
        var params = {
            DirId: dirId,
            DocId: docId
        };

        $.post("CopyLibraryDocument", params, function (result) {
            if (result) {
                updateDocumentList(dirId, $('#GroupsList tr[data-dir-id="' + dirId + '"] .grouplink').text());

                if (typeof callback === "function") {
                    callback();
                }
            }
        });
    }

    /* 
    * show edit modal for document, empty if new doc
    */
    function editDocument(id)
    {
        var editParams = {
            docId: (typeof id === 'number') ? id : 0,
            currentDirectoryId: $('#current_directory_id').val()
        };

        $.get("LibraryDocument", editParams, function (result)
        {
            $('#document_modal_content').empty().append(result);
            $('#library_document_modal').fadeIn(200);
        });
    }

    /* 
    * add or edit directory
    */
    function editDirectory(dirId, dirName)
    {
        if (dirName)
        {
            $('#library_directory_edit_id').val(dirId);
            $('#library_directory_parent_id').val(0);
            $('#library_directory_modal').find('h1').text("Edit Directory");
            $('#directory_name_toedit').val(dirName);
            $('#library_directory_modal_add_btn').text("Update");
        }
        else
        {
            $('#library_directory_edit_id').val(0);
            $('#library_directory_parent_id').val(dirId);
            $('#library_directory_modal').find('h1').text("Add Sub Directory");
            $('#directory_name_toedit').val('');
            $('#library_directory_modal_add_btn').text("Add");
        }
    
        $('#library_directory_modal').fadeIn(200);
    }

    /* 
    * save directory changes (or add new sub-directory)
    */
    function saveDirectory()
    {
        var dirName = $('#directory_name_toedit').val();

        if (dirName.length < 1) return;

        var params = {
            DirName: dirName
        };

        var dirId = $('#library_directory_edit_id').val();
        params.DirId = (dirId > 0) ? dirId : $('#library_directory_parent_id').val();
        
        var postRoute = (dirId > 0) ? "LibraryEditDirectory" : "LibraryAddSubDirectory";

        $.post(postRoute, params, function ()
        {
            if (dirId === $('#current_directory_id').val())
            {
                $('#document_set_title').text(dirName);
            }
            updateDirectoryList(function ()
            {
               $('#library_directory_modal').fadeOut(200);
            });
        });
    }

    /*
    * show modal for moving existing document, or copying existing document
    */
    function showDocumentMoveModal(e, isCopy)
    {
        e.preventDefault();
        var docToMoveId = $(this).parents('tr').attr('data-doc-id');
        var currentDirectoryId = $('#current_directory_id').val();

        $.get("LibraryDirectories", function (result)
        {
            if (result)
            {
                $('#moving_document_id').val(docToMoveId);

                var directoryList = $('#move_document_listing');
                directoryList.empty().append(result);
                directoryList.find('tr[data-dir-id="' + currentDirectoryId + '"], ').remove();
                directoryList.find('tr[data-depth="0"]').remove();
                directoryList.find('td.actions').remove();
                directoryList.find('td.radiocell').show();

                $('#library_document_modal_move_btn').off('click').on('click', function ()
                {
                    submitDocumentMoveOrCopy(isCopy);
                });

                $('#library_move_document_modal').find('h1').text((isCopy) ? "Copy Document" : "Move Document");
                $('#library_move_document_modal').fadeIn(200);
                
            }

        });
    };

    /*
     * toggles directory protection
     */
    function toggleDirectoryProtection(dirId)
    {
        $.post("LibraryToggleDirectoryProtection", { dirId: dirId }, function (result)
        {
            updateDirectoryList();
        });
    }

    /* 
    * submit document move or copy
    */
    function submitDocumentMoveOrCopy(isCopy)
    {
        var DirId = $('#move_document_listing input[name="dir_destination"]:checked').val();

        if (!DirId) return;

        var oldDirId = $('#current_directory_id').val();
        var DocId = $('#moving_document_id').val();

        var callback = function ()
        {
            $('#library_move_document_modal').fadeOut(200);
        };

        if (isCopy)
        {
            copyDocument(DirId, DocId, callback);
        }
        else
        {
            moveDocument(oldDirId, DirId, DocId, callback);
        }
    };

    /* 
    * event listeners:
    * delete selected documents
    */
    $('#UserList').on('click', 'input[name="docToDelete"]', function (e)
    {
       ($('input[name="docToDelete"]:checked').length > 0) ? $('#DeleteMultipleDocBtn').show() :  $('#DeleteMultipleDocBtn').hide();
    });

    $('#DeleteMultipleDocBtn').on('click', function (e)
    {
        e.preventDefault();

        var $docsToDelete = $('input[name="docToDelete"]:checked');
        if ($docsToDelete.length === 0) return;

        var dirId = $('#current_directory_id').val(),
            docIds = [];

        $docsToDelete.each(function()
        {
            docIds.push($(this).val());
        });

        confirmModal.showConfirmMsg("Delete Documents", "Are you sure you want to delete these documents?", function ()
        {
            $.ajax({
                type: "POST",
                url : "LibraryDeleteDocuments",
                data : { DocIds: docIds, DirId: dirId },
                dataType : "json",
                traditional : true,
                success: function (result) {
                    updateDocumentList(dirId);
                    $('#confirm_modal').fadeOut(200);
                }
            });
        });
    });

    /* 
    * event listeners:
    * get list of documents when directory's link is clicked
    */
    $('#GroupsList').on('click', '.grouplink', function (e)
    {
        e.preventDefault();

        var newTitle = $(this).text();
        var isLibraryHome = ($(this).parents('tr').attr('data-depth') == 0);
        var directoryId = parseFloat($(this).parents('tr').attr('data-dir-id'));

        updateDocumentList(directoryId, newTitle, isLibraryHome);
    });

    /* 
    * event listeners:
    * show modal for editing existing document
    */
    $('#content').on('click', '.editDocLink', function (e)
    {
        e.preventDefault();
        var documentId = parseFloat($(this).parents('tr').attr('data-doc-id'));
        editDocument(documentId);
    });

    /* 
    * event listeners:
    * show modal for editing existing directory
    */
    $('#GroupsList').on('click', '.editdirlink', function ()
    {
        var dirId = parseFloat($(this).parents('tr').attr('data-dir-id'));
        var dirName = $(this).parents('tr').find('.grouplink').text();
        editDirectory(dirId, dirName);
    });

   /* 
   * event listeners:
   * show modal for adding sub-directory
   */
    $('#GroupsList').on('click', '.adddirlink', function ()
    {
        var dirId = parseFloat($(this).parents('tr').attr('data-dir-id'));
        editDirectory(dirId);
    });

    /*
     * Protect / unprotect directories
     */
    $('#GroupsList').on('click', '.protectdirlink', function (e)
    {
        e.preventDefault();

        var dirId = parseFloat($(this).parents('tr').attr('data-dir-id'));
        toggleDirectoryProtection(dirId);
    });

    /* 
    * event listeners:
    * save (or add) directory
    */
    $('#library_directory_modal_add_btn').on('click', saveDirectory);

    /* 
    * event listeners:
    * show modal for moving existing directory
    */
    $('#GroupsList').on('click', '.movedirlink', function (e) {

        e.preventDefault();

        var dirToMoveId = $(this).parents('tr').attr('data-dir-id');

        $.get("LibraryDirectories", function (result)
        {
            if(result)
            {
                $('#moving_directory_id').val(dirToMoveId);
                $('#move_directory_listing').empty().append(result);
                $('#move_directory_listing tr[data-dir-id="' + dirToMoveId + '"]').remove();
                $('#move_directory_listing td.actions').remove();
                $('#move_directory_listing td.radiocell').show();
            }

            $('#library_move_directory_modal').fadeIn(200);
        });
    });

    /*
    * event listeners:
    * move or copy document
    */
    $('#UserList').on('click', '.moveDocLink', function (e) { showDocumentMoveModal.call(this, e, false); });
    $('#UserList').on('click', '.copyDocLink', function (e) { showDocumentMoveModal.call(this, e, true); });

    /*
    * event listeners:
    * move directory
    */
    $('#library_directory_modal_move_btn').on('click', function ()
    {
        if (!$('#move_directory_listing input[name="dir_destination"]:checked').val()) return;

        var params = {
            DirId : $('#moving_directory_id').val(),
            NewParentDirId: $('#move_directory_listing input[name="dir_destination"]:checked').val()
        };

        $.post("LibraryDirectoryMove", params, function (result)
        {
            if(result)
            {
                updateDirectoryList(function ()
                {
                    $('#library_move_directory_modal').fadeOut(200);
                });
            }
        });
    });

    /* 
    * event listeners:
    * show modal for adding new document
    */
    $('#addMembersBtn').on('click', editDocument);

    /*
    * event listeners:
    * edit or add document
    */
    $('#documentEditIframe').load(function ()
    {
        try
        {
            var response = JSON.parse($(this).contents().find('body').text());
            $('#library_document_modal [class^="pagemessage"]').remove();

            if (response.error)
            {
                $('#library_document_modal h1').after('<div class="pagemessage-error h-slim">' + response.error + '</div>');
            }
            else
            {
                updateDocumentList($('#current_directory_id').val());
            }

            this.contentDocument.location.href = '/Images/lightboxmask.png';
        }
        catch(error){/*error gets generated when location is changed */} 
    });

    /*
    * event listeners:
    * show confirm modal before deleting document
    */
    $('#UserList').on('click', '.deleteDocLink', function (e)
    {
        e.preventDefault();
        var docId = $(this).parents('tr').attr('data-doc-id');
        var dirId = $('#current_directory_id').val();
        if (!docId) return;

        confirmModal.showConfirmMsg("Delete Document", "Are you sure you want to delete this document from this location?", function ()
        {
            $.post("LibraryDeleteDocument", { DocId: docId, DirId : dirId }, function (result)
            {
                updateDocumentList(dirId);
                $('#confirm_modal').fadeOut(200);
            });
        });
    });

    /*
   * event listeners:
   * show confirm modal before deleting directory
   */
    $('#GroupsList').on('click', '.deletedirlink', function (e)
    {
        e.preventDefault();
        var dirId = $(this).parents('tr').attr('data-dir-id');

        confirmModal.showConfirmMsg("Delete Directory", "Are you sure you want to delete this directory?", function ()
        {
            $.post("LibraryDeleteDirectory", { DirId: dirId }, function (result)
            {
                updateDirectoryList(function () { $('#confirm_modal').fadeOut(200); });
            });
        });
    });

    /* 
    * detect scroll and move groups list accordingly
    */
    var groupsColumn = $('#GroupsListCol');
    var groupsListTableHolder = $('#GroupsList');
    var userListTableHolder = $('#UserList');
    var userGroupsOffset = 126;
    var $window = $(window);

    $window.on('scroll', function (e) {
        if ($window.scrollTop() >= userGroupsOffset && !groupsColumn.hasClass('following'))
            groupsColumn.addClass('following');
        else if ($window.scrollTop() < userGroupsOffset && groupsColumn.hasClass('following'))
            groupsColumn.removeClass('following');
        else
            return false;
    });

    /* 
    * toggle full width group list
    */
    $('#widthToggleBtn').on('click', function (e) {
        $('#GroupsListCol, #groupslist-bg').toggleClass('fullwidth');
        $(this).toggleClass('fullwidth');
    });

    /* 
    * toggle opening and closing group lists
    */
    $('#GroupsListCol').on('click', 'tr', function (e) {
        if ($(e.target).hasClass('actions') || $(e.target).hasClass('radiocell') || $(e.target).hasClass('grouplink') || $(e.target).is('input[type="radio"]') || $(e.target).is('a')) return;

        var targetGroup = $(this).is('tr') ? $(this) : $(this).parents('tr');
        var groupLevel = targetGroup.attr('data-depth');
        var groupChildCount = targetGroup.attr('data-childcount');

        if (groupLevel === undefined || groupLevel == 0 || groupChildCount === undefined) return;

        groupLevel = parseInt(groupLevel);
        groupChildCount = parseInt(groupChildCount);

        if (groupChildCount === 0) return;

        var targetGroupIsOpen = targetGroup.hasClass('open');
        var allGroups = targetGroup.parents("tbody").find("tr");
        var i = allGroups.index(targetGroup) + 1;

        for (i; i < allGroups.length; i++) {
            var $group = $(allGroups[i]);
            var $groupLevel = parseInt($group.attr('data-depth'));

            if ($groupLevel === groupLevel) {
                break;
            }

            if ($groupLevel === groupLevel + 1) {
                (targetGroupIsOpen) ? $group.removeClass('open').hide() : $group.show();
            }

            else if ($groupLevel > groupLevel + 1) {
                if (targetGroupIsOpen) {
                    $group.removeClass('open').hide();
                }
            }
        }

        targetGroup.toggleClass('open');

    });

    /* 
    * drag and drop documents if browser supports it
    */
    if (Modernizr.draganddrop == true)
    {
        groupsListTableHolder.on('dragover', 'tr', function (e)
        {
            e.preventDefault();
        }).on('drop', 'tr', function (e)
        {
            e.preventDefault();

            var userId = e.originalEvent.dataTransfer.getData("Text");
            if (!$('#UserList tr[data-doc-id="' + userId + '"]').length || $(this).attr('data-depth') == 0)
            {
                return;
            }  
            if (userId)
            {
                moveDocument($("#current_directory_id").val(), $(this).attr('data-dir-id'), userId);
            }
        });

        userListTableHolder.on('dragstart', '.movedocimg', function (e) {
            e.originalEvent.dataTransfer.setData("Text", $(this).parents('tr').attr('data-doc-id'));
        });
    }

});