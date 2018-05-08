using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Common.DaikinUniversity
{
    public enum TrainingType
    {
        All,
        [Description("Online Class")]
        Course,
        [Description("Quick Course")]
        SCO,
        Curriculum,
        Library,
        Material,
        Test,
        Closed,
        Event,
        Video,
        [Description("Program")]
        SocialLearningProgram,

    }
}
