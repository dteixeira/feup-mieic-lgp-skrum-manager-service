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
            // Fetch table reference and count the number of entries.
            var persons = UserServiceTest.context.GetTable<SkrumManagerService.Person>();
            int initialCount = persons.Count();

            // Create a person representation.
            ServiceDataTypes.Person person = new ServiceDataTypes.Person();
            person.PersonID = -1;
            person.Admin = false;
            person.Email = "test@email.pt";
            person.JobDescription = "This is my job. It makes me happy!";
            person.Name = "John Doe";
            person.Password = null;
            person.PhotoURL = null;

            // Add person through service.
            person = UserServiceTest.client.CreatePerson(person);

            // If person is null, something went wrong.
            Assert.IsNotNull(person);

            // Check if a new record was really created.
            int currentCount = persons.Count();
            Assert.AreNotEqual(initialCount, currentCount);

            // Check if a new person id was attributed.
            Assert.AreNotEqual(person.PersonID, -1);

            // Check if the correct data was added to the database.
            Assert.IsNotNull(persons.FirstOrDefault(p => p.Name == person.Name));
        }
    }
}