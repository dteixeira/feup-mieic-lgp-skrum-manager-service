using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Projects
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ProjectService : IProjectService
    {
        /// <summary>
        /// Creates a new project in the database.
        /// </summary>
        /// <param name="project">Contains the information of the project to be created.</param>
        /// <returns>Created projects information</returns>
        public ServiceDataTypes.Project CreateProject(ServiceDataTypes.Project project)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var created = new SkrumManagerService.Project
                    {
                        AlertLimit = project.AlertLimit,
                        Name = project.Name,
                        Password = SkrumManagerService.ServiceHelper.HashPassword(project.Password),
                        Speed = project.Speed,
                        SprintDuration = project.SprintDuration,
                        CurrentStoryNumber = 1
                    };
                    context.Projects.InsertOnSubmit(created);
                    context.SubmitChanges();

                    // Notify clients.
                    Notifications.NotificationService.Instance.NotifyClients(ServiceDataTypes.NotificationType.GlobalProjectModification, -1);
                    return this.GetProjectByID(created.ProjectID);
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
        /// Updates a project record on the database.
        /// </summary>
        /// <param name="project">Contains the new data of the project.</param>
        /// <returns>Updated projects information</returns>
        public ServiceDataTypes.Project UpdateProject(ServiceDataTypes.Project project)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var updated = context.Projects.FirstOrDefault(p => p.ProjectID == project.ProjectID);
                    updated.AlertLimit = project.AlertLimit;
                    updated.Name = project.Name;
                    updated.Speed = project.Speed;
                    updated.SprintDuration = project.SprintDuration;
                    context.SubmitChanges();
                    return this.GetProjectByID(updated.ProjectID);
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
        /// Updates the password of a project in the database.
        /// </summary>
        /// <param name="projectID">The ID of the project to be updated</param>
        /// <param name="password">The new password for the project</param>
        /// <returns>Updated projects information</returns>
        public ServiceDataTypes.Project UpdateProjectPassword(int projectID, string password)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var project = context.Projects.FirstOrDefault(p => p.ProjectID == projectID);
                    project.Password = SkrumManagerService.ServiceHelper.HashPassword(password);
                    context.SubmitChanges();
                    return this.GetProjectByID(project.ProjectID);
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
        /// Deletes a project in the database.
        /// </summary>
        /// <param name="projectID">The ID of the project to be deleted.</param>
        /// <returns>True if the deleting is successful, false otherwise</returns>
        public bool DeleteProject(int projectID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var project = context.Projects.FirstOrDefault(p => p.ProjectID == projectID);

                    // Needed as the database won't handle this as a cascade on delete.
                    context.Stories.DeleteAllOnSubmit(project.Stories);
                    context.Projects.DeleteOnSubmit(project);
                    context.SubmitChanges();
                    return true;
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
        /// Returns the information of a project in the database, searching by its ID.
        /// </summary>
        /// <param name="projectID">The Id of the project to be searched.</param>
        /// <returns>The information of the project if found, null otherwise</returns>
        public ServiceDataTypes.Project GetProjectByID(int projectID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var project = context.Projects.FirstOrDefault(p => p.ProjectID == projectID);
                    return new ServiceDataTypes.Project
                    {
                        AlertLimit = project.AlertLimit,
                        Name = project.Name,
                        Password = project.Password == null ? null : "",
                        ProjectID = project.ProjectID,
                        Speed = project.Speed,
                        SprintDuration = project.SprintDuration,
                        Meetings = (
                            from m in project.Meetings.AsEnumerable()
                            let meeting = this.GetMeetingByID(m.MeetingID)
                            where meeting != null
                            select meeting
                        ).ToList<ServiceDataTypes.Meeting>(),
                        Sprints = (
                            from s in project.Sprints.AsEnumerable()
                            let sprint = this.GetSprintByID(s.SprintID)
                            where sprint != null
                            select sprint
                        ).ToList<ServiceDataTypes.Sprint>()
                    };
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
        /// Returns the information of a project in the database, searching by its name.
        /// </summary>
        /// <param name="name">The name of the project to be searched.</param>
        /// <returns>The information of the project if found, null otherwise</returns>
        public ServiceDataTypes.Project GetProjectByName(string name)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var project = context.Projects.FirstOrDefault(p => p.Name == name);
                    return this.GetProjectByID(project.ProjectID);
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
        /// Creates a new sprint in the database.
        /// </summary>
        /// <param name="sprint">Contains the information of the sprint to be created.</param>
        /// <returns>Created sprints information</returns>
        public ServiceDataTypes.Sprint CreateSprint(ServiceDataTypes.Sprint sprint)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var created = new SkrumManagerService.Sprint
                    {
                        BeginDate = sprint.BeginDate,
                        Closed = sprint.Closed,
                        EndDate = sprint.EndDate,
                        Number = sprint.Number,
                        ProjectID = sprint.ProjectID
                    };
                    context.Sprints.InsertOnSubmit(created);
                    context.SubmitChanges();
                    return this.GetSprintByID(created.SprintID);
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
        /// Deletes a Sprint record in the database.
        /// </summary>
        /// <param name="sprintID">Contains the ID of the Sprint to be deleted.</param>
        /// <returns>True if the deleting is successful, false otherwise.</returns>
        public bool DeleteSprint(int sprintID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var sprint = context.Sprints.FirstOrDefault(s => s.SprintID == sprintID);
                    context.Sprints.DeleteOnSubmit(sprint);
                    context.SubmitChanges();
                    return true;
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
        /// Updates a sprint record on the database.
        /// </summary>
        /// <param name="sprint">Contains the new data of the sprint.</param>
        /// <returns>Updated sprints information</returns>
        public ServiceDataTypes.Sprint UpdateSprint(ServiceDataTypes.Sprint sprint)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var updated = context.Sprints.FirstOrDefault(s => s.SprintID == sprint.SprintID);
                    updated.BeginDate = sprint.BeginDate;
                    updated.Closed = sprint.Closed;
                    updated.EndDate = sprint.EndDate;
                    updated.Number = sprint.Number;
                    context.SubmitChanges();
                    return this.GetSprintByID(updated.SprintID);
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
        /// Returns the information of a sprint in the database, searching by its ID.
        /// </summary>
        /// <param name="sprintID">The Id of the sprint to be searched.</param>
        /// <returns>The information of the sprint if found, null otherwise</returns>
        public ServiceDataTypes.Sprint GetSprintByID(int sprintID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var sprint = context.Sprints.FirstOrDefault(s => s.SprintID == sprintID);
                    return new ServiceDataTypes.Sprint
                    {
                        BeginDate = sprint.BeginDate,
                        Closed = sprint.Closed,
                        EndDate = sprint.EndDate,
                        Number = sprint.Number,
                        ProjectID = sprint.ProjectID,
                        SprintID = sprint.SprintID,
                        Stories = (
                            from s in sprint.StorySprints.AsEnumerable()
                            let story = this.GetStoryByID(s.StoryID)
                            where story != null
                            select story
                        ).ToList<ServiceDataTypes.Story>()
                    };
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
        /// Creates a new story in the database.
        /// </summary>
        /// <param name="story">Contains the information of the story to be created.</param>
        /// <returns>Created story's information</returns>
        public ServiceDataTypes.Story CreateStory(ServiceDataTypes.Story story)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var project = context.Projects.FirstOrDefault(p => p.ProjectID == story.ProjectID);
                    var stories = this.GetAllStoriesInProject(story.ProjectID);
                    var previousStory = stories.LastOrDefault();
                    var created = new SkrumManagerService.Story
                    {
                        CreationDate = story.CreationDate,
                        Description = story.Description,
                        PreviousStory = previousStory == null ? null : (int?)previousStory.StoryID,
                        ProjectID = story.ProjectID,
                        State = context.StoryStates.FirstOrDefault(ss => ss.State == story.State.ToString()).StoryStateID,
                        Number = project.CurrentStoryNumber
                    };
                    project.CurrentStoryNumber++;
                    context.Stories.InsertOnSubmit(created);
                    context.SubmitChanges();
                    return this.GetStoryByID(created.StoryID);
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
        /// Deletes a story in the database.
        /// </summary>
        /// <param name="storyID">The ID of the story to be deleted.</param>
        /// <returns>True if the deleting is successful, false otherwise</returns>
        public bool DeleteStory(int storyID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    // Update the story order.
                    var story = context.Stories.FirstOrDefault(s => s.StoryID == storyID);
                    var newStory = context.Stories.FirstOrDefault(s => s.PreviousStory == story.StoryID);
                    if (newStory != null)
                    {
                        newStory.PreviousStory = story.PreviousStory;
                    }

                    // Submit changes.
                    context.Stories.DeleteOnSubmit(story);
                    context.SubmitChanges();
                    return true;
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
        /// Updates a story record on the database.
        /// </summary>
        /// <param name="story">Contains the new data of the story.</param>
        /// <returns>Updated story information</returns>
        public ServiceDataTypes.Story UpdateStory(ServiceDataTypes.Story story)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var updated = context.Stories.FirstOrDefault(s => s.StoryID == story.StoryID);
                    updated.CreationDate = story.CreationDate;
                    updated.Description = story.Description;
                    updated.PreviousStory = story.PreviousStory;
                    updated.State = context.StoryStates.FirstOrDefault(ss => ss.State == story.State.ToString()).StoryStateID;
                    context.SubmitChanges();
                    return this.GetStoryByID(updated.StoryID);
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
        /// Returns the information of a story in the database, searching by its ID.
        /// </summary>
        /// <param name="storyID">The Id of the story to be searched.</param>
        /// <returns>The information of the story if found, null otherwise</returns>
        public ServiceDataTypes.Story GetStoryByID(int storyID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var story = context.Stories.FirstOrDefault(s => s.StoryID == storyID);
                    return new ServiceDataTypes.Story
                    {
                        CreationDate = story.CreationDate,
                        Description = story.Description,
                        PreviousStory = story.PreviousStory,
                        ProjectID = story.ProjectID,
                        State = (ServiceDataTypes.StoryState)System.Enum.Parse(typeof(ServiceDataTypes.StoryState), story.StoryState.State),
                        StoryID = story.StoryID,
                        Number = story.Number,
                        Tasks = (
                            from t in story.Tasks.AsEnumerable()
                            let task = this.GetTaskByID(t.TaskID)
                            where task != null
                            select task
                        ).ToList<ServiceDataTypes.Task>(),
                        StorySprints = (
                            from ss in story.StorySprints
                            select new ServiceDataTypes.StorySprint
                            {
                                Points = ss.Points,
                                Priority = (ServiceDataTypes.StoryPriority)System.Enum.Parse(typeof(ServiceDataTypes.StoryPriority), ss.StoryPriority.Priority),
                                SprintID = ss.SprintID,
                                StoryID = ss.StoryID
                            }
                        ).ToList<ServiceDataTypes.StorySprint>()
                    };
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
        /// <returns>Created tasks information</returns>
        public ServiceDataTypes.Task CreateTask(ServiceDataTypes.Task task)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var created = new SkrumManagerService.Task
                    {
                        CreationDate = task.CreationDate,
                        Description = task.Description,
                        Estimation = task.Estimation,
                        State = context.TaskStates.FirstOrDefault(ts => ts.State == task.State.ToString()).TaskStateID,
                        StoryID = task.StoryID
                    };
                    context.Tasks.InsertOnSubmit(created);
                    context.SubmitChanges();
                    return this.GetTaskByID(created.TaskID);
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
        /// Deletes a task in the database.
        /// </summary>
        /// <param name="taskID">The ID of the task to be deleted.</param>
        /// <returns>True if the deleting is successful, false otherwise</returns>
        public bool DeleteTask(int taskID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var task = context.Tasks.FirstOrDefault(t => t.TaskID == taskID);
                    context.Tasks.DeleteOnSubmit(task);
                    context.SubmitChanges();
                    return true;
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
        /// Updates a task record on the database.
        /// </summary>
        /// <param name="task">Contains the new data of the task.</param>
        /// <returns>Updated task information</returns>
        public ServiceDataTypes.Task UpdateTask(ServiceDataTypes.Task task)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var updated = context.Tasks.FirstOrDefault(t => t.TaskID == task.TaskID);
                    updated.CreationDate = task.CreationDate;
                    updated.Description = task.Description;
                    updated.Estimation = task.Estimation;
                    updated.State = context.TaskStates.FirstOrDefault(ts => ts.State == task.State.ToString()).TaskStateID;
                    context.SubmitChanges();
                    return this.GetTaskByID(updated.TaskID);
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
        /// Returns the information of a task in the database, searching by its ID.
        /// </summary>
        /// <param name="taskID">The Id of the task to be searched.</param>
        /// <returns>The information of the task if found, null otherwise</returns>
        public ServiceDataTypes.Task GetTaskByID(int taskID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var task = context.Tasks.FirstOrDefault(t => t.TaskID == taskID);
                    return new ServiceDataTypes.Task
                    {
                        CreationDate = task.CreationDate,
                        Description = task.Description,
                        Estimation = task.Estimation,
                        State = (ServiceDataTypes.TaskState)System.Enum.Parse(typeof(ServiceDataTypes.TaskState), task.TaskState.State),
                        StoryID = task.StoryID,
                        TaskID = task.TaskID,
                        PersonTasks = (
                            from pt in task.PersonTasks
                            select new ServiceDataTypes.PersonTask
                            {
                                CreationDate = pt.CreationDate,
                                PersonID = pt.PersonID,
                                SpentTime = pt.SpentTime,
                                TaskID = pt.TaskID
                            }
                        ).ToList<ServiceDataTypes.PersonTask>()
                    };
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
        /// Creates a new meeting in the database.
        /// </summary>
        /// <param name="meeting">Contains the information of the meeting to be created.</param>
        /// <returns>Created meeting information</returns>
        public ServiceDataTypes.Meeting CreateMeeting(ServiceDataTypes.Meeting meeting)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var created = new SkrumManagerService.Meeting
                    {
                        Date = meeting.Date,
                        Notes = meeting.Notes,
                        Number = meeting.Number,
                        ProjectID = meeting.ProjectID
                    };
                    context.Meetings.InsertOnSubmit(created);
                    context.SubmitChanges();
                    return this.GetMeetingByID(created.MeetingID);
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
        /// Deletes a meeting in the database.
        /// </summary>
        /// <param name="meetingID">The ID of the meeting to be deleted.</param>
        /// <returns>True if the deleting is successful, false otherwise</returns>
        public bool DeleteMeeting(int meetingID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var meeting = context.Meetings.FirstOrDefault(m => m.MeetingID == meetingID);
                    context.Meetings.DeleteOnSubmit(meeting);
                    context.SubmitChanges();
                    return true;
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
        /// Updates a meeting record on the database.
        /// </summary>
        /// <param name="meeting">Contains the new data of the meeting.</param>
        /// <returns>Updated meeting information</returns>
        public ServiceDataTypes.Meeting UpdateMeeting(ServiceDataTypes.Meeting meeting)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var updated = context.Meetings.FirstOrDefault(m => m.MeetingID == meeting.MeetingID);
                    updated.Date = meeting.Date;
                    updated.Notes = meeting.Notes;
                    updated.Number = meeting.Number;
                    context.SubmitChanges();
                    return this.GetMeetingByID(updated.MeetingID);
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
        /// Returns the information of a meeting in the database, searching by its ID.
        /// </summary>
        /// <param name="meetingID">The Id of the meeting to be searched.</param>
        /// <returns>The information of the meeting if found, null otherwise</returns>
        public ServiceDataTypes.Meeting GetMeetingByID(int meetingID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var meeting = context.Meetings.FirstOrDefault(m => m.MeetingID == meetingID);
                    return new ServiceDataTypes.Meeting
                    {
                        Date = meeting.Date,
                        MeetingID = meeting.MeetingID,
                        Notes = meeting.Notes,
                        Number = meeting.Number,
                        ProjectID = meeting.ProjectID
                    };
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
        /// Returns all the projects in the system
        /// </summary>
        /// <returns>A List with the information about every project, if any. Null otherwise</returns>
        public List<ServiceDataTypes.Project> GetAllProjects()
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    return (
                        from p in context.Projects.AsEnumerable()
                        let project = this.GetProjectByID(p.ProjectID)
                        where project != null
                        select project
                    ).ToList<ServiceDataTypes.Project>();
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
        /// Verifies if a given password is the real password of a given project
        /// </summary>
        /// <param name="projectID">The ID of the desired project</param>
        /// <param name="password"> Contains the password sent by the client</param>
        /// <returns>True if the password is the correct one, false otherwise</returns>
        public bool LoginProject(int projectID, string password)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var project = context.Projects.FirstOrDefault(p => p.ProjectID == projectID);
                    return project.Password == SkrumManagerService.ServiceHelper.HashPassword(password) && project.Password != null;
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
        /// Get all the sprints in a given project
        /// </summary>
        /// <param name="projectID">Contains the ID of the project to be searched.</param>
        /// <returns>A List containing all the sprints of the desired project, if found, null otherwise</returns>
        public List<ServiceDataTypes.Sprint> GetAllSprintsInProject(int projectID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var project = GetProjectByID(projectID);
                    return project.Sprints;
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
        /// Get all the stories in a given project
        /// </summary>
        /// <param name="projectID">Contains the ID of the project to be searched.</param>
        /// <returns>A List containing all the stories of the desired project, if found, null otherwise</returns>
        public List<ServiceDataTypes.Story> GetAllStoriesInProject(int projectID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var project = context.Projects.FirstOrDefault(p => p.ProjectID == projectID);
                    return (
                        from s in project.Stories.AsEnumerable()
                        let story = this.GetStoryByID(s.StoryID)
                        where story != null
                        select story
                    ).ToList<ServiceDataTypes.Story>();
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
        /// Get all the tasks in a given project
        /// </summary>
        /// <param name="projectID">Contains the ID of the project to be searched.</param>
        /// <returns>A List containing all the tasks of the desired project, if found, null otherwise</returns>
        public List<ServiceDataTypes.Task> GetAllTasksInProject(int projectID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var project = context.Projects.FirstOrDefault(p => p.ProjectID == projectID);
                    return (
                        from s in project.Stories
                        from t in s.Tasks.AsEnumerable()
                        let task = this.GetTaskByID(t.TaskID)
                        where task != null
                        select task
                    ).ToList<ServiceDataTypes.Task>();
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
        /// Get all the stories in a given project which are not assigned to any sprint
        /// </summary>
        /// <param name="projectID">Contains the ID of the project to be searched.</param>
        /// <returns>A List containing all the stories of the desired project, if found, null otherwise</returns>
        public List<ServiceDataTypes.Story> GetAllStoriesWithoutSprintInProject(int projectID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var project = context.Projects.FirstOrDefault(p => p.ProjectID == projectID);
                    return (
                        from s in project.Stories.AsEnumerable()
                        where s.StorySprints.Count() == 0
                        let story = this.GetStoryByID(s.StoryID)
                        where story != null
                        select story
                    ).ToList<ServiceDataTypes.Story>();
                }
            }
            catch (System.Exception e)
            {
                // Returns null if any problem occurs.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Get all the meetings in a given project
        /// </summary>
        /// <param name="projectID">Contains the ID of the project to be searched.</param>
        /// <returns>A List containing all the meetings of the desired project, if found, null otherwise</returns>
        public List<ServiceDataTypes.Meeting> GetAllMeetingsInProject(int projectID)
        {
            try
            {
                var project = this.GetProjectByID(projectID);
                return project.Meetings;
            }
            catch (System.Exception e)
            {
                // Returns null if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Get all the tasks in a given project with a given state
        /// </summary>
        /// <param name="projectID">Contains the ID of the project to be searched.</param>
        /// <param name="state">The state to search for in each task</param>
        /// <returns>A List containing all the tasks of the desired project with the desired state, if found, null otherwise</returns>
        public List<ServiceDataTypes.Task> GetAllTasksInProjectByState(int projectID, ServiceDataTypes.TaskState state)
        {
            try
            {
                var tasks = this.GetAllTasksInProject(projectID).Where(t => t.State == state);
                return tasks.ToList<ServiceDataTypes.Task>();
            }
            catch (System.Exception e)
            {
                // Returns null if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Get all the stories in a given project with a given state
        /// </summary>
        /// <param name="projectID">Contains the ID of the project to be searched.</param>
        /// <param name="state">The story state to search for in each task</param>
        /// <returns>A List containing all the stories of the desired project with the desired state, if found, null otherwise</returns>
        public List<ServiceDataTypes.Story> GetAllStoriesInProjectByState(int projectID, ServiceDataTypes.StoryState state)
        {
            try
            {
                var stories = this.GetAllStoriesInProject(projectID).Where(s => s.State == state);
                return stories.ToList<ServiceDataTypes.Story>();
            }
            catch (System.Exception e)
            {
                // Returns null if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Get all the stories in a given sprint
        /// </summary>
        /// <param name="sprintID">Contains the ID of the sprint to be searched.</param>
        /// <returns>A List containing all the stories of the desired sprint, if found, null otherwise</returns>
        public List<ServiceDataTypes.Story> GetAllStoriesInSprint(int sprintID)
        {
            try
            {
                var sprint = this.GetSprintByID(sprintID);
                return sprint.Stories;
            }
            catch (System.Exception e)
            {
                // Returns null if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Get all the tasks in a given sprint
        /// </summary>
        /// <param name="sprintID">Contains the ID of the sprint to be searched.</param>
        /// <returns>A List containing all the tasks of the desired sprint, if found, null otherwise</returns>
        public List<ServiceDataTypes.Task> GetAllTasksInSprint(int sprintID)
        {
            try
            {
                var sprint = this.GetSprintByID(sprintID);
                return (
                    from story in sprint.Stories
                    from task in story.Tasks
                    select task
                ).ToList<ServiceDataTypes.Task>();
            }
            catch (System.Exception e)
            {
                // Returns null if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Associates a Story to a sprint
        /// </summary>
        /// <param name="storySprint">The object containg all the information about the association of a story and a sprint</param>
        /// <returns>Created StorySprint information</returns>
        public ServiceDataTypes.StorySprint AddStoryInSprint(ServiceDataTypes.StorySprint storySprint)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var existent = context.StorySprints.FirstOrDefault(ss => ss.SprintID == storySprint.SprintID && ss.StoryID == storySprint.StoryID);

                    // Updates if it already exists.
                    if (existent != null)
                    {
                        existent.Points = storySprint.Points;
                        existent.Priority = context.StoryPriorities.FirstOrDefault(sp => sp.Priority == storySprint.Priority.ToString()).StoryPriorityID;
                        context.SubmitChanges();
                        return new ServiceDataTypes.StorySprint
                        {
                            Points = existent.Points,
                            Priority = (ServiceDataTypes.StoryPriority)System.Enum.Parse(typeof(ServiceDataTypes.StoryPriority), existent.StoryPriority.Priority),
                            SprintID = existent.SprintID,
                            StoryID = existent.StoryID
                        };
                    }
                    else
                    {
                        var created = new SkrumManagerService.StorySprint
                        {
                            Points = storySprint.Points,
                            Priority = context.StoryPriorities.FirstOrDefault(sp => sp.Priority == storySprint.Priority.ToString()).StoryPriorityID,
                            SprintID = storySprint.SprintID,
                            StoryID = storySprint.StoryID
                        };
                        context.StorySprints.InsertOnSubmit(created);
                        context.SubmitChanges();
                        return new ServiceDataTypes.StorySprint
                        {
                            Points = created.Points,
                            Priority = (ServiceDataTypes.StoryPriority)System.Enum.Parse(typeof(ServiceDataTypes.StoryPriority), created.StoryPriority.Priority),
                            SprintID = created.SprintID,
                            StoryID = created.StoryID
                        };
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
        /// Get all the tasks in a given story
        /// </summary>
        /// <param name="storyID">Contains the ID of the story to be searched.</param>
        /// <returns>A List containing all the tasks of the desired story, if found, null otherwise</returns>
        public List<ServiceDataTypes.Task> GetAllTasksInStory(int storyID)
        {
            try
            {
                var story = this.GetStoryByID(storyID);
                return story.Tasks;
            }
            catch (System.Exception e)
            {
                // Returns null if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Changes the order of the stories in a project
        /// </summary>
        /// <param name="projectID">Contains the information of the story to be created.</param>
        /// <param name="ordered">A List containing the new order for the stories inside the sprint</param>
        /// <returns>A List containing the stories inside a sprint with a new order</returns>
        public List<ServiceDataTypes.Story> UpdateStoryOrder(int projectID, List<int> ordered)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var project = context.Projects.FirstOrDefault(p => p.ProjectID == projectID);
                    
                    // Update first story so it has no parent reference.
                    int previous = ordered.First();
                    ordered.RemoveAt(0);
                    project.Stories.FirstOrDefault(s => s.StoryID == previous).PreviousStory = null;

                    // Update all other stories.
                    while (ordered.Count() > 0)
                    {
                        int current = ordered.First();
                        ordered.RemoveAt(0);
                        project.Stories.FirstOrDefault(s => s.StoryID == current).PreviousStory = previous;
                        previous = current;
                    }
                    context.SubmitChanges();
                    return this.GetAllStoriesInProject(project.ProjectID);
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
        /// Adds a record of a persons time spent working on a task to the database
        /// </summary>
        /// <param name="personTask">Object that contains the information of the person, task and time spent.</param>
        /// <returns>Created PersonTask's information</returns>
        public ServiceDataTypes.PersonTask AddWorkInTask(ServiceDataTypes.PersonTask personTask)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var existent = context.PersonTasks.FirstOrDefault(pt => pt.PersonID == personTask.PersonID && pt.TaskID == personTask.TaskID);

                    // Updates the spent time if it already exists.
                    if (existent != null)
                    {
                        existent.SpentTime += personTask.SpentTime;
                        context.SubmitChanges();
                        return new ServiceDataTypes.PersonTask
                        {
                            CreationDate = existent.CreationDate,
                            PersonID = existent.PersonID,
                            SpentTime = existent.SpentTime,
                            TaskID = existent.TaskID
                        };
                    }
                    else
                    {
                        var created = new SkrumManagerService.PersonTask
                        {
                            CreationDate = personTask.CreationDate,
                            PersonID = personTask.PersonID,
                            SpentTime = personTask.SpentTime,
                            TaskID = personTask.TaskID
                        };
                        context.PersonTasks.InsertOnSubmit(created);
                        context.SubmitChanges();
                        return new ServiceDataTypes.PersonTask
                        {
                            CreationDate = created.CreationDate,
                            PersonID = created.PersonID,
                            SpentTime = created.SpentTime,
                            TaskID = created.TaskID
                        };
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
    }
}