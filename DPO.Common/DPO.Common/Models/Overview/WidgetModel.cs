using System.Collections.Generic;

namespace DPO.Common
{
    public class WidgetModel
    {
        public WidgetModel()
        {
            Data = new List<WidgetData>();
        }

        public Dictionary<string, WidgetSetting> AdditionalSettings { get; set; }

        public object Data { get; set; }
        public string TemplateId { get; set; }
        public string Title { get; set; }
    }
}