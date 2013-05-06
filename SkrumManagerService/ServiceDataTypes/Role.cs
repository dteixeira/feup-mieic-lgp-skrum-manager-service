using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ServiceDataTypes
{
    [DataContract]
    public class Role
    {
        private ServiceDataTypes.RoleDescription roleDescription;
        private double? assignedTime;
        private int? personID;
        private int? projectID;

        [DataMember]
        public ServiceDataTypes.RoleDescription RoleDescription 
        {
            get { return this.roleDescription; }
            set { this.roleDescription = value; }
        }

        [DataMember]
        public double? AssignedTime
        {
            get { return this.assignedTime; }
            set { this.assignedTime = value; }
        }

        [DataMember]
        public int? PersonID
        {
            get { return this.personID; }
            set { this.personID = value; }
        }

        [DataMember]
        public int? ProjectID
        {
            get { return this.projectID; }
            set { this.projectID = value; }
        }
    }
}
