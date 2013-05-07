using System.ServiceModel;
using System.Linq;

namespace Taskboards
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class TaskboardService : ITaskboardService
    {
        /// <summary>
        /// Creates a new story in the database.
        /// </summary>
        /// <param name="story">Contains the information of the story to be created.</param>
        /// <returns>Created story's information</returns>
        public ServiceDataTypes.Story CreateStory(ServiceDataTypes.Story story)
        {
            try
            {
                    /*
                    private System.DateTime creationDate;
                    private string description;
                    private int? nextStory;
                    private int? projectID;
                    private ServiceDataTypes.StoryState state;
                    private int? storyID;
                    private System.Collections.Generic.List<Task> tasks;*/

                SkrumManagerService.SkrumDataclassesDataContext context = new SkrumManagerService.SkrumDataclassesDataContext();

                // Create a database person instance.
                SkrumManagerService.Story created = new SkrumManagerService.Story();
                created.Description = story.Description;
                created.NextStory = story.NextStory == null ? 0 : (int)story.NextStory;
                created.ProjectID = story.ProjectID == null ? 1 : (int)story.ProjectID;
                /*!!!!!!!!!!!!!!*/
                //created.State = story.State;
                //created.RoleDescription = context.GetTable<SkrumManagerService.RoleDescription>().First(r => r.Description == role.RoleDescription.ToString());
                
                // Saves the person to the database.
                
                context.GetTable<SkrumManagerService.Story>().InsertOnSubmit(created);
                context.SubmitChanges();
                context.Dispose();

                // Return the person's info.
                return this.GetStoryByID(created.StoryID);
            }
            catch (System.Exception)
            {
                // Returns null if anything goes wrong.
                return null;
            }
        }

        public bool DeleteStory(int storyID)
        {
            throw new System.NotImplementedException();
        }

        public ServiceDataTypes.Story GetStoryByID(int storyID)
        {
            throw new System.NotImplementedException();
        }

        public ServiceDataTypes.Story UpdateStory(ServiceDataTypes.Story person)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Creates a new task in the database.
        /// </summary>
        /// <param name="task">Contains the information of the task to be created.</param>
        /// <returns>Created task's information</returns>
        public ServiceDataTypes.Task CreateTask(ServiceDataTypes.Task task)
        {
            try
            {
                /*// Create a database person instance.
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
                return this.GetPersonByID(created.PersonID);*/
                return null; // REMOVE
            }
            catch (System.Exception)
            {
                // Returns null if anything goes wrong.
                return null;
            }
        }

        public bool DeleteTask(int taskID)
        {
            throw new System.NotImplementedException();
        }

        public ServiceDataTypes.Task GetTaskByID(int taskID)
        {
            throw new System.NotImplementedException();
        }

        public ServiceDataTypes.Task UpdateTask(ServiceDataTypes.Task task)
        {
            throw new System.NotImplementedException();
        }

        public bool InsertWorkTime(int userID, int taskID, double spentTime)
        {
            throw new System.NotImplementedException();
        }
    }
}