using System.ServiceModel;

namespace Notifications
{
    public interface INotificationServiceCallback
    {
        [OperationContract]
        void ProjectChanged();
    }
}