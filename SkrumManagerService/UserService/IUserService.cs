using ServiceDataTypes;
using System.ServiceModel;

namespace Users
{
    [ServiceContract]
    public interface IUserService
    {
        [OperationContract]
        Person CreatePerson(Person person);

        [OperationContract]
        bool DeletePerson(Person person);

        [OperationContract]
        Person GetPersonByID(Person person);

        [OperationContract]
        Person UpdatePerson(Person person);
    }
}