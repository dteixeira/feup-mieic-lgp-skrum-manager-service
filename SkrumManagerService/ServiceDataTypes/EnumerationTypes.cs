using System.Runtime.Serialization;

namespace ServiceDataTypes
{
    [DataContract]
    public enum RoleDescription
    {
        [EnumMember]
        Null,

        [EnumMember]
        ProjectManager,

        [EnumMember]
        ScrumMaster,

        [EnumMember]
        ProductOwner,

        [EnumMember]
        TeamMember
    }

    [DataContract]
    public enum StoryPriority
    {
        [EnumMember]
        Null,

        [EnumMember]
        Must,

        [EnumMember]
        Should,

        [EnumMember]
        Could,

        [EnumMember]
        Wont
    }

    [DataContract]
    public enum StoryState
    {
        [EnumMember]
        Null,

        [EnumMember]
        InProgress,

        [EnumMember]
        Completed,

        [EnumMember]
        Abandoned
    }
}