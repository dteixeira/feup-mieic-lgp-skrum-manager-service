using System.Runtime.Serialization;

namespace ServiceDataTypes
{
    /// <summary>
    /// Represents the amount of work someone put in a task.
    /// </summary>
    [DataContract]
    public class PersonTask
    {
        private System.DateTime? creationDate;
        private int? personID;
        private int? personTaskID;
        private double? spentTime;
        private int? taskID;

        /// <summary>
        /// Instantiates a task object.
        /// </summary>
        public PersonTask()
        {
        }

        /// <summary>
        /// Task's creation date.
        /// </summary>
        [DataMember]
        public System.DateTime? CreationDate
        {
            get { return this.creationDate; }
            set { this.creationDate = value; }
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
        /// PersonTask's database ID.
        /// </summary>
        [DataMember]
        public int? PersonTaskID
        {
            get { return this.personTaskID; }
            set { this.personTaskID = value; }
        }

        /// <summary>
        /// Time that person spent on the task.
        /// </summary>
        [DataMember]
        public double? SpentTime
        {
            get { return this.spentTime; }
            set { this.spentTime = value; }
        }

        /// <summary>
        /// ID of the task.
        /// </summary>
        [DataMember]
        public int? TaskID
        {
            get { return this.taskID; }
            set { this.taskID = value; }
        }
    }
}