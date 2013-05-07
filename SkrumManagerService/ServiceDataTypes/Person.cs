using System.Runtime.Serialization;

namespace ServiceDataTypes
{
    [DataContract]
    public class Person
    {
        private bool? admin;
        private string email;
        private string jobDescription;
        private string name;
        private string password;
        private int? personID;
        private string photoURL;
        private System.Collections.Generic.List<Role> roles;
        private System.Collections.Generic.List<Task> tasks;
        private System.Collections.Generic.List<Task> ownedTasks;

        /// <summary>
        /// Default constructor of the Person class.
        /// </summary>
        public Person()
        {
        }

        /// <summary>
        /// Represents if this person is admin.
        /// </summary>
        [DataMember]
        public bool? Admin
        {
            get { return this.admin; }
            set { this.admin = value; }
        }

        /// <summary>
        /// The email of this person.
        /// </summary>
        [DataMember]
        public string Email
        {
            get { return this.email; }
            set { this.email = value; }
        }

        /// <summary>
        /// The job description of this person. Might be null.
        /// </summary>
        [DataMember]
        public string JobDescription
        {
            get { return this.jobDescription; }
            set { this.jobDescription = value; }
        }

        /// <summary>
        /// The name of this person.
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// Represents this person's password.
        /// </summary>
        [DataMember]
        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        /// <summary>
        /// The database ID of this person.
        /// </summary>
        [DataMember]
        public int? PersonID
        {
            get { return this.personID; }
            set { this.personID = value; }
        }

        /// <summary>
        /// The URL for this person's photo. Might be null.
        /// </summary>
        [DataMember]
        public string PhotoURL
        {
            get { return this.photoURL; }
            set { this.photoURL = value; }
        }

        /// <summary>
        /// List of roles associated with this person.
        /// </summary>
        [DataMember]
        public System.Collections.Generic.List<Role> Roles
        {
            get { return this.roles; }
            set { this.roles = value; }
        }

        /// <summary>
        /// List of tasks associated with this person.
        /// </summary>
        [DataMember]
        public System.Collections.Generic.List<Task> Tasks
        {
            get { return this.tasks; }
            set { this.tasks = value; }
        }

        /// <summary>
        /// List of tasks owned by this person.
        /// </summary>
        [DataMember]
        public System.Collections.Generic.List<Task> OwnedTasks
        {
            get { return this.ownedTasks; }
            set { this.ownedTasks = value; }
        }
    }
}