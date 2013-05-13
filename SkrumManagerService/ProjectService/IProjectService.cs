using ServiceDataTypes;
using System.ServiceModel;

namespace Projects
{
    [ServiceContract]
    public interface IProjectService
    {
        /*
         * Project CRUD
         */

        [OperationContract]
        Project CreateProject(Project project);

        [OperationContract]
        Project UpdateProject(Project project);

        [OperationContract]
        Project UpdateProjectPassword(int projectID, string password);

        [OperationContract]
        bool DeleteProject(int projectID);

        [OperationContract]
        Project GetProjectByID(int projectID);

        [OperationContract]
        Project GetProjectByName(string name);

        /*
         * Sprint CRUD
         */

        [OperationContract]
        Sprint CreateSprint(Sprint sprint);

        [OperationContract]
        bool DeleteSprint(int sprintID);

        [OperationContract]
        Sprint UpdateSprint(Sprint sprint);

        [OperationContract]
        Sprint GetSprintByID(int sprintID);

        /*
         * Story CRUD
         */

        [OperationContract]
        Story CreateStory(Story story);

        [OperationContract]
        bool DeleteStory(int storyID);

        [OperationContract]
        Story UpdateStory(Story story);

        [OperationContract]
        Story GetStoryByID(int storyID);

        /*
         * Task CRUD
         */

        [OperationContract]
        Task CreateTask(Task task);

        [OperationContract]
        bool DeleteTask(int taskID);

        [OperationContract]
        Task UpdateTask(Task task);

        [OperationContract]
        Task GetTaskByID(int taskID);

        /*
         * Meeting CRUD
         */

        [OperationContract]
        Meeting CreateMeeting(Meeting meeting);

        [OperationContract]
        bool DeleteMeeting(int meetingID);

        [OperationContract]
        Meeting UpdateMeeting(Meeting meeting);

        [OperationContract]
        Meeting GetMeetingByID(int meetingID);

        /*
         * System-wise operations
         */

        [OperationContract]
        System.Collections.Generic.List<Project> GetAllProjects();

        /*
         * Project utilities
         */

        [OperationContract]
        bool LoginProject(int projectID, string password);

        [OperationContract]
        System.Collections.Generic.List<Sprint> GetAllSprintsInProject(int projectID);

        [OperationContract]
        System.Collections.Generic.List<Story> GetAllStoriesInProject(int projectID);

        [OperationContract]
        System.Collections.Generic.List<Task> GetAllTasksInProject(int projectID);

        [OperationContract]
        System.Collections.Generic.List<Story> GetAllStoriesWithoutSprintInProject(int projectID);

        [OperationContract]
        System.Collections.Generic.List<Meeting> GetAllMeetingsInProject(int projectID);

        [OperationContract]
        System.Collections.Generic.List<Task> GetAllTasksInProjectByState(int projectID, TaskState state);

        [OperationContract]
        System.Collections.Generic.List<Story> GetAllStoriesInProjectByState(int projectID, StoryState state);

        /*
         * Sprint utilities
         */

        [OperationContract]
        System.Collections.Generic.List<Story> GetAllStoriesInSprint(int sprintID);

        [OperationContract]
        System.Collections.Generic.List<Task> GetAllTasksInSprint(int sprintID);

        [OperationContract]
        StorySprint AddStoryInSprint(StorySprint storySprint);

        /*
         * Story utilities
         */

        [OperationContract]
        System.Collections.Generic.List<Task> GetAllTasksInStory(int storyID);

        [OperationContract]
        System.Collections.Generic.List<Story> UpdateStoryOrder(int projectID, System.Collections.Generic.List<int> ordered);

        /*
         * Task utilities
         */

        [OperationContract]
        PersonTask AddWorkInTask(PersonTask personTask);
    }
}