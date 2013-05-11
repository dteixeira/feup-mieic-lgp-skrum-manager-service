using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using ServiceDataTypes;

namespace ServiceTester
{
    /// <summary>
    /// This partial class is used only to implement concret tests.
    /// All classwise setup and teardown should be made on the other
    /// part of the implementation.
    /// </summary>
    public partial class ServiceTest
    {
        private UserService.UserServiceClient users;
        private ProjectService.ProjectServiceClient projects;

        public UserService.UserServiceClient Users
        {
            get { return this.users; }
        }

        public ProjectService.ProjectServiceClient Projects
        {
            get { return this.projects; }
        }

        [TestInitialize]
        public void InitializeTest()
        {
            // Reset the database.
            TestHelper.ClearDatabase(ServiceTest.context);

            // Create shortcuts for services for easier usage.
            this.users = ServiceTest.userClient;
            this.projects = ServiceTest.projectClient;
        }

        [TestMethod]
        public void CreatePersonTest()
        {
            // Create a valid person, should pass.
            Person validPerson = new Person
            {
                Email = "valid@email.domain",
                JobDescription = null,
                Name = "Valid Person",
                Password = null,
                PhotoURL = null
            };
            validPerson = this.users.CreatePerson(validPerson);
            Assert.IsNotNull(validPerson, "Failed to create a person.");

            // Create person with same email, should fail.
            Person sameEmailPerson = new Person
            {
                Email = "valid@email.domain",
                JobDescription = null,
                Name = "Same Email Person",
                Password = null,
                PhotoURL = null
            };
            sameEmailPerson = this.users.CreatePerson(sameEmailPerson);
            Assert.IsNull(sameEmailPerson, "Created a person with an already existing email.");

            // Create person with invalid fields, should fail.
            Person invalidPerson = new Person
            {
                Email = "invalid@email.domain",
                JobDescription = null,
                Name = null,
                Password = null,
                PhotoURL = null
            };
            invalidPerson = this.users.CreatePerson(invalidPerson);
            Assert.IsNull(invalidPerson, "Created a person with invalid fields.");

            // Create admin, should pass.
            Person validAdmin = new Person
            {
                Email = "valid_admin@email.domain",
                JobDescription = "I'm an admin.",
                Name = "Valid Admin",
                Password = "123456",
                PhotoURL = null
            };
            validAdmin = this.users.CreatePerson(validAdmin);
            Assert.IsNotNull(validAdmin, "Failed to create an admin.");
        }

        [TestMethod]
        public void UpdatePersonTest()
        {
            // Create a new person, update name, should pass.
            Person updatablePerson = new Person
            {
                Email = "updatable@email.domain",
                JobDescription = "Not Null",
                Name = "Updatable Person",
                Password = null,
                PhotoURL = "Not Null Too"
            };
            updatablePerson = this.users.CreatePerson(updatablePerson);
            Assert.AreEqual(updatablePerson.Name, "Updatable Person", "Returned person name was not the intended one.");
            updatablePerson.Name = "Updated Person";
            updatablePerson = this.users.UpdatePerson(updatablePerson);
            Assert.IsNotNull(updatablePerson, "Failed to update the person's info.");
            Assert.AreEqual(updatablePerson.Name, "Updated Person", "Returned person name was not the intended one.");

            // Do an invalid update, should fail.
            Person invalidPerson = new Person
            {
                Email = "invalid@email.domain",
                JobDescription = "Not Null",
                Name = "Invalid Person",
                Password = null,
                PhotoURL = "Not Null Too"
            };
            invalidPerson = this.users.CreatePerson(invalidPerson);
            invalidPerson.Name = null;
            invalidPerson = this.users.UpdatePerson(invalidPerson);
            Assert.IsNull(invalidPerson, "Person was updated despite being invalid.");

            // Update password through UpdatePerson method, nothing should change.
            Person changelessPerson = new Person
            {
                Email = "no_change@email.domain",
                JobDescription = "Not Null",
                Name = "Changeless Person",
                Password = null,
                PhotoURL = "Not Null Too"
            };
            changelessPerson = this.users.CreatePerson(changelessPerson);
            Assert.IsNull(changelessPerson.Password, "Password is not null as expected");
            changelessPerson.Password = "123456";
            changelessPerson = this.users.UpdatePerson(changelessPerson);
            Assert.IsNull(changelessPerson.Password, "Password was updated even though it shouldn't be.");
        }

