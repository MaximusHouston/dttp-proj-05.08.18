
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using AutoMapper;

namespace DPO.Common
{
    [Serializable]
    public class WidgetContainerModel : SearchWidgetContainer
    {
        public WidgetContainerModel()
            : base()
        {
            Widget = new WidgetModel();
            AvailableWidgetTypes = new List<WidgetModel>();
        }

        [JsonIgnore]
        public List<WidgetModel> AvailableWidgetTypes { get; set; }

        [JsonIgnore]
        public DropDownModel BusinessesInGroup { get; set; }

        [JsonIgnore]
        public DropDownModel FinancialYears { get; set; }

        [JsonIgnore]
        public DropDownModel ProjectExportTypes { get; set; }

        [JsonIgnore]
        public DropDownModel ProjectOpenStatusTypes { get; set; }

        [JsonIgnore]
        public DropDownModel ProjectStatusTypes { get; set; }

        [JsonIgnore]
        public DropDownModel ProjectTypes { get; set; }
        
        [JsonIgnore]
        public DropDownModel ProjectDateTypes { get; set; }

        [JsonIgnore]
        public DropDownModel UsersInGroup { get; set; }

        public WidgetModel Widget { get; set; }

        public string ToJsonString(bool includeDropDownModels)
        {
            return JsonConvert.SerializeObject(
                this,
                Newtonsoft.Json.Formatting.Indented,
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                });
        }

        public ProjectsModel ToSearchProjectModel()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<WidgetContainerModel, ProjectsModel>());
            var mapper = config.CreateMapper();

            return mapper.Map<ProjectsModel>(this);
        }
    }
}