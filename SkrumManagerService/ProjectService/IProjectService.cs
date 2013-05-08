using System.Collections.Generic;
using ServiceDataTypes;
using System.ServiceModel;

namespace Projects
{
    [ServiceContract]
    public interface IProjectService
    {
        /**
         * Project CRUD
         */

        [OperationContract]
        Project CreateProject(Project project);

        [OperationContract]
        bool DeleteProject(int projectID);

        [OperationContract]
        Project GetProjectByID(int projectID);

        [OperationContract]
        Project UpdateProject(Project project);

        [OperationContract]
        Project GetProjectByName(string name);

        /**
         * Sprint CRUD
         */

        [OperationContract]
        Project CreateSprint(Sprint sprint);

        [OperationContract]
        bool DeleteSprint(int sprintID);

        [OperationContract]
        Project UpdateSprint(Sprint sprint);

        /**
         * Meeting CRUD
         */

        [OperationContract]
        Project CreateMeeting(Meeting meeting);

        [OperationContract]
        bool DeleteMeeting(int meetingID);

        [OperationContract]
        Project UpdateMeeting(Meeting meeting);

        /**
         * Stories in Sprints / Projects
         */

        [OperationContract]
        List<Story> GetAllStoriesByProject(int projectID);

        [OperationContract]
        List<Story> GetAllStoriesBySprint(int sprintID);

        [OperationContract]
        List<Story> GetAllStoriesWithoutSprint(int projectID);      
         
    }
}