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
        Sprint CreateSprint(Sprint sprint);

        [OperationContract]
        bool DeleteSprint(int sprintID);

        [OperationContract]
        Sprint UpdateSprint(Sprint sprint);

        [OperationContract]
        Sprint GetSprintByID(int sprintID);

        /**
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

        /**
         * Stories in Sprints / Projects
         */

        [OperationContract]
        List<Story> GetAllStoriesByProject(int projectID);

        [OperationContract]
        List<Story> GetAllStoriesBySprint(int sprintID);

        [OperationContract]
        List<Story> GetAllStoriesWithoutSprint(int projectID);   
   
        /**
         * TODO
         */

        [OperationContract]
        Story CreateStory(Story story);

        [OperationContract]
        bool DeleteStory(int storyID);

        [OperationContract]
        Story GetStoryByID(int storyID);

        [OperationContract]
        Story UpdateStory(Story person);

        //------------------------------//

        [OperationContract]
        Story CreateTask(Task task);

        [OperationContract]
        bool DeleteTask(int taskID);

        [OperationContract]
        Task GetTaskByID(int taskID);

        [OperationContract]
        Task UpdateTask(Task task);

        //------------------------------//

        [OperationContract]
        bool InsertWorkTime(int userID, int taskID, double spentTime);

        [OperationContract]
        List<Task> GetAllTasks();

        [OperationContract]
        List<Task> GetAllTasksByProject(int projectID);

        [OperationContract]
        List<Story> GetAllStories();
         
    }
}