        [TestMethod]
        public void UpdatePersonPasswordTest()
        {
            // Create a person with a password, should pass.
            Person person = new Person
            {
                Email = "person@email.domain",
                JobDescription = "Not Null",
                Name = "Person",
                Password = "123456",
                PhotoURL = "Not Null Too"
            };
            person = this.users.CreatePerson(person);
            Assert.IsNotNull(person.Password, "Password is null despite being passed.");

            // Remove the password, should pass.
            person = this.users.UpdatePersonPassword(person.PersonID, null);
            Assert.IsNull(person.Password, "Password was not changed.");

            // Give the password again, should pass.
            person = this.users.UpdatePersonPassword(person.PersonID, "654321");
            Assert.IsNotNull(person.Password, "Password was not changed.");

            // Change password of unexistent person, should fail.
            person = this.users.UpdatePersonPassword(-1, null);
            Assert.IsNull(person, "Return success even though the person to updated did not exist.");
        }

        [TestMethod]
        public void DeletePersonTest()
        {
            // Create a person and delete it, should pass.
            Person person = new Person
            {
                Email = "person@email.domain",
                JobDescription = "Not Null",
                Name = "Person",
                Password = null,
                PhotoURL = "Not Null Too"
            };
            person = this.users.CreatePerson(person);
            Assert.IsTrue(this.users.DeletePerson(person.PersonID), "Failed to delete person.");

            // Try to delete same person again, should fail.
            Assert.IsFalse(this.users.DeletePerson(person.PersonID), "Return success despite the person being already deleted.");
        }

        [TestMethod]
        public void GetPersonByIDTest()
        {
            // Create a new person.
            Person person = new Person
            {
                Email = "person@email.domain",
                JobDescription = "Not Null",
                Name = "Person",
                Password = null,
                PhotoURL = "Not Null Too"
            };
            person = this.users.CreatePerson(person);

            // Try to retrieve the persos by its ID.
            person = this.users.GetPersonByID(person.PersonID);
            Assert.IsNotNull(person, "Failed to retrieve the person");
            Assert.AreEqual(person.Email, "person@email.domain", "Retrieved person is not the same.");

            // Try to retrieve invalid person.
            person = this.users.GetPersonByID(-1);
            Assert.IsNull(person, "Person was retrieved despite the ID being invalid.");
        }

        [TestMethod]
        public void GetPersonByEmailTest()
        {
            // Create a new person.
            Person person = new Person
            {
                Email = "person@email.domain",
                JobDescription = "Not Null",
                Name = "Person",
                Password = null,
                PhotoURL = "Not Null Too"
            };
            person = this.users.CreatePerson(person);

            // Try to retrieve the person by its email.
            person = this.users.GetPersonByEmail("person@email.domain");
            Assert.IsNotNull(person, "Failed to retrieve person by its email.");

            // Try to retrieve person by an inexistant email.
            person = this.users.GetPersonByEmail("invalid@email.domain");
            Assert.IsNull(person, "Retrieved a person by an invalid email.");
        }

