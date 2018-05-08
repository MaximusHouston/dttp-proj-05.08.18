 
using System.Collections.Generic;

namespace DPO.Common
{
    public class DecisionTreeDependancyMap
    {
        public static List<DecisionTreeCellMap> map()
        {
            return new List<DecisionTreeCellMap> 
            {
                new DecisionTreeCellMap { Index = 1, Name = "A1"},
                new DecisionTreeCellMap { Index = 2, Name = "B1"},
                new DecisionTreeCellMap { Index = 4, Name = "A2"},
                new DecisionTreeCellMap { Index = 5, Name = "B2"}
            };
        }
    }

    public class DecisionTreeCellMap
    {
        public int Index { get; set; }
        public string Name { get; set; }
    }
}
