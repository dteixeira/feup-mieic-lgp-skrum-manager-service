using System.Runtime.Serialization;

namespace ServiceDataTypes
{
    [DataContract]
    public enum RoleDescription
    {
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
        InProgress,

        [EnumMember]
        Completed,

        [EnumMember]
        Abandoned
    }
}