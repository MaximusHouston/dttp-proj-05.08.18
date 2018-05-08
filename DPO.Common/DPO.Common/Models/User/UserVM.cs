 

namespace DPO.Common.Models.User
{
    public class UserVM
    {
        public UserVM()
        {
            UserModel = new UserModel();
            UserGroupsModel = new UserGroupsModel();
         
        }
        
        public UserModel UserModel { get; set; }
        public UserGroupsModel UserGroupsModel { get; set;}
        public bool ShowGroup { get; set;}
        public long? BusinessId { get; set; }
        public long? UserId { get; set; }
    }
}
