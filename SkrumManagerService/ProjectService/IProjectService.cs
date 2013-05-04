using System.Collections.Generic;
using ServiceDataTypes;
using System.ServiceModel;

namespace Projects
{
    [ServiceContract]
    public interface IProjectService
    {
        [OperationContract]
        Project CreateProject(Project project);

        [OperationContract]
        bool DeleteProject(Project project);

        [OperationContract]
        Project GetProjectByID(int projectID);

        [OperationContract]
        Project UpdateProject(Project project);

        //--------------------//

        [OperationContract]
        Sprint CreateSprint(Sprint sprint);

        [OperationContract]
        bool DeleteSprint(Sprint sprint);

        [OperationContract]
        Sprint GetSprintByID(int sprintID);

        [OperationContract]
        Sprint UpdateSprint(Sprint sprint);

        //--------------------//

        [OperationContract]
        List<Person> GetPersonsinProject(Project project);

        [OperationContract]
        List<Sprint> GetSprintsinProject(Project project);

        [OperationContract]
        Person GiveRole(Person person, Project project, RoleDescription role, float assignedTime);

        [OperationContract]
        List<Sprint> GetClosedSprints(int projectID);

    }
}