 
namespace DPO.Common
{
    public class UserGroupItemModel
    {
        public UserGroupItemModel()
        {

        }

        public long GroupId { get; set; }
        public string GroupName { get; set; }
        public int Level { get; set; }
        public int ChildCountDeep { get; set; }
        public int ViewableChildCount { get; set; }
        public int MemberCount { get; set; }   
    }
}
