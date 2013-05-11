using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceTester.UserService;
using ServiceTester.ProjectService;
using System.Linq;
using System.ServiceModel;

namespace ServiceTester
{
    /// <summary>
    /// Implements base functionality for user service tests.
    /// All setup and teardown classwise code should be written here.
    /// </summary>
    [TestClass]
    public partial class ServiceTest
    {
        private static UserServiceClient userClient;
        private static ProjectServiceClient projectClient;
        private static SkrumManagerService.SkrumDataclassesDataContext context;
        private static ServiceHost userHost;
        private static ServiceHost projectHost;

        /// <summary>
        /// Classwise test teardown operations.
        /// </summary>
        [ClassCleanup]
        public static void CleanupClass()
        {
            // Stop the clients.
            ServiceTest.userClient.Close();
            ServiceTest.projectClient.Close();

            // Stop the services.
            ServiceTest.userHost.Close();
            ServiceTest.projectHost.Close();

            // Dispose of the database context.
            ServiceTest.context.Dispose();
        }

        /// <summary>
        /// Classwise test setup operations.
        /// </summary>
        /// <param name="context">Test context param</param>
        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            // Initialize the services.
            ServiceTest.userHost = new ServiceHost(typeof(Users.UserService));
            ServiceTest.userHost.Open();
            ServiceTest.projectHost = new ServiceHost(typeof(Projects.ProjectService));
            ServiceTest.projectHost.Open();

            // Initialize the clients.
            ServiceTest.userClient = new UserServiceClient();
            ServiceTest.userClient.Open();
            ServiceTest.projectClient = new ProjectServiceClient();
            ServiceTest.projectClient.Open();

            // Initialize database connection, clean and seed the database.
            ServiceTest.context = new SkrumManagerService.SkrumDataclassesDataContext();
        }
    }
}