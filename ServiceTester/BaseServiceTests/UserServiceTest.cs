﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceTester.UserService;
using System.Linq;
using System.ServiceModel;

namespace ServiceTester
{
    /// <summary>
    /// Implements base functionality for user service tests.
    /// All setup and teardown classwise code should be written here.
    /// </summary>
    [TestClass]
    public partial class UserServiceTest
    {
        private static UserServiceClient client;
        private static SkrumManagerService.SkrumDataclassesDataContext context;
        private static ServiceHost host;

        /// <summary>
        /// Classwise test teardown operations.
        /// </summary>
        [ClassCleanup]
        public static void CleanupClass()
        {
            // Stop the client.
            UserServiceTest.client.Close();

            // Stop the service.
            UserServiceTest.host.Close();

            // Dispose of the database context.
            UserServiceTest.context.Dispose();
        }

        /// <summary>
        /// Classwise test setup operations.
        /// </summary>
        /// <param name="context">Test context param</param>
        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            // Initialize the service.
            UserServiceTest.host = new ServiceHost(typeof(Users.UserService));
            UserServiceTest.host.Open();

            // Initialize the client.
            UserServiceTest.client = new UserServiceClient();
            UserServiceTest.client.Open();

            // Initialize database connection, clean and seed the database.
            UserServiceTest.context = new SkrumManagerService.SkrumDataclassesDataContext();
            TestHelper.ClearDatabase(UserServiceTest.context);
            UserServiceTest.SeedDatabase();
        }

        /// <summary>
        /// Any seeding data should be added here.
        /// </summary>
        private static void SeedDatabase()
        {
            // Get all needed tables.
            var persons = UserServiceTest.context.GetTable<SkrumManagerService.Person>();
            var projects = UserServiceTest.context.GetTable<SkrumManagerService.Project>();
            var roles = UserServiceTest.context.GetTable<SkrumManagerService.Role>();
            var tasks = UserServiceTest.context.GetTable<SkrumManagerService.Task>();
            var storyStates = UserServiceTest.context.GetTable<SkrumManagerService.StoryState>();
            var roleDescriptions = UserServiceTest.context.GetTable<SkrumManagerService.RoleDescription>();

            // Create new system admin.
            SkrumManagerService.Person admin = new SkrumManagerService.Person
            {
                Admin = true,
                Email = "admin@email.domain",
                JobDescription = "System Admin",
                Name = "Admin",
                Password = "ba3253876aed6bc22d4a6ff53d8406c6ad864195ed144ab5c87621b6c233b548baeae6956df346ec8c17f5ea10f35ee3cbc514797ed7ddd3145464e2a0bab413",
                PhotoURL = null
            };
            persons.InsertOnSubmit(admin);

            // Create simple incomplete user.
            SkrumManagerService.Person incompletePerson = new SkrumManagerService.Person
            {
                Admin = false,
                Email = "incomplete_user@email.domain",
                JobDescription = "Bad employee, bad",
                Name = "Incomplete User",
                Password = null,
                PhotoURL = null
            };
            persons.InsertOnSubmit(incompletePerson);

            // Create new default user with tasks and roles.
            SkrumManagerService.Person completePerson = new SkrumManagerService.Person
            {
                Admin = false,
                Email = "complete_user@email.domain",
                JobDescription = "Being a good employee.",
                Name = "Complete User",
                Password = null,
                PhotoURL = null
            };
            persons.InsertOnSubmit(completePerson);

            // Create a new project.
            SkrumManagerService.Project project = new SkrumManagerService.Project
            {
                AlertLimit = 1,
                Speed = 1,
                SprintDuration = 1,
                Password = null,
                Name = "Project 1"
            };

            // Create two new stories for the previous project.
            SkrumManagerService.StoryState state = storyStates.FirstOrDefault(s => s.State == ServiceDataTypes.StoryState.InProgress.ToString());
            SkrumManagerService.Story story1 = new SkrumManagerService.Story
            {
                CreationDate = System.DateTime.Now,
                Description = "This is US01",
                Project = project,
                StoryState = state,
            };
            SkrumManagerService.Story story2 = new SkrumManagerService.Story
            {
                CreationDate = System.DateTime.Now,
                Description = "This is US02",
                Project = project,
                StoryState = state,
            };
            story2.Stories.Add(story1);

            // Create some tasks for each story.
            foreach (SkrumManagerService.Story story in project.Stories)
            {
                for (int i = 0; i < 3; ++i)
                {
                    // Create tasks owned and contributed to by the user.
                    SkrumManagerService.Task task1 = new SkrumManagerService.Task
                    {
                        CreationDate = System.DateTime.Now,
                        Description = "This is task 00" + i,
                        Estimation = new System.Random().Next(1, 10),
                        Person = completePerson,
                        Story = story
                    };
                    task1.PersonTasks.Add(new SkrumManagerService.PersonTask
                    {
                        CreationDate = System.DateTime.Now,
                        Person = completePerson,
                        SpentTime = new System.Random().Next(1, 10)
                    });

                    // Create tasks that the user does not own.
                    SkrumManagerService.Task task2 = new SkrumManagerService.Task
                    {
                        CreationDate = System.DateTime.Now,
                        Description = "This is task 00" + i,
                        Estimation = new System.Random().Next(1, 10),
                        Person = incompletePerson,
                        Story = story
                    };
                    task2.PersonTasks.Add(new SkrumManagerService.PersonTask
                    {
                        CreationDate = System.DateTime.Now,
                        Person = completePerson,
                        SpentTime = new System.Random().Next(1, 10)
                    });
                }
            }

            // Create two roles for the complete person.
            SkrumManagerService.RoleDescription teamMember = roleDescriptions.FirstOrDefault(r => r.Description == ServiceDataTypes.RoleDescription.TeamMember.ToString());
            SkrumManagerService.RoleDescription projectManager = roleDescriptions.FirstOrDefault(r => r.Description == ServiceDataTypes.RoleDescription.ProjectManager.ToString());
            SkrumManagerService.Role role1 = new SkrumManagerService.Role
            {
                AssignedTime = 0.4,
                Password = null,
                Person = completePerson,
                Project = project,
                RoleDescription = teamMember
            };
            SkrumManagerService.Role role2 = new SkrumManagerService.Role
            {
                AssignedTime = 0.6,
                Person = completePerson,
                Project = project,
                RoleDescription = projectManager,
                Password = "690437692d902cfd23005bda16631d83644899e78dc0a489da6dca3cb9f9c0cdcd9dd533bc59102dc90155223df777672328c9149354de239f48c58f0a1d44a6"
            };

            // Commit all changes.
            UserServiceTest.context.SubmitChanges();
        }
    }
}