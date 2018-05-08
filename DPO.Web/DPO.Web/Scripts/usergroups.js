var UserGroupsPageHandler = function (config) {
    var self = this;
    var userGroupsCollapsed = false;

    self.options = $.extend({}, config);
    self.currentGroupId = "0";

    self.init = function () {
        
        $.when(self.loadGroupList(), self.loadUserList(self.currentGroupId)).then(function () {
            self.addEventListeners();
        });
        
        //else, handle errors...
    };

    self.refreshPage = function (groupId) {
        var groupToShowAfterRefresh = (typeof (groupId) === "string") ? groupId : self.currentGroupId;

        $.when(self.loadUserList(groupToShowAfterRefresh), self.loadGroupList()).then(function () {
            var groupName = $('#GroupsList tr[data-groupid="' + groupToShowAfterRefresh + '"]').attr('data-groupname');
            $('#currentGroupTitle').text(groupName);
            $('.modal:visible').fadeOut(200);
        });
    };

    self.loadData = function (route, target, params, successcallback, failcallback) {
        var pendingLoad = $.Deferred();
        params = params || {};

        $.post(route, $.param(params, true), function (result) {
            if (typeof target === "string")
                $(target).empty().append(result);
            if (typeof successcallback === 'function')
                successcallback(target, result);
            pendingLoad.resolve();
        }).fail(function (error) {
            if (typeof failcallback === 'function')
                failcallback(target, error);
            pendingLoad.reject();
            //handle errors...
        });

        return pendingLoad.promise();
    };

    self.loadUserList = function (groupId, filter, targetElement, success, fail) {
        var userListParams = {};

        if (typeof filter === "string" && filter.length > 0)
            userListParams.filter = filter;
        if (typeof groupId === "string")
            userListParams.groupid = groupId;

        targetElement = targetElement || '#UserList';

        return self.loadData(self.options.usersRoute, targetElement, userListParams, success, fail);
    };

    self.loadGroupList = function (filter, isGroupFiltering, targetElement, success, fail) {

        var groupsListParams = {
            filter: filter || '',
            isgroupfiltering: isGroupFiltering || false
        };

        targetElement = targetElement || '#GroupsList';

        return self.loadData(self.options.groupsRoute, targetElement, groupsListParams, success, fail);
    };

    self.addUsersToGroup = function () {
        var selectedUsers = $('#AddMembersToGroupList td.selections input[type=checkbox]:checked');
        if (selectedUsers.length == 0)
            return;

        var userIdsToMove = [];
        $.each(selectedUsers, function (index) {
            userIdsToMove.push($(this).val());
        });

        var addUsersParams = {
            userIds: userIdsToMove,
            groupId: self.currentGroupId
        };

        return self.loadData(self.options.moveUsersRoute, null, addUsersParams, self.refreshPage);
    };

    self.moveIndividualUserToGroup = function (groupid, userid) {
        var moveUsersParams = {
            userIds: [userid],
            groupId: groupid
        };

        return self.loadData(self.options.moveUsersRoute, null, moveUsersParams, self.refreshPage);
    };

    self.moveUsersToGroup = function (groupid) {
        var selectedUsers = $('#UserList table tbody td.selections input[type=checkbox]:checked');
        if (selectedUsers.length == 0)
            return;

        var groupIdThisTime;

        if (typeof groupid === "string") {
            groupIdThisTime = groupid;
        } else {
            var selectedRadioButton = $('#MoveMembersToGroupList table tbody input[type="radio"]:checked');
            if (selectedRadioButton.length !== 1)
                return;

            groupIdThisTime = selectedRadioButton.val();
        }

        var userIdsToMove = [];

        $.each(selectedUsers, function (index) {
            userIdsToMove.push($(this).val());
        });

        var moveUsersParams = {
            userIds: userIdsToMove,
            groupId: groupIdThisTime
        };

        return self.loadData(self.options.moveUsersRoute, null, moveUsersParams, self.refreshPage);
    };

    self.unallocateUsers = function () {
        return self.moveUsersToGroup("0");
    };

    self.addGroup = function (newName, parentId) {
        var addParams = {
            name: newName,
            parentId: parentId || 0
        };

        return self.loadData(self.options.createGroupRoute, '', addParams, self.refreshPage);
    };

    self.editGroup = function () {
        if (!$('#editingGroupId').val() || !$('#editingGroupName').val())
            return;

        var editParams = {
            groupId: $('#editingGroupId').val(),
            newName: $('#editingGroupName').val()
        };

        return self.loadData(self.options.editGroupRoute, null, editParams, self.refreshPage);
    };

    self.moveGroup = function () {
        var selectedRadioButton = $('#MoveGroupList table tbody input[type="radio"]:checked');
        if (selectedRadioButton.length !== 1)
            return;

        var moveParams = {
            fromGroupId: $('#moveGroupId').val(),
            toGroupId: selectedRadioButton.val()
        }

        return self.loadData(self.options.moveGroupRoute, null, moveParams, self.refreshPage);
    };

    self.toggleGroupAdmin = function (userId, makeOwner) {
        var toggleParams = {
            userId: userId,
            makerOwner: makeOwner,
            groupId: self.currentGroupId
        }

        return self.loadData(self.options.toggleGroupAdminRoute, null, toggleParams, function () {
            self.loadUserList(self.currentGroupId);
        });
    };

    self.selectionButtonsToggle = function (tableHolderId, selectionButtonsId) {
        var tableThisTime = $(tableHolderId).find('table');
        var checkBoxesThisTime = tableThisTime.find('td.selections input');
        var selectedCheckboxesThisTime = tableThisTime.find('td.selections input:checked');
        var selectAllCheckBoxThisTime = tableThisTime.find('th.selections input');

        selectAllCheckBoxThisTime.attr('checked', selectedCheckboxesThisTime.length === checkBoxesThisTime.length);

        if (selectionButtonsId) {
            (selectedCheckboxesThisTime.length > 0) ? $(selectionButtonsId).show() : $(selectionButtonsId).hide();
        }
    };

    self.setAddMembersTableColumns = function () {
        $('#AddMembersToGroupList .usergroupname').show();
        $('#AddMembersToGroupList .usergroupowner').hide();
    };

    self.selectAllToggle = function (e, tableHolderId) {
        $(tableHolderId + ' td.selections input').attr('checked', $(e.target).is(':checked') === true);
    };

    self.addEventListeners = function () {
        //set-once listeners
        //detect scroll and move groups list accordingly
        var groupsColumn = $('#GroupsListCol');
        var groupsListTableHolder = $('#GroupsList');
        var userListTableHolder = $('#UserList');
        var addMembersToGroupList = $('#AddMembersToGroupList');
        var userGroupsOffset = 131;
        var $window = $(window);

        $window.on('scroll', function (e) {
            if ($window.scrollTop() >= userGroupsOffset && !groupsColumn.hasClass('following'))
                groupsColumn.addClass('following');
            else if ($window.scrollTop() < userGroupsOffset && groupsColumn.hasClass('following'))
                groupsColumn.removeClass('following');
            else
                return false;
        });

        //enable populating of main user list via click
        groupsListTableHolder.on('click', 'a.grouplink', function (e) {
            var groupId = $(this).parents('tr').attr('data-groupid');
            var groupName = $(this).parents('tr').attr('data-groupname');

            $('#currentGroupTitle').text(groupName);

            self.currentGroupId = groupId;
            self.loadUserList(self.currentGroupId);

            $('#UserSelectionBtns').hide();
            (self.currentGroupId === "0") ? $('#unAllocateBtn').hide() : $('#unAllocateBtn').show();
        });

        //add group - show modal
        groupsListTableHolder.on('click', 'a.createlink', function (e) {
            $('#newGroupParentId').val($(this).parents('tr').attr('data-groupid'));
            $('#group_create_modal').fadeIn(200);
        });

        //add group - show modal
        $('#content').on('click', '#creategroupBtn', function (e) {
            $('#newGroupParentId').val($(this).attr('data-usergroupid'));
            $('#group_create_modal').fadeIn(200);
        });

        //collapse/expand user groups
        $('#content').on('click', '#collapseGroupBtn', function (e) {
           
            var allGroups = $("#GroupsList").find("[data-grouplevel]");
            if (!userGroupsCollapsed) {// collapse
                allGroups.removeClass("open");
                var childrenGroups = $("#GroupsList").find("tr[data-grouplevel!=0]").hide();
                userGroupsCollapsed = true;
                $("#collapseGroupBtn")[0].innerHTML = "Expand";
            } else {//expand
                allGroups.addClass("open");
                var childrenGroups = $("#GroupsList").find("tr[data-grouplevel!=0]").show();
                userGroupsCollapsed = false;
                $("#collapseGroupBtn")[0].innerHTML = "Collapse"
            }
            

            //allGroups.toggleClass("open");
            //var childrenGroups = $("#GroupsList").find("tr[data-grouplevel!=0]").toggle();
        });

        //edit group - show modal
        groupsListTableHolder.on('click', 'a.editlink', function (e) {
            $('#editingGroupId').val($(this).parents('tr').attr('data-groupid'));
            $('#editingGroupName').val($(this).parents('tr').attr('data-groupname'));
            $('#group_edit_modal').fadeIn(200);
        });

        //move group - show modal
        groupsListTableHolder.on('click', 'a.movelink', function (e) {
            $('#moveGroupId').val($(this).parents('tr').attr('data-groupid'));
            var groupToMoveName = $(this).parents('tr').attr('data-groupname');

            self.loadGroupList(null, null, '#MoveGroupList', function () {
                var currentMoveGroupsList = $('#MoveGroupList');

                currentMoveGroupsList.find('td.actions').hide();
                currentMoveGroupsList.find('td.radiocell').show();
                currentMoveGroupsList.find('tr[data-groupid="' + 0 + '"]').hide();
                currentMoveGroupsList.find('tr[data-groupid="' + $('#moveGroupId').val() + '"]').hide();

                $('#move_group_modal').find('h1').text('Move ' + groupToMoveName);
            });

            $('#move_group_modal').fadeIn(200);
        });

        //delete group - show confirm modal
        groupsListTableHolder.on('click', 'a.deletelink', function (e) {
            var groupToDeleteId = $(this).parents('tr').attr('data-groupid');
            var confirmText = self.options.deletegrouptext;

            self.currentGroupId = (groupToDeleteId === self.currentGroupId) ? "0" : self.currentGroupId;

            confirmModal.showConfirmMsg(confirmText.title, confirmText.msg, function () {
                self.loadData(self.options.deleteGroupRoute, null, { groupId: groupToDeleteId }, self.refreshPage);
            });
        });

        //toggle opening and closing group lists
        $('#MoveMembersToGroupList, #GroupsListCol, #MoveGroupList').on('click', 'tr', function (e) {
            if ($(e.target).hasClass('actions') || $(e.target).hasClass('radiocell') || $(e.target).hasClass('grouplink') || $(e.target).is('input[type="radio"]')) return;

            var targetGroup = $(this).is('tr') ? $(this) : $(this).parents('tr');
            var groupLevel = targetGroup.attr('data-grouplevel');
            var groupChildCount = targetGroup.attr('data-childcount');

            if (groupLevel === undefined || groupChildCount === undefined) return;

            groupLevel = parseInt(groupLevel);
            groupChildCount = parseInt(groupChildCount);

            if (groupChildCount === 0) return;

            var targetGroupIsOpen = targetGroup.hasClass('open');
            var allGroups = targetGroup.parents("tbody").find("tr");
            var i = allGroups.index(targetGroup) + 1;

            for (i; i < allGroups.length; i++) {
                var $group = $(allGroups[i]);
                var $groupLevel = parseInt($group.attr('data-grouplevel'));

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

        //selection buttons toggle - main user list
        userListTableHolder.on('change', 'td.selections input', function (e) {
            self.selectionButtonsToggle('#UserList', '#UserSelectionBtns');
        });

        //select all - main user list
        userListTableHolder.on('change', 'th.selections input', function (e) {
            self.selectAllToggle(e, '#UserList');
            self.selectionButtonsToggle('#UserList', '#UserSelectionBtns');
        });

        userListTableHolder.on('click', '.usergroupadmintoggle', function (e) {
            var userId = $(this).parents('tr').attr('data-userid');
            var makeOwner = ($(this).attr('data-isgroupowner').toLowerCase() === "true") ? false : true;

            self.toggleGroupAdmin(userId, makeOwner);
        });

        //selection buttons toggle - add members to group list
        addMembersToGroupList.on('change', 'td.selections input', function (e) {
            self.selectionButtonsToggle('#AddMembersToGroupList', '#addMembersSubmitBtn');
        });

        //select all - add members to group list
        addMembersToGroupList.on('change', 'th.selections input', function (e) {
            self.selectAllToggle(e, '#AddMembersToGroupList');
            self.selectionButtonsToggle('#AddMembersToGroupList', '#addMembersSubmitBtn');
        });

        //buttons
        $('#addMembersBtn').on('click', function (e) {
            $('#addMembersTitle').text("Add to: " + $('#currentGroupTitle').text());
            $('#addMembersSubmitBtn').hide();
            $('#addMembersTableTitle').text("Unallocated Users");

            $('#add_members_modal').fadeIn(200);
            //currently showing unallocated users first
            self.loadUserList("0", null, '#AddMembersToGroupList', self.setAddMembersTableColumns);
        });

        $('#moveMembersBtn').on('click', function (e) {
            self.loadGroupList(null, null, '#MoveMembersToGroupList', function () {
                var currentMoveMembersList = $('#MoveMembersToGroupList');
                currentMoveMembersList.find('td.actions').hide();
                currentMoveMembersList.find('td.radiocell').show();
                currentMoveMembersList.find('tr[data-groupid="' + self.currentGroupId + '"]').hide();
            });

            $('#move_members_modal').fadeIn(200);
        });

        $('#addMembersSubmitBtn').on('click', self.addUsersToGroup);
        $('#moveUsersSubmitBtn').on('click', self.moveUsersToGroup);
        $('#groupEditSubmitBtn').on('click', self.editGroup);
        $('#unAllocateBtn').on('click', self.unallocateUsers);
        $('#moveGroupSubmitBtn').on('click', self.moveGroup);

        //toggle full width group list
        $('#widthToggleBtn').on('click', function (e) {
            $('#GroupsListCol, #groupslist-bg').toggleClass('fullwidth');
            $(this).toggleClass('fullwidth');
        });

        //add group - submit new group
        $('#groupCreateSubmitBtn').on('click', function () {
            if ($('#newGroupName').val().trim().length == 0)
                return;
            self.addGroup($('#newGroupName').val(), $('#newGroupParentId').val());
        });

        //add members modal - search functionality
        $('#add_members_modal .search-btn').on('click', function (e) {
            var searchTerm = $(this).parents('tr').find('input').val();

            $('#addMembersTableTitle').text("Search results");
            self.loadUserList(null, searchTerm, '#AddMembersToGroupList', self.setAddMembersTableColumns);
        });

        //add drag and drop from user list to group list
        if (Modernizr.draganddrop == true) {
            groupsListTableHolder.on('dragover', 'tr', function (e) {
                e.preventDefault();
            }).on('drop', 'tr', function (e) {
                e.preventDefault();

                var userId = e.originalEvent.dataTransfer.getData("Text");
                if (!$('#UserList img[data-userid="' + userId + '"]').length)
                    return;
                if (userId)
                    self.moveIndividualUserToGroup($(this).attr('data-groupid'), userId);
            });

            userListTableHolder.on('dragstart', '.userimg', function (e) {
                e.originalEvent.dataTransfer.setData("Text", $(this).attr('data-userid'));
            });
        }
        ;
    };

    //finally...
    self.init();
  
};

$(document).ready(function () {
    //alert('ok');
})