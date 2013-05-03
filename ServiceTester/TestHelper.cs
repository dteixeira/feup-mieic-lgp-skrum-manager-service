using System.Data.Linq;

namespace ServiceTester
{
    /// <summary>
    /// This class is used as a collection of utility methods for the
    /// unit tests.
    /// </summary>
    public class TestHelper
    {
        /// <summary>
        /// Deletes all records on the database.
        /// </summary>
        /// <param name="context">Database context object</param>
        public static void ClearDatabase(DataContext context)
        {
            context.ExecuteCommand("DELETE FROM [dbo] . [PersonTask]");
            context.ExecuteCommand("DELETE FROM [dbo] . [StorySprint]");
            context.ExecuteCommand("DELETE FROM [dbo] . [Meeting]");
            context.ExecuteCommand("DELETE FROM [dbo] . [Role]");
            context.ExecuteCommand("DELETE FROM [dbo] . [Story]");
            context.ExecuteCommand("DELETE FROM [dbo] . [Sprint]");
            context.ExecuteCommand("DELETE FROM [dbo] . [Project]");
            context.ExecuteCommand("DELETE FROM [dbo] . [Person]");
            context.ExecuteCommand("DELETE FROM [dbo] . [Task]");
        }
    }
}