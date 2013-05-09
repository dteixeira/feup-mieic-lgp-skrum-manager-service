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
        ///// <summary>
        ///// Tests user creation.
        ///// </summary>
        //[TestMethod]
        //public void CreateUserTest()
        //{
        //    // Create a person representation.
        //    ServiceDataTypes.Person person = new ServiceDataTypes.Person
        //    {
        //        Admin = false,
        //        Email = "testable_user@email.domain",
        //        JobDescription = "This is my job. It makes me happy!",
        //        Name = "John Doe",
        //        Password = null,
        //        PhotoURL = null
        //    };

        //    // Add person through service.
        //    person = UserServiceTest.client.CreatePerson(person);

        //    // If person is null, something went wrong.
        //    Assert.IsNotNull(person, "The person was not created correctly.");

        //    // Check if a new person id was attributed.
        //    Assert.IsNotNull(person.PersonID, "The person was not given a database ID, or it was not updated.");

        //    // Test that is not possible to create a user with the same email.
        //    person.PersonID = null;
        //    person = UserServiceTest.client.CreatePerson(person);
        //    Assert.IsNull(person, "The person was created again, despite having the same email");
        //}

        //[TestMethod]
        //public void DeleteUserTest()
        //{
        //    // Create a person representation.
        //    ServiceDataTypes.Person person = new ServiceDataTypes.Person
        //    {
        //        Admin = false,
        //        Email = "deletable_user@email.domain",
        //        JobDescription = "I have no friends...",
        //        Name = "Forgetable Joe",
        //        Password = null,
        //        PhotoURL = null
        //    };

        //    // Add person through service.
        //    person = UserServiceTest.client.CreatePerson(person);
        //    person = UserServiceTest.client.GetPersonByID(person.PersonID == null ? -1 : (int)person.PersonID);

        //    // If person is null, something went wrong.
        //    Assert.IsNotNull(person, "Could not find that person.");

        //    // Delete person, should succed and return true.
        //    Assert.IsTrue(UserServiceTest.client.DeletePerson((int)person.PersonID), "Failed to delete the person.");

        //    // Tries to find the deleted person, should fail and return null.
        //    int personID = (int)person.PersonID;
        //    person = UserServiceTest.client.GetPersonByID((int)person.PersonID);
        //    Assert.IsNull(person, "The person's info is still being returned after being deleted.");

        //    // Tries to delete the person again, should fail and return false.
        //    Assert.IsFalse(UserServiceTest.client.DeletePerson(personID), "Deleted person successfully, even though no longer existed.");
        //}

        //[TestMethod]
        //public void GetAllPersonsTest()
        //{
        //    // Get all people in the database, at least two.
        //    ServiceDataTypes.Person[] persons = UserServiceTest.client.GetAllPersons();
        //    Assert.IsNotNull(persons, "List of people was not returned.");
        //    Assert.IsTrue(persons.Length >= 2, "Method returned list of less than two people.");

        //    // Checks people's names.
        //    Assert.IsNotNull(persons.FirstOrDefault(p => p.Name == "Complete User"), "\"Complete User\" was not returned.");
        //    Assert.IsNotNull(persons.FirstOrDefault(p => p.Name == "Admin"), "\"Admin\" was not returned.");
        //}

        //[TestMethod]
        //public void GetPersonByEmailTest()
        //{
        //    // Get a person by email.
        //    ServiceDataTypes.Person person = UserServiceTest.client.GetPersonByEmail("complete_user@email.domain");
        //    Assert.IsNotNull(person, "The person was not found.");

        //    // Tests case insensitive email matching.
        //    person = UserServiceTest.client.GetPersonByEmail("CoMpLeTe_UsEr@EmAiL.DoMaIn");
        //    Assert.IsNotNull(person, "The person was not found.");

        //    // Forced fail by searching for an inexistence email.
        //    person = UserServiceTest.client.GetPersonByEmail("fictitious_user@email.domain");
        //    Assert.IsNull(person, "The person was found, even though it didn't exist.");
        //}

        //[TestMethod]
        //public void GetPersonByIDTest()
        //{
        //    // Create a person representation.
        //    ServiceDataTypes.Person person = new ServiceDataTypes.Person
        //    {
        //        Admin = false,
        //        Email = "findable_user@email.domain",
        //        JobDescription = "Hey, I'm over here.",
        //        Name = "Dependable Mary",
        //        Password = null,
        //        PhotoURL = null
        //    };

        //    // Add person through service.
        //    person = UserServiceTest.client.CreatePerson(person);
        //    person = UserServiceTest.client.GetPersonByID(person.PersonID == null ? -1 : (int)person.PersonID);

        //    // If person is null, something went wrong.
        //    Assert.IsNotNull(person, "Could not find that person.");
        //}

        //[TestMethod]
        //public void GetPersonsInProjectTest()
        //{
        //    // Get a project.
        //    SkrumManagerService.Project project = UserServiceTest.context.GetTable<SkrumManagerService.Project>().First();

        //    // List all users for this project.
        //    ServiceDataTypes.Person[] persons = UserServiceTest.client.GetPersonsInProject(project.ProjectID);
        //    Assert.IsNotNull(persons, "Could not find users associated with the project.");

        //    // Check if the only user if "Complete User"
        //    ServiceDataTypes.Person person = persons[0];
        //    Assert.AreEqual(persons[0].Name, "Complete User", "Could not find the user.");

        //    // This user should have two roles, be the owner of six tasks,
        //    // and be involved in twelve tasks.
        //    Assert.AreEqual(person.Roles.Count(), 2, "Incorrect number of roles.");
        //    Assert.AreEqual(person.Tasks.Count(), 12, "Incorrect number of associated tasks.");
        //    Assert.AreEqual(person.OwnedTasks.Count(), 6, "Incorrect number of owned tasks.");

        //    // Verify that one the persons is both a ProjectManager and a
        //    // TeamMember for this project.
        //    Assert.IsNotNull(person.Roles.FirstOrDefault(r => r.RoleDescription == ServiceDataTypes.RoleDescription.ProjectManager), "The user is not the manager of this project.");
        //    Assert.IsNotNull(person.Roles.FirstOrDefault(r => r.RoleDescription == ServiceDataTypes.RoleDescription.TeamMember), "The user is not a team member in this project.");

        //    // Verify that the user cannot be logged in as an admin.
        //    person.Password = "123456";
        //    Assert.IsFalse(UserServiceTest.client.LoginAdmin(person), "Login successful even though person is not an admin.");
        //}

        //[TestMethod]
        //public void LoginAdminTest()
        //{
        //    // Get the admin.
        //    SkrumManagerService.Person person = UserServiceTest.context.GetTable<SkrumManagerService.Person>().FirstOrDefault(p => p.Admin);
        //    Assert.IsNotNull(person, "Could not get the admin through the database.");

        //    // Get admin date through the service.
        //    ServiceDataTypes.Person admin = UserServiceTest.client.GetPersonByID(person.PersonID);
        //    Assert.IsNotNull(admin, "Could not get the admin though the service.");

        //    // Login the admin.
        //    admin.Password = "123456";
        //    Assert.IsTrue(UserServiceTest.client.LoginAdmin(admin), "Admin failed to login.");

        //    // Try to login with invalid credentials.
        //    admin.Password = "";
        //    Assert.IsFalse(UserServiceTest.client.LoginAdmin(admin), "Login successful with invalid credentials.");
        //}

        //[TestMethod]
        //public void LoginProjectAdminTest()
        //{
        //    // Get a project.
        //    SkrumManagerService.Project project = UserServiceTest.context.GetTable<SkrumManagerService.Project>().First();

        //    // Get project admin.
        //    var roleDescription = UserServiceTest.context.GetTable<SkrumManagerService.RoleDescription>();
        //    SkrumManagerService.RoleDescription role = roleDescription.Where(r => r.Description == ServiceDataTypes.RoleDescription.ProjectManager.ToString()).First();
        //    SkrumManagerService.Person person = project.Roles.Where(r => r.RoleDescription == role).First().Person;
        //    Assert.IsNotNull(person, "Failed to get the project manager.");

        //    // Get project admin info.
        //    ServiceDataTypes.Person admin = UserServiceTest.client.GetPersonByID(person.PersonID);
        //    Assert.IsNotNull(admin, "Failed to get the project manager through the service.");

        //    // Login project manager.
        //    ServiceDataTypes.Role adminRole = admin.Roles.First();
        //    adminRole.Password = "654321";
        //    Assert.IsTrue(UserServiceTest.client.LoginProjectAdmin(adminRole), "Failed to login project manager.");

        //    // Try to login non project manager.
        //    adminRole.Password = "";
        //    Assert.IsFalse(UserServiceTest.client.LoginProjectAdmin(adminRole), "Login project manager incorrectly: no password.");
        //}

        //[TestMethod]
        //public void UpdateUserTest()
        //{
        //    // Create a person representation.
        //    ServiceDataTypes.Person person = new ServiceDataTypes.Person
        //    {
        //        Admin = false,
        //        Email = "updatable_user@email.domain",
        //        JobDescription = "I'm a master of disguise!",
        //        Name = "Ninja Tina",
        //        Password = null,
        //        PhotoURL = null
        //    };

        //    // Add person through service.
        //    person = UserServiceTest.client.CreatePerson(person);

        //    // Check initial values.
        //    Assert.IsNotNull(person, "Error creating the person.");
        //    Assert.AreEqual(person.Name, "Ninja Tina", "The person's name changed incorrectly.");
        //    Assert.AreEqual(person.Email, "updatable_user@email.domain", "The person's email changed incorrectly.");
        //    Assert.AreEqual(person.JobDescription, "I'm a master of disguise!", "The person's job description changed incorrectly.");

        //    // Update the user.
        //    person.JobDescription = null;
        //    person.Name = "Ninja Turtle Tina";
        //    person.Email = "morphable_user@email.domain";
        //    person.Admin = null;
        //    person.JobDescription = null;
        //    person = UserServiceTest.client.UpdatePerson(person);

        //    // Check modified values.
        //    Assert.IsNotNull(person, "Error updating the person.");
        //    Assert.AreNotEqual(person.Name, "Ninja Tina", "The person's name was not updated.");
        //    Assert.AreNotEqual(person.Email, "updatable_user@email.domain", "The person's email was no updated.");
        //    Assert.AreEqual(person.JobDescription, "I'm a master of disguise!", "The person's job description changed incorrectly.");
        //}

        //[TestMethod]
        //public void GiveRoleTest()
        //{
        //    // Get a person.
        //    ServiceDataTypes.Person person = UserServiceTest.client.GetPersonByEmail("complete_user@email.domain");
        //    var project = UserServiceTest.context.Projects.FirstOrDefault();
        //    Assert.IsNotNull(person, "Person was not found.");
        //    int before = person.Roles.Count();

        //    // Create a new role.
        //    ServiceDataTypes.Role role = new ServiceDataTypes.Role
        //    {
        //        AssignedTime = 0.4,
        //        Password = null,
        //        PersonID = person.PersonID,
        //        RoleDescription = ServiceDataTypes.RoleDescription.ScrumMaster,
        //        ProjectID = project.ProjectID
        //    };
        //    person = UserServiceTest.client.GiveRole(role);
        //    Assert.IsNotNull(person, "Error creating the new role.");
        //    int after = person.Roles.Count();
        //    Assert.AreNotEqual(before, after, "Person as same number of roles after adding a new one.");

        //    // Check if invalid role creation detects an error.
        //    role.ProjectID = null;
        //    role.RoleID = null;
        //    person = UserServiceTest.client.GiveRole(role);
        //    Assert.IsNull(person, "Success was returned creating an invalid role.");
        //}

        //[TestMethod]
        //public void DeleteRoleTest()
        //{
        //    // Get a person and create a new role.
        //    ServiceDataTypes.Person person = UserServiceTest.client.GetPersonByEmail("complete_user@email.domain");
        //    var project = UserServiceTest.context.Projects.FirstOrDefault();
        //    Assert.IsNotNull(person, "Person was not found.");
        //    ServiceDataTypes.Role role = new ServiceDataTypes.Role
        //    {
        //        AssignedTime = 0.4,
        //        Password = null,
        //        PersonID = person.PersonID,
        //        RoleDescription = ServiceDataTypes.RoleDescription.ScrumMaster,
        //        ProjectID = project.ProjectID
        //    };
        //    person = UserServiceTest.client.GiveRole(role);
        //    Assert.IsNotNull(person, "Error creating the new role.");
        //    int before = person.Roles.Count();
        //    role = person.Roles.FirstOrDefault(r => r.RoleDescription == ServiceDataTypes.RoleDescription.ScrumMaster);
        //    Assert.IsNotNull(person, "Error retrieving the new role.");

        //    // Try to delete the role.
        //    Assert.IsTrue(UserServiceTest.client.DeleteRole((int)role.RoleID), "Error deleting the role.");
        //    person = UserServiceTest.client.GetPersonByEmail("complete_user@email.domain");
        //    int after = person.Roles.Count();
        //    Assert.AreNotEqual(before, after, "Number of roles was not modified after removing one.");

        //    // Try to delete role again, should fail.
        //    Assert.IsFalse(UserServiceTest.client.DeleteRole((int)role.RoleID), "Success returned even though the role no longer existed.");
        //}
    }
}