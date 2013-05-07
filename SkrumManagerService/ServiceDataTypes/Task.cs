using System.Runtime.Serialization;

namespace ServiceDataTypes
{
    /// <summary>
    /// Represents a task of a certain story.
    /// </summary>
    [DataContract]
    public class Task
    {
        private System.DateTime creationDate;
        private string description;
        private int? estimation;
        private int? personID;
        private int? storyID;
        private int? taskID;

        /// <summary>
        /// Instantiates a task object.
        /// </summary>
        public Task()
        {
        }

        /// <summary>
        /// Task's creation date.
        /// </summary>
        [DataMember]
        public System.DateTime CreationDate
        {
            get { return this.creationDate; }
            set { this.creationDate = value; }
        }

        /// <summary>
        /// Task's description.
        /// </summary>
        [DataMember]
        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        /// <summary>
        /// Task's estimation points.
        /// </summary>
        [DataMember]
        public int? Estimation
        {
            get { return this.estimation; }
            set { this.estimation = value; }
        }

        /// <summary>
        /// ID of the person that owns this task.
        /// </summary>
        [DataMember]
        public int? PersonID
        {
            get { return this.personID; }
            set { this.personID = value; }
        }

        /// <summary>
        /// ID of the story that contains this task.
        /// </summary>
        [DataMember]
        public int? StoryID
        {
            get { return this.storyID; }
            set { this.storyID = value; }
        }

        /// <summary>
        /// Task's database ID.
        /// </summary>
        [DataMember]
        public int? TaskID
        {
            get { return this.taskID; }
            set { this.taskID = value; }
        }
    }
}