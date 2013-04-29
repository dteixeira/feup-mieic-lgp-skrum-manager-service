using System.ServiceModel;

namespace Taskboards
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class TaskboardService : ITaskboardService
    {
    }
}