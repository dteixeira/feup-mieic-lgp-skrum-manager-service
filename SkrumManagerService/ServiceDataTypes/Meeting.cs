using System.Runtime.Serialization;

namespace ServiceDataTypes
{
    [DataContract]
    public class Meeting
    {
        private System.DateTime date;
        private int? meetingID;
        private string notes;
        private int? number;
        private int? projectID;

        /// <summary>
        /// Default constructor of the Meeting class.
        /// </summary>
        public Meeting()
        {
        }

        /// <summary>
        /// Represents this Meetings date.
        /// </summary>
        [DataMember]
        public System.DateTime Date
        {
            get { return this.date; }
            set { this.date = value; }
        }

        /// <summary>
        /// The database ID of this Meeting.
        /// </summary>
        [DataMember]
        public int? MeetingID
        {
            get { return this.meetingID; }
            set { this.meetingID = value; }
        }

        /// <summary>
        /// The notes registered in this Meeting
        /// </summary>
        [DataMember]
        public string Notes
        {
            get { return this.notes; }
            set { this.notes = value; }
        }

        /// <summary>
        /// The sprint duration parameter for this project
        /// </summary>
        [DataMember]
        public int? Number
        {
            get { return this.number; }
            set { this.number = value; }
        }

        /// <summary>
        /// The Database ID of the project of this Meeting.
        /// </summary>
        [DataMember]
        public int? ProjectID
        {
            get { return this.projectID; }
            set { this.projectID = value; }
        }
    }
}