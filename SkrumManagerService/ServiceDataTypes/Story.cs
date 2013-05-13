using System.Runtime.Serialization;

namespace ServiceDataTypes
{
    /// <summary>
    /// Represents a user story.
    /// </summary>
    [DataContract]
    public class Story
    {
        private System.DateTime creationDate;
        private string description;
        private int? previousStory;
        private int projectID;
        private ServiceDataTypes.StoryState state;
        private int storyID;
        private System.Collections.Generic.List<StorySprint> storySprints;
        private System.Collections.Generic.List<Task> tasks;
        private int number;

        /// <summary>
        /// Instantiates a new Story object.
        /// </summary>
        public Story()
        {
        }

        /// <summary>
        /// Story's creation date.
        /// </summary>
        [DataMember]
        public System.DateTime CreationDate
        {
            get { return this.creationDate; }
            set { this.creationDate = value; }
        }

        /// <summary>
        /// Story's description.
        /// </summary>
        [DataMember]
        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        /// <summary>
        /// ID of the next story in order.
        /// </summary>
        [DataMember]
        public int? PreviousStory
        {
            get { return this.previousStory; }
            set { this.previousStory = value; }
        }

        /// <summary>
        /// ID of the project that contains this user story.
        /// </summary>
        [DataMember]
        public int ProjectID
        {
            get { return this.projectID; }
            set { this.projectID = value; }
        }

        /// <summary>
        /// Story's state.
        /// </summary>
        [DataMember]
        public ServiceDataTypes.StoryState State
        {
            get { return this.state; }
            set { this.state = value; }
        }

        /// <summary>
        /// Story's database ID.
        /// </summary>
        [DataMember]
        public int StoryID
        {
            get { return this.storyID; }
            set { this.storyID = value; }
        }

        /// <summary>
        /// Ordinal number of the story in the project, used
        /// to create story identifiers.
        /// </summary>
        [DataMember]
        public int Number
        {
            get { return this.number; }
            set { this.number = value; }
        }

        /// <summary>
        /// Represents the priority and points given to the story
        /// in a specific sprint.
        /// </summary>
        [DataMember]
        public System.Collections.Generic.List<StorySprint> StorySprints
        {
            get { return this.storySprints; }
            set { this.storySprints = value; }
        }

        /// <summary>
        /// Story's list of associated tasks.
        /// </summary>
        [DataMember]
        public System.Collections.Generic.List<Task> Tasks
        {
            get { return this.tasks; }
            set { this.tasks = value; }
        }
    }
}