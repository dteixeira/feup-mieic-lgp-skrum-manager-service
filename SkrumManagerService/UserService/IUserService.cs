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
        bool DeletePerson(int personID);

        [OperationContract]
        Person GetPersonByID(int personID);

        [OperationContract]
        Person UpdatePerson(Person person);

        [OperationContract]
        System.Collections.Generic.List<Person> GetPersonsInProject(int projectID);

        [OperationContract]
        bool LoginAdmin(Person person);

        [OperationContract]
        bool LoginProjectAdmin(Role role);

        [OperationContract]
        System.Collections.Generic.List<Person> GetAllPersons();

        [OperationContract]
        Person GetPersonByEmail(string email);
    }
}