        [TestMethod]
        public void CreateRoleTest()
        {
            Project project = TestHelper.CreateDefaultProject(this);
            Person person = TestHelper.CreateDefaultPerson(this);

            // Create a new role for the person in the project, should pass.
            Role role = new Role
            {
                AssignedTime = 0.6,
                Password = null,
                PersonID = person.PersonID,
                ProjectID = project.ProjectID,
                RoleDescription = RoleDescription.TeamMember
            };
            role = this.users.CreateRole(role);
            Assert.IsNotNull(role, "Failed to create the role.");
            Assert.AreEqual(role.RoleDescription, RoleDescription.TeamMember, "Returned values differ from original ones.");

            // Create a new role with invalid project, should fail.
            role = new Role
            {
                AssignedTime = 0.6,
                Password = null,
                PersonID = person.PersonID,
                ProjectID = -1,
                RoleDescription = RoleDescription.TeamMember
            };
            role = this.users.CreateRole(role);
            Assert.IsNull(role, "Created role despite being invalid.");

            // Create a new role with invalid user, should fail.
            role = new Role
            {
                AssignedTime = 0.6,
                Password = null,
                PersonID = -1,
                ProjectID = project.ProjectID,
                RoleDescription = RoleDescription.TeamMember
            };
            role = this.users.CreateRole(role);
            Assert.IsNull(role, "Created role despite being invalid.");

            // Create a new role with password, should pass.
            role = new Role
            {
                AssignedTime = 0.6,
                Password = "123456",
                PersonID = person.PersonID,
                ProjectID = project.ProjectID,
                RoleDescription = RoleDescription.ProjectManager
            };
            role = this.users.CreateRole(role);
            Assert.IsNotNull(role, "Failed to create role with password");
            Assert.IsNotNull(role, "Role password was not set.");
        }

        [TestMethod]
        public void UpdateRoleTest()
        {
            Person person = TestHelper.CreateDefaultPerson(this);
            Project project = TestHelper.CreateDefaultProject(this);
            Role role = TestHelper.CreateDefaultRole(this, project, person);

            // Change role description and assigned time, should pass.
            role.AssignedTime = 0.5;
            role.RoleDescription = RoleDescription.ScrumMaster;
            role = this.users.UpdateRole(role);
            Assert.IsNotNull(role, "Failed to update role.");
            Assert.AreEqual(role.AssignedTime, 0.5, "Assigned time was not updated correctly.");
            Assert.AreEqual(role.RoleDescription, RoleDescription.ScrumMaster, "Role description was not updated correctly.");

            // Check that updating the password does nothing.
            role.Password = "654321";
            role = this.users.UpdateRole(role);
            Assert.IsNotNull(role, "Error updating the role.");
            Assert.IsNull(role.Password, "Password was updated incorrectly.");

            // Update an invalid role, should fail.
            role.RoleID = -1;
            role = this.users.UpdateRole(role);
            Assert.IsNull(role, "Updated role despite being invalid.");
        }

        [TestMethod]
        public void UpdateRolePasswordTest()
        {
            Person person = TestHelper.CreateDefaultPerson(this);
            Project project = TestHelper.CreateDefaultProject(this);
            Role role = TestHelper.CreateDefaultRole(this, project, person);
            Assert.IsNull(role.Password, "Password was not null as expected.");

            // Give a new password, should pass.
            role = this.users.UpdateRolePassword(role.RoleID, "123456");
            Assert.IsNotNull(role, "Failed to update the role.");
            Assert.IsNotNull(role.Password, "Password was not updated.");

            // Remove the password, should pass.
            role = this.users.UpdateRolePassword(role.RoleID, null);
            Assert.IsNotNull(role, "Failed to update the role.");
            Assert.IsNull(role.Password, "Password was not removed.");

            // Update invalid role, should fail.
            role = this.users.UpdateRolePassword(-1, "123456");
            Assert.IsNull(role, "Role was updated despite being invalid.");
        }

        [TestMethod]
        public void DeleteRoleTest()
        {
            Person person = TestHelper.CreateDefaultPerson(this);
            Project project = TestHelper.CreateDefaultProject(this);
            Role role = TestHelper.CreateDefaultRole(this, project, person);

            // Delete the role, should pass.
            Assert.IsTrue(this.users.DeleteRole(role.RoleID), "Failed to delete the role.");

            // Try to delete the same role, should fail.
            Assert.IsFalse(this.users.DeleteRole(role.RoleID), "Returned success despite the role being already deleted.");
        }

