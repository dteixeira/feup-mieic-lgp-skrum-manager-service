using ServiceDataTypes;
using System.ServiceModel;
using System.Collections.Generic;

namespace Taskboards
{
    [ServiceContract]
    public interface ITaskboardService
    {
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