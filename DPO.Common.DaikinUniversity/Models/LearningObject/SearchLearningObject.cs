namespace DPO.Common.DaikinUniversity
{
    public class SearchLearningObject
    {
        /// <summary>
        /// The user ID.  Returns only items available to this user ID.
        /// </summary>
        public string ActorID { get; set; }

        /// <summary>
        /// xml or json
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Cornerstone generated learning object ID.  This is required.
        /// </summary>
        public string ObjectID { get; set; }
    }
}