using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Common;
using DPO.Domain;
using DPO.Common;

namespace DaikinProjectOffice.Tests
{
    [TestFixture]
    public class UserServiceTests: TestAdmin
    {
      UserServices service;

      public UserServiceTests()
      {
         service = new UserServices(this.TContext);
      }
        
      [Test]
      public void TestUserServices_Super_Admin_Can_See_All_Requiring_Approval()
      {
            var sa = GetUserSessionModel("daikincity@daikincomfort.com");
            
            var search = new SearchUser
            {
                Approved = false,
                PageSize = 0
            };
          
            var response = service.GetUserListModel(null, search);
            var result = response.Model as List<UserListModel>;
            var count = this.TContext.Users.Where(u => u.Approved == false).Count();

            Assert.That(result.Count(), Is.EqualTo(count));
        }

    }
}