        [TestMethod]
        public void GetRoleByIDTest()
        {
            Person person = TestHelper.CreateDefaultPerson(this);
            Project project = TestHelper.CreateDefaultProject(this);
            Role role = TestHelper.CreateDefaultRole(this, project, person);

            // Get the role by ID, should pass.
            role = this.users.GetRoleByID(role.RoleID);
            Assert.IsNotNull(role, "Failed to get the role.");
            Assert.AreEqual(role.AssignedTime, 1.0, "Returned role is not what was expected.");
            Assert.AreEqual(role.RoleDescription, RoleDescription.TeamMember, "Returned role is not what was expected.");

            // Get a role with an invalid ID, should fail.
            role = this.users.GetRoleByID(-1);
            Assert.IsNull(role, "Retrieved an inexistent role.");
        }

        [TestMethod]
        public void GetAllPeopleInProjectTest()
        {
            // Creates 5 people.
            Project project1 = TestHelper.CreateDefaultProject(this);
            project1.Name = "Project 2";
            Project project2 = this.projects.CreateProject(project1);
            for (int i = 0; i < 5; ++i)
            {
                Person person = new Person
                {
                    Email = "person_" + i + "@email.domain.",
                    JobDescription = null,
                    Name = "Person " + i,
                    Password = null,
                    PhotoURL = null,
                };
                person = this.users.CreatePerson(person);
                TestHelper.CreateDefaultRole(this, project1, person);
                TestHelper.CreateDefaultRole(this, project2, person);
            }

            // Get all people in the project1, should pass.
            var people = this.users.GetAllPeopleInProject(project1.ProjectID);
            Assert.IsNotNull(people, "Failed to retrieve people.");
            Assert.AreEqual(people.Length, 5, "Wrong number of people returned.");

            // Get all people in the project2, should pass.
            people = this.users.GetAllPeopleInProject(project2.ProjectID);
            Assert.IsNotNull(people, "Failed to retrieve people.");
            Assert.AreEqual(people.Length, 5, "Wrong number of people returned.");

            // Get all people in invalid project, should fail.
            people = this.users.GetAllPeopleInProject(-1);
            Assert.IsNull(people, "Retrieved people despite being an invalid project.");
        }

        [TestMethod]
        public void GetAllRolesInProjectTest()
        {
            // Creates 5 people.
            Project project1 = TestHelper.CreateDefaultProject(this);
            project1.Name = "Project 2";
            Project project2 = this.projects.CreateProject(project1);
            for (int i = 0; i < 5; ++i)
            {
                Person person = new Person
                {
                    Email = "person_" + i + "@email.domain.",
                    JobDescription = null,
                    Name = "Person " + i,
                    Password = null,
                    PhotoURL = null,
                };
                person = this.users.CreatePerson(person);
                TestHelper.CreateDefaultRole(this, project1, person);
                TestHelper.CreateDefaultRole(this, project2, person);
            }

            // Get all people in the project1, should pass.
            var roles = this.users.GetAllRolesInProject(project1.ProjectID);
            Assert.IsNotNull(roles, "Failed to retrieve people.");
            Assert.AreEqual(roles.Length, 5, "Wrong number of roles returned.");

            // Get all people in the project2, should pass.
            roles = this.users.GetAllRolesInProject(project2.ProjectID);
            Assert.IsNotNull(roles, "Failed to retrieve people.");
            Assert.AreEqual(roles.Length, 5, "Wrong number of roles returned.");

            // Get all people in invalid project, should fail.
            roles = this.users.GetAllRolesInProject(-1);
            Assert.IsNull(roles, "Retrieved roles despite being an invalid project.");
        }

        [TestMethod]
        public void GetAllPeopleTest()
        {
            // Get all people, should pass and return an empty array.
            var people = this.Users.GetAllPeople();
            Assert.IsNotNull(people, "Failed to retrieve people.");
            Assert.AreEqual(people.Length, 0, "Incorrect number of people returned.");

            // Creates 20 people.
            for (int i = 0; i < 20; ++i)
            {
                Person person = new Person
                {
                    Email = "person_" + i + "@email.domain.",
                    JobDescription = null,
                    Name = "Person " + i,
                    Password = null,
                    PhotoURL = null,
                };
                person = this.users.CreatePerson(person);
            }

            // Get all people, should always pass.
            people = this.Users.GetAllPeople();
            Assert.IsNotNull(people, "Failed to retrieve people.");
            Assert.AreEqual(people.Length, 20, "Incorrect number of people returned.");
        }

        [TestMethod]
        public void LoginAdminTest()
        {
            // Create an admin.
            var admin = TestHelper.CreateDefaultPerson(this);
            this.users.UpdatePersonPassword(admin.PersonID, "123456");

            // Try to login admin, should pass.
            Assert.IsTrue(this.users.LoginAdmin(admin.PersonID, "123456"), "Failed to log in the admin.");

            // Try to login with invalid password, should fail.
            Assert.IsFalse(this.users.LoginAdmin(admin.PersonID, "654321"), "Returned success despite invalid password.");

            // Try to login someone who's not an admin, should fail.
            this.users.UpdatePersonPassword(admin.PersonID, null);
            Assert.IsFalse(this.users.LoginAdmin(admin.PersonID, "123456"), "Returned success despite not being an admin.");
            Assert.IsFalse(this.users.LoginAdmin(admin.PersonID, null), "Returned success despite not being an admin.");

            // Try to login with invalid role, should fail.
            Assert.IsFalse(this.users.LoginAdmin(-1, "123456"), "Returned success despite not being an admin.");
        }

        [TestMethod]
        public void GetAllTasksInPersonTest()
        {
            Project project = TestHelper.CreateDefaultProject(this);
            Person person = TestHelper.CreateDefaultPerson(this);
            Story story = TestHelper.CreateDefaultStory(this, project);

            // Get tasks in person, should pass and return an empty list.
            var tasks = this.users.GetAllTasksInPerson(person.PersonID);
            Assert.IsNotNull(tasks, "Failed to retrieve the tasks.");
            Assert.AreEqual(tasks.Length, 0, "Incorrect number of tasks returned.");

            // Create 3 tasks for this person.
            for (int i = 0; i < 3; ++i)
            {
                Task task = new Task
                {
                    CreationDate = System.DateTime.Now,
                    Description = "Task " + i,
                    Estimation = i,
                    State = TaskState.Waiting,
                    StoryID = story.StoryID
                };
                task = this.projects.CreateTask(task);
                PersonTask personTask = new PersonTask
                {
                    CreationDate = System.DateTime.Now,
                    PersonID = person.PersonID,
                    TaskID = task.TaskID,
                    SpentTime = 5
                };
                this.projects.AddWorkInTask(personTask);
            }
            tasks = this.users.GetAllTasksInPerson(person.PersonID);
            Assert.IsNotNull(tasks, "Failed to retrieve the tasks.");
            Assert.AreEqual(tasks.Length, 3, "Incorrect number of tasks returned.");

            // Get tasks for invalid user, should fail.
            tasks = this.users.GetAllTasksInPerson(-1);
            Assert.IsNull(tasks, "Returned tasks despite the user not existing.");
        }

        [TestMethod]
        public void GetAllRolesInPersonTest()
        {
            Project project1 = TestHelper.CreateDefaultProject(this);
            project1.Name = "Project 2";
            Project project2 = this.projects.CreateProject(project1);
            Person person = TestHelper.CreateDefaultPerson(this);

            // Get person roles, should pass and return an empty list.
            var roles = this.users.GetAllRolesInPerson(person.PersonID);
            Assert.IsNotNull(roles, "Failed to retrieve the roles.");
            Assert.AreEqual(roles.Length, 0, "Incorrect number of roles returned.");

            // Create 3 roles in each project.
            for (int i = 0; i < 3; ++i)
            {
                TestHelper.CreateDefaultRole(this, project1, person);
                TestHelper.CreateDefaultRole(this, project2, person);
            }

            // Get person roles, should pass.
            roles = this.users.GetAllRolesInPerson(person.PersonID);
            Assert.IsNotNull(roles, "Failed to retrieve the roles.");
            Assert.AreEqual(roles.Length, 6, "Incorrect number of roles returned.");

            // Get all roles in an invalid person, should fail.
            roles = this.users.GetAllRolesInPerson(-1);
            Assert.IsNull(roles, "Retrieved roles despite being an invalid project.");
        }

