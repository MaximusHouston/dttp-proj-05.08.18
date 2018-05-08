
using System.Collections.Generic;


namespace DPO.Common
{
    public class WidgetSetting
    {
        public string SettingName { get; set; }

        public string SettingLabel { get; set; }

        public string SelectedOption { get; set; }

        public List<KeyValuePair<string, string>> Options { get; set;}
    }
}
