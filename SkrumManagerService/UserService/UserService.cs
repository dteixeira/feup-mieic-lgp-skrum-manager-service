using System.Linq;
using System.ServiceModel;

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
                    created.Password = encoder.GetString(digest);
                }

                // Saves the person to the database.
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                context.GetTable<SkrumManagerService.Person>().InsertOnSubmit(created);
                context.SubmitChanges();
                context.Dispose();

                // Return the person's info.
                return this.GetPersonByID(created.PersonID);
            }
            catch (System.Exception)
            {
                // Returns null if anything goes wrong.
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
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var persons = context.GetTable<SkrumManagerService.Person>();
                var result = persons.FirstOrDefault(p => p.PersonID == personID);
                if (result != null)
                {
                    persons.DeleteOnSubmit(result);
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
        /// Gives all persons involved in a project.
        /// </summary>
        /// <param name="projectID">ID of the project</param>
        /// <returns>A list of the people involved in the project.</returns>
        public System.Collections.Generic.List<ServiceDataTypes.Person> GetPeronsInProject(int projectID)
        {
            try
            {
                System.Collections.Generic.List<ServiceDataTypes.Person> result = new System.Collections.Generic.List<ServiceDataTypes.Person>();
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var roles = context.GetTable<SkrumManagerService.Role>();
                var results = from p in roles
                              where p.ProjectID == projectID
                              select p.PersonID;

                // Return null if nothing was found.
                if (results == null || results.Count() == 0)
                {
                    return null;
                }
                else
                {
                    // Finds all users.
                    foreach (var id in results)
                    {
                        ServiceDataTypes.Person person = GetPersonByID(id);
                        if (person != null)
                            result.Add(person);
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
        /// Returns a person's information using that person's ID to search.
        /// </summary>
        /// <param name="person">Person instance containing the ID to search for</param>
        /// <returns>The filled Person instance if found, null otherwise.</returns>
        public ServiceDataTypes.Person GetPersonByID(int personID)
        {
            try
            {
                // Creates a database context, searches and then disposes of it.
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var result = context.GetTable<SkrumManagerService.Person>().FirstOrDefault(p => p.PersonID == personID);
                context.Dispose();

                // Return null if no result was found, or a the filled person instance.
                if (result == null)
                {
                    return null;
                }
                else
                {
                    ServiceDataTypes.Person person = new ServiceDataTypes.Person();
                    person.Name = result.Name;
                    person.Admin = result.Admin;
                    person.Email = result.Email;
                    person.JobDescription = result.JobDescription;
                    person.PhotoURL = result.PhotoURL;

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

                    // Generate task.
                    person.Tasks = (result.Tasks.Select(p => new ServiceDataTypes.Task
                    {
                        CreationDate = p.CreationDate,
                        Description = p.Description,
                        Estimation = p.Estimation,
                        PersonID = p.PersonID,
                        StoryID = p.StoryID,
                        TaskID = p.TaskID
                    })).ToList();

                    // Returns the person's info.
                    return person;
                }
            }
            catch (System.Exception)
            {
                // Returns null if any problem occurs.
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
                if (person.Admin == false || !(bool)person.Admin || person.PersonID == null || person.Password != null)
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
                    string password = encoder.GetString(digest);

                    // Tries to find the user.
                    SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                    var persons = context.GetTable<SkrumManagerService.Person>();
                    var result = persons.FirstOrDefault(p => p.PersonID == person.PersonID &&
                        p.Admin == (bool)person.Admin &&
                        p.Password == password);
                    context.Dispose();

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
            catch (System.Exception)
            {
                // Returns false if any problem occurs.
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
                    string password = encoder.GetString(digest);

                    // Check if the role exists.
                    SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                    var roles = context.GetTable<SkrumManagerService.Role>();
                    var result = roles.FirstOrDefault(r => r.ProjectID == role.ProjectID &&
                        r.PersonID == role.PersonID &&
                        r.Password == password);
                    context.Dispose();

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
            catch (System.Exception)
            {
                // Returns false if any problem occurs.
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
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
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
                    context.Dispose();

                    // Get and return the person's info.
                    return this.GetPersonByID(result.PersonID);
                }
            }
            catch (System.Exception)
            {
                // Returns null if any problem occurs.
                return null;
            }
        }
    }
}