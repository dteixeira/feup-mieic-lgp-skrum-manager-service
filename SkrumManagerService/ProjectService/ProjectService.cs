using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Projects
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ProjectService : IProjectService
    {
        public ServiceDataTypes.Project CreateProject(ServiceDataTypes.Project project)
        {
            try
            {
                // Create a database project instance and assume some
                // default values if they're not given.
                SkrumManagerService.Project created = new SkrumManagerService.Project();
                created.SprintDuration = project.SprintDuration == null ? 1 : (int)project.SprintDuration;
                created.AlertLimit = project.AlertLimit == null ? 1 : (int)project.AlertLimit;
                created.Speed = project.Speed == null ? 1 : (int)project.Speed;
                created.Name = project.Name;

                // Hash the password if it exists.
                if (project.Password != null)
                {
                    System.Security.Cryptography.SHA512 sha512 = new System.Security.Cryptography.SHA512Managed();
                    System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                    byte[] digest = sha512.ComputeHash(encoder.GetBytes(project.Password));
                    sha512.Dispose();
                    string password = System.BitConverter.ToString(digest);
                    password = password.Replace("-", "");
                    created.Password = password;
                }

                // Saves the project to the database.
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    context.GetTable<SkrumManagerService.Project>().InsertOnSubmit(created);
                    context.SubmitChanges();
                }

                // Treat the Project instance.
                return this.GetProjectByID(created.ProjectID);
            }
            catch (System.Exception e)
            {
                // Returns null if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        public bool DeleteProject(int projectID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var projects = context.GetTable<SkrumManagerService.Project>();
                    var result = projects.FirstOrDefault(p => p.ProjectID == projectID);
                    if (result != null)
                    {
                        projects.DeleteOnSubmit(result);
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
                // Returns false if any problem occurs.
                System.Console.WriteLine(e.Message);
                return false;
            }
        }

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
                        ProjectID = project.ProjectID,
                        Password = project.Password,
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

        public ServiceDataTypes.Project UpdateProject(ServiceDataTypes.Project project)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    // Gets the project.
                    var projects = context.GetTable<SkrumManagerService.Project>();
                    var result = projects.FirstOrDefault(p => p.ProjectID == project.ProjectID);
                    if (result == null)
                    {
                        // Return null if user no longer exists.
                        return null;
                    }
                    else
                    {
                        if (project.Name != null)
                        {
                            result.Name = project.Name;
                        }
                        if (project.SprintDuration != null)
                        {
                            result.SprintDuration = (int)project.SprintDuration;
                        }
                        if (project.AlertLimit != null)
                        {
                            result.AlertLimit = (int)project.AlertLimit;
                        }
                        if (project.Speed != null)
                        {
                            result.Speed = (int)project.Speed;
                        }
                        if (project.Password != null)
                        {
                            System.Security.Cryptography.SHA512 sha512 = new System.Security.Cryptography.SHA512Managed();
                            System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                            byte[] digest = sha512.ComputeHash(encoder.GetBytes(project.Password));
                            sha512.Dispose();
                            string password = System.BitConverter.ToString(digest);
                            password = password.Replace("-", "");
                            result.Password = password;
                        }

                        // Update the person's information.
                        context.SubmitChanges();
                    }
                }

                // Fetches and returns updated project info.
                return this.GetProjectByID((int)project.ProjectID);
            }
            catch (System.Exception e)
            {
                // Returns null if any problem occurs.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        public ServiceDataTypes.Project GetProjectByName(string name)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var project = context.GetTable<SkrumManagerService.Project>().FirstOrDefault(p => p.Name == name);
                    return this.GetProjectByID(project.ProjectID);
                }
            }
            catch (System.Exception e)
            {
                // Returns null if any problem occurs.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        public ServiceDataTypes.Sprint CreateSprint(ServiceDataTypes.Sprint sprint)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var created = new SkrumManagerService.Sprint
                    {
                        BeginDate = (System.DateTime)sprint.BeginDate,
                        Closed = (bool)sprint.Closed,
                        EndDate = (System.DateTime)sprint.EndDate,
                        Number = (int)sprint.Number,
                        ProjectID = (int)sprint.ProjectID
                    };
                    context.Sprints.InsertOnSubmit(created);
                    context.SubmitChanges();
                    return this.GetSprintByID(created.SprintID);
                }
            }
            catch (System.Exception e)
            {
                // Returns null if any problem occurs.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

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
                // Returns false if any problem occurs.
                System.Console.WriteLine(e.Message);
                return false;
            }
        }

        public ServiceDataTypes.Sprint UpdateSprint(ServiceDataTypes.Sprint sprint)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var updated = context.Sprints.FirstOrDefault(s => s.SprintID == sprint.SprintID);
                    if (sprint.BeginDate != null)
                    {
                        updated.BeginDate = (System.DateTime)sprint.BeginDate;
                    }
                    if (sprint.Closed != null)
                    {
                        updated.Closed = (bool)sprint.Closed;
                    }
                    if (sprint.EndDate != null)
                    {
                        updated.EndDate = sprint.EndDate;
                    }
                    if (sprint.Number != null)
                    {
                        updated.Number = (int)sprint.Number;
                    }
                    context.SubmitChanges();
                    return this.GetSprintByID(updated.SprintID);
                }
            }
            catch (System.Exception e)
            {
                // Returns null if any problem occurs.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

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

        public ServiceDataTypes.Meeting CreateMeeting(ServiceDataTypes.Meeting meeting)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var created = new SkrumManagerService.Meeting
                    {
                        Date = (System.DateTime)meeting.Date,
                        Notes = meeting.Notes,
                        Number = (int)meeting.Number,
                        ProjectID = (int)meeting.ProjectID
                    };
                    context.Meetings.InsertOnSubmit(created);
                    context.SubmitChanges();
                    return this.GetMeetingByID(created.MeetingID);
                }
            }
            catch (System.Exception e)
            {
                // Returns null if any problem occurs.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

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
                // Returns false if any problem occurs.
                System.Console.WriteLine(e.Message);
                return false;
            }
        }

        public ServiceDataTypes.Meeting UpdateMeeting(ServiceDataTypes.Meeting meeting)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var updated = context.Meetings.FirstOrDefault(m => m.MeetingID == meeting.MeetingID);
                    if (meeting.Notes != null)
                    {
                        updated.Notes = meeting.Notes;
                    }
                    if (meeting.Date != null)
                    {
                        updated.Date = (System.DateTime)meeting.Date;
                    }
                    if (meeting.Number != null)
                    {
                        updated.Number = (int)meeting.Number;
                    }
                    context.SubmitChanges();
                    return this.GetMeetingByID(updated.MeetingID);
                }
            }
            catch (System.Exception e)
            {
                // Returns null if any problem occurs.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

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

        public List<ServiceDataTypes.Story> GetAllStoriesByProject(int projectID)
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
                // Returns null if any problem occurs.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<ServiceDataTypes.Story> GetAllStoriesBySprint(int sprintID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var sprint = context.Sprints.FirstOrDefault(s => s.SprintID == sprintID);
                    return (
                        from s in sprint.StorySprints.AsEnumerable()
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

        public List<ServiceDataTypes.Story> GetAllStoriesWithoutSprint(int projectID)
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

        public ServiceDataTypes.Story CreateStory(ServiceDataTypes.Story story)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteStory(int storyID)
        {
            throw new System.NotImplementedException();
        }

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
                        NextStory = story.NextStory,
                        ProjectID = story.ProjectID,
                        State = (ServiceDataTypes.StoryState)System.Enum.Parse(typeof(ServiceDataTypes.StoryState), story.StoryState.State),
                        StoryID = story.StoryID,
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
                                StoryID = ss.StoryID,
                                StorySprintID = ss.StorySprintID
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

        public ServiceDataTypes.Story UpdateStory(ServiceDataTypes.Story person)
        {
            throw new System.NotImplementedException();
        }

        public ServiceDataTypes.Story CreateTask(ServiceDataTypes.Task task)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteTask(int taskID)
        {
            throw new System.NotImplementedException();
        }

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
                        PersonID = task.PersonID,
                        StoryID = task.StoryID,
                        TaskID = task.TaskID,
                        PersonTasks = (
                            from pt in task.PersonTasks
                            select new ServiceDataTypes.PersonTask
                            {
                                CreationDate = pt.CreationDate,
                                PersonID = pt.PersonID,
                                PersonTaskID = pt.PersonTaskID,
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

        public ServiceDataTypes.Task UpdateTask(ServiceDataTypes.Task task)
        {
            throw new System.NotImplementedException();
        }

        public bool InsertWorkTime(int userID, int taskID, double spentTime)
        {
            throw new System.NotImplementedException();
        }

        public List<ServiceDataTypes.Task> GetAllTasks()
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    return (
                        from t in context.Tasks.AsEnumerable()
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

        public List<ServiceDataTypes.Task> GetAllTasksByProject(int projectID)
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

        public List<ServiceDataTypes.Story> GetAllStories()
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    return (
                        from s in context.Stories.AsEnumerable()
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
    }
}