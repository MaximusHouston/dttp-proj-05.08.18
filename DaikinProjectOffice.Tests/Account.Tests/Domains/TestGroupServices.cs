//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DPO.Common;
using DPO.Data;
using System.Transactions;
using System.Data.Entity.Core.Objects;
using System.Data.Common;
using DPO.Domain;
using System.Net.Mail;
using DPO.Resources;
using NUnit.Framework;
using NUnit.Common;

namespace DaikinProjectOffice.Tests
{

   [TestFixture]
   public partial class TestUserGroupServices : TestAdmin
   {
      UserSessionModel model = new UserSessionModel();

      UserGroupsServices service;

      public TestUserGroupServices()
      {
          service = new UserGroupsServices(this.TContext);
      }


      [Test]
      public void TestUserGroupServices_Group_Tree_In_Correct_Order()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var result = service.GroupsListModel(sa, null).Model as UserGroupsModel;

          string levelsequence = "";

          result.UserGroups.ForEach(m => levelsequence += m.Level);

          Assert.IsTrue(levelsequence == "011011");

      }

      [Test]
      public void TestUserGroupServices_Group_Tree_Shows_Only_Groups_For_The_User()
      {
          var rm = GetUserSessionModel("USRM2@Somewhere.com");

          var result1 = service.GroupsListModel(rm, null).Model as UserGroupsModel;

          Assert.IsTrue(result1.UserGroups.Count == 2);

      }

      [Test]
      public void TestUserGroupServices_Group_Tree_Show_Unallocated_Users()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var am2 = GetUserSessionModel("USAM2@Somewhere.com");

          var usr = GetUserSessionModel("US8@Somewhere.com");

          var am4 = GetUserSessionModel("USAM4@Somewhere.com");

          var result = service.GroupsListModel(sa, null).Model as UserGroupsModel;

          Assert.IsTrue(result.UnAllocatedGroup.MemberCount == 3);

          var result1 = service.GroupsListModel(am2, null).Model as UserGroupsModel;

          Assert.IsTrue(result1.UnAllocatedGroup.MemberCount == 1);

          var result2 = service.GroupsListModel(usr, null).Model as UserGroupsModel;

          Assert.IsTrue(result2.UnAllocatedGroup.MemberCount ==0);

          var result3 = service.GroupsListModel(am4, null).Model as UserGroupsModel;

