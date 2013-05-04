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
                created.SprintDuration = project.SprintDuration;
                created.AlertLimit = project.AlertLimit;
                created.Speed = project.Speed;

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
        public bool DeleteProject(ServiceDataTypes.Project project)
        {
            try
            {
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var projects = context.GetTable<SkrumManagerService.Project>();
                var result = projects.FirstOrDefault(p => p.ProjectID == project.ProjectID);
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
                        result.SprintDuration = project.SprintDuration;
                    }
                    if (project.AlertLimit != null)
                    {
                        result.AlertLimit = project.AlertLimit;
                    }
                    if (project.Speed != null)
                    {
                        result.Speed = project.Speed;
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
                System.DateTime date_beg = new System.DateTime();
                System.DateTime date_end = new System.DateTime();
                date_beg = System.DateTime.ParseExact(sprint.BeginDate, "yyyy-MM-dd HH:mm tt", null);
                date_end = System.DateTime.ParseExact(sprint.EndDate, "yyyy-MM-dd HH:mm tt", null);

                // Create a database sprint instance.
                SkrumManagerService.Sprint created = new SkrumManagerService.Sprint();
                created.Number = sprint.Number;
                created.BeginDate = date_beg;
                created.EndDate = date_end;
                created.Closed = sprint.Closed;
                created.ProjectID = sprint.ProjectID;

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
                    string date_beg;
                    date_beg = result.BeginDate.ToString("yyyy-MM-dd HH:mm tt");
                    string date_end;
                    date_end = result.EndDate.ToString("yyyy-MM-dd HH:mm tt");

                    ServiceDataTypes.Sprint sprint = new ServiceDataTypes.Sprint();
                    sprint.SprintID = sprintID;
                    sprint.Number = result.Number;
                    sprint.BeginDate = date_beg;
                    sprint.EndDate = date_end;
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
                        result.Number = sprint.Number;
                    }
                    if (sprint.BeginDate != null)
                    {
                        System.DateTime date_beg = new System.DateTime();                        
                        date_beg = System.DateTime.ParseExact(sprint.BeginDate, "yyyy-MM-dd HH:mm tt", null);                        
                        result.BeginDate = date_beg;
                    }
                    if (sprint.EndDate != null)
                    {
                        System.DateTime date_end = new System.DateTime();
                        date_end = System.DateTime.ParseExact(sprint.EndDate, "yyyy-MM-dd HH:mm tt", null);
                        result.EndDate = date_end;
                    }
                    if (sprint.Closed != null)
                    {
                        result.Closed = (bool)sprint.Closed;
                    }
                    //ProjectID não deverá ser alterado

                    // Update the sprint information.
                    context.SubmitChanges();
                    context.Dispose();

                    // Update the object data and return.
                    string date_beg1;
                    date_beg1 = result.BeginDate.ToString("yyyy-MM-dd HH:mm tt");
                    string date_end1;
                    date_end1 = result.EndDate.ToString("yyyy-MM-dd HH:mm tt");

                    sprint.Number = result.Number;
                    sprint.BeginDate = date_beg1;
                    sprint.EndDate = date_end1;
                    sprint.Closed = result.Closed;
                    return sprint;
                }
            }
            catch (System.Exception)
            {
                // Returns null if any problem occurs.
                return null;
            }
        }

        //----------------------------------//

        List<ServiceDataTypes.Person> GetPersonsinProject(ServiceDataTypes.Project project)
        {
            List<ServiceDataTypes.Person> result=new List<ServiceDataTypes.Person>();
            return result;
        }


    }
}