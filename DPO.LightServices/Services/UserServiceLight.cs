using DPO.Domain;
using DPO.Common;
using DPO.Model.Light;
using System.Linq;


namespace DPO.Services.Light
{
    public class UserServiceLight : BaseServices
    {
        //public UserServiceLight() : base() { }

        public ServiceResponse GetUsers(UserSessionModel user) {
            var query = from entity in this.Context.Users
                        select new UserAutoComplete { 
                            userFullName = entity.FirstName + " " + entity.MiddleName + " " + entity.LastName
                        };
            this.Response.Model = query.ToList();
            return this.Response;
        }

        public ServiceResponse GetCurrentUser(UserSessionModel user)
        {
            this.Response.Model = user;
            return this.Response;
        }

        //public ServiceResponse GetProjectOwners(UserSessionModel user)
        //{
        //    var query = from _user in this.Context.Users
        //                where (from project in this.Context.Projects
        //                       select project.OwnerId).Contains(_user.UserId)
        //                select new UserAutoComplete
        //                {
        //                    userFullName = _user.FirstName + (_user.MiddleName!=null ? " " + _user.MiddleName:"") + " " + _user.LastName
        //                };
        //    this.Response.Model = query.ToList();
        //    return this.Response;
        //}
    }
}
