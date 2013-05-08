using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ServiceTester
{
    /// <summary>
    /// This partial class is used only to implement concret tests.
    /// All classwise setup and teardown should be made on the other
    /// part of the implementation.
    /// </summary>
    public partial class ProjectServiceTest
    {
        [TestMethod]
        public void CreateProjectTest()
        {
            // Tries to create a valid project.
            ServiceDataTypes.Project project = new ServiceDataTypes.Project
            {
                AlertLimit = 1,
                Speed = 1,
                SprintDuration = 1,
                Password = null,
                Name = "Creatable Project"
            };
            project = ProjectServiceTest.client.CreateProject(project);
            Assert.IsNotNull(project, "The project could not be created.");
            Assert.IsNotNull(project.ProjectID, "No database ID was given to the project.");

            // Tries to create a project with the same name as the
            // previous, and should fail.
            project.ProjectID = null;
            project = ProjectServiceTest.client.CreateProject(project);
            Assert.IsNull(project, "Project was created despite its name being in use already.");

            // Tries to create a project with no name and should fail.
            project = new ServiceDataTypes.Project();
            project = ProjectServiceTest.client.CreateProject(project);
            Assert.IsNull(project, "Project was created despite not having a name.");
        }

        [TestMethod]
        public void DeleteProjectTest()
        {
            // Tries to create a valid project.
            ServiceDataTypes.Project project = new ServiceDataTypes.Project
            {
                AlertLimit = 1,
                Speed = 1,
                SprintDuration = 1,
                Password = null,
                Name = "Deletable Project"
            };
            project = ProjectServiceTest.client.CreateProject(project);
            Assert.IsNotNull(project, "The project could not be created.");
            Assert.IsNotNull(project.ProjectID, "No database ID was given to the project.");

            // Tries to delete the project.
            Assert.IsTrue(ProjectServiceTest.client.DeleteProject((int)project.ProjectID), "Failed to delete the project.");

            // Tries to delete the same project and should fail.
            Assert.IsFalse(ProjectServiceTest.client.DeleteProject((int)project.ProjectID), "Returned success even though the project didn't exist.");

            // Check that the project is no longer found.
            Assert.IsNull(ProjectServiceTest.client.GetProjectByID((int)project.ProjectID), "Project was returned despite being deleted.");
        }

        [TestMethod]
        public void GetProjectByIDTest()
        {
            // Get a project directly from the database.
            var project = ProjectServiceTest.context.GetTable<SkrumManagerService.Project>().First(p => p.Name == "Complete Project");

            // Check if the project can be retrieved by its ID.
            var result = ProjectServiceTest.client.GetProjectByID(project.ProjectID);
            Assert.IsNotNull(result, "Project could not be returned from its ID.");
            Assert.AreEqual(result.ProjectID, project.ProjectID, "The returned project's ID is different.");
            Assert.AreEqual(result.Name, project.Name, "The returned project's name is different.");

            // Check advance projects info.
            Assert.AreEqual(result.SprintDuration, 2, "The returned project's sprint duration is incorrect.");
            Assert.AreEqual(result.Speed, 1, "The returned project's speed is incorrect.");
            Assert.AreEqual(result.AlertLimit, 5, "The returned project's alert limit is incorrect.");
            Assert.AreEqual(result.Meetings.Count(), 3, "The returned project has an incorrect number of meetings associated.");

            // Check if an invalid ID returns a project, it should fail.
            Assert.IsNull(ProjectServiceTest.client.GetProjectByID(-1), "Success was returned despite invalid ID.");
        }

        [TestMethod]
        public void UpdateProjectTest()
        {
            // Get a project directly from the database.
            var project = ProjectServiceTest.context.GetTable<SkrumManagerService.Project>().First(p => p.Name == "Incomplete Project");

            // Fetches the project and changes some information.
            var result = ProjectServiceTest.client.GetProjectByID(project.ProjectID);
            Assert.IsNotNull(result, "Project could not be returned from its ID.");
            result.Speed = 20;
            result.SprintDuration = 4;

            // Updates the project's info and checks for modifications.
            result = ProjectServiceTest.client.UpdateProject(result);
            Assert.IsNotNull(result, "The project update failed.");
            Assert.AreEqual(result.SprintDuration, 4, "The project's sprint duration was not set correctly.");
            Assert.AreEqual(result.Speed, 20, "The project's speed was not set correctly.");
            Assert.AreEqual(result.Name, project.Name, "The project's was changed.");

            // Check that the name can't be changed to something that already exists.
            result.Name = "Complete Project";
            result = ProjectServiceTest.client.UpdateProject(result);
            Assert.IsNull(result, "Project was updated despite its invalid name.");

            // Checks updates with inexistent test.
            result = ProjectServiceTest.client.GetProjectByID(project.ProjectID);
            result.ProjectID = -1;
            result = ProjectServiceTest.client.UpdateProject(result);
            Assert.IsNull(result, "Project was updated despite not existing.");
        }

        [TestMethod]
        public void GetProjectByNameTest()
        {
            // Tries to find two projects by their name.
            var completeProject = ProjectServiceTest.client.GetProjectByName("Complete Project");
            Assert.IsNotNull(completeProject, "Could not find \"Complete Project\" by name");

            var incompleteProject = ProjectServiceTest.client.GetProjectByName("Complete Project");
            Assert.IsNotNull(incompleteProject, "Could not find \"Incomplete Project\" by name");

            // Tries to find an inexistent project and should fail.
            var imaginaryProject = ProjectServiceTest.client.GetProjectByName("Imaginary Project");
            Assert.IsNull(imaginaryProject, "Found \"Imaginary Project\" by name despite not existing.");
        }

        [TestMethod]
        public void CreateSprintTest()
        {
            // Find a project by name.
            var project = ProjectServiceTest.client.GetProjectByName("Complete Project");
            Assert.IsNotNull(project, "Could not find \"Complete Project\" by name");
            int before = project.Sprints.Count();

            // Tries to add a new project.
            ServiceDataTypes.Sprint sprint = new ServiceDataTypes.Sprint
            {
                BeginDate = System.DateTime.Now,
                Closed = false,
                EndDate = System.DateTime.Now,
                Number = 1,
                ProjectID = project.ProjectID
            };
            sprint = ProjectServiceTest.client.CreateSprint(sprint);
            Assert.IsNotNull(sprint, "Error creating the sprint.");
            project = ProjectServiceTest.client.GetProjectByID((int)project.ProjectID);
            int after = project.Sprints.Count();
            Assert.AreNotEqual(before, after, "Project has the same number of sprints after adding a new one.");

            // Tries to create an invalid sprint and should fail.
            sprint.ProjectID = null;
            sprint = ProjectServiceTest.client.CreateSprint(sprint);
            Assert.IsNull(sprint, "Returned success even though the sprint was invalid.");
        }

        [TestMethod]
        public void DeleteSprintTest()
        {
            // Find a project by name and adds a new sprint.
            var project = ProjectServiceTest.client.GetProjectByName("Complete Project");
            Assert.IsNotNull(project, "Could not find \"Complete Project\" by name");
            ServiceDataTypes.Sprint sprint = new ServiceDataTypes.Sprint
            {
                BeginDate = System.DateTime.Now,
                Closed = false,
                EndDate = System.DateTime.Now,
                Number = 1,
                ProjectID = project.ProjectID
            };
            sprint = ProjectServiceTest.client.CreateSprint(sprint);
            Assert.IsNotNull(sprint, "Error creating the sprint.");
            project = ProjectServiceTest.client.GetProjectByName("Complete Project");
            int before = project.Sprints.Count();

            // Tries to remove a sprint.
            sprint = project.Sprints.FirstOrDefault();
            Assert.IsTrue(ProjectServiceTest.client.DeleteSprint((int)sprint.SprintID), "Error deleting the sprint.");
            project = ProjectServiceTest.client.GetProjectByName("Complete Project");
            int after = project.Sprints.Count();
            Assert.AreNotEqual(before, after, "Project has the same number of sprints after removing one.");

            // Tries to remove the sprint again and should fail.
            Assert.IsFalse(ProjectServiceTest.client.DeleteSprint((int)sprint.SprintID), "Returned success even though the sprint no longer existed.");
        }

        [TestMethod]
        public void UpdateSprintTest()
        {
            // Find a project by name and adds a new sprint.
            var project = ProjectServiceTest.client.GetProjectByName("Complete Project");
            Assert.IsNotNull(project, "Could not find \"Complete Project\" by name");
            ServiceDataTypes.Sprint sprint = new ServiceDataTypes.Sprint
            {
                BeginDate = System.DateTime.Now,
                Closed = false,
                EndDate = System.DateTime.Now,
                Number = 2,
                ProjectID = project.ProjectID
            };
            sprint = ProjectServiceTest.client.CreateSprint(sprint);
            Assert.IsNotNull(sprint, "Error creating the sprint.");

            // Change the sprint's info.
            sprint.Number = 3;
            sprint = ProjectServiceTest.client.UpdateSprint(sprint);
            Assert.IsNotNull(sprint, "Error updating the sprint.");

            // Check changed values.
            Assert.IsNull(project.Sprints.FirstOrDefault(s => s.Number == 2), "Previous version of the sprint still exists.");
            Assert.AreEqual(sprint.Number, 3, "Meeting number was not updated");

            // Tries to updated an invalid sprint and should fail.
            sprint.ProjectID = null;
            sprint = ProjectServiceTest.client.UpdateSprint(sprint);
            Assert.IsNotNull(sprint, "Returned success even though the sprint was invalid.");
        }

        [TestMethod]
        public void CreateMeetingTest()
        {
            // Find a project by name.
            var project = ProjectServiceTest.client.GetProjectByName("Complete Project");
            Assert.IsNotNull(project, "Could not find \"Complete Project\" by name");
            int before = project.Meetings.Count();

            // Tries to add a new project.
            ServiceDataTypes.Meeting meeting = new ServiceDataTypes.Meeting
            {
                Notes = "I'm a note!",
                Date = System.DateTime.Now,
                Number = 1,
                ProjectID = project.ProjectID
            };
            meeting = ProjectServiceTest.client.CreateMeeting(meeting);
            Assert.IsNotNull(meeting, "Error creating the meeting.");
            project = ProjectServiceTest.client.GetProjectByName("Complete Project");
            int after = project.Meetings.Count();
            Assert.AreNotEqual(before, after, "Project has the same number of meetings after adding a new one.");

            // Tries to create an invalid meeting and should fail.
            meeting.ProjectID = null;
            meeting = ProjectServiceTest.client.CreateMeeting(meeting);
            Assert.IsNull(meeting, "Returned success even though the meeting was invalid.");
        }

        [TestMethod]
        public void DeleteMeetingTest()
        {
            // Find a project by name and adds a new meeting.
            var project = ProjectServiceTest.client.GetProjectByName("Complete Project");
            Assert.IsNotNull(project, "Could not find \"Complete Project\" by name");
            ServiceDataTypes.Meeting meeting = new ServiceDataTypes.Meeting
            {
                Notes = "I'm a note!",
                Date = System.DateTime.Now,
                Number = 1,
                ProjectID = project.ProjectID
            };
            meeting = ProjectServiceTest.client.CreateMeeting(meeting);
            Assert.IsNotNull(meeting, "Error creating the meeting.");
            project = ProjectServiceTest.client.GetProjectByName("Complete Project");
            int before = project.Meetings.Count();

            // Tries to remove a meeting.
            Assert.IsTrue(ProjectServiceTest.client.DeleteMeeting((int)meeting.MeetingID), "Error deleting the meeting.");
            project = ProjectServiceTest.client.GetProjectByName("Complete Project");
            int after = project.Meetings.Count();
            Assert.AreNotEqual(before, after, "Project has the same number of meetings after removing one.");

            // Tries to remove the meeting again and should fail.
            Assert.IsFalse(ProjectServiceTest.client.DeleteMeeting((int)meeting.MeetingID), "Returned success even though the meeting no longer existed.");
        }

        [TestMethod]
        public void UpdateMeetingTest()
        {
            // Find a project by name and adds a new meeting.
            var project = ProjectServiceTest.client.GetProjectByName("Incomplete Project");
            Assert.IsNotNull(project, "Could not find \"Complete Project\" by name");
            ServiceDataTypes.Meeting meeting = new ServiceDataTypes.Meeting
            {
                Notes = "I'm a note!",
                Date = System.DateTime.Now,
                Number = 2,
                ProjectID = project.ProjectID
            };
            meeting = ProjectServiceTest.client.CreateMeeting(meeting);
            Assert.IsNotNull(meeting, "Error creating the meeting.");

            // Change the meeting's info.
            meeting.Number = 3;
            meeting = ProjectServiceTest.client.UpdateMeeting(meeting);
            Assert.IsNotNull(meeting, "Error updating the meeting.");

            // Check changed values.
            Assert.IsNull(project.Meetings.FirstOrDefault(s => s.Number == 2), "Previous version of the meeting still exists.");
            Assert.AreEqual(meeting.Number, 3, "Meeting number was not updated");

            // Tries to updated an invalid meeting and should fail.
            meeting.ProjectID = null;
            meeting = ProjectServiceTest.client.UpdateMeeting(meeting);
            Assert.IsNotNull(meeting, "Returned success even though the meeting was invalid.");
        }
    }
}