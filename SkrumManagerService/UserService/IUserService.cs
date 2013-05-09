using ServiceDataTypes;
using System.ServiceModel;

namespace Users
{
    [ServiceContract]
    public interface IUserService
    {
        /*
         * Person CRUD
         */

        [OperationContract]
        Person CreatePerson(Person person);

        [OperationContract]
        Person UpdatePerson(Person person);

        [OperationContract]
        Person UpdatePersonPassword(int personID, string password);

        [OperationContract]
        bool DeletePerson(int personID);

        [OperationContract]
        Person GetPersonByID(int personID);

        [OperationContract]
        Person GetPersonByEmail(string email);

        /*
         * Role CRUD
         */

        [OperationContract]
        Role CreateRole(Role role);

        [OperationContract]
        Role UpdateRole(Role role);

        [OperationContract]
        Role UpdateRolePassword(int roleID, string password);

        [OperationContract]
        bool DeleteRole(int roleID);

        [OperationContract]
        Role GetRoleByID(int roleID);

        /*
         * Project-wise operations
         */

        [OperationContract]
        System.Collections.Generic.List<Person> GetAllPeopleInProject(int projectID);

        [OperationContract]
        System.Collections.Generic.List<Role> GetAllRolesInProject(int projectID);

        /*
         * System-wise operations
         */

        [OperationContract]
        System.Collections.Generic.List<Person> GetAllPeople();

        /*
         * Person utilities
         */

        [OperationContract]
        bool LoginAdmin(int personID, string password);

        [OperationContract]
        System.Collections.Generic.List<Task> GetAllTasksInPerson(int personID);

        [OperationContract]
        System.Collections.Generic.List<Role> GetAllRolesInPerson(int personID);

        /*
         * Role utilities
         */

        [OperationContract]
        bool LoginProjectAdmin(int roleID, string password);

        /* 
         * Task utilities
         */

        [OperationContract]
        System.Collections.Generic.List<Person> GetAllPeopleWorkingInTask(int taskID);
    }
}