using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceTester.DataService;
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
        private static DataServiceClient dataClient;
        private static SkrumManagerService.SkrumDataclassesDataContext context;
        private static ServiceHost dataHost;

        /// <summary>
        /// Classwise test teardown operations.
        /// </summary>
        [ClassCleanup]
        public static void CleanupClass()
        {
            // Stop the clients.
            ServiceTest.dataClient.Close();

            // Stop the services.
            ServiceTest.dataHost.Close();

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
            ServiceTest.dataHost = new ServiceHost(typeof(Data.DataService));
            ServiceTest.dataHost.Open();

            // Initialize the clients.
            ServiceTest.dataClient = new DataServiceClient();
            ServiceTest.dataClient.Open();

            // Initialize database connection, clean and seed the database.
            ServiceTest.context = new SkrumManagerService.SkrumDataclassesDataContext();
        }
    }
}