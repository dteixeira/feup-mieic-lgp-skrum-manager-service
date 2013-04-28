using System.ServiceModel;

namespace Samples
{
    [ServiceContract]
    public interface ISampleService
    {
        [OperationContract]
        void SampleOperation();
    }
}