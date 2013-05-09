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
        private int estimation;
        private System.Collections.Generic.List<PersonTask> personTasks;
        private TaskState state;
        private int storyID;
        private int taskID;

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
        public int Estimation
        {
            get { return this.estimation; }
            set { this.estimation = value; }
        }

        /// <summary>
        /// Represents who worked in this task and how much they worked.
        /// </summary>
        [DataMember]
        public System.Collections.Generic.List<PersonTask> PersonTasks
        {
            get { return this.personTasks; }
            set { this.personTasks = value; }
        }

        /// <summary>
        /// Represents the task's state.
        /// </summary>
        [DataMember]
        public TaskState State
        {
            get { return this.state; }
            set { this.state = value; }
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

        /// <summary>
        /// Task's database ID.
        /// </summary>
        [DataMember]
        public int TaskID
        {
            get { return this.taskID; }
            set { this.taskID = value; }
        }
    }
}