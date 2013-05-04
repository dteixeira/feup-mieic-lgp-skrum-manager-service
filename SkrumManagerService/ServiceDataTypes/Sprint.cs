using System.Runtime.Serialization;

namespace ServiceDataTypes
{
    class Sprint
    {
        private int sprintID;
        private int number;
        private string beginDate;
        private string endDate;
        private bool closed;
        private int projectID;

        /// <summary>
        /// Default constructor of the Sprint class.
        /// </summary>
        public Sprint()
        {
        }

        /// <summary>
        /// The Database ID of this sprint.
        /// </summary>
        [DataMember]
        public int SprintID
        {
            get { return this.sprintID; }
            set { this.sprintID = value; }
        }

        /// <summary>
        /// The number of a Sprint in a Project.
        /// </summary>
        [DataMember]
        public int Number
        {
            get { return this.number; }
            set { this.number = value; }
        }

        /// <summary>
        /// The begining date for the project
        /// </summary>
        [DataMember]
        public string BeginDate
        {
            get { return this.beginDate; }
            set { this.beginDate = value; }
        }

        /// <summary>
        /// The end date for the sprint
        /// </summary>
        [DataMember]
        public string EndDate
        {
            get { return this.endDate; }
            set { this.endDate = value; }
        }

        /// <summary>
        /// The speed paramenter for this project.
        /// </summary>
        [DataMember]
        public bool Closed
        {
            get { return this.closed; }
            set { this.closed = value; }
        }

        /// <summary>
        /// The Database ID of the project this sprint is inserted on.
        /// </summary>
        [DataMember]
        public int ProjectID
        {
            get { return this.projectID; }
            set { this.projectID = value; }
        }
        

    }
}