          Assert.IsTrue(result3.UnAllocatedGroup.MemberCount == 2);

      }
      [Test]
      public void TestUserGroupServices_Show_Users_For_A_Given_GroupId()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var am2 = GetUserSessionModel("USAM2@Somewhere.com");

          var usr = GetUserSessionModel("us8@Somewhere.com");

          var am4 = GetUserSessionModel("USAM4@Somewhere.com");

          var groups = service.GroupsListModel(sa, null).Model as UserGroupsModel;

          var users = service.GroupUsersListModel(sa, null, groups.UserGroups[2].GroupId).Model as List<UserListModel>;

          Assert.IsTrue(groups.UserGroups[2].MemberCount == users.Count);

          var users2 = service.GroupUsersListModel(sa, null, groups.UserGroups[4].GroupId).Model as List<UserListModel>;

          Assert.IsTrue(groups.UserGroups[4].MemberCount == users2.Count + 1);


      }

      [Test]
      public void TestUserGroupServices_Move_Users_Between_Groups()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var rm2 = GetUserSessionModel("USRM2@Somewhere.com");

          var us4 = GetUserSessionModel("US4@Somewhere.com");

          var us5 = GetUserSessionModel("US5@Somewhere.com");

          // Get current users from am4
          var before = service.GroupUsersListModel(rm2, null, null).Model as List<UserListModel>;

          var grp = this.TContext.Groups.Where(g => g.Name == "Test Dallas").FirstOrDefault();

          service.GroupUsersMove(sa, new long[] { us4.UserId, us5.UserId}, grp.GroupId);

          var after = service.GroupUsersListModel(rm2, null, null).Model as List<UserListModel>;

          Assert.IsTrue(before.Count == after.Count);
      }

      [Test]
      public void TestUserGroupServices_Move_All_Users_Out_Should_leave_member_count_zero()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var us6 = GetUserSessionModel("USAM6@Somewhere.com");

          var moveto = this.TContext.Groups.Where(g => g.Name == "Test Chicago").FirstOrDefault();

          // Get current users from am4
          var grp = this.TContext.Groups.Where(g => g.Name == "Test Dallas").FirstOrDefault();

          var savemembercount = grp.MemberCount;

          Assert.IsTrue(savemembercount == 2);

          service.GroupUsersMove(sa, new long[] { us6.UserId }, moveto.GroupId);

          this.TContext.ObjectContext.Refresh(RefreshMode.StoreWins, grp);

          Assert.IsTrue(grp.MemberCount == 1);
      }


      [Test]
      public void TestUserGroupServices_Prevent_Moving_Users_Between_Groups_Outside_User_Scope()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var de1 = GetUserSessionModel("USDE1@Somewhere.com");

          var us4 = GetUserSessionModel("US4@Somewhere.com");

          var us5 = GetUserSessionModel("US5@Somewhere.com");

          var before = service.GroupUsersListModel(de1, null, null).Model as List<UserListModel>;

          var grp = this.TContext.Groups.Where(g => g.Name == "Test Florida").FirstOrDefault(); // group outside scope

          var response = service.GroupUsersMove(de1, new long[] { us4.UserId, us5.UserId }, grp.GroupId);

          Assert.IsTrue(response.Messages.Items.Any(m => m.Text == ResourceModelUserGroups.UG003));

          var after = service.GroupUsersListModel(de1, null, null).Model as List<UserListModel>;

          Assert.IsTrue(before.Count== after.Count);
      }

      [Test]
      public void TestUserGroupServices_Prevent_Moving_Users_Outside_User_Scope()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var am3 = GetUserSessionModel("USAM3@Somewhere.com");

          var us1 = GetUserSessionModel("US1@Somewhere.com"); //not allowed

          var us5 = GetUserSessionModel("US5@Somewhere.com");

          var before = service.GroupUsersListModel(am3, null, null).Model as List<UserListModel>;

          var grp = this.TContext.Groups.Where(g => g.Name == "Test Florida").FirstOrDefault();

          var response = service.GroupUsersMove(am3, new long[] { us1.UserId, us5.UserId }, grp.GroupId);

          Assert.IsTrue(response.Messages.Items.Any(m => m.Text == ResourceModelUserGroups.UG003));

          var after = service.GroupUsersListModel(am3, null, null).Model as List<UserListModel>;

          Assert.IsTrue(before.Count == after.Count);
      }

      [Test]
      public void TestUserGroupServices_Prevent_Moving_If_Not_Group_Owner()
      {
          var user = GetUserSessionModel("USRM1@Somewhere.com");

          var us4 = GetUserSessionModel("US4@Somewhere.com");

          var us5 = GetUserSessionModel("US5@Somewhere.com");

          // Get current users from am4
          var grp = this.TContext.Groups.Where(g => g.Name == "Test Dallas").FirstOrDefault();

          var response = service.GroupUsersMove(user, new long[] { us4.UserId, us5.UserId }, grp.GroupId);

          Assert.IsTrue(response.Messages.Items.Any(m => m.Text == ResourceModelUserGroups.UG003));

      }

      [Test]
      public void TestUserGroupServices__Same_User_Changing_Own_GroupOwnership_NotAllowed()
      {
          var am6 = GetUserSessionModel("USAM6@Somewhere.com");

          var response = service.GroupUserMakeOwner(am6, am6.GroupId.Value, am6.UserId, true);

          Assert.IsTrue(response.Messages.Items.Any(m => m.Text == ResourceModelUserGroups.UG014));
      }

      [Test]
      public void TestUserGroupServices__Same_User_Changing_GroupOwnership_Of_User_InSame_Group_Not_Allowed()
      {
          var am6 = GetUserSessionModel("USAM6@Somewhere.com");

          var us5 = GetUserSessionModel("US5@Somewhere.com");

          // Get current users from am4
          var response = service.GroupUserMakeOwner(am6, us5.GroupId.Value, us5.UserId, true);

          Assert.IsTrue(response.Messages.Items.Any(m => m.Text == ResourceModelUserGroups.UG015));
      }

      [Test]
      public void TestUserGroupServices__Same_User_Changing_GroupOwnership_Unallocated_User_Not_Allowed()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var us = GetUserSessionModel("US7@Somewhere.com");

          // Get current users from am4
          var response = service.GroupUserMakeOwner(sa, us.GroupId.Value, us.UserId, true);

          Assert.IsTrue(response.Messages.Items.Any(m => m.Text == ResourceModelUserGroups.UG016));

      }


      [Test]
      public void TestUserGroupServices_Delete_User_From_Group_Goes_To_Unallocated()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var usr = GetUserSessionModel("US5@Somewhere.com");

          var result = service.GroupsListModel(sa, null).Model as UserGroupsModel;

          Assert.IsTrue(result.UnAllocatedGroup.MemberCount == 3);

          var result1 = service.GroupUsersMove(sa, new long[] { usr.UserId }, 0);

          result = service.GroupsListModel(sa, null).Model as UserGroupsModel;

          Assert.IsTrue(result.UnAllocatedGroup.MemberCount == 4);
      }

      [Test]
      public void TestUserGroupServices_Move_Group()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var grp = this.TContext.Groups.Where(g => g.Name == "Test Dallas").FirstOrDefault();

          var newParentGroup = this.TContext.Groups.Where(g => g.Name == "Test Florida").FirstOrDefault();

          var result1 = service.GroupMove(sa, grp.GroupId,newParentGroup.GroupId);

          this.TContext.ObjectContext.Refresh(RefreshMode.StoreWins, grp);

          Assert.IsTrue(grp.ParentGroupId == newParentGroup.GroupId);
      }

      [Test]
      public void TestUserGroupServices_Group_Cant_Move_To_Child_Group()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var grp = this.TContext.Groups.Where(g => g.Name == "Test Eastern").FirstOrDefault();

          var newParentGroup = this.TContext.Groups.Where(g => g.Name == "Test Dallas").FirstOrDefault();

          var response = service.GroupMove(sa, grp.GroupId, newParentGroup.GroupId);

          Assert.IsTrue(response.Messages.Items.Any(m => m.Text == ResourceModelUserGroups.UG006));

          var grp1 = this.TContext.Groups.Where(g => g.Name == "Test Eastern").FirstOrDefault();

          //make sure nothing has changed
          Assert.IsTrue(grp1.ParentGroupId == grp.ParentGroupId);
      }


      [Test]
      public void TestUserGroupServices_Group_Cant_Move_Out_Of_User_Scope()
      {
          var rm1 = GetUserSessionModel("USRM1@Somewhere.com");

          var parent = this.TContext.Groups.Where(g => g.Name == "Test Eastern").FirstOrDefault();

          var child = this.TContext.Groups.Where(g => g.Name == "Test Dallas").FirstOrDefault();

          var acccessparent = this.TContext.Groups.Where(g => g.Name == "Test Central").FirstOrDefault();

          var acccesschild = this.TContext.Groups.Where(g => g.Name == "Test Florida").FirstOrDefault();

          // no access to parent
          var response = service.GroupMove(rm1, child.GroupId, acccessparent.GroupId );

          Assert.IsTrue(response.Messages.Items.Any(m => m.Text == ResourceModelUserGroups.UG005));

          this.TContext.ObjectContext.Refresh(RefreshMode.StoreWins, parent);

          this.TContext.ObjectContext.Refresh(RefreshMode.StoreWins, child);

          //make sure nothing has changed
          Assert.IsTrue(parent.GroupId == child.ParentGroupId);

          // do it the other way to make sure

          response = service.GroupMove(rm1, acccesschild.GroupId, parent.GroupId);

          Assert.IsTrue(response.Messages.Items.Any(m => m.Text == ResourceModelUserGroups.UG007));

          this.TContext.ObjectContext.Refresh(RefreshMode.StoreWins, parent);

          this.TContext.ObjectContext.Refresh(RefreshMode.StoreWins, child);

          //make sure nothing has changed
          Assert.IsTrue(parent.GroupId == child.ParentGroupId);
      }



      [Test]
      public void TestUserGroupServices_Move_Group_Alters_GroupPaths()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var grp = this.TContext.Groups.Where(g => g.Name == "Test Dallas").FirstOrDefault();

          var savepath = grp.Path;

          var newParentGroup = this.TContext.Groups.Where(g => g.Name == "Test Florida").FirstOrDefault();

          var result1 = service.GroupMove(sa, grp.GroupId, newParentGroup.GroupId);

          var newGrp = this.TContext.Groups.Where(g => g.Name == "Test Dallas").FirstOrDefault();

          Assert.IsTrue(savepath != newGrp.Path);

          Assert.IsTrue(newGrp.Path.IndexOf(newParentGroup.GroupId.ToString() + "\\" + newGrp.GroupId.ToString()) >= 1);


      }

      [Test]
      public void TestUserGroupServices_Move_Group_Alters_ChildCounts()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var grp = this.TContext.Groups.Where(g => g.Name == "Test Eastern").FirstOrDefault();

          var grpSave = grp.ChildrenCountDeep;

          var newParentGroup = this.TContext.Groups.Where(g => g.Name == "Test Central").FirstOrDefault();

          var parentSave = newParentGroup.ChildrenCountDeep;

          var result1 = service.GroupMove(sa, grp.GroupId, newParentGroup.GroupId);

          this.TContext.ObjectContext.Refresh(RefreshMode.StoreWins, newParentGroup);

          Assert.IsTrue(grpSave + parentSave + 1  == newParentGroup.ChildrenCountDeep);

      }

      [Test]
      public void TestUserGroupServices_Move_Users_Between_Groups_Alters_Member_Counts()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var rm2 = GetUserSessionModel("USRM2@Somewhere.com");

          var am4 = GetUserSessionModel("USAM4@Somewhere.com");

          var us4 = GetUserSessionModel("US4@Somewhere.com");

          var us5 = GetUserSessionModel("US5@Somewhere.com");

          // Get current users from am4
          var grp = this.TContext.Groups.Where(g => g.Name == "Test Dallas").FirstOrDefault();

          var savecount = grp.MemberCount;

          service.GroupUsersMove(sa, new long[] { us4.UserId, us5.UserId }, grp.GroupId);

          this.TContext.ObjectContext.Refresh(RefreshMode.StoreWins, grp);

          Assert.IsTrue(savecount == grp.MemberCount-2);
      }




      [Test]
      public void TestUserGroupServices_Create_Group()
      {
          var sa = GetUserSessionModel("User15@test.com");

          var toplevel = this.TContext.Groups.Where(g => g.Name == "Test Daikin").FirstOrDefault();
          var save1 = toplevel.ChildrenCountDeep;

          var before = this.TContext.Groups.Where(g => g.Name == "Test Dallas").FirstOrDefault();
          var save2 = before.ChildrenCount;

          var result = service.GroupCreate(sa, "Test Dallas DownTown", before.GroupId);

          var after = this.TContext.Groups.Where(g => g.Name == "Test Dallas").FirstOrDefault();

          var toplevelafter = this.TContext.Groups.Where(g => g.Name == "Test Daikin").FirstOrDefault();

          var newgrp = this.TContext.Groups.Where(g => g.Name == "Test Dallas DownTown").FirstOrDefault();

          Assert.IsTrue(after.ChildrenCount == save2 + 1);

          Assert.IsTrue(toplevelafter.ChildrenCountDeep == save1 + 1);

          Assert.IsTrue(newgrp.Path.IndexOf(after.GroupId.ToString() + "\\" + newgrp.GroupId.ToString()) >= 1);

      }


      [Test]
      public void TestUserGroupServices_Create_Group_When_User_Not_Owner()
      {
          var own = GetUserSessionModel("USAM3@Somewhere.com");

          var before = this.TContext.Groups.Where(g => g.Name == "Test Dallas").FirstOrDefault();

          var response = service.GroupCreate(own, "Test Dallas DownTown", before.GroupId);

          Assert.IsTrue(response.Messages.Items.Any(m => m.Text == ResourceModelUserGroups.UG007));

      }

      [Test]
      public void TestUserGroupServices_Create_Group_Fails_With_Invalid_Name()
      {
          var own = GetUserSessionModel("USSA0@Somewhere.com");

          var before = this.TContext.Groups.Where(g => g.Name == "Test Dallas").FirstOrDefault();

          var response = service.GroupCreate(own, "   ", before.GroupId);

          Assert.IsTrue(response.Messages.Items.Any(m => m.Text == ResourceModelUserGroups.UG001));
      }

      [Test]
      public void TestUserGroupServices_Delete_Group()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var toplevel = this.TContext.Groups.Where(g => g.Name == "Test Daikin").FirstOrDefault();


          var before = this.TContext.Groups.Where(g => g.Name == "Test Dallas").FirstOrDefault();

          var result = service.GroupCreate(sa, "Test Dallas DownTown", before.GroupId);

          var newgrp = this.TContext.Groups.Where(g => g.Name == "Test Dallas DownTown").FirstOrDefault();

          Assert.IsTrue(newgrp != null);

          result = service.GroupDelete(sa, newgrp.GroupId);

          newgrp = this.TContext.Groups.Where(g => g.Name == "Test Dallas DownTown").FirstOrDefault();

          var toplevelafter = this.TContext.Groups.Where(g => g.Name == "Test Daikin").FirstOrDefault();

          var after = this.TContext.Groups.Where(g => g.Name == "Test Dallas").FirstOrDefault();

          // make nothing has changed
          Assert.IsTrue(toplevelafter.ChildrenCountDeep  == toplevel.ChildrenCountDeep);

          Assert.IsTrue(before.ChildrenCount == after.ChildrenCount);

          Assert.IsTrue(newgrp == null);

      }

      [Test]
      public void TestUserGroupServices_Delete_Group_Fails_When_User_Not_Owner()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var before = this.TContext.Groups.Where(g => g.Name == "Test Dallas").FirstOrDefault();

          var result = service.GroupCreate(sa, "Test Dallas DownTown", before.GroupId);

          var newgrp = this.TContext.Groups.Where(g => g.Name == "Test Dallas DownTown").FirstOrDefault();

          var own = GetUserSessionModel("USAM3@Somewhere.com");

          var response = service.GroupDelete(own, newgrp.GroupId);

          Assert.IsTrue(response.Messages.Items.Any(m => m.Text == ResourceModelUserGroups.UG005));
      }

      [Test]
      public void TestUserGroupServices_Delete_Group_Only_When_No_Users_And_No_Children()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var group = this.TContext.Groups.Where(g => g.Name == "Test Eastern").FirstOrDefault();

          var response = service.GroupDelete(sa, group.GroupId);

          Assert.IsTrue(response.Messages.Items.Any(m => m.Text == ResourceModelUserGroups.UG010));

          group.MemberCount = 0;

          this.service.Response.Messages.Clear();

          response = service.GroupDelete(sa, group.GroupId);

          Assert.IsTrue(response.Messages.Items.Any(m => m.Text == ResourceModelUserGroups.UG011));


      }

      [Test]
      public void TODO_TestUserGroupServices_User_Switch_OnAndOff_GroupOwnership()
      {
          Assert.Fail();
      }


   }
}