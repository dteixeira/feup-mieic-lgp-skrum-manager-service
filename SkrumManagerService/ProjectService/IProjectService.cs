using System.Collections.Generic;
using ServiceDataTypes;
using Users;
using System.ServiceModel;

namespace Projects
{
    [ServiceContract]
    public interface IProjectService
    {
        [OperationContract]
        Project CreateProject(Project project);

        [OperationContract]
        bool DeleteProject(int projectID);

        [OperationContract]
        Project GetProjectByID(int projectID);

        [OperationContract]
        Project UpdateProject(Project project);

        //--------------------//

        [OperationContract]
        Sprint CreateSprint(Sprint sprint);

        [OperationContract]
        bool DeleteSprint(int sprintID);

        [OperationContract]
        Sprint GetSprintByID(int sprintID);

        [OperationContract]
        Sprint UpdateSprint(Sprint sprint);

        //--------------------//
        
        [OperationContract]
        List<Sprint> GetSprintsInProject(int projectID);

        [OperationContract]
        Role GiveRole(Role role);

        [OperationContract]
        List<Sprint> GetClosedSprints(int projectID);

        //--------------------//

        [OperationContract]
        Meeting CreateMeeting(Meeting meeting);

        [OperationContract]
        bool DeleteMeeting(int meetingID);

        [OperationContract]
        Meeting GetMeetingByID(int meetingID);

        [OperationContract]
        Meeting UpdateMeeting(Meeting meeting);

        [OperationContract]
        List<Meeting> GetMeetingsInProject(int projectID);

        [OperationContract]
        List<Meeting> GetMeetingsOnDate(System.DateTime date, int projectID);


    }
}