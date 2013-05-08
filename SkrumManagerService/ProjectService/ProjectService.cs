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
                    var result = context.GetTable<SkrumManagerService.Project>().FirstOrDefault(p => p.ProjectID == projectID);

                    ServiceDataTypes.Project project = (
                         from p in context.GetTable<SkrumManagerService.Project>()
                         where p.ProjectID == projectID
                         select new ServiceDataTypes.Project
                         {
                             ProjectID = p.ProjectID,
                             AlertLimit = p.AlertLimit,
                             Name = p.Name,
                             Speed = p.Speed,
                             SprintDuration = p.SprintDuration,
                             Meetings = (
                                from m in p.Meetings
                                select new ServiceDataTypes.Meeting
                                {
                                    Date = m.Date,
                                    MeetingID = m.MeetingID,
                                    Notes = m.Notes,
                                    Number = m.Number,
                                    ProjectID = m.ProjectID
                                }
                             ).ToList<ServiceDataTypes.Meeting>(),
                             Sprints = (
                                from s in p.Sprints
                                select new ServiceDataTypes.Sprint
                                {
                                    BeginDate = s.BeginDate,
                                    Closed = s.Closed,
                                    EndDate = s.EndDate,
                                    Number = s.Number,
                                    ProjectID = s.ProjectID,
                                    SprintID = s.SprintID
                                }
                             ).ToList<ServiceDataTypes.Sprint>()
                         }).FirstOrDefault();
                    return project;
                }
            }
            catch (System.Exception e)
            {
                // Returns null if any problem occurs.
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

        public ServiceDataTypes.Project CreateSprint(ServiceDataTypes.Sprint sprint)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var project = context.GetTable<SkrumManagerService.Project>().FirstOrDefault(p => p.ProjectID == sprint.ProjectID);
                    project.Sprints.Add(new SkrumManagerService.Sprint
                    {
                        BeginDate = sprint.BeginDate,
                        Closed = (bool)sprint.Closed,
                        EndDate = sprint.EndDate,
                        Number = (int)sprint.Number
                    });
                    context.SubmitChanges();
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

        public ServiceDataTypes.Project UpdateSprint(ServiceDataTypes.Sprint sprint)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var updated = context.Sprints.FirstOrDefault(s => s.SprintID == sprint.SprintID);
                    if (sprint.BeginDate != null)
                    {
                        updated.BeginDate = sprint.BeginDate;
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
                    return this.GetProjectByID(updated.ProjectID);
                }
            }
            catch (System.Exception e)
            {
                // Returns null if any problem occurs.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        public ServiceDataTypes.Project CreateMeeting(ServiceDataTypes.Meeting meeting)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var project = context.Projects.FirstOrDefault(p => p.ProjectID == meeting.ProjectID);
                    project.Meetings.Add(new SkrumManagerService.Meeting
                    {
                        Date = meeting.Date,
                        Notes = meeting.Notes,
                        Number = (int)meeting.Number
                    });
                    context.SubmitChanges();
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

        public ServiceDataTypes.Project UpdateMeeting(ServiceDataTypes.Meeting meeting)
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
                        updated.Date = meeting.Date;
                    }
                    if (meeting.Number != null)
                    {
                        updated.Number = (int)meeting.Number;
                    }
                    context.SubmitChanges();
                    return this.GetProjectByID(updated.ProjectID);
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
        /// Fetches a list of all the stories in a given project.
        /// </summary>
        /// <returns>A list containing the information about all the stories in the given project.</returns>
        public System.Collections.Generic.List<ServiceDataTypes.Story> GetAllStoriesByProject(int projectID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var project = context.Projects.FirstOrDefault(p => p.ProjectID == projectID);
                    var stories = (
                        from s in project.Stories
                        select new ServiceDataTypes.Story
                        {
                            CreationDate = s.CreationDate,
                            Description = s.Description,
                            NextStory = s.NextStory,
                            ProjectID = s.ProjectID,
                            StoryID = s.StoryID,
                            Tasks = (
                                from t in s.Tasks
                                select new ServiceDataTypes.Task {
                                    CreationDate = t.CreationDate,
                                    Description = t.Description,
                                    Estimation = t.Estimation,
                                    PersonID = t.PersonID,
                                    StoryID = t.StoryID,
                                    TaskID = t.TaskID
                                }).ToList<ServiceDataTypes.Task>()
                        }).ToList<ServiceDataTypes.Story>();
                    return stories;
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
        /// Fetches a list of all the stories in a given sprint.
        /// </summary>
        /// <returns>A list containing the information about all the stories in the given sprint.</returns>
        public System.Collections.Generic.List<ServiceDataTypes.Story> GetAllStoriesBySprint(int sprintID)
        {
            try
            {
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var sprint = context.GetTable<SkrumManagerService.Sprint>().FirstOrDefault(p => p.SprintID == sprintID);
                if (sprint == null) return null;

                ServiceDataTypes.Project project = GetProjectByID(sprint.ProjectID);
                if (project == null) return null;

                foreach (ServiceDataTypes.Sprint spr in project.Sprints)
                    if (spr.SprintID == sprintID) return spr.Stories;

                return null;
            }
            catch (System.Exception e)
            {
                // Returns null if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Fetches a list of all the stories not inside a sprint.
        /// </summary>
        /// <returns>A list containing the information about all the stories in the given project.</returns>
        public System.Collections.Generic.List<ServiceDataTypes.Story> GetAllStoriesWithoutSprint(int projectID)
        {
            try
            {
                List<ServiceDataTypes.Story> result = GetAllStoriesByProject(projectID);
                ServiceDataTypes.Project project = GetProjectByID(projectID);
                foreach (ServiceDataTypes.Sprint spr in project.Sprints)
                {
                    result.Except(spr.Stories);
                }
                return null;
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