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
            ServiceHost dataServiceHost = new ServiceHost(typeof(Data.DataService));

            // Runs all the services.
            notificationServiceHost.Open();
            dataServiceHost.Open();

            System.Console.WriteLine("The services are running. Press any key to terminate.");
            System.Console.ReadKey();
            System.Console.WriteLine("Closing...");

            // Close all services.
            notificationServiceHost.Close();
            dataServiceHost.Close();
        }
    }
}