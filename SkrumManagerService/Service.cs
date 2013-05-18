using System.ServiceModel;
using System.ServiceProcess;

public class SkrumManagerWindowsService : ServiceBase
{
    public ServiceHost notificationServiceHost = null;
    public ServiceHost dataServiceHost = null;

    /// <summary>
    /// Creates a new manager service instance.
    /// </summary>
    public SkrumManagerWindowsService()
    {
        // Name the Windows Service.
        this.ServiceName = "SkrumManagerWindowsService";
    }

    /// <summary>
    /// Program entry point
    /// </summary>
    public static void Main()
    {
        ServiceBase.Run(new SkrumManagerWindowsService());
    }

    /// <summary>
    /// This method is called when the service attempts to start.
    /// </summary>
    /// <param name="args">Starting arguments</param>
    protected override void OnStart(string[] args)
    {
        // This behavior should be explored. An exception is raised here
        // but as long as it is caught, the service goes on as expected.
        try
        {
            // Close any service that is still running.
            if (this.notificationServiceHost != null)
            {
                this.notificationServiceHost.Close();
            }
            if (this.dataServiceHost != null)
            {
                this.dataServiceHost.Close();
            }

            // Creates new instances of the services.
            this.notificationServiceHost = new ServiceHost(typeof(Notifications.NotificationService));
            this.notificationServiceHost.Open();
            this.dataServiceHost = new ServiceHost(typeof(Data.DataService));
            this.dataServiceHost.Open();
        }
        catch (System.Exception e)
        {
            System.IO.File.WriteAllLines(@"C:\Users\Administrator\Desktop\log.txt", new string[] { e.Message });
            System.Console.WriteLine(e.Message);
        }
    }

    /// <summary>
    /// This method is called when the service attempts to stop.
    /// </summary>
    protected override void OnStop()
    {
        try
        {
            // Close any service that is still running.
            if (this.notificationServiceHost != null)
            {
                this.notificationServiceHost.Close();
                this.notificationServiceHost = null;
            }
            if (this.dataServiceHost != null)
            {
                this.dataServiceHost.Close();
                this.dataServiceHost = null;
            }
        }
        catch (System.Exception e)
        {
            System.Console.WriteLine(e.Message);
        }
    }
}