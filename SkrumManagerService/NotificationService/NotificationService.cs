using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Notifications
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class NotificationService : INotificationService
    {
        private static NotificationService instance = null;
        private Dictionary<int, List<OperationContext>> clients = null;

        /// <summary>
        /// Instantiates a new NotificationService instance.
        /// </summary>
        public NotificationService()
        {
            // Instantiates the singleton object.
            if (NotificationService.instance == null)
                NotificationService.instance = this;

            // Creates a new client list;
            this.clients = new Dictionary<int, List<OperationContext>>();
        }

        /// <summary>
        /// Returns the singleton instance of NotificationService.
        /// </summary>
        public static NotificationService Instance
        {
            get
            {
                if (NotificationService.instance == null)
                {
                    NotificationService.instance = new NotificationService();
                }
                return NotificationService.instance;
            }
            set { NotificationService.instance = value; }
        }

        /// <summary>
        /// Returns the registered client list.
        /// </summary>
        public Dictionary<int, List<OperationContext>> Clients
        {
            get { return this.clients; }
            set { this.clients = value; }
        }

        /// <summary>
        /// Notifies all clients registered for a certain project that some
        /// changes occurred.
        /// </summary>
        /// <param name="projectID">ID of the project in which the change occurred</param>
        public void NotifyClients(ServiceDataTypes.NotificationType type, int projectID)
        {
            lock (this.clients)
            {
                // Handles project modification notifications, only to specific clients.
                if (type == ServiceDataTypes.NotificationType.ProjectModification)
                {
                    if (this.clients.ContainsKey(projectID))
                    {
                        foreach (OperationContext callback in this.clients[projectID])
                        {
                            try
                            {
                                callback.GetCallbackChannel<INotificationServiceCallback>().DataChanged(type);
                            }
                            catch (System.Exception)
                            {
                                this.clients[projectID].Remove(callback);
                            }
                        }
                    }
                }
                // Handles environment-wise notifications, to every registered client. 
                else
                {
                    foreach (int key in this.clients.Keys)
                    {
                        foreach (OperationContext callback in this.clients[key])
                        {
                            try
                            {
                                callback.GetCallbackChannel<INotificationServiceCallback>().DataChanged(type);
                            }
                            catch (System.Exception)
                            {
                                this.clients[key].Remove(callback);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Subscribes a client to receive notifications about a
        /// specific project's notification.
        /// </summary>
        /// <param name="projectID">ID of the project to be notified of</param>
        public void Subscribe(int projectID)
        {
            lock (this.clients)
            {
                OperationContext context = OperationContext.Current;
                if (!this.clients.ContainsKey(projectID))
                {
                    this.clients[projectID] = new List<OperationContext>();
                }
                List<OperationContext> notified = this.clients[projectID];
                notified.Add(context);
            }
        }

        /// <summary>
        /// Unsubscribes a client's callback notifications.
        /// </summary>
        public void Unsubscribe(int projectID)
        {
            var context = OperationContext.Current;
            lock (this.clients)
            {
                if (this.clients.ContainsKey(projectID))
                {
                    var client = this.clients[projectID].FirstOrDefault(c => c.SessionId == context.SessionId);
                    this.clients[projectID].Remove(client);
                }
            }
        }
    }
}