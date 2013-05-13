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
            // Create a new project and check some values, should pass.
            Project project = new Project
            {
                AlertLimit = 1,
                Name = "Created Project",
                Password = null,
                Speed = 5,
                SprintDuration = 3
            };
            project = this.projects.CreateProject(project);
            Assert.IsNotNull(project, "Failed to create the project.");
            Assert.AreEqual(project.Speed, 5, "Speed is different than expected.");
            Assert.AreEqual(project.Name, "Created Project", "Name is different than expected.");
            Assert.AreEqual(project.Meetings.Count(), 0, "Number of meetings is different than expected.");

            // Try to create a project with the same name, should fail.
            project = this.projects.CreateProject(project);
            Assert.IsNull(project, "Project was created despite its name not being unique.");

            // Try to create a project with password, should pass.
            project = new Project
            {
                AlertLimit = 1,
                Name = "Password Project",
                Password = "123456",
                Speed = 5,
                SprintDuration = 3
            };
            project = this.projects.CreateProject(project);
            Assert.IsNotNull(project, "Failed to create the project.");
            Assert.IsNotNull(project.Password, "Password was not set.");
        }

        [TestMethod]
        public void UpdateProjectTest()
        {
            // Create and modify a default project, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            project.Name = "Updated Project";
            project.Speed = 20;
            project = this.projects.UpdateProject(project);
            Assert.IsNotNull(project, "Failed to update the project.");
            Assert.AreEqual(project.Speed, 20, "Speed is not as expected.");
            Assert.AreEqual(project.Name, "Updated Project", "Name is not as expected.");

            // Try to update password, it should pass but make no effect.
            project.Password = "123456";
            project = this.projects.UpdateProject(project);
            Assert.IsNotNull(project, "Failed to update the project.");
            Assert.IsNull(project.Password, "Password was updated incorrectly.");

            // Create a new project, try to update the name for an already
            // existing one, should fail.
            Project project2 = TestHelper.CreateDefaultProject(this);
            project2.Name = "Updated Project";
            project2 = this.projects.UpdateProject(project2);
            Assert.IsNull(project2, "Project was created despite not having an unique name.");
        }

        [TestMethod]
        public void UpdateProjectPasswordTest()
        {
            // Create a new project without password.
            Project project = TestHelper.CreateDefaultProject(this);
            Assert.IsNull(project.Password, "Password is not null as expected.");

            // Update the password, should pass.
            project = this.projects.UpdateProjectPassword(project.ProjectID, "123456");
            Assert.IsNotNull(project, "Failed to update the password.");
            Assert.IsNotNull(project.Password, "Password was not updated correctly.");

            // Remove the password, should pass.
            project = this.projects.UpdateProjectPassword(project.ProjectID, null);
            Assert.IsNotNull(project, "Failed to update the password.");
            Assert.IsNull(project.Password, "Password was not removed correctly.");

            // Try to update invalid project, should fail.
            project = this.projects.UpdateProjectPassword(-1, "123456");
            Assert.IsNull(project, "Returned success despite invalid project.");
        }

        [TestMethod]
        public void DeleteProjectTest()
        {
            // Create a default project and delete it, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            Assert.IsTrue(this.projects.DeleteProject(project.ProjectID), "Failed to delete the project.");

            // Try to delete the project again, should fail.
            Assert.IsFalse(this.projects.DeleteProject(project.ProjectID), "Returned success despite invalid project.");
        }

        [TestMethod]
        public void GetProjectByIDTest()
        {
            // Create a new project and fetch it by its ID.
            Project project = TestHelper.CreateDefaultProject(this);
            project.Name = "Fetchable Project";
            this.projects.UpdateProject(project);
            project = this.projects.GetProjectByID(project.ProjectID);
            Assert.IsNotNull(project, "Failed to retrieve the project.");
            Assert.AreEqual(project.Name, "Fetchable Project", "Project name is not as expected.");

            // Try to fetch an invalid project, should fail.
            project = this.projects.GetProjectByID(-1);
            Assert.IsNull(project, "Returned success despite invalid project.");
        }

        [TestMethod]
        public void GetProjectByNameTest()
        {
            // Create a new project and fetch it by its ID.
            Project project = TestHelper.CreateDefaultProject(this);
            project.Name = "Fetchable Project";
            this.projects.UpdateProject(project);
            project = this.projects.GetProjectByName("Fetchable Project");
            Assert.IsNotNull(project, "Failed to retrieve the project.");
            Assert.AreEqual(project.Name, "Fetchable Project", "Project name is not as expected.");

            // Try to fetch an invalid project, should fail.
            project = this.projects.GetProjectByName("Nonexistent Project");
            Assert.IsNull(project, "Returned success despite invalid project.");
        }

        [TestMethod]
        public void CreateSprintTest()
        {
            // Create a sprint, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            Sprint sprint = TestHelper.CreateDefaultSprint(this, project);
            Assert.IsNotNull(sprint, "Failed to create a sprint.");

            // Create an invalid sprint, should fail.
            sprint.ProjectID = -1;
            sprint = this.projects.CreateSprint(sprint);
            Assert.IsNull(sprint, "Created the sprint despite being invalid.");
        }

        [TestMethod]
        public void DeleteSprintTest()
        {
            // Create and delete a sprint, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            Sprint sprint = TestHelper.CreateDefaultSprint(this, project);
            Assert.IsTrue(this.projects.DeleteSprint(sprint.SprintID), "Failed to delete the sprint.");

            // Try to delete the sprint again, should fail.
            Assert.IsFalse(this.projects.DeleteSprint(sprint.SprintID), "Returned success despite invalid sprint.");

            // Check that deleting a project should delete the connected sprints.
            sprint = TestHelper.CreateDefaultSprint(this, project);
            Assert.IsNotNull(sprint, "Failed to create sprint.");
            this.projects.DeleteProject(project.ProjectID);
            Assert.IsFalse(this.projects.DeleteSprint(sprint.SprintID), "Returned success despite invalid sprint.");
        }

        [TestMethod]
        public void UpdateSprintTest()
        {
            // Create and update a sprint, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            Sprint sprint = TestHelper.CreateDefaultSprint(this, project);
            sprint.Number = 7;
            sprint = this.projects.UpdateSprint(sprint);
            Assert.IsNotNull(sprint, "Failed to update the sprint.");
            Assert.AreEqual(sprint.Number, 7, "Number is not as expected.");

            // Try to update invalid sprint, should fail.
            sprint.SprintID = -1;
            sprint = this.projects.UpdateSprint(sprint);
            Assert.IsNull(sprint, "Returned success despite invalid sprint.");
        }

        [TestMethod]
        public void GetSprintByIDTest()
        {
            // Create and fetch it by ID, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            Sprint sprint = TestHelper.CreateDefaultSprint(this, project);
            sprint.Number = 3;
            this.projects.UpdateSprint(sprint);
            sprint = this.projects.GetSprintByID(sprint.SprintID);
            Assert.IsNotNull(sprint, "Failed to fetch the sprint.");
            Assert.AreEqual(sprint.Number, 3, "Number is not as expected.");

            // Try to fetch invalid sprint, should fail.
            sprint = this.projects.GetSprintByID(-1);
            Assert.IsNull(sprint, "Returned success despite invalid sprint.");
        }

        [TestMethod]
        public void CreateStoryTest()
        {
            // Create a story, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            Story story1 = TestHelper.CreateDefaultStory(this, project);
            Assert.IsNotNull(story1, "Failed to create a story.");
            Assert.IsNull(story1.PreviousStory, "Story references another one when it shouldn't.");
            Assert.AreEqual(story1.Number, 1, "Number is not as expected.");

            // Create a new task, see if it points to the previous one, should pass.
            Story story2 = TestHelper.CreateDefaultStory(this, project);
            story1 = this.projects.GetStoryByID(story1.StoryID);
            Assert.IsNotNull(story2, "Failed to create a second story.");
            Assert.AreEqual(story2.Number, 2, "Number is not as expected.");
            Assert.AreEqual(story2.PreviousStory, story1.StoryID, "Second story does not point to the first one.");
            Assert.IsNull(story1.PreviousStory, "Story points in story when it shouldn't.");

            // Create a new task, see if it points to the previous one, should pass.
            Story story3 = TestHelper.CreateDefaultStory(this, project);
            Assert.IsNotNull(story3, "Failed to create a second story.");
            Assert.AreEqual(story3.Number, 3, "Number is not as expected.");
            Assert.AreEqual(story3.PreviousStory, story2.StoryID, "Second story does not point to the first one.");

            // Try to create an invalid story, should fail.
            story1.ProjectID = -1;
            story1 = this.Projects.CreateStory(story1);
            Assert.IsNull(story1, "Returned success despite invalid story.");
        }

        [TestMethod]
        public void DeleteStoryTest()
        {
            // Create and delete a story, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            Story story1 = TestHelper.CreateDefaultStory(this, project);
            Story story2 = TestHelper.CreateDefaultStory(this, project);
            Story story3 = TestHelper.CreateDefaultStory(this, project);
            Story story4 = TestHelper.CreateDefaultStory(this, project);
            Assert.IsTrue(this.projects.DeleteStory(story1.StoryID), "Failed to delete the story.");

            // Check if story2 reference was updated.
            Assert.AreEqual(story2.PreviousStory, story1.StoryID, "Story2 does not point to previous story.");
            story2 = this.projects.GetStoryByID(story2.StoryID);
            Assert.IsNull(story2.PreviousStory, "The story still points to an invalid previous story.");

            // Delete story3, story2 should stay the same, story 4 should be changed.
            Assert.AreEqual(story4.PreviousStory, story3.StoryID, "Story4 does not point to previous story.");
            Assert.IsTrue(this.projects.DeleteStory(story3.StoryID), "Failed to delete the story.");
            story2 = this.projects.GetStoryByID(story2.StoryID);
            story4 = this.projects.GetStoryByID(story4.StoryID);
            Assert.IsNull(story2.PreviousStory, "Story2 points to an invalid story.");
            Assert.AreEqual(story4.PreviousStory, story2.StoryID, "Story4 does not point to previous story.");

            // Delete another story, check if updates correctly.
            Assert.IsTrue(this.projects.DeleteStory(story2.StoryID), "Failed to delete the story.");
            story4 = this.projects.GetStoryByID(story4.StoryID);
            Assert.IsNull(story4.PreviousStory, "Story4 points to an invalid story.");

            // Try to delete the last story.
            Assert.IsTrue(this.projects.DeleteStory(story4.StoryID), "Failed to delete the story.");

            // Try to delete the story again, should fail.
            Assert.IsFalse(this.projects.DeleteStory(story4.StoryID), "Returned success despite invalid story.");

            // Check that creating a new story should continue its number;
            Story story5 = TestHelper.CreateDefaultStory(this, project);
            Assert.AreEqual(5, story5.Number, "Number is not as expected.");

            // Check that deleting a project should delete all stories, should pass.
            story1 = TestHelper.CreateDefaultStory(this, project);
            Assert.IsTrue(this.projects.DeleteProject(project.ProjectID), "Failed to delete the project.");
            Assert.IsFalse(this.projects.DeleteStory(story1.StoryID), "Returned success despite the story should be already deleted.");
        }

        [TestMethod]
        public void UpdateStoryTest()
        {
            // Create and update a story, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            Story story = TestHelper.CreateDefaultStory(this, project);
            story.State = StoryState.Completed;
            story = this.projects.UpdateStory(story);
            Assert.IsNotNull(story, "Failed to update the story.");
            Assert.AreEqual(story.State, StoryState.Completed, "State is not as expected.");

            // Try to update an invalid story, should fail.
            story.StoryID = -1;
            story = this.projects.UpdateStory(story);
            Assert.IsNull(story, "Returned success despite invalid story.");
        }

        [TestMethod]
        public void GetStoryByIDTest()
        {
            // Create and fetch a story, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            Story story = TestHelper.CreateDefaultStory(this, project);
            story.State = StoryState.Completed;
            this.projects.UpdateStory(story);
            story = this.projects.GetStoryByID(story.StoryID);
            Assert.IsNotNull(story, "Failed to fetch the story.");
            Assert.AreEqual(story.State, StoryState.Completed, "State is not as expected.");

            // Try to fetch invalid story, should fail.
            story = this.projects.GetStoryByID(-1);
            Assert.IsNull(story, "Returned success despite story being invalid.");
        }

        [TestMethod]
        public void CreateTaskTest()
        {
            // Create task, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            Story story = TestHelper.CreateDefaultStory(this, project);
            Task task = TestHelper.CreateDefaultTask(this, story);
            Assert.IsNotNull(task, "Failed to create the task.");
            
            // Try to create task with invalid story, should fail.
            task.StoryID = -1;
            task = this.projects.CreateTask(task);
            Assert.IsNull(task, "Returned success despite invalid task.");
        }

        [TestMethod]
        public void DeleteTaskTest()
        {
            // Create and delete a task, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            Story story = TestHelper.CreateDefaultStory(this, project);
            Task task = TestHelper.CreateDefaultTask(this, story);
            Assert.IsTrue(this.projects.DeleteTask(task.TaskID), "Failed to delete the task.");

            // Try to delete the task again, should fail.
            Assert.IsFalse(this.projects.DeleteTask(task.TaskID), "Returned success despite invalid task.");

            // Check if deleting the story deletes the associated tasks.
            task = TestHelper.CreateDefaultTask(this, story);
            Assert.IsTrue(this.projects.DeleteStory(story.StoryID), "Failed to delete the story.");
            Assert.IsFalse(this.projects.DeleteTask(task.TaskID), "Task was not deleted automaticaly.");

            // Check if deleting the project deletes the associated tasks.
            story = TestHelper.CreateDefaultStory(this, project);
            task = TestHelper.CreateDefaultTask(this, story);
            Assert.IsTrue(this.projects.DeleteProject(project.ProjectID), "Failed to delete the project.");
            Assert.IsFalse(this.projects.DeleteTask(task.TaskID), "Task was not deleted automaticaly.");
        }

        [TestMethod]
        public void UpdateTaskTest()
        {
            // Create and update a task, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            Story story = TestHelper.CreateDefaultStory(this, project);
            Task task = TestHelper.CreateDefaultTask(this, story);
            Assert.AreEqual(task.Estimation, 1, "Estimation is not as expected.");
            Assert.AreEqual(task.Description, "Default Task", "Description is not as expected.");
            task.Estimation = 20;
            task.Description = "I am a description.";
            task = this.projects.UpdateTask(task);
            Assert.IsNotNull(task, "Failed to update the task.");
            Assert.AreEqual(task.Estimation, 20, "Estimation is not as expected.");
            Assert.AreEqual(task.Description, "I am a description.", "Description is not as expected.");

            // Try to update an invalid task, should fail.
            task.TaskID = -1;
            task = this.projects.UpdateTask(task);
            Assert.IsNull(task, "Returned success despite invalid task.");
        }

        [TestMethod]
        public void GetTaskByIDTest()
        {
            // Create and fetch a task by its ID, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            Story story = TestHelper.CreateDefaultStory(this, project);
            Task task = TestHelper.CreateDefaultTask(this, story);
            task = this.projects.GetTaskByID(task.TaskID);
            Assert.IsNotNull(task, "Failed to fetch the task.");

            // Try to fetch an invalid task, should fail.
            task = this.projects.GetTaskByID(-1);
            Assert.IsNull(task, "Returned success despite invalid task.");
        }

        [TestMethod]
        public void CreateMeetingTest()
        {
            // Create a simple meeting, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            Meeting meeting = TestHelper.CreateDefaultMeeting(this, project);
            Assert.IsNotNull(meeting, "Failed to create a meeting.");

            // Try to create an invalid meeting, should fail.
            meeting.ProjectID = -1;
            meeting = this.projects.CreateMeeting(meeting);
            Assert.IsNull(meeting, "Returned success despite invalid meeting.");
        }

        [TestMethod]
        public void DeleteMeetingTest()
        {
            // Create and delete a simple meeting, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            Meeting meeting = TestHelper.CreateDefaultMeeting(this, project);
            Assert.IsTrue(this.projects.DeleteMeeting(meeting.MeetingID), "Failed to delete the meeting.");

            // Try to delete the same meeting, should fail.
            Assert.IsFalse(this.projects.DeleteMeeting(meeting.MeetingID), "Returned success despite invalid meeting.");
        }

        [TestMethod]
        public void UpdateMeetingTest()
        {
            // Create and update a meeting, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            Meeting meeting = TestHelper.CreateDefaultMeeting(this, project);
            Assert.AreEqual(1, meeting.Number, "Number is different than expected.");
            meeting.Number = 20;
            meeting = this.projects.UpdateMeeting(meeting);
            Assert.IsNotNull(meeting, "Failed to update the meeting.");
            Assert.AreEqual(20, meeting.Number, "Number is different than expected.");

            // Try to update invalid meeting, should fail.
            meeting.MeetingID = -1;
            meeting = this.projects.UpdateMeeting(meeting);
            Assert.IsNull(meeting, "Returned success despite invalid meeting.");
        }

        [TestMethod]
        public void GetMeetingByIDTest()
        {
            // Create and fetch a meeting, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            Meeting meeting = TestHelper.CreateDefaultMeeting(this, project);
            meeting = this.projects.GetMeetingByID(meeting.MeetingID);
            Assert.IsNotNull(meeting, "Failed to fetch the meeting.");

            // Try to get an invalid meeting.
            meeting = this.projects.GetMeetingByID(-1);
            Assert.IsNull(meeting, "Returned success despite invalid meeting.");
        }

        [TestMethod]
        public void GetAllProjectsTest()
        {
            // Fetch all projects, should pass and return an empty list.
            var projects = this.projects.GetAllProjects();
            Assert.IsNotNull(projects, "Failed to retrieve the projects.");
            Assert.AreEqual(projects.Count(), 0, "Incorrect number of projects returned.");

            // Create some projects, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            project.Name = "Project 2";
            this.projects.CreateProject(project);
            project.Name = "Project 3";
            this.projects.CreateProject(project);
            projects = this.projects.GetAllProjects();
            Assert.IsNotNull(projects, "Failed to retrieve the projects.");
            Assert.AreEqual(projects.Count(), 3, "Incorrect number of projects returned.");
        }

        [TestMethod]
        public void LoginProjectTest()
        {
            // Create a project and login, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            Assert.IsFalse(this.projects.LoginProject(project.ProjectID, "654321"), "Returned success despite invalid login.");
            this.projects.UpdateProjectPassword(project.ProjectID, "654321");
            Assert.IsTrue(this.projects.LoginProject(project.ProjectID, "654321"), "Failed to login.");

            // Try to login with invalid credentials, should fail.
            Assert.IsFalse(this.projects.LoginProject(project.ProjectID, "123456"), "Returned success despite invalid login.");

            // Try to login with invalid project, should fail.
            Assert.IsFalse(this.projects.LoginProject(-1, "654321"), "Returned success despite invalid login.");
        }

        [TestMethod]
        public void GetAllSprintsInProjectTest()
        {
            // Get sprints in project, should pass and return an empty list.
            Project project = TestHelper.CreateDefaultProject(this);
            var sprints = this.projects.GetAllSprintsInProject(project.ProjectID);
            Assert.IsNotNull(sprints, "Failed to retrieve sprints.");
            Assert.AreEqual(sprints.Count(), 0, "Incorrect number of projects.");

            // Add some projets, should pass.
            Sprint sprint = TestHelper.CreateDefaultSprint(this, project);
            sprint = TestHelper.CreateDefaultSprint(this, project);
            sprints = this.projects.GetAllSprintsInProject(project.ProjectID);
            project.Name = "Project 2";
            Project project2 = this.projects.CreateProject(project);
            sprint = TestHelper.CreateDefaultSprint(this, project2);
            Assert.IsNotNull(sprints, "Failed to retrieve sprints.");
            Assert.AreEqual(sprints.Count(), 2, "Incorrect number of projects.");

            // Try to get sprints from invalid project, should fail.
            sprints = this.projects.GetAllSprintsInProject(-1);
            Assert.IsNull(sprints, "Returned success despite invalid project.");
        }

        [TestMethod]
        public void GetAllStoriesInProjectTest()
        {
            // Fetch all stories in the project, should pass and return an empty list.
            Project project = TestHelper.CreateDefaultProject(this);
            var stories = this.projects.GetAllStoriesInProject(project.ProjectID);
            Assert.IsNotNull(stories, "Failed to fetch all stories.");
            Assert.AreEqual(0, stories.Count(), "Incorrect number of stories returned.");

            // Create some stories.
            for (int i = 0; i < 5; ++i)
            {
                TestHelper.CreateDefaultStory(this, project);
            }
            stories = this.projects.GetAllStoriesInProject(project.ProjectID);
            Assert.IsNotNull(stories, "Failed to fetch all stories.");
            Assert.AreEqual(5, stories.Count(), "Incorrect number of stories returned.");

            // Try to fetch stories from invalid project, should fail.
            stories = this.projects.GetAllStoriesInProject(-1);
            Assert.IsNull(stories, "Returned success despite invalid project.");
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
            // Fetch all stories in the project, should pass and return an empty list.
            Project project = TestHelper.CreateDefaultProject(this);
            var meetings = this.projects.GetAllMeetingsInProject(project.ProjectID);
            Assert.IsNotNull(meetings, "Failed to fetch all meetings.");
            Assert.AreEqual(0, meetings.Count(), "Incorrect number of meetings returned.");

            // Create some stories.
            for (int i = 0; i < 5; ++i)
            {
                TestHelper.CreateDefaultMeeting(this, project);
            }
            meetings = this.projects.GetAllMeetingsInProject(project.ProjectID);
            Assert.IsNotNull(meetings, "Failed to fetch all stories.");
            Assert.AreEqual(5, meetings.Count(), "Incorrect number of stories returned.");

            // Try to fetch stories from invalid project, should fail.
            meetings = this.projects.GetAllMeetingsInProject(-1);
            Assert.IsNull(meetings, "Returned success despite invalid project.");
        }

        [TestMethod]
        public void GetAllTasksInProjectByStateTest()
        {
            Assert.IsTrue(false, "Not implemented.");
        }

        [TestMethod]
        public void GetAllStoriesInProjectByStateTest()
        {
            // Fetch all stories in the project, should pass and return an empty list.
            Project project = TestHelper.CreateDefaultProject(this);
            var stories = this.projects.GetAllStoriesInProjectByState(project.ProjectID, StoryState.Completed);
            Assert.IsNotNull(stories, "Failed to fetch all stories.");
            Assert.AreEqual(0, stories.Count(), "Incorrect number of stories returned.");

            // Create some stories.
            for (int i = 0; i < 5; ++i)
            {
                Story story = TestHelper.CreateDefaultStory(this, project);
                if (i < 3)
                {
                    story.State = StoryState.Completed;
                    this.projects.UpdateStory(story);
                }
            }
            stories = this.projects.GetAllStoriesInProjectByState(project.ProjectID, StoryState.InProgress);
            Assert.IsNotNull(stories, "Failed to fetch all stories.");
            Assert.AreEqual(2, stories.Count(), "Incorrect number of stories returned.");
            stories = this.projects.GetAllStoriesInProjectByState(project.ProjectID, StoryState.Completed);
            Assert.IsNotNull(stories, "Failed to fetch all stories.");
            Assert.AreEqual(3, stories.Count(), "Incorrect number of stories returned.");

            // Try to fetch stories from invalid project, should fail.
            stories = this.projects.GetAllStoriesInProjectByState(-1, StoryState.InProgress);
            Assert.IsNull(stories, "Returned success despite invalid project.");
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
            // Create several stories and check their order.
            Project project = TestHelper.CreateDefaultProject(this);
            Story story1 = TestHelper.CreateDefaultStory(this, project);
            Story story2 = TestHelper.CreateDefaultStory(this, project);
            Story story3 = TestHelper.CreateDefaultStory(this, project);
            Story story4 = TestHelper.CreateDefaultStory(this, project);
            Assert.IsNull(story1.PreviousStory, "Incorrect previous story.");
            Assert.AreEqual(story1.StoryID, story2.PreviousStory, "Incorrect previous story.");
            Assert.AreEqual(story2.StoryID, story3.PreviousStory, "Incorrect previous story.");
            Assert.AreEqual(story3.StoryID, story4.PreviousStory, "Incorrect previous story.");

            // Update the order and check again.
            var stories = this.projects.UpdateStoryOrder(project.ProjectID, new int[] { story3.StoryID, story2.StoryID, story4.StoryID, story1.StoryID });
            Assert.IsNotNull(stories, "Failed to update story order.");
            story1 = this.projects.GetStoryByID(story1.StoryID);
            story2 = this.projects.GetStoryByID(story2.StoryID);
            story3 = this.projects.GetStoryByID(story3.StoryID);
            story4 = this.projects.GetStoryByID(story4.StoryID);
            Assert.IsNull(story3.PreviousStory, "Incorrect previous story.");
            Assert.AreEqual(story3.StoryID, story2.PreviousStory, "Incorrect previous story.");
            Assert.AreEqual(story2.StoryID, story4.PreviousStory, "Incorrect previous story.");
            Assert.AreEqual(story4.StoryID, story1.PreviousStory, "Incorrect previous story.");
        }

        [TestMethod]
        public void AddWorkInTaskTest()
        {
            // Create a new task without work, should pass.
            Project project = TestHelper.CreateDefaultProject(this);
            Person person = TestHelper.CreateDefaultPerson(this);
            Story story = TestHelper.CreateDefaultStory(this, project);
            Task task = TestHelper.CreateDefaultTask(this, story);
            Assert.AreEqual(0, task.PersonTasks.Count(), "Incorrect number of tasks.");

            // Work for someone, should pass.
            PersonTask personTask = new PersonTask
            {
                CreationDate = System.DateTime.Now,
                PersonID = person.PersonID,
                SpentTime = 30,
                TaskID = task.TaskID
            };
            personTask = this.projects.AddWorkInTask(personTask);
            person = this.users.GetPersonByID(person.PersonID);
            task = this.projects.GetTaskByID(task.TaskID);
            Assert.IsNotNull(personTask, "Failed to create a PersonTask.");
            Assert.AreEqual(1, task.PersonTasks.Count(), "Incorrect number of person tasks.");
            Assert.AreEqual(person.PersonID, task.PersonTasks.First().PersonID, "Incorrect person ID");
            Assert.AreEqual(30, task.PersonTasks.First().SpentTime, "Incorrect spent time.");
            Assert.AreEqual(task.TaskID, person.Tasks.First().TaskID, "Incorrect task association.");

            // Add more work to same person, should pass.
            personTask = this.projects.AddWorkInTask(personTask);
            task = this.projects.GetTaskByID(task.TaskID);
            Assert.IsNotNull(personTask, "Failed to create a PersonTask.");
            Assert.AreEqual(1, task.PersonTasks.Count(), "Incorrect number of person tasks.");
            Assert.AreEqual(60, task.PersonTasks.First().SpentTime, "Incorrect spent time.");

            // Add work to new person, should pass.
            person.Email = "test@email.domain";
            person = this.users.CreatePerson(person);
            personTask.PersonID = person.PersonID;
            personTask = this.projects.AddWorkInTask(personTask);
            task = this.projects.GetTaskByID(task.TaskID);
            Assert.IsNotNull(personTask, "Failed to create a PersonTask.");
            Assert.AreEqual(2, task.PersonTasks.Count(), "Incorrect number of person tasks.");
        }
    }
}