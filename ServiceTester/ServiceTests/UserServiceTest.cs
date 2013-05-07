using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ServiceTester
{
    /// <summary>
    /// This partial class is used only to implement concret tests.
    /// All classwise setup and teardown should be made on the other
    /// part of the implementation.
    /// </summary>
    public partial class UserServiceTest
    {
        /// <summary>
        /// Tests user creation.
        /// </summary>
        [TestMethod]
        public void CreateUserTest()
        {
            // Create a person representation.
            ServiceDataTypes.Person person = new ServiceDataTypes.Person 
            {
                Admin = false,
                Email = "testable_user@email.domain",
                JobDescription = "This is my job. It makes me happy!",
                Name = "John Doe",
                Password = null,
                PhotoURL = null
            };

            // Add person through service.
            person = UserServiceTest.client.CreatePerson(person);

            // If person is null, something went wrong.
            Assert.IsNotNull(person);

            // Check if a new person id was attributed.
            Assert.AreNotEqual(person.PersonID, -1);
        }

        [TestMethod]
        public void GetPersonByIDTest()
        {
            // Create a person representation.
            ServiceDataTypes.Person person = new ServiceDataTypes.Person
            {
                Admin = false,
                Email = "findable_user@email.domain",
                JobDescription = "Hey, I'm over here.",
                Name = "Dependable Mary",
                Password = null,
                PhotoURL = null
            };

            // Add person through service.
            person = UserServiceTest.client.CreatePerson(person);
            person = UserServiceTest.client.GetPersonByID(person.PersonID == null ? -1 : (int)person.PersonID);

            // If person is null, something went wrong.
            Assert.IsNotNull(person);
        }

        [TestMethod]
        public void DeleteUserTest()
        {
            // Create a person representation.
            ServiceDataTypes.Person person = new ServiceDataTypes.Person
            {
                Admin = false,
                Email = "deletable_user@email.domain",
                JobDescription = "I have no friends...",
                Name = "Forgetable Joe",
                Password = null,
                PhotoURL = null
            };

            // Add person through service.
            person = UserServiceTest.client.CreatePerson(person);
            person = UserServiceTest.client.GetPersonByID(person.PersonID == null ? -1 : (int)person.PersonID);

            // If person is null, something went wrong.
            Assert.IsNotNull(person);

            // Delete person, should succed and return true.
            Assert.IsTrue(UserServiceTest.client.DeletePerson((int)person.PersonID));

            // Tries to find the deleted person, should fail and return null.
            int personID = (int)person.PersonID;
            person = UserServiceTest.client.GetPersonByID((int)person.PersonID);
            Assert.IsNull(person);

            // Tries to delete the person again, should fail and return false.
            Assert.IsFalse(UserServiceTest.client.DeletePerson(personID));
        }

        [TestMethod]
        public void UpdateUserTest()
        {
            // Create a person representation.
            ServiceDataTypes.Person person = new ServiceDataTypes.Person
            {
                Admin = false,
                Email = "updatable_user@email.domain",
                JobDescription = "I'm a master of disguise!",
                Name = "Ninja Tina",
                Password = null,
                PhotoURL = null
            };

            // Add person through service.
            person = UserServiceTest.client.CreatePerson(person);

            // Check initial values.
            Assert.IsNotNull(person);
            Assert.AreEqual(person.Name, "Ninja Tina");
            Assert.AreEqual(person.Email, "updatable_user@email.domain");
            Assert.AreEqual(person.JobDescription, "I'm a master of disguise!");

            // Update the user.
            person.JobDescription = null;
            person.Name = "Ninja Turtle Tina";
            person.Email = "morphable_user@email.domain";
            person.Admin = null;
            person.JobDescription = null;
            person = UserServiceTest.client.UpdatePerson(person);

            // Check modified values.
            Assert.IsNotNull(person);
            Assert.AreNotEqual(person.Name, "Ninja Tina");
            Assert.AreNotEqual(person.Email, "updatable_user@email.domain");
            Assert.AreEqual(person.JobDescription, "I'm a master of disguise!");
        }

        [TestMethod]
        public void GetPersonsInProjectTest()
        {
            // Get a project.
            SkrumManagerService.Project project = UserServiceTest.context.GetTable<SkrumManagerService.Project>().First();

            // List all users for this project.
            ServiceDataTypes.Person[] persons = UserServiceTest.client.GetPersonsInProject(project.ProjectID);
            Assert.IsNotNull(persons);

            // Check if the only user if "Complete User"
            ServiceDataTypes.Person person = persons[0];
            Assert.AreEqual(persons[0].Name, "Complete User");

            // This user should have two roles, be the owner of six tasks, 
            // and be involved in six tasks.
            Assert.AreEqual(person.Roles.Count(), 2);
            Assert.AreEqual(person.Tasks.Count(), 6);
            Assert.AreEqual(person.OwnedTasks.Count(), 6);

            // Verify that one the persons is both a ProjectManager and a
            // TeamMember for this project.
            Assert.IsNotNull(person.Roles.FirstOrDefault(r => r.RoleDescription == ServiceDataTypes.RoleDescription.ProjectManager));
            Assert.IsNotNull(person.Roles.FirstOrDefault(r => r.RoleDescription == ServiceDataTypes.RoleDescription.TeamMember));

            // Verify that the user cannot be logged in as an admin.
            person.Password = "123456";
            Assert.IsFalse(UserServiceTest.client.LoginAdmin(person), "Login successful even though person is not an admin.");
        }

        [TestMethod]
        public void LoginAdminTest()
        {
            // Get the admin.
            SkrumManagerService.Person person = UserServiceTest.context.GetTable<SkrumManagerService.Person>().FirstOrDefault(p => p.Admin);
            Assert.IsNotNull(person, "Could not get the admin through the database.");

            // Get admin date through the service.
            ServiceDataTypes.Person admin = UserServiceTest.client.GetPersonByID(person.PersonID);
            Assert.IsNotNull(admin, "Could not get the admin though the service.");

            // Login the admin.
            admin.Password = "123456";
            Assert.IsTrue(UserServiceTest.client.LoginAdmin(admin), "Admin failed to login.");

            // Try to login with invalid credentials.
            admin.Password = "";
            Assert.IsFalse(UserServiceTest.client.LoginAdmin(admin), "Login successful with invalid credentials.");
        }

        [TestMethod]
        public void LoginProjectAdminTest()
        {
            // Get a project.
            SkrumManagerService.Project project = UserServiceTest.context.GetTable<SkrumManagerService.Project>().First();

            // Get project admin.
            var roleDescription = UserServiceTest.context.GetTable<SkrumManagerService.RoleDescription>();
            SkrumManagerService.RoleDescription role = roleDescription.Where(r => r.Description == ServiceDataTypes.RoleDescription.ProjectManager.ToString()).First();
            SkrumManagerService.Person person = project.Roles.Where(r => r.RoleDescription == role).First().Person;
            Assert.IsNotNull(person, "Failed to get the project manager.");

            // Get project admin info.
            ServiceDataTypes.Person admin = UserServiceTest.client.GetPersonByID(person.PersonID);
            Assert.IsNotNull(admin, "Failed to get the project manager through the service.");

            // Login project manager.
            ServiceDataTypes.Role adminRole = admin.Roles.First();
            adminRole.Password = "654321";
            Assert.IsTrue(UserServiceTest.client.LoginProjectAdmin(adminRole), "Failed to login project manager.");

            // Try to login non project manager.
            adminRole.Password = "";
            Assert.IsFalse(UserServiceTest.client.LoginProjectAdmin(adminRole), "Login project manager incorrectly: no password.");
        }
    }
}