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
        /// <param name="person">Contains the information of the user to be created.</param>
        /// <returns>Created person's information</returns>
        public ServiceDataTypes.Person CreatePerson(ServiceDataTypes.Person person)
        {
            try
            {
                // Create a database person instance.
                SkrumManagerService.Person created = new SkrumManagerService.Person();
                created.Name = person.Name;
                created.Email = person.Email;
                created.JobDescription = person.JobDescription;
                created.PhotoURL = person.PhotoURL;
                created.Admin = person.Admin == null ? false : (bool)person.Admin;

                // Hash the password if it exists.
                if (person.Password != null)
                {
                    System.Security.Cryptography.SHA512 sha512 = new System.Security.Cryptography.SHA512Managed();
                    System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                    byte[] digest = sha512.ComputeHash(encoder.GetBytes(person.Password));
                    sha512.Dispose();
                    string password = System.BitConverter.ToString(digest);
                    password = password.Replace("-", "");
                    created.Password = password;
                }

                // Saves the person to the database.
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    context.GetTable<SkrumManagerService.Person>().InsertOnSubmit(created);
                    context.SubmitChanges();
                }

                // Return the person's info.
                return this.GetPersonByID(created.PersonID);
            }
            catch (System.Exception e)
            {
                // Returns null if anything goes wrong.
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Deletes someones record.
        /// </summary>
        /// <param name="person">Person to delete</param>
        /// <returns>true if the person was deleted, false otherwise.</returns>
        public bool DeletePerson(int personID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var persons = context.GetTable<SkrumManagerService.Person>();
                    var result = persons.FirstOrDefault(p => p.PersonID == personID);
                    if (result != null)
                    {
                        persons.DeleteOnSubmit(result);
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
        /// Gives all persons involved in a project.
        /// </summary>
        /// <param name="projectID">ID of the project</param>
        /// <returns>A list of the people involved in the project.</returns>
        public System.Collections.Generic.List<ServiceDataTypes.Person> GetPersonsInProject(int projectID)
        {
            try
            {
                System.Collections.Generic.List<ServiceDataTypes.Person> result = new System.Collections.Generic.List<ServiceDataTypes.Person>();
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    SkrumManagerService.Project project = context.GetTable<SkrumManagerService.Project>().FirstOrDefault(p => p.ProjectID == projectID);
                    if (project == null)
                    {
                        // Return null if the project no longer exists.
                        return null;
                    }
                    else
                    {
                        // Gets all the people in the project.
                        var results = project.Roles.Select(r => r.Person.PersonID).Distinct();
                        foreach (int personID in results)
                        {
                            ServiceDataTypes.Person person = this.GetPersonByID(personID);
                            if(person != null)
                            {
                                result.Add(person);
                            }
                        }
                    }
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
        /// Returns a person's information using that person's ID to search.
        /// </summary>
        /// <param name="person">Person instance containing the ID to search for</param>
        /// <returns>The filled Person instance if found, null otherwise.</returns>
        public ServiceDataTypes.Person GetPersonByID(int personID)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var result = context.GetTable<SkrumManagerService.Person>().FirstOrDefault(p => p.PersonID == personID);

                    // Return null if no result was found, or a the filled person instance.
                    if (result == null)
                    {
                        return null;
                    }
                    else
                    {
                        ServiceDataTypes.Person person = new ServiceDataTypes.Person()
                        {
                            PersonID = personID,
                            Name = result.Name,
                            Admin = result.Admin,
                            Email = result.Email,
                            JobDescription = result.JobDescription,
                            PhotoURL = result.PhotoURL
                        };

                        // Generate roles.
                        person.Roles = (result.Roles.Select(p => new ServiceDataTypes.Role
                        {
                            AssignedTime = p.AssignedTime,
                            Password = p.Password,
                            PersonID = p.PersonID,
                            ProjectID = p.ProjectID,
                            RoleDescription = (ServiceDataTypes.RoleDescription)System.Enum.Parse(typeof(ServiceDataTypes.RoleDescription), p.RoleDescription.Description),
                            RoleID = p.RoleID
                        })).ToList();

                        // Generate owned tasks.
                        person.OwnedTasks = (result.Tasks.Select(p => new ServiceDataTypes.Task
                        {
                            CreationDate = p.CreationDate,
                            Description = p.Description,
                            Estimation = p.Estimation,
                            PersonID = p.PersonID,
                            StoryID = p.StoryID,
                            TaskID = p.TaskID
                        })).ToList();

                        // Generate associated tasks.
                        var personTasks = context.GetTable<SkrumManagerService.PersonTask>();
                        person.Tasks = personTasks.Where(t => t.PersonID == person.PersonID).Select(p => new ServiceDataTypes.Task
                        {
                            CreationDate = p.Task.CreationDate,
                            Description = p.Task.Description,
                            Estimation = p.Task.Estimation,
                            PersonID = p.Task.PersonID,
                            StoryID = p.Task.StoryID,
                            TaskID = p.Task.TaskID
                        }).ToList();

                        // Returns the person's info.
                        return person;
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
        /// Logs a given person as an admin.
        /// </summary>
        /// <param name="person">Person to login</param>
        /// <returns>true if login is valid, false otherwise.</returns>
        public bool LoginAdmin(ServiceDataTypes.Person person)
        {
            try
            {
                if (person.Admin == null || !(bool)person.Admin || person.PersonID == null || person.Password == null)
                {
                    // Return false if information is missing.
                    return false;
                }
                else
                {
                    // Hash the password.
                    System.Security.Cryptography.SHA512 sha512 = new System.Security.Cryptography.SHA512Managed();
                    System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                    byte[] digest = sha512.ComputeHash(encoder.GetBytes(person.Password));
                    sha512.Dispose();
                    string password = System.BitConverter.ToString(digest);
                    password = password.Replace("-", "");

                    using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                    {
                        // Tries to find the user.
                        var persons = context.GetTable<SkrumManagerService.Person>();

                        System.Console.WriteLine("{0}\n{1}", persons.FirstOrDefault(p => p.Admin).Password, password);

                        var result = persons.FirstOrDefault(p => p.PersonID == person.PersonID &&
                            p.Admin == (bool)person.Admin &&
                            p.Password == password);

                        if (result == null)
                        {
                            // Return false if no user if found.
                            return false;
                        }
                        else
                        {
                            // Return true if login is valid.
                            return true;
                        }
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

        public bool LoginProjectAdmin(ServiceDataTypes.Role role)
        {
            try
            {
                if (role.ProjectID == null || role.PersonID == null || role.Password == null)
                {
                    // Return false if information is missing.
                    return false;
                }
                else
                {
                    // Hash the password.
                    System.Security.Cryptography.SHA512 sha512 = new System.Security.Cryptography.SHA512Managed();
                    System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                    byte[] digest = sha512.ComputeHash(encoder.GetBytes(role.Password));
                    sha512.Dispose();
                    string password = System.BitConverter.ToString(digest);
                    password = password.Replace("-", "");

                    // Check if the role exists.
                    using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                    {
                        var roles = context.GetTable<SkrumManagerService.Role>();
                        var result = roles.FirstOrDefault(r => r.ProjectID == role.ProjectID &&
                            r.PersonID == role.PersonID &&
                            r.Password == password);

                        if (result == null)
                        {
                            // Return false if user no longer exists.
                            return false;
                        }
                        else
                        {
                            // Return true if the role exists.
                            return true;
                        }
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
        /// Updates a person's information with the given values.
        /// </summary>
        /// <param name="person">Contains the new values</param>
        /// <returns>The person's current information.</returns>
        public ServiceDataTypes.Person UpdatePerson(ServiceDataTypes.Person person)
        {
            try
            {
                using (SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext())
                {
                    var persons = context.GetTable<SkrumManagerService.Person>();
                    var result = persons.FirstOrDefault(p => p.PersonID == person.PersonID);
                    if (result == null)
                    {
                        // Return null if user no longer exists.
                        return null;
                    }
                    else
                    {
                        if (person.Name != null)
                        {
                            result.Name = person.Name;
                        }
                        if (person.Email != null)
                        {
                            result.Email = person.Email;
                        }
                        if (person.PhotoURL != null)
                        {
                            result.PhotoURL = person.PhotoURL;
                        }
                        if (person.JobDescription != null)
                        {
                            result.JobDescription = person.JobDescription;
                        }
                        if (person.Admin != null)
                        {
                            result.Admin = (bool)person.Admin;
                        }
                        if (result.Admin && person.Password != null)
                        {
                            System.Security.Cryptography.SHA512 sha512 = new System.Security.Cryptography.SHA512Managed();
                            System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                            byte[] digest = sha512.ComputeHash(encoder.GetBytes(person.Password));
                            sha512.Dispose();
                            result.Password = encoder.GetString(digest);
                        }

                        // Update the person's information.
                        context.SubmitChanges();

                        // Get and return the person's info.
                        return this.GetPersonByID(result.PersonID);
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