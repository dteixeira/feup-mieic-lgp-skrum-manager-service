using System.Runtime.Serialization;

namespace ServiceDataTypes
{
    [DataContract]
    public class Project
    {
        private int? projectID;
        private string password;
        private int? sprintDuration;
        private int? alertLimit;
        private int? speed;

        /// <summary>
        /// Default constructor of the Project class.
        /// </summary>
        public Project()
        {
        }

        /// <summary>
        /// The Database ID of this project.
        /// </summary>
        [DataMember]
        public int? ProjectID
        {
            get { return this.projectID; }
            set { this.projectID = value; }
        }

        /// <summary>
        /// Represents this project's password.
        /// </summary>
        [DataMember]
        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }
        
        /// <summary>
        /// The sprint duration parameter for this project
        /// </summary>
        [DataMember]
        public int? SprintDuration
        {
            get { return this.sprintDuration; }
            set { this.sprintDuration = value; }
        }

        /// <summary>
        /// The alert limit parameter for this project
        /// </summary>
        [DataMember]
        public int? AlertLimit
        {
            get { return this.alertLimit; }
            set { this.alertLimit = value; }
        }

        /// <summary>
        /// The speed paramenter for this project.
        /// </summary>
        [DataMember]
        public int? Speed
        {
            get { return this.speed; }
            set { this.speed = value; }
        }
                
        
    }
}
