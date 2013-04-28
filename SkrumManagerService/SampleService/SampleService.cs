using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Samples
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    class SampleService : ISampleService
    {
        public void SampleOperation()
        {
            // Notify all clients of project 0.
            Notifications.NotificationService.Instance.NotifyClients(0);
        }
    }
}
