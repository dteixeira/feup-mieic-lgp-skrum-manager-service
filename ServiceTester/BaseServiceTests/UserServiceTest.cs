using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceTester.UserService;
using System.Data.Linq;
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
        private static DataContext context;
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
        }
    }
}