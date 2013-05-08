using System.ServiceModel;
using System.Linq;


namespace Taskboards
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class TaskboardService : ITaskboardService
    {
        /// <summary>
        /// Creates a new story in the database.
        /// </summary>
        /// <param name="story">Contains the information of the story to be created.</param>
        /// <returns>Created story's information</returns>
        public ServiceDataTypes.Story CreateStory(ServiceDataTypes.Story story)
        {
            try
            {
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();

                // Create a database Story instance.
                SkrumManagerService.Story created = new SkrumManagerService.Story();
                created.Description = story.Description;
                created.NextStory = story.NextStory == null ? 0 : (int)story.NextStory;
                created.ProjectID = story.ProjectID == null ? 1 : (int)story.ProjectID;
                created.CreationDate = story.CreationDate;

               
                var states = context.GetTable<SkrumManagerService.StoryState>();
                var estado = states.FirstOrDefault(p => p.State == story.State.ToString());
             
                if (estado==null) created.State = 0;
                else created.State = estado.StoryStateID;
                           
                // Saves the person to the database.
                
                context.GetTable<SkrumManagerService.Story>().InsertOnSubmit(created);
                context.SubmitChanges();
                context.Dispose();

                // Return the person's info.
                return this.GetStoryByID(created.StoryID);
         }
         catch (System.Exception)
         {
             // Returns null if anything goes wrong.
             return null;
         }
        }

        /// <summary>
        /// Deletes a story record.
        /// </summary>
        /// <param name="storyID">ID of the Story to delete</param>
        /// <returns>true if the story was deleted, false otherwise.</returns>
        public bool DeleteStory(int storyID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var stories = context.GetTable<SkrumManagerService.Story>();
                    var result = stories.FirstOrDefault(p => p.StoryID == storyID);
                    if (result != null)
                    {
                        stories.DeleteOnSubmit(result);
                        context.SubmitChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (System.Exception e)
            {
                // Returns false if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Returns a story's information using that story's ID to search.
        /// </summary>
        /// <param name="storyID">Id of the story to search for</param>
        /// <returns>The filled Story instance if found, null otherwise.</returns>
        public ServiceDataTypes.Story GetStoryByID(int storyID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var result = context.GetTable<SkrumManagerService.Story>().FirstOrDefault(p => p.StoryID == storyID);

                    // Return null if no result was found, or the filled story instance.
                    if (result == null)
                    {
                        return null;
                    }
                    else
                    {
                        var desc = (from p in context.GetTable<SkrumManagerService.StoryState>()
                                   where p.StoryStateID == result.State
                                   select p.State).FirstOrDefault();
                        ServiceDataTypes.Story story = new ServiceDataTypes.Story()
                        {

                            /*
                             private System.Collections.Generic.List<Task> tasks;*/
                            StoryID = storyID,
                            CreationDate = result.CreationDate,
                            Description = result.Description,
                            NextStory = result.NextStory,
                            ProjectID = result.ProjectID,
                            State = (ServiceDataTypes.StoryState)System.Enum.Parse(typeof(ServiceDataTypes.StoryState), desc)
                        };

                        // Generate tasks in story.
                        story.Tasks= (result.Tasks.Select(p => new ServiceDataTypes.Task
                        {
                            CreationDate = p.CreationDate,
                            Description = p.Description,
                            Estimation = p.Estimation,
                            PersonID = p.PersonID,
                            StoryID = p.StoryID,
                            TaskID = p.TaskID
                        })).ToList();

                        // Returns the person's info.
                        return story;
                    }
                }
            }
            catch (System.Exception e)
            {
                // Returns null if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Updates a storys information with the given values.
        /// </summary>
        /// <param name="story">Contains the new values</param>
        /// <returns>The tasks current information.</returns>
        public ServiceDataTypes.Story UpdateStory(ServiceDataTypes.Story story)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var stories = context.GetTable<SkrumManagerService.Story>();
                    var result = stories.FirstOrDefault(p => p.StoryID == story.StoryID);
                    if (result == null)
                    {
                        // Return null if user no longer exists.
                        return null;
                    }
                    else
                    {
                        var desc = (from p in context.GetTable<SkrumManagerService.StoryState>()
                                    where p.State == story.State.ToString()
                                    select p.StoryStateID).FirstOrDefault();
                        if (story.CreationDate != null)
                        {
                            result.CreationDate = story.CreationDate;
                        }
                        if (story.Description != null)
                        {
                            result.Description = story.Description;
                        }
                        if (story.NextStory != null)
                        {
                            result.NextStory = (int)story.NextStory;
                        }
                        if (story.ProjectID != null)
                        {
                            result.ProjectID = (int)story.ProjectID;
                        }
                        if (story.State != null)
                        {
                            result.State = desc;
                        }
                        
                        // Update the person's information.
                        context.SubmitChanges();

                        // Get and return the person's info.
                        return this.GetStoryByID(result.StoryID);
                    }
                }
            }
            catch (System.Exception e)
            {
                // Returns null if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Creates a new task in the database.
        /// </summary>
        /// <param name="task">Contains the information of the task to be created.</param>
        /// <returns>Created task's information</returns>
        public ServiceDataTypes.Story CreateTask(ServiceDataTypes.Task task)
        {

            try
            {
                // Create a database task instance.
                SkrumManagerService.Task created = new SkrumManagerService.Task();
                created.Description = task.Description;
                created.Estimation = task.Estimation == null ? 1 : (int) task.Estimation;
                created.PersonID = task.PersonID == null ? 0 : (int) task.PersonID;
                created.StoryID = task.StoryID == null ? 0 : (int) task.StoryID;
                

                // Saves the person to the database.
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    context.GetTable<SkrumManagerService.Task>().InsertOnSubmit(created);
                    context.SubmitChanges();
                }

                // Return the person's info.
                return this.GetStoryByID(created.StoryID);
            }
            catch (System.Exception e)
            {
                // Returns null if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Deletes a task record.
        /// </summary>
        /// <param name="taskID">ID of the task to delete</param>
        /// <returns>true if the task was deleted, false otherwise.</returns>
        public bool DeleteTask(int taskID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var tasks = context.GetTable<SkrumManagerService.Task>();
                    var result = tasks.FirstOrDefault(p => p.TaskID == taskID);
                    if (result != null)
                    {
                        tasks.DeleteOnSubmit(result);
                        context.SubmitChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (System.Exception e)
            {
                // Returns false if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Returns a tasks information using that tasks ID to search.
        /// </summary>
        /// <param name="taskID">Id of the task to search for</param>
        /// <returns>The filled Task instance if found, null otherwise.</returns>
        public ServiceDataTypes.Task GetTaskByID(int taskID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var result = context.GetTable<SkrumManagerService.Task>().FirstOrDefault(p => p.TaskID == taskID);

                    // Return null if no result was found, or the filled task instance.
                    if (result == null)
                    {
                        return null;
                    }
                    else
                    {
                        ServiceDataTypes.Task task = new ServiceDataTypes.Task()
                        {
                            TaskID = taskID,
                            Description = result.Description,
                            Estimation = result.Estimation,
                            PersonID = result.PersonID,
                            StoryID = result.StoryID,
                        };

                        // Returns the tasks info.
                        return task;
                    }
                }
            }
            catch (System.Exception e)
            {
                // Returns null if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Updates a tasks information with the given values.
        /// </summary>
        /// <param name="task">Contains the new values</param>
        /// <returns>The tasks current information.</returns>
        public ServiceDataTypes.Task UpdateTask(ServiceDataTypes.Task task)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var tasks = context.GetTable<SkrumManagerService.Task>();
                    var result = tasks.FirstOrDefault(p => p.TaskID == task.TaskID);
                    if (result == null)
                    {
                        // Return null if user no longer exists.
                        return null;
                    }
                    else
                    {
                        if (task.Description != null)
                        {
                            result.Description = task.Description;
                        }
                        if (task.Estimation != null)
                        {
                            result.Estimation = (int)task.Estimation;
                        }
                        if (task.PersonID != null)
                        {
                            result.PersonID = (int)task.PersonID;
                        }
                        if (task.StoryID != null)
                        {
                            result.StoryID = (int)task.StoryID;
                        }
                       
                        // Update the person's information.
                        context.SubmitChanges();

                        // Get and return the person's info.
                        return this.GetTaskByID(result.TaskID);
                    }
                }
            }
            catch (System.Exception e)
            {
                // Returns null if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        public bool InsertWorkTime(int userID, int taskID, double spentTime)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Fetches a list of all the system's tasks.
        /// </summary>
        /// <returns>A list containing the information about all the tasks in the system.</returns>
        public System.Collections.Generic.List<ServiceDataTypes.Task> GetAllTasks()
        {
            try
            {
                System.Collections.Generic.List<ServiceDataTypes.Task> tasks = null;
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    tasks = (
                        from id in
                            (
                                from p in context.GetTable<SkrumManagerService.Task>()
                                select p.TaskID
                            ).AsEnumerable()
                        let task = this.GetTaskByID(id)
                        where task != null
                        select task).ToList();
                }
                return tasks;
            }
            catch (System.Exception e)
            {
                // Returns null if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return null;
            }

        }

        /// <summary>
        /// Fetches a list of all the tasks in a given project.
        /// </summary>
        /// <returns>A list containing the information about all the tasks in the project.</returns>
        public System.Collections.Generic.List<ServiceDataTypes.Task> GetAllTasksByProject(int projectID)
        {
            try
            {
                System.Collections.Generic.List<ServiceDataTypes.Task> result = null;
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var projects = context.GetTable<SkrumManagerService.Project>();
                var project = projects.FirstOrDefault(p => p.ProjectID == projectID);

                if (project == null) return null;

                var tasks =
                        from s in project.Stories
                        from t in s.Tasks
                        select t;

                if (tasks == null) return null;

                foreach (var task in tasks)
                {
                    result.Add(GetTaskByID(task.TaskID));
                }

                return result;
            }
            catch (System.Exception e)
            {
                // Returns null if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Fetches a list of all the stories in the system.
        /// </summary>
        /// <returns>A list containing the information about all the stories in the system.</returns>
        public System.Collections.Generic.List<ServiceDataTypes.Story> GetAllStories()
        {
            try
            {
                System.Collections.Generic.List<ServiceDataTypes.Story> stories = null;
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    stories = (
                        from id in
                            (
                                from p in context.GetTable<SkrumManagerService.Story>()
                                select p.StoryID
                            ).AsEnumerable()
                        let story = this.GetStoryByID(id)
                        where story != null
                        select story).ToList();
                }
                return stories;
            }
            catch (System.Exception e)
            {
                // Returns null if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        
    }
}