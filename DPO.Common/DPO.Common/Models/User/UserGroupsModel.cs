 
using System.Collections.Generic;
 
namespace DPO.Common
{
    public class UserGroupsModel: Search
    {
        public UserGroupsModel(){
           
        }
        public long? UserGroupId { get; set; }
        public List<UserGroupItemModel> UserGroups { get; set; }
        public UserGroupItemModel UnAllocatedGroup { get; set; }
    }
}
