using System.ServiceModel;
using System.Linq;
using System.Collections.Generic;

namespace Projects
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ProjectService : IProjectService
    {

        /// <summary>
        /// Creates a new project in the database.
        /// </summary>
        /// <param name="project">Contains the information of the project to be created.</param>
        /// <returns>Created project's information</returns>
        public ServiceDataTypes.Project CreateProject(ServiceDataTypes.Project project)
        {
            try
            {
                // Create a database project instance.
                SkrumManagerService.Project created = new SkrumManagerService.Project();
                created.SprintDuration = project.SprintDuration == null ? 1 : (int)project.SprintDuration;
                created.AlertLimit = project.AlertLimit == null ? 1 : (int)project.AlertLimit;
                created.Speed = project.Speed == null ? 1 : (int)project.Speed;

                // Hash the password if it exists.
                if (project.Password != null)
                {
                    System.Security.Cryptography.SHA512 sha512 = new System.Security.Cryptography.SHA512Managed();
                    System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                    byte[] digest = sha512.ComputeHash(encoder.GetBytes(project.Password));
                    sha512.Dispose();
                    created.Password = encoder.GetString(digest);
                }

                // Saves the project to the database.
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                context.GetTable<SkrumManagerService.Project>().InsertOnSubmit(created);
                context.SubmitChanges();
                context.Dispose();

                // Treat the Project instance.
                project.ProjectID = created.ProjectID;
                project.Password = null;
                return project;
            }
            catch (System.Exception)
            {
                // Returns null if anything goes wrong.
                return null;
            }
        }

        /// <summary>
        /// Deletes a project record.
        /// </summary>
        /// <param name="project">Project to delete</param>
        /// <returns>true if the project was deleted, false otherwise.</returns>
        public bool DeleteProject(int projectID)
        {
            try
            {
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var projects = context.GetTable<SkrumManagerService.Project>();
                var result = projects.FirstOrDefault(p => p.ProjectID == projectID);
                if (result != null)
                {
                    projects.DeleteOnSubmit(result);
                    context.SubmitChanges();
                    context.Dispose();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception)
            {
                // Returns false if any problem occurs.
                return false;
            }
        }

        /// <summary>
        /// Returns a person's information using that person's ID to search.
        /// </summary>
        /// <param name="projectID">Person instance containing the ID to search for</param>
        /// <returns>The filled Person instance if found, null otherwise.</returns>
        public ServiceDataTypes.Project GetProjectByID(int projectID)
        {
            try
            {
                // Creates a database context, searches and then disposes of it.
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var result = context.GetTable<SkrumManagerService.Project>().FirstOrDefault(p => p.ProjectID == projectID);
                context.Dispose();

                // Return null if no result was found, or a the filled person instance.
                if (result == null)
                {
                    return null;
                }
                else
                {
                    ServiceDataTypes.Project project = new ServiceDataTypes.Project();
                    project.ProjectID = projectID;
                    project.Password = result.Password;
                    project.SprintDuration = result.SprintDuration;
                    project.AlertLimit = result.AlertLimit;
                    project.Speed = result.Speed;
                    return project;
                }
            }
            catch (System.Exception)
            {
                // Returns null if any problem occurs.
                return null;
            }
        }

        /// <summary>
        /// Updates a projects information with the given values.
        /// </summary>
        /// <param name="project">Contains the new values</param>
        /// <returns>The projects current information.</returns>
        public ServiceDataTypes.Project UpdateProject(ServiceDataTypes.Project project)
        {
            try
            {
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var projects = context.GetTable<SkrumManagerService.Project>();
                var result = projects.FirstOrDefault(p => p.ProjectID == project.ProjectID);
                if (result == null)
                {
                    // Return null if user no longer exists.
                    return null;
                }
                else
                {

                    if (project.SprintDuration != null)
                    {
                        result.SprintDuration = project.SprintDuration == null ? result.SprintDuration : (int)project.SprintDuration;
                    }
                    if (project.AlertLimit != null)
                    {
                        result.AlertLimit = project.AlertLimit == null ? result.AlertLimit : (int)project.AlertLimit;
                    }
                    if (project.Speed != null)
                    {
                        result.Speed = project.Speed == null ? result.Speed : (int)project.Speed;
                    }
                    if (project.Password != null)
                    {
                        System.Security.Cryptography.SHA512 sha512 = new System.Security.Cryptography.SHA512Managed();
                        System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                        byte[] digest = sha512.ComputeHash(encoder.GetBytes(project.Password));
                        sha512.Dispose();
                        result.Password = encoder.GetString(digest);
                    }

                    // Update the person's information.
                    context.SubmitChanges();
                    context.Dispose();

                    // Update the object data and return.
                    project.SprintDuration = result.SprintDuration;
                    project.AlertLimit = result.AlertLimit;
                    project.Speed = result.Speed;

                    return project;
                }
            }
            catch (System.Exception)
            {
                // Returns null if any problem occurs.
                return null;
            }
        }

        //----------------------------------//

        /// <summary>
        /// Creates a new sprint in the database.
        /// </summary>
        /// <param name="sprint">Contains the information of the sprint to be created.</param>
        /// <returns>Created sprints information</returns>
        public ServiceDataTypes.Sprint CreateSprint(ServiceDataTypes.Sprint sprint)
        {
            try
            {
                // Create a database sprint instance.
                SkrumManagerService.Sprint created = new SkrumManagerService.Sprint();
                created.Number = sprint.Number == null ? 1 : (int)sprint.Number;
                created.BeginDate = sprint.BeginDate;
                created.EndDate = sprint.EndDate;
                created.Closed = sprint.Closed == null ? false : (bool)sprint.Closed;
                created.ProjectID = sprint.ProjectID == null ? 1 : (int)sprint.ProjectID;

                // Saves the sprint to the database.
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                context.GetTable<SkrumManagerService.Sprint>().InsertOnSubmit(created);
                context.SubmitChanges();
                context.Dispose();

                // Treat the Sprint instance.
                sprint.SprintID = created.SprintID;
                return sprint;
            }
            catch (System.Exception)
            {
                // Returns null if anything goes wrong.
                return null;
            }
        }

        /// <summary>
        /// Deletes a sprint record.
        /// </summary>
        /// <param name="sprint">Sprint to delete</param>
        /// <returns>true if the sprint was deleted, false otherwise.</returns>
        public bool DeleteSprint(ServiceDataTypes.Sprint sprint)
        {
            try
            {
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var sprints = context.GetTable<SkrumManagerService.Sprint>();
                var result = sprints.FirstOrDefault(p => p.SprintID == sprint.SprintID);
                if (result != null)
                {
                    sprints.DeleteOnSubmit(result);
                    context.SubmitChanges();
                    context.Dispose();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception)
            {
                // Returns false if any problem occurs.
                return false;
            }
        }

        /// <summary>
        /// Returns a sprints information using that sprints ID to search.
        /// </summary>
        /// <param name="sprintID">Sprint ID to search for</param>
        /// <returns>The filled Sprint instance if found, null otherwise.</returns>
        public ServiceDataTypes.Sprint GetSprintByID(int sprintID)
        {
            try
            {
                // Creates a database context, searches and then disposes of it.
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var result = context.GetTable<SkrumManagerService.Sprint>().FirstOrDefault(p => p.SprintID == sprintID);
                context.Dispose();

                // Return null if no result was found, or a the filled person instance.
                if (result == null)
                {
                    return null;
                }
                else
                {
                    ServiceDataTypes.Sprint sprint = new ServiceDataTypes.Sprint();
                    sprint.SprintID = sprintID;
                    sprint.Number = result.Number;
                    sprint.BeginDate = result.BeginDate;
                    sprint.EndDate = result.EndDate;
                    sprint.Closed = result.Closed;
                    sprint.ProjectID = result.ProjectID;
                    return sprint;
                }
            }
            catch (System.Exception)
            {
                // Returns null if any problem occurs.
                return null;
            }
        }

        /// <summary>
        /// Updates a sprints information with the given values.
        /// </summary>
        /// <param name="sprint">Contains the new values</param>
        /// <returns>The sprint's current information.</returns>
        public ServiceDataTypes.Sprint UpdateSprint(ServiceDataTypes.Sprint sprint)
        {
            try
            {
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var sprints = context.GetTable<SkrumManagerService.Sprint>();
                var result = sprints.FirstOrDefault(p => p.SprintID == sprint.SprintID);
                if (result == null)
                {
                    // Return null if user no longer exists.
                    return null;
                }
                else
                {
                    if (sprint.Number != null)
                    {
                        result.Number = sprint.Number == null ? result.Number : (int)sprint.Number;
                    }
                    if (sprint.BeginDate != null)
                    {
                        result.BeginDate = sprint.BeginDate;
                    }
                    if (sprint.EndDate != null)
                    {
                        result.EndDate = sprint.EndDate;
                    }
                    if (sprint.Closed != null)
                    {
                        result.Closed = (bool)sprint.Closed;
                    }
                    if (sprint.ProjectID != null)
                    {
                        result.ProjectID = sprint.ProjectID == null ? result.ProjectID : (int)sprint.ProjectID;
                    }


                    // Update the sprint information.
                    context.SubmitChanges();
                    context.Dispose();

                    sprint.Number = result.Number;
                    sprint.BeginDate = result.BeginDate;
                    sprint.EndDate = result.EndDate;
                    sprint.Closed = result.Closed;
                    sprint.ProjectID = result.ProjectID;
                    return sprint;
                }
            }
            catch (System.Exception)
            {
                // Returns null if any problem occurs.
                return null;
            }
        }

        /// <summary>
        /// Gives all sprints of a project.
        /// </summary>
        /// <param name="projectID">Contains the projects ID</param>
        /// <returns>A list of the sprints (open or closed) of the project</returns>
        public List<ServiceDataTypes.Sprint> GetSprintsInProject(int projectID)
        {
            try
            {
                List<ServiceDataTypes.Sprint> result = new List<ServiceDataTypes.Sprint>();
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var sprints = context.GetTable<SkrumManagerService.Sprint>();
                var results = from p in sprints
                              where p.ProjectID == projectID
                              select p.SprintID;

                if (results == null || results.Count() == 0)
                    return null;
                else
                {
                    foreach (var id in results)
                    {
                        ServiceDataTypes.Sprint spr = new ServiceDataTypes.Sprint();
                        spr = GetSprintByID(id);
                        if (spr!=null)
                            result.Add(spr);
                    }
                }

                return result;
            }
            catch (System.Exception)
            {
                // Returns null if any problem occurs.
                return null;
            }
        }

        /// <summary>
        /// Gives all closed sprints of a project.
        /// </summary>
        /// <param name="projectID">Contains the projectID of the project</param>
        /// <returns>A list of the closed sprints of a project</returns>
        public List<ServiceDataTypes.Sprint> GetClosedSprints(int projectID)
        {
            try
            {
                List<ServiceDataTypes.Sprint> result = new List<ServiceDataTypes.Sprint>();

                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var sprints = context.GetTable<SkrumManagerService.Sprint>();
                var results = from p in sprints
                              where (p.ProjectID == projectID && p.Closed == true)
                              select p.SprintID;

                if (results == null || results.Count()==0 )
                    return null;
                else
                {
                    foreach (var id in results)
                    {
                        ServiceDataTypes.Sprint spr = new ServiceDataTypes.Sprint();
                        spr = GetSprintByID(id);
                        if (spr != null)
                            result.Add(spr);
                    }
                }

                return result;
            }
            catch (System.Exception)
            {
                // Returns null if any problem occurs.
                return null;
            }
        }

        /// <summary>
        /// Gives a person a new role in a project
        /// </summary>
        /// <param name="role">The role object to be inserted in the database</param>
        /// <returns>The newly created role</returns>
        public ServiceDataTypes.Role GiveRole(ServiceDataTypes.Role role)
        {
            try
            {
                 
        /*private string description;
        private string password;
        private int? personID;
        private int? projectID;
        private ServiceDataTypes.RoleDescription roleDescription;
        private int? roleID;*/
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();

                // Create a database project instance.
                SkrumManagerService.Role created = new SkrumManagerService.Role();
                created.AssignedTime = role.AssignedTime == null ? 1.0 : (double)role.AssignedTime;
                created.PersonID = role.PersonID == null ? 1 : (int)role.PersonID;
                created.ProjectID = role.ProjectID == null ? 1 : (int)role.ProjectID;
                created.RoleDescription = context.GetTable<SkrumManagerService.RoleDescription>().First(r => r.Description == role.RoleDescription.ToString());

                // Hash the password if it exists.
                if (role.Password != null)
                {
                    System.Security.Cryptography.SHA512 sha512 = new System.Security.Cryptography.SHA512Managed();
                    System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                    byte[] digest = sha512.ComputeHash(encoder.GetBytes(role.Password));
                    sha512.Dispose();
                    created.Password = encoder.GetString(digest);
                }

                // Saves the project to the database.
                
                context.GetTable<SkrumManagerService.Role>().InsertOnSubmit(created);
                context.SubmitChanges();
                context.Dispose();

                // Treat the Project instance.
                role.RoleID = created.ProjectID;
                role.Password = null;
                return role;
            }
            catch (System.Exception)
            {
                // Returns null if anything goes wrong.
                return null;
            } 
        }

        /// <summary>
        /// Creates a new meeting in the database.
        /// </summary>
        /// <param name="project">Contains the information of the meeting to be created.</param>
        /// <returns>Created meetings information</returns>
        public ServiceDataTypes.Meeting CreateMeeting(ServiceDataTypes.Meeting meeting)
        {
            try
            {
                // Create a database project instance.
                SkrumManagerService.Meeting created = new SkrumManagerService.Meeting();
                created.Date = meeting.Date;
                created.Notes = meeting.Notes;
                created.Number = meeting.Number == null ? 1 : (int)meeting.Number;
                created.ProjectID = meeting.ProjectID == null ? 1 : (int)meeting.ProjectID;
                             
                // Saves the project to the database.
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                context.GetTable<SkrumManagerService.Meeting>().InsertOnSubmit(created);
                context.SubmitChanges();
                context.Dispose();

                // Treat the Project instance.
                meeting.MeetingID = created.ProjectID;
                return meeting;
            }
            catch (System.Exception)
            {
                // Returns null if anything goes wrong.
                return null;
            }
        }

        /// <summary>
        /// Deletes a meeting record.
        /// </summary>
        /// <param name="meetingID">The ID of the meeting to be deleted</param>
        /// <returns>true if the meeting was deleted, false otherwise.</returns>
        public bool DeleteMeeting(int meetingID)
        {
            try
            {
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var meetings = context.GetTable<SkrumManagerService.Meeting>();
                var result = meetings.FirstOrDefault(p => p.MeetingID == meetingID);
                if (result != null)
                {
                    meetings.DeleteOnSubmit(result);
                    context.SubmitChanges();
                    context.Dispose();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception)
            {
                // Returns false if any problem occurs.
                return false;
            }
        }


        /// <summary>
        /// Returns a meetings information using that meetings ID to search.
        /// </summary>
        /// <param name="sprintID">Meeting ID to search for</param>
        /// <returns>The filled Meeting instance if found, null otherwise.</returns>
        public ServiceDataTypes.Meeting GetMeetingByID(int meetingID)
        {
            try
            {
                // Creates a database context, searches and then disposes of it.
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var result = context.GetTable<SkrumManagerService.Meeting>().FirstOrDefault(p => p.MeetingID == meetingID);
                context.Dispose();

                // Return null if no result was found, or a the filled person instance.
                if (result == null)
                {
                    return null;
                }
                else
                {
                    ServiceDataTypes.Meeting meeting = new ServiceDataTypes.Meeting();
                    meeting.MeetingID = meetingID;
                    meeting.Number = result.Number;
                    meeting.Date = result.Date;
                    meeting.Notes = result.Notes;
                    meeting.ProjectID = result.ProjectID;

                    return meeting;
                }
            }
            catch (System.Exception)
            {
                // Returns null if any problem occurs.
                return null;
            }
        }

        /// <summary>
        /// Updates a meeting information with the given values.
        /// </summary>
        /// <param name="meeting">Contains the new values</param>
        /// <returns>The meetings current information.</returns>
        public ServiceDataTypes.Meeting UpdateMeeting(ServiceDataTypes.Meeting meeting)
        {
            try
            {
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var meetings = context.GetTable<SkrumManagerService.Meeting>();
                var result = meetings.FirstOrDefault(p => p.MeetingID == meeting.MeetingID);
                if (result == null)
                {
                    // Return null if user no longer exists.
                    return null;
                }
                else
                {
                    if (meeting.Number != null)
                    {
                        result.Number = meeting.Number == null ? result.Number : (int)meeting.Number;
                    }
                    if (meeting.Date != null)
                    {
                        result.Date = meeting.Date;
                    }
                    if (meeting.Notes != null)
                    {
                        result.Notes = meeting.Notes;
                    }
                    if (meeting.ProjectID != null)
                    {
                        result.ProjectID = meeting.ProjectID == null ? result.ProjectID : (int)meeting.ProjectID;
                    }


                    // Update the sprint information.
                    context.SubmitChanges();
                    context.Dispose();

                    meeting.Number = result.Number;
                    meeting.Date = result.Date;
                    meeting.Notes = result.Notes;
                    meeting.ProjectID = result.ProjectID;
                    return meeting;
                }
            }
            catch (System.Exception)
            {
                // Returns null if any problem occurs.
                return null;
            }
        }

        /// <summary>
        /// Returns a list of Meetings of a given project.
        /// </summary>
        /// <param name="projectID">Id of the project to search for</param>
        /// <returns>List of occurrences found, if any, null otherwise.</returns>
        public List<ServiceDataTypes.Meeting> GetMeetingsInProject(int projectID)
        {
            try
            {
                List<ServiceDataTypes.Meeting> result = new List<ServiceDataTypes.Meeting>();
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var meetings = context.GetTable<SkrumManagerService.Meeting>();
                var results = from p in meetings
                              where p.ProjectID == projectID
                              select p.MeetingID;

                if (results == null || results.Count() == 0)
                    return null;
                else
                {
                    foreach (var id in results)
                    {
                        ServiceDataTypes.Meeting meet = new ServiceDataTypes.Meeting();
                        meet = GetMeetingByID(id);
                        if (meet != null)
                            result.Add(meet);
                    }
                }

                return result;
            }
            catch (System.Exception)
            {
                // Returns null if any problem occurs.
                return null;
            }
        }

        /// <summary>
        /// Returns a list of Meetings of a given project in a given date.
        /// </summary>
        /// <param name="date">Date to search for</param>
        /// <param name="projectID">Id of the project to search for</param>
        /// <returns>List of occurrences found, if any, null otherwise.</returns>
        public List<ServiceDataTypes.Meeting> GetMeetingsOnDate(System.DateTime date, int projectID)
        {
            try
            {
                List<ServiceDataTypes.Meeting> result = new List<ServiceDataTypes.Meeting>();
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var meetings = context.GetTable<SkrumManagerService.Meeting>();
                var results = from p in meetings
                              where p.ProjectID == projectID && p.Date==date
                              select p.MeetingID;

                if (results == null || results.Count() == 0)
                    return null;
                else
                {
                    foreach (var id in results)
                    {
                        ServiceDataTypes.Meeting meet = new ServiceDataTypes.Meeting();
                        meet = GetMeetingByID(id);
                        if (meet != null)
                            result.Add(meet);
                    }
                }

                return result;
            }
            catch (System.Exception)
            {
                // Returns null if any problem occurs.
                return null;
            }
        }


        public bool DeleteSprint(int sprintID)
        {
            throw new System.NotImplementedException();
        }
    }
}