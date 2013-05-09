using System.Linq;
using System.ServiceModel;
using System.ComponentModel;

namespace Users
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class UserService : IUserService
    {
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

        public bool DeletePerson(int personID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var person = context.Persons.FirstOrDefault(p => p.PersonID == personID);
                    context.Persons.DeleteOnSubmit(person);
                    context.SubmitChanges();
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

        public bool DeleteRole(int roleID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var role = context.Roles.FirstOrDefault(r => r.RoleID == roleID);
                    context.Roles.DeleteOnSubmit(role);
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