using System.Runtime.Serialization;

namespace ServiceDataTypes
{
    /// <summary>
    /// Represents a person's role in a given project.
    /// </summary>
    [DataContract]
    public class Role
    {
        private double? assignedTime;
        private string password;
        private int? personID;
        private int? projectID;
        private ServiceDataTypes.RoleDescription roleDescription;
        private int? roleID;

        /// <summary>
        /// Instantiates a new role object.
        /// </summary>
        public Role()
        {
        }

        /// <summary>
        /// Represents the percentage of the person's time that
        /// is spent in this role.
        /// </summary>
        [DataMember]
        public double? AssignedTime
        {
            get { return this.assignedTime; }
            set { this.assignedTime = value; }
        }

        /// <summary>
        /// Project admin's password.
        /// </summary>
        [DataMember]
        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        /// <summary>
        /// ID of the persons that has this role.
        /// </summary>
        [DataMember]
        public int? PersonID
        {
            get { return this.personID; }
            set { this.personID = value; }
        }

        /// <summary>
        /// ID of the project that has this role.
        /// </summary>
        [DataMember]
        public int? ProjectID
        {
            get { return this.projectID; }
            set { this.projectID = value; }
        }

        /// <summary>
        /// Role's description.
        /// </summary>
        [DataMember]
        public ServiceDataTypes.RoleDescription RoleDescription
        {
            get { return this.roleDescription; }
            set { this.roleDescription = value; }
        }

        /// <summary>
        /// Role's database ID.
        /// </summary>
        [DataMember]
        public int? RoleID
        {
            get { return this.roleID; }
            set { this.roleID = value; }
        }
    }
}