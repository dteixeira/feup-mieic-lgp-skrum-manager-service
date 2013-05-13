using System.ComponentModel;
using System.Data.Linq;
using ServiceDataTypes;

namespace ServiceTester
{
    /// <summary>
    /// This class is used as a collection of utility methods for the
    /// unit tests.
    /// </summary>
    public class TestHelper
    {
        /// <summary>
        /// Deletes all records on the database.
        /// </summary>
        /// <param name="context">Database context object</param>
        public static void ClearDatabase(DataContext context)
        {
            context.ExecuteCommand("DELETE FROM [dbo] . [PersonTask]");
            context.ExecuteCommand("DELETE FROM [dbo] . [StorySprint]");
            context.ExecuteCommand("DELETE FROM [dbo] . [Meeting]");
            context.ExecuteCommand("DELETE FROM [dbo] . [Role]");
            context.ExecuteCommand("DELETE FROM [dbo] . [Task]");
            context.ExecuteCommand("DELETE FROM [dbo] . [Story]");
            context.ExecuteCommand("DELETE FROM [dbo] . [Sprint]");
            context.ExecuteCommand("DELETE FROM [dbo] . [Project]");
            context.ExecuteCommand("DELETE FROM [dbo] . [Person]");
        }

        public static void DumpObject(object dump)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(dump))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(dump);
                System.Console.WriteLine("{0}={1}", name, value);
            }
        }

        public static Person CreateDefaultPerson(ServiceTest tester)
        {
            Person person = new Person()
            {
                Email = "default@email.domain",
                JobDescription = "I'm a default person entity.",
                Name = "Default Person",
                Password = null,
                PhotoURL = "http://default.com"
            };
            return tester.Users.CreatePerson(person);
        }

        public static Project CreateDefaultProject(ServiceTest tester)
        {
            Project project = new Project()
            {
                AlertLimit = 1,
                Name = "Default Project",
                Password = null,
                Speed = 1,
                SprintDuration = 1
            };
            return tester.Projects.CreateProject(project);
        }

        public static Role CreateDefaultRole(ServiceTest tester, Project project, Person person)
        {
            Role role = new Role
            {
                AssignedTime = 1.0,
                Password = null,
                PersonID = person.PersonID,
                ProjectID = project.ProjectID,
                RoleDescription = RoleDescription.TeamMember
            };
            return tester.Users.CreateRole(role);
        }

        public static Story CreateDefaultStory(ServiceTest tester, Project project)
        {
            Story story = new Story
            {
                CreationDate = System.DateTime.Now,
                Description = "Default Story",
                PreviousStory = null,
                ProjectID = project.ProjectID,
                State = StoryState.InProgress,
            };
            return tester.Projects.CreateStory(story);
        }

        public static Task CreateDefaultTask(ServiceTest tester, Story story)
        {
            Task task = new Task
            {
                CreationDate = System.DateTime.Now,
                Description = "Default Task",
                Estimation = 1,
                State = TaskState.Waiting,
                StoryID = story.StoryID
            };
            return tester.Projects.CreateTask(task);
        }

        public static Sprint CreateDefaultSprint(ServiceTest tester, Project project)
        {
            Sprint sprint = new Sprint
            {
                BeginDate = System.DateTime.Now,
                Closed = false,
                EndDate = null,
                Number = 1,
                ProjectID = project.ProjectID
            };
            return tester.Projects.CreateSprint(sprint);
        }

        public static Meeting CreateDefaultMeeting(ServiceTest tester, Project project)
        {
            Meeting meeting = new Meeting
            {
                Date = System.DateTime.Now,
                Notes = "I am a note.",
                Number = 1,
                ProjectID = project.ProjectID
            };
            return tester.Projects.CreateMeeting(meeting);
        }
    }
}