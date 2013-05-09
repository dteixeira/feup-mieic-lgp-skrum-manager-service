using System.Runtime.Serialization;

namespace ServiceDataTypes
{
    /// <summary>
    /// Represents a scrum project.
    /// </summary>
    [DataContract]
    public class Project
    {
        private int alertLimit;
        private System.Collections.Generic.List<Meeting> meetings;
        private string name;
        private string password;
        private int projectID;
        private int speed;
        private int sprintDuration;
        private System.Collections.Generic.List<Sprint> sprints;

        /// <summary>
        /// Default constructor of the Project class.
        /// </summary>
        public Project()
        {
        }

        /// <summary>
        /// The alert limit parameter for this project.
        /// </summary>
        [DataMember]
        public int AlertLimit
        {
            get { return this.alertLimit; }
            set { this.alertLimit = value; }
        }

        /// <summary>
        /// List of meetings in virtue of this project.
        /// </summary>
        [DataMember]
        public System.Collections.Generic.List<Meeting> Meetings
        {
            get { return this.meetings; }
            set { this.meetings = value; }
        }

        /// <summary>
        /// The project's unique name (used as an identifier).
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
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
        /// The Database ID of this project.
        /// </summary>
        [DataMember]
        public int ProjectID
        {
            get { return this.projectID; }
            set { this.projectID = value; }
        }

        /// <summary>
        /// The speed paramenter for this project.
        /// </summary>
        [DataMember]
        public int Speed
        {
            get { return this.speed; }
            set { this.speed = value; }
        }

        /// <summary>
        /// The sprint duration parameter for this project.
        /// </summary>
        [DataMember]
        public int SprintDuration
        {
            get { return this.sprintDuration; }
            set { this.sprintDuration = value; }
        }

        /// <summary>
        /// List of the project's sprints.
        /// </summary>
        [DataMember]
        public System.Collections.Generic.List<Sprint> Sprints
        {
            get { return this.sprints; }
            set { this.sprints = value; }
        }
    }
}