using System.Linq;
using System.ServiceModel;
using System.ComponentModel;

namespace Users
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class UserService : IUserService
    {
        /// <summary>
        /// Creates a new person in the database.
        /// </summary>
        /// <param name="person">Contains the information of the person to be created.</param>
        /// <returns>Created persons information</returns>
        public ServiceDataTypes.Person CreatePerson(ServiceDataTypes.Person person)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    SkrumManagerService.Person created = new SkrumManagerService.Person
                    {
                        Email = person.Email,
                        JobDescription = person.JobDescription,
                        Name = person.Name,
                        Password = SkrumManagerService.ServiceHelper.HashPassword(person.Password),
                        PhotoURL = person.PhotoURL
                    };
                    context.Persons.InsertOnSubmit(created);
                    context.SubmitChanges();

                    // Notify clients.
                    Notifications.NotificationService.Instance.NotifyClients(ServiceDataTypes.NotificationType.GlobalPersonModification, -1);
                    return this.GetPersonByID(created.PersonID);
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
        /// Updates a person record on the database.
        /// </summary>
        /// <param name="person">Contains the new data of the person.</param>
        /// <returns>Updated persons information</returns>
        public ServiceDataTypes.Person UpdatePerson(ServiceDataTypes.Person person)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var updated = context.Persons.FirstOrDefault(p => p.PersonID == person.PersonID);
                    updated.Email = person.Email;
                    updated.JobDescription = person.JobDescription;
                    updated.Name = person.Name;
                    updated.PhotoURL = person.PhotoURL;
                    context.SubmitChanges();

                    // Notify clients.
                    Notifications.NotificationService.Instance.NotifyClients(ServiceDataTypes.NotificationType.GlobalPersonModification, -1);
                    return this.GetPersonByID(updated.PersonID);
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
        /// Updates the password of a person in the database.
        /// </summary>
        /// <param name="personID">The ID of the person to be updated</param>
        /// <param name="password">The new password for the person</param>
        /// <returns>Updated persons information</returns>
        public ServiceDataTypes.Person UpdatePersonPassword(int personID, string password)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var person = context.Persons.FirstOrDefault(p => p.PersonID == personID);
                    person.Password = SkrumManagerService.ServiceHelper.HashPassword(password);
                    context.SubmitChanges();
                    return this.GetPersonByID(person.PersonID);
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
        /// Deletes a person in the database.
        /// </summary>
        /// <param name="personID">The ID of the person to be deleted.</param>
        /// <returns>True if the deleting is successful, false otherwise</returns>
        public bool DeletePerson(int personID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var person = context.Persons.FirstOrDefault(p => p.PersonID == personID);
                    context.Persons.DeleteOnSubmit(person);
                    context.SubmitChanges();

                    // Notify clients.
                    Notifications.NotificationService.Instance.NotifyClients(ServiceDataTypes.NotificationType.GlobalPersonModification, -1);
                    return true;
                }
            }
            catch (System.Exception e)
            {
                // Returns null if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Returns the information of a person in the database, searching by its ID.
        /// </summary>
        /// <param name="personID">The Id of the person to be searched.</param>
        /// <returns>The information of the person if found, null otherwise</returns>
        public ServiceDataTypes.Person GetPersonByID(int personID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var person = context.Persons.FirstOrDefault(p => p.PersonID == personID);
                    return new ServiceDataTypes.Person
                    {
                        Email = person.Email,
                        JobDescription = person.JobDescription,
                        Name = person.Name,
                        Password = person.Password == null ? null : "",
                        PersonID = person.PersonID,
                        PhotoURL = person.PhotoURL,
                        Roles = (
                            from r in person.Roles
                            select new ServiceDataTypes.Role
                            {
                                AssignedTime = r.AssignedTime,
                                Password = r.Password == null ? null : "",
                                PersonID = r.PersonID,
                                ProjectID = r.ProjectID,
                                RoleDescription = (ServiceDataTypes.RoleDescription)System.Enum.Parse(typeof(ServiceDataTypes.RoleDescription), r.RoleDescription.Description),
                                RoleID = r.RoleID
                            }
                        ).ToList<ServiceDataTypes.Role>(),
                        Tasks = (
                            from t in person.PersonTasks
                            select new ServiceDataTypes.Task
                            {
                                CreationDate = t.Task.CreationDate,
                                Description = t.Task.Description,
                                Estimation = t.Task.Estimation,
                                StoryID = t.Task.StoryID,
                                TaskID = t.Task.TaskID,
                                State = (ServiceDataTypes.TaskState)System.Enum.Parse(typeof(ServiceDataTypes.TaskState), t.Task.TaskState.State),
                                PersonTasks = (
                                    from pt in t.Task.PersonTasks
                                    select new ServiceDataTypes.PersonTask
                                    {
                                        CreationDate = pt.CreationDate,
                                        PersonID = pt.PersonID,
                                        SpentTime = pt.SpentTime,
                                        TaskID = pt.TaskID
                                    }
                                ).ToList<ServiceDataTypes.PersonTask>()
                            }
                        ).ToList<ServiceDataTypes.Task>()
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
        /// Returns the information of a person in the database, searching by its email.
        /// </summary>
        /// <param name="email">The email address of the person to be searched.</param>
        /// <returns>The information of the person if found, null otherwise</returns>
        public ServiceDataTypes.Person GetPersonByEmail(string email)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var person = context.Persons.FirstOrDefault(p => System.String.Compare(p.Email, email, true) == 0);
                    return this.GetPersonByID(person.PersonID);
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
        /// Creates a new role (Skrum Master, Team Member, Owner...) in the database.
        /// </summary>
        /// <param name="role">Contains the information of the role to be created.</param>
        /// <returns>Created roles information</returns>
        public ServiceDataTypes.Role CreateRole(ServiceDataTypes.Role role)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    SkrumManagerService.Role created = new SkrumManagerService.Role
                    {
                        AssignedTime = role.AssignedTime,
                        Description = context.RoleDescriptions.FirstOrDefault(rd => rd.Description == role.RoleDescription.ToString()).RoleDescriptionID,
                        Password = SkrumManagerService.ServiceHelper.HashPassword(role.Password),
                        PersonID = role.PersonID,
                        ProjectID = role.ProjectID,
                    };
                    context.Roles.InsertOnSubmit(created);
                    context.SubmitChanges();

                    // Notify clients.
                    Notifications.NotificationService.Instance.NotifyClients(ServiceDataTypes.NotificationType.ProjectModification, created.ProjectID);
                    return this.GetRoleByID(created.RoleID);
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
        /// Updates a role record on the database.
        /// </summary>
        /// <param name="role">Contains the new data of the role.</param>
        /// <returns>Updated roles information</returns>
        public ServiceDataTypes.Role UpdateRole(ServiceDataTypes.Role role)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var updated = context.Roles.FirstOrDefault(r => r.RoleID == role.RoleID);
                    updated.AssignedTime = role.AssignedTime;
                    updated.Description = context.RoleDescriptions.FirstOrDefault(rd => rd.Description == role.RoleDescription.ToString()).RoleDescriptionID;
                    context.SubmitChanges();

                    // Notify clients.
                    Notifications.NotificationService.Instance.NotifyClients(ServiceDataTypes.NotificationType.ProjectModification, updated.ProjectID);
                    return this.GetRoleByID(updated.RoleID);
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
        /// Updates the password of a role in the database.
        /// </summary>
        /// <param name="roleID">The ID of the role to be updated</param>
        /// <param name="password">The new password for the role</param>
        /// <returns>Updated roles information</returns>
        public ServiceDataTypes.Role UpdateRolePassword(int roleID, string password)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var role = context.Roles.FirstOrDefault(r => r.RoleID == roleID);
                    role.Password = SkrumManagerService.ServiceHelper.HashPassword(password);
                    context.SubmitChanges();
                    return this.GetRoleByID(role.RoleID);
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
        /// Deletes a role in the database.
        /// </summary>
        /// <param name="roleID">The ID of the role to be deleted.</param>
        /// <returns>True if the deleting is successful, false otherwise</returns>
        public bool DeleteRole(int roleID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var role = context.Roles.FirstOrDefault(r => r.RoleID == roleID);
                    context.Roles.DeleteOnSubmit(role);
                    context.SubmitChanges();

                    // Notify clients.
                    Notifications.NotificationService.Instance.NotifyClients(ServiceDataTypes.NotificationType.ProjectModification, role.ProjectID);
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
        /// Returns the information of a role in the database, searching by its ID.
        /// </summary>
        /// <param name="roleID">The Id of the role to be searched.</param>
        /// <returns>The information of the role if found, null otherwise</returns>
        public ServiceDataTypes.Role GetRoleByID(int roleID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var role = context.Roles.FirstOrDefault(r => r.RoleID == roleID);
                    return new ServiceDataTypes.Role
                    {
                        AssignedTime = role.AssignedTime,
                        Password = role.Password == null ? null : "",
                        PersonID = role.PersonID,
                        ProjectID = role.ProjectID,
                        RoleDescription = (ServiceDataTypes.RoleDescription)System.Enum.Parse(typeof(ServiceDataTypes.RoleDescription), role.RoleDescription.Description),
                        RoleID = role.RoleID
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
        /// Get all the persons involved in a given project
        /// </summary>
        /// <param name="projectID">Contains the ID of the project to be searched.</param>
        /// <returns>A List containing all the persons in the desired project, if found, null otherwise</returns>
        public System.Collections.Generic.List<ServiceDataTypes.Person> GetAllPeopleInProject(int projectID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var project = context.Projects.FirstOrDefault(p => p.ProjectID == projectID);
                    return (
                        from id in (
                            from r in project.Roles
                            select r.PersonID
                        ).Distinct().AsEnumerable()
                        let person = this.GetPersonByID(id)
                        where person != null
                        select this.GetPersonByID(id)
                    ).ToList<ServiceDataTypes.Person>();
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
        /// Get all the roles in a given project
        /// </summary>
        /// <param name="projectID">Contains the ID of the project to be searched.</param>
        /// <returns>A List containing all the roles in the desired project, if found, null otherwise</returns>
        public System.Collections.Generic.List<ServiceDataTypes.Role> GetAllRolesInProject(int projectID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var project = context.Projects.FirstOrDefault(p => p.ProjectID == projectID);
                    return (
                        from r in project.Roles.AsEnumerable()
                        let role = this.GetRoleByID(r.RoleID)
                        where role != null
                        select role
                    ).ToList<ServiceDataTypes.Role>();
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
        /// Returns all the people in the system
        /// </summary>
        /// <returns>A List with the information about every person, if any. Null otherwise</returns>
        public System.Collections.Generic.List<ServiceDataTypes.Person> GetAllPeople()
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    return (
                        from p in context.Persons.AsEnumerable()
                        let person = this.GetPersonByID(p.PersonID)
                        where person != null
                        select person
                    ).ToList<ServiceDataTypes.Person>();
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
        /// Verifies if a given password is the real password of a given user
        /// </summary>
        /// <param name="personID">The ID of the desired user</param>
        /// <param name="password"> Contains the password sent by the client</param>
        /// <returns>True if the password is the correct one, false otherwise</returns>
        public bool LoginAdmin(int personID, string password)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var person = context.Persons.FirstOrDefault(p => p.PersonID == personID);
                    return person.Password == SkrumManagerService.ServiceHelper.HashPassword(password) && person.Password != null;
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
        /// Get all the tasks where a person is involved
        /// </summary>
        /// <param name="personID">Contains the ID of the person to be searched.</param>
        /// <returns>A List containing all the tasks in which the person is involved, if found, null otherwise</returns>
        public System.Collections.Generic.List<ServiceDataTypes.Task> GetAllTasksInPerson(int personID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    return this.GetPersonByID(personID).Tasks;
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
        /// Get all the roles associated to a single person
        /// </summary>
        /// <param name="personID">Contains the ID of the person to be searched.</param>
        /// <returns>A List containing all the roles of the desired person, if found, null otherwise</returns>
        public System.Collections.Generic.List<ServiceDataTypes.Role> GetAllRolesInPerson(int personID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    return this.GetPersonByID(personID).Roles;
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
        /// Verifies if a given password is the password of the admin of a project / of a given role
        /// </summary>
        /// <param name="roleID">The ID of the desired role</param>
        /// <param name="password"> Contains the password sent by the client</param>
        /// <returns>True if the password is the correct one, false otherwise</returns>
        public bool LoginProjectAdmin(int roleID, string password)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var role = context.Roles.FirstOrDefault(r => r.RoleID == roleID);
                    return role.Password == SkrumManagerService.ServiceHelper.HashPassword(password) && role.Password != null;
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
        /// Get all the persons involved in a given task
        /// </summary>
        /// <param name="taskID">Contains the ID of the task to be searched.</param>
        /// <returns>A List containing all the persons associated to the task, if found, null otherwise</returns>
        public System.Collections.Generic.List<ServiceDataTypes.Person> GetAllPeopleWorkingInTask(int taskID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var task = context.Tasks.FirstOrDefault(t => t.TaskID == taskID);
                    return (
                       from pt in task.PersonTasks.AsEnumerable()
                       let person = this.GetPersonByID(pt.PersonID)
                       where person != null
                       select person
                    ).ToList<ServiceDataTypes.Person>();
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