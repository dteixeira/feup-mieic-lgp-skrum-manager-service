using System.Runtime.Serialization;

namespace ServiceDataTypes
{
    [DataContract]
    public class Sprint
    {
        private System.DateTime beginDate;
        private bool? closed;
        private System.DateTime endDate;
        private int? number;
        private int? projectID;
        private int? sprintID;
        private System.Collections.Generic.List<Story> stories;

        /// <summary>
        /// Default constructor of the Sprint class.
        /// </summary>
        public Sprint()
        {
        }

        /// <summary>
        /// The begining date for the project
        /// </summary>
        [DataMember]
        public System.DateTime BeginDate
        {
            get { return this.beginDate; }
            set { this.beginDate = value; }
        }

        /// <summary>
        /// The speed paramenter for this project.
        /// </summary>
        [DataMember]
        public bool? Closed
        {
            get { return this.closed; }
            set { this.closed = value; }
        }

        /// <summary>
        /// The end date for the sprint
        /// </summary>
        [DataMember]
        public System.DateTime EndDate
        {
            get { return this.endDate; }
            set { this.endDate = value; }
        }

        /// <summary>
        /// The number of a Sprint in a Project.
        /// </summary>
        [DataMember]
        public int? Number
        {
            get { return this.number; }
            set { this.number = value; }
        }

        /// <summary>
        /// The Database ID of the project this sprint is inserted on.
        /// </summary>
        [DataMember]
        public int? ProjectID
        {
            get { return this.projectID; }
            set { this.projectID = value; }
        }

        /// <summary>
        /// The Database ID of this sprint.
        /// </summary>
        [DataMember]
        public int? SprintID
        {
            get { return this.sprintID; }
            set { this.sprintID = value; }
        }

        /// <summary>
        /// List of stories associated with this sprint.
        /// </summary>
        [DataMember]
        public System.Collections.Generic.List<Story> Stories
        {
            get { return this.stories; }
            set { this.stories = value; }
        }
    }
}