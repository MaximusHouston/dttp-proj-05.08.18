using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DPO.Common.DaikinUniversity
{
    public class GlobalSearchTrainingItem
    {
        public List<AvailabilityItem> Availabilities { get; set; }

        public List<AvailableLanguageItem> AvailableLanguages { get; set; }

        public List<string> Competencies { get; set; }

        /// <summary>
        /// UTC Create Date
        /// </summary>
        public DateTime? CreateDate { get; set; }

        [JsonProperty("Customfields")]
        public List<CustomFieldItem> CustomFields { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Duration in minutes
        /// </summary>
        public int? Duration { get; set; }

        public List<InstructorItem> Instructors { get; set; }

        public string Location { get; set; }

        /// <summary>
        /// UTC Modify Date
        /// </summary>
        public DateTime? ModifyDate { get; set; }

        public string ObjectId { get; set; }

        public decimal? Price { get; set; }

        public string Provider { get; set; }

        public List<SkillItem> Skills { get; set; }

        public List<SubjectItem> Subjects { get; set; }

        public string Title { get; set; }

        public List<TrainingPurposeItem> TrainingPurposes { get; set; }

        public string TrainingSubType { get; set; }

        public string TrainingType { get; set; }

        public string Version { get; set; }
    }
}