using System.Runtime.Serialization;

namespace ServiceDataTypes
{
    /// <summary>
    /// Represents a task of a certain story.
    /// </summary>
    [DataContract]
    public class StorySprint
    {
        private int points;
        private int sprintID;
        private int storyID;

        /// <summary>
        /// Instantiates a task object.
        /// </summary>
        public StorySprint()
        {
        }

        /// <summary>
        /// Points given to the story in this sprint.
        /// </summary>
        [DataMember]
        public int Points
        {
            get { return this.points; }
            set { this.points = value; }
        }

        /// <summary>
        /// Sprint's database ID.
        /// </summary>
        [DataMember]
        public int SprintID
        {
            get { return this.sprintID; }
            set { this.sprintID = value; }
        }

        /// <summary>
        /// ID of the story that contains this task.
        /// </summary>
        [DataMember]
        public int StoryID
        {
            get { return this.storyID; }
            set { this.storyID = value; }
        }
    }
}