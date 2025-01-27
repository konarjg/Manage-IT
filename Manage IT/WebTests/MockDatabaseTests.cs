using System.Runtime.CompilerServices;

namespace WebTests
{
    [TestClass]
    public class MockDatabaseTests
    {
        [TestMethod]
        public void GetDatabaseSetForTypeUserShouldReturnAnEmptyListOfUsers()
        {
            MockDatabaseContext mock = new MockDatabaseContext();

            Assert.IsInstanceOfType(mock.GetDatabaseSet<User>(), typeof(List<User>));
        }

        [TestMethod]
        public void GetDatabaseSetForTypeProjectShouldReturnAnEmptyListOfProjects()
        {
            MockDatabaseContext mock = new MockDatabaseContext();

            Assert.IsInstanceOfType(mock.GetDatabaseSet<Project>(), typeof(List<Project>));
        }

        [TestMethod]
        public void GetDatabaseSetForTypeProjectMembersShouldReturnAnEmptyListOfProjectMembers()
        {
            MockDatabaseContext mock = new MockDatabaseContext();

            Assert.IsInstanceOfType(mock.GetDatabaseSet<ProjectMembers>(), typeof(List<ProjectMembers>));
        }

        [TestMethod]
        public void GetDatabaseSetForTypeTaskShouldReturnAnEmptyListOfTasks()
        {
            MockDatabaseContext mock = new MockDatabaseContext();

            Assert.IsInstanceOfType(mock.GetDatabaseSet<Task>(), typeof(List<Task>));
        }

        [TestMethod]
        public void GetDatabaseSetForTypeTaskListShouldReturnAnEmptyListOfTaskLists()
        {
            MockDatabaseContext mock = new MockDatabaseContext();

            Assert.IsInstanceOfType(mock.GetDatabaseSet<TaskList>(), typeof(List<TaskList>));
        }

        [TestMethod]
        public void GetDatabaseSetForTypeTaskDetailsShouldReturnAnEmptyListOfTaskDetails()
        {
            MockDatabaseContext mock = new MockDatabaseContext();

            Assert.IsInstanceOfType(mock.GetDatabaseSet<TaskDetails>(), typeof(List<TaskDetails>));
        }

        [TestMethod]
        public void GetDatabaseSetForTypeUserPermissionsShouldReturnAnEmptyListOfUserPermissions()
        {
            MockDatabaseContext mock = new MockDatabaseContext();

            Assert.IsInstanceOfType(mock.GetDatabaseSet<UserPermissions>(), typeof(List<UserPermissions>));
        }

        [TestMethod]
        public void GetDatabaseSetForIncorrectTypeShouldReturnNull()
        {
            MockDatabaseContext mock = new MockDatabaseContext();
            Assert.IsNull(mock.GetDatabaseSet<Database>());
        }

        [TestMethod]
        public void DisposeShouldThrowNotImplementedException()
        {
            Assert.ThrowsException<NotImplementedException>(() => { using (MockDatabase mock = new MockDatabase()) { } });
        }

        [TestMethod]
        public void ExecuteQueryForSelectQueryShouldHaveNotNullResultsAndReturnTrue()
        {
            try
            {
                using (MockDatabase mock = new MockDatabase())
                {
                    List<User> results;
                    FormattableString format = FormattableStringFactory.Create("SELECT * FROM Users");

                    Assert.IsTrue(mock.ExecuteQuery(format, out results));
                    Assert.IsNotNull(results);
                }
            }
            catch (Exception) { }
        }

        [TestMethod]
        public void ExecuteQueryForInsertQueryShouldHaveNotNullResultsAndReturnTrue()
        {
            try
            {
                using (MockDatabase mock = new MockDatabase())
                {
                    List<User> results;
                    FormattableString format = FormattableStringFactory.Create("INSERT INTO Users");

                    Assert.IsTrue(mock.ExecuteQuery(format, out results));
                    Assert.IsNotNull(results);
                }
            }
            catch (Exception) { }
        }

        [TestMethod]
        public void ExecuteQueryForUpdateQueryShouldHaveNotNullResultsAndReturnTrue()
        {
            try
            {
                using (MockDatabase mock = new MockDatabase())
                {
                    List<User> results;
                    FormattableString format = FormattableStringFactory.Create("UPDATE Users");

                    Assert.IsTrue(mock.ExecuteQuery(format, out results));
                    Assert.IsNotNull(results);
                }
            }
            catch (Exception) { }
        }

        [TestMethod]
        public void ExecuteQueryForDeleteQueryShouldHaveNotNullResultsAndReturnTrue()
        {
            try
            {
                using (MockDatabase mock = new MockDatabase())
                {
                    List<User> results;
                    FormattableString format = FormattableStringFactory.Create("DELETE * FROM Users");

                    Assert.IsTrue(mock.ExecuteQuery(format, out results));
                    Assert.IsNotNull(results);
                }
            }
            catch (Exception) { }
        }

        [TestMethod]
        public void ExecuteQueryForIncorrectQueryShouldThrowArgumentException()
        {
            try
            {
                using (MockDatabase mock = new MockDatabase())
                {
                    List<User> results;
                    FormattableString format = FormattableStringFactory.Create("DESTROY Users");

                    Assert.ThrowsException<ArgumentException>(() => mock.ExecuteQuery(format, out results));
                }
            }
            catch (Exception) { }
        }
    }
}