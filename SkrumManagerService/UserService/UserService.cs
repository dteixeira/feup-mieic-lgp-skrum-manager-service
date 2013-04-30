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

                // Treat the Person instance.
                person.PersonID = created.PersonID;
                person.Password = null;
                return person;
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
        public bool DeletePerson(ServiceDataTypes.Person person)
        {
            try
            {
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var persons = context.GetTable<SkrumManagerService.Person>();
                var result = persons.FirstOrDefault(p => p.PersonID == person.PersonID);
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
        /// Returns a person's information using that person's ID to search.
        /// </summary>
        /// <param name="person">Person instance containing the ID to search for</param>
        /// <returns>The filled Person instance if found, null otherwise.</returns>
        public ServiceDataTypes.Person GetPersonByID(ServiceDataTypes.Person person)
        {
            try
            {
                // Creates a database context, searches and then disposes of it.
                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();
                var result = context.GetTable<SkrumManagerService.Person>().FirstOrDefault(p => p.PersonID == person.PersonID);
                context.Dispose();

                // Return null if no result was found, or a the filled person instance.
                if (result == null)
                {
                    return null;
                }
                else
                {
                    person.Name = result.Name;
                    person.Admin = result.Admin;
                    person.Email = result.Email;
                    person.JobDescription = result.JobDescription;
                    person.PhotoURL = result.PhotoURL;
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

                    // Update the object data and return.
                    person.Name = result.Name;
                    person.Admin = result.Admin;
                    person.Email = result.Email;
                    person.JobDescription = result.JobDescription;
                    person.PhotoURL = result.PhotoURL;
                    return person;
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