using System.ServiceModel;

namespace Notifications
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(INotificationServiceCallback))]
    public interface INotificationService
    {
        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void Subscribe(int projectID);

        [OperationContract(IsOneWay = false, IsInitiating = false, IsTerminating = true)]
        void Unsubscribe(int projectID);
    }
}