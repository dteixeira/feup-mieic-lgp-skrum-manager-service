using System.ServiceModel;

namespace Notifications
{
    public interface INotificationServiceCallback
    {
        [OperationContract]
        void DataChanged(ServiceDataTypes.NotificationType notification);
    }
}