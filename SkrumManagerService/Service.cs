using System.ServiceModel;
using System.ServiceProcess;

public class SkrumManagerWindowsService : ServiceBase
{
    public ServiceHost notificationServiceHost = null;
    public ServiceHost sampleServiceHost = null;

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
            // Close the service if it is already running for some reason.
            if (this.notificationServiceHost != null)
            {
                this.notificationServiceHost.Close();
            }

            // Close sample service.
            if (this.sampleServiceHost != null)
            {
                this.sampleServiceHost.Close();
            }

            // Creates and runs the service host instance.
            this.notificationServiceHost = new ServiceHost(typeof(Notifications.NotificationService));
            this.notificationServiceHost.Open();

            // Creates and runs a new sample service host instance.
            this.sampleServiceHost = new ServiceHost(typeof(Samples.SampleService));
            this.sampleServiceHost.Open();
        }
        catch (System.Exception e)
        {
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
            // Close the service if it is still running.
            if (this.notificationServiceHost != null)
            {
                this.notificationServiceHost.Close();
                this.notificationServiceHost = null;
            }

            // Close the sample service if it is still running.
            if (this.sampleServiceHost != null)
            {
                this.sampleServiceHost.Close();
                this.sampleServiceHost = null;
            }
        }
        catch (System.Exception e)
        {
            System.Console.WriteLine(e.Message);
        }
    }
}