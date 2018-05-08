 
using System.Collections.Generic;
 

namespace DPO.Common
{
    public class MockData
    {
        /*
        public static List<FloorConfigurationLayoutNodeModel> nodes = new List<FloorConfigurationLayoutNodeModel>
        {
            //hotel
            //top nodes
            new FloorConfigurationLayoutNodeModel{
                nodeId = 1,
                layoutId = 1,
                id = 1
            },

            new FloorConfigurationLayoutNodeModel{
                nodeId = 888,
                parentNodeId = 1,
                id = 12
            },

            new FloorConfigurationLayoutNodeModel{
                nodeId = 889,
                parentNodeId = 1,
                id = 5
            },

            new FloorConfigurationLayoutNodeModel{
                nodeId = 887,
                parentNodeId = 889,
                id = 8
            },



            new FloorConfigurationLayoutNodeModel{
                nodeId = 2,
                layoutId = 2,
                id = 4
            },
            new FloorConfigurationLayoutNodeModel{
                nodeId = 3,
                layoutId = 4,
                id = 1
            },
            new FloorConfigurationLayoutNodeModel{
                nodeId = 4,
                layoutId = 5,
                id = 3,
            },

            //sub-node
            new FloorConfigurationLayoutNodeModel{
                nodeId = 5,
                parentNodeId = 4,
                id = 5
            },

            //top nodes
            new FloorConfigurationLayoutNodeModel{
                nodeId = 6,
                layoutId = 6,
                id = 4
            },

            new FloorConfigurationLayoutNodeModel{
                nodeId = 7,
                layoutId = 7,
                id = 3,
            },

            //sub-node
            new FloorConfigurationLayoutNodeModel{
                nodeId = 8,
                parentNodeId = 7,
                id = 5
            },

            //conv store
            // 8 is baseline
            //top nodes
            new FloorConfigurationLayoutNodeModel{
                nodeId = 9,
                layoutId = 9,
                id = 12,
            },

            new FloorConfigurationLayoutNodeModel{
                nodeId = 10,
                layoutId = 10,
                id = 12,
            },

            //sub nodes
            new FloorConfigurationLayoutNodeModel{
                nodeId = 11,
                parentNodeId = 9,
                id = 12
            },
        };
         * */
        public static FloorConfigurationLayoutsModel layouts = new FloorConfigurationLayoutsModel
        {
            layout = new List<FloorConfigurationLayoutModel>
            {
                //hotel
                    //public spaces
                    new FloorConfigurationLayoutModel{
                        id = 1,
                        configId = 2,
                        name = "Zone Level"  
                    },
                    new FloorConfigurationLayoutModel{
                        id = 2,
                        configId = 2,
                        name = "System Level"
                    },
                    new FloorConfigurationLayoutModel{
                        id = 4,
                        configId = 3,
                        name = "Zone Level"
                    }
                    
            }
        };
    }
}
