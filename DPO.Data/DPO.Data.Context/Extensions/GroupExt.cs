using DPO.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Data
{
    public partial class Group
    {
        public string GetPath
        {
            get
            {
                if (this.ParentGroup == null && this.ParentGroupId.HasValue)
                {
                    throw new Exception(Resources.DataMessages.DM010);
                }
                if (this.ParentGroup != null)
                {
                    return this.ParentGroup.GetPath + "\\" + this.GroupId.ToString();
                }
                return "\\" + this.GroupId.ToString();
            }
        }

        // Import helpers
        public Group ParentGroup;
        
        public List<Group> ChildGroups = new List<Group>();

        public int CalcChildrenDeepCount(int level)
        {
            if (level > 50)
            {
                throw new Exception("Too many group levels");
            }

            if (this.ChildGroups.Count == 0)
            {
                return 1;
            }
            else
            {
                level++;
                return this.ChildGroups.Sum(g => g.CalcChildrenDeepCount(level)) + 1;
            }
            
        }

        public int RelativePath(List<Group> parents)
        {
            // Search all parent to find this groups parent then work out level
            foreach (var parent in parents)
            {
                var fromGroupId = parent.GroupId.ToString();

                int pos = this.Path.IndexOf(fromGroupId, 0);

                if (pos != -1)
                {
                    return this.Path.Substring(pos + fromGroupId.Length).Split('\\').Length - 1;
                }
            }
            return 0;


            
        }
    }
}