        [TestMethod]
        public void LoginProjectAdminTest()
        {
            // Create a project admin.
            Person person = TestHelper.CreateDefaultPerson(this);
            Project project = TestHelper.CreateDefaultProject(this);
            Role role = TestHelper.CreateDefaultRole(this, project, person);
            this.users.UpdateRolePassword(role.RoleID, "654321");

            // Try to login admin, should pass.
            Assert.IsTrue(this.users.LoginProjectAdmin(role.RoleID, "654321"), "Failed to log in the admin.");

            // Try to login with invalid password, should fail.
            Assert.IsFalse(this.users.LoginProjectAdmin(role.RoleID, "123456"), "Returned success despite invalid password.");

            // Try to login someone who's not an admin, should fail.
            this.users.UpdateRolePassword(role.RoleID, null);
            Assert.IsFalse(this.users.LoginProjectAdmin(role.RoleID, "654321"), "Returned success despite not being an admin.");
            Assert.IsFalse(this.users.LoginProjectAdmin(role.RoleID, null), "Returned success despite not being an admin.");

            // Try to login with invalid role, should fail.
            Assert.IsFalse(this.users.LoginProjectAdmin(-1, "654321"), "Returned success despite not being an admin.");
        }

        [TestMethod]
        public void GetAllPeopleWorkingInTaskTest()
        {
            Project project = TestHelper.CreateDefaultProject(this);
            Story story = TestHelper.CreateDefaultStory(this, project);
            Task task = TestHelper.CreateDefaultTask(this, story);

            // Get all people working in the task, should pass and return an empty list.
            var people = this.users.GetAllPeopleWorkingInTask(task.TaskID);
            Assert.IsNotNull(people, "Failed to retrieve people working in task.");
            Assert.AreEqual(people.Length, 0, "Incorrect number of people returned.");

            // Create 5 people and put them working on this task.
            for (int i = 0; i < 5; ++i)
            {
                Person person = new Person
                {
                    Email = "person_" + i + "@email.domain",
                    JobDescription = null,
                    Name = "Person " + i,
                    Password = null,
                    PhotoURL = null
                };
                person = this.users.CreatePerson(person);
                PersonTask personTask = new PersonTask
                {
                    CreationDate = System.DateTime.Now,
                    PersonID = person.PersonID,
                    SpentTime = i,
                    TaskID = task.TaskID
                };
                this.projects.AddWorkInTask(personTask);
            }

            // Get all people working in the task, should pass.
            people = this.users.GetAllPeopleWorkingInTask(task.TaskID);
            Assert.IsNotNull(people, "Failed to retrieve people working in task.");
            Assert.AreEqual(people.Length, 5, "Incorrect number of people returned.");

            // Get people working in task that does not exist, should fail.
            people = this.users.GetAllPeopleWorkingInTask(-1);
            Assert.IsNull(people, "Returned people even though the task didn't exist.");
        }

        [TestMethod]
        public void CreateProjectTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void UpdateProjectTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void UpdateProjectPasswordTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void DeleteProjectTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void GetProjectByIDTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void GetProjectByNameTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void CreateSprintTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void DeleteSprintTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void UpdateSprintTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void GetSprintByIDTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void CreateStoryTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void DeleteStoryTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void UpdateStoryTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void GetStoryByIDTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void CreateTaskTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void DeleteTaskTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void UpdateTaskTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void GetTaskByIDTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void CreateMeetingTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void DeleteMeetingTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void UpdateMeetingTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void GetMeetingByIDTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        public void GetAllProjectsTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void LoginProjectTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void GetAllSprintsInProjectTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void GetAllStoriesInProjectTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void GetAllTasksInProjectTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void GetAllStoriesWithoutSprintInProjectTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void GetAllMeetingsInProjectTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void GetAllTasksInProjectByStateTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void GetAllStoriesInProjectByStateTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void GetAllStoriesInSprintTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void GetAllTasksInSprintTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void AddStoryInSprintTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void GetAllTasksInStoryTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void UpdateStoryOrderTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void AddWorkInTaskTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }
    }
}