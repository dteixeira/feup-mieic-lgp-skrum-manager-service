using System.ServiceModel;

namespace Projects
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ProjectService : IProjectService
    {
    }
}