using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

[RunInstaller(true)]
public class ProjectInstaller : Installer
{
    private ServiceProcessInstaller process;
    private ServiceInstaller service;

    /// <summary>
    /// Creates a new service installer instance.
    /// </summary>
    public ProjectInstaller()
    {
        this.process = new ServiceProcessInstaller();
        this.process.Account = ServiceAccount.LocalSystem;
        this.service = new ServiceInstaller();
        this.service.ServiceName = "SkrumManagerWindowsService";
        Installers.Add(this.process);
        Installers.Add(this.service);
    }
}