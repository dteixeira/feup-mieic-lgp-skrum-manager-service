using System.ServiceModel;

namespace ServiceDebugger
{
    public class Program
    {
        /// <summary>
        /// Runs the service debugger program.
        /// </summary>
        /// <param name="args">Command line arguments no used</param>
        private static void Main(string[] args)
        {
            // Instantiates hosts for all needed services.
            ServiceHost notificationServiceHost = new ServiceHost(typeof(Notifications.NotificationService));
            ServiceHost projectServiceHost = new ServiceHost(typeof(Projects.ProjectService));
            ServiceHost userServiceHost = new ServiceHost(typeof(Users.UserService));

            // Runs all the services.
            notificationServiceHost.Open();
            projectServiceHost.Open();
            userServiceHost.Open();

            System.Console.WriteLine("The services are running. Press any key to terminate.");
            System.Console.ReadKey();
            System.Console.WriteLine("Closing...");

            // Close all services.
            notificationServiceHost.Close();
            projectServiceHost.Close();
            userServiceHost.Close();
        }
    }
}