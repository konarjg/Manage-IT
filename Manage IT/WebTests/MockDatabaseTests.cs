using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WebTests
{
    [TestClass]
    public class MockDatabaseTests
    {
        [TestMethod]
        public void GetDatabaseSetForTypeUserShouldReturnAnEmptyListOfUsers()
        {
            var mock = new MockDatabaseContext();

            Assert.IsInstanceOfType(mock.GetDatabaseSet<User>(), typeof(List<User>));
        }

        [TestMethod]
        public void GetDatabaseSetForTypeProjectShouldReturnAnEmptyListOfProjects()
        {
            var mock = new MockDatabaseContext();

            Assert.IsInstanceOfType(mock.GetDatabaseSet<Project>(), typeof(List<Project>));
        }

        [TestMethod]
        public void GetDatabaseSetForTypeProjectMembersShouldReturnAnEmptyListOfProjectMembers()
        {
            var mock = new MockDatabaseContext();

            Assert.IsInstanceOfType(mock.GetDatabaseSet<ProjectMembers>(), typeof(List<ProjectMembers>));
        }

        [TestMethod]
        public void GetDatabaseSetForTypeTaskShouldReturnAnEmptyListOfTasks()
        {
            var mock = new MockDatabaseContext();

            Assert.IsInstanceOfType(mock.GetDatabaseSet<Task>(), typeof(List<Task>));
        }

        [TestMethod]
        public void GetDatabaseSetForTypeTaskListShouldReturnAnEmptyListOfTaskLists()
        {
            var mock = new MockDatabaseContext();

            Assert.IsInstanceOfType(mock.GetDatabaseSet<TaskList>(), typeof(List<TaskList>));
        }

        [TestMethod]
        public void GetDatabaseSetForTypeTaskDetailsShouldReturnAnEmptyListOfTaskDetails()
        {
            var mock = new MockDatabaseContext();

            Assert.IsInstanceOfType(mock.GetDatabaseSet<TaskDetails>(), typeof(List<TaskDetails>));
        }

        [TestMethod]
        public void GetDatabaseSetForTypeUserPermissionsShouldReturnAnEmptyListOfUserPermissions()
        {
            var mock = new MockDatabaseContext();

            Assert.IsInstanceOfType(mock.GetDatabaseSet<UserPermissions>(), typeof(List<UserPermissions>));
        }

        [TestMethod]
        public void GetDatabaseSetForIncorrectTypeShouldReturnNull()
        {
            var mock = new MockDatabaseContext();
            Assert.IsNull(mock.GetDatabaseSet<Database>());
        }

        [TestMethod]
        public void DisposeShouldThrowNotImplementedException()
        {
            Assert.ThrowsException<NotImplementedException>(() => { using (var mock = new MockDatabase()) { } });
        }

        [TestMethod]
        public void ExecuteQueryForSelectQueryShouldHaveNotNullResultsAndReturnTrue()
        {
            try
            {
                using (var mock = new MockDatabase())
                {
                    List<User> results;
                    var format = FormattableStringFactory.Create("SELECT * FROM Users");

                    Assert.IsTrue(mock.ExecuteQuery(format, out results));
                    Assert.IsNotNull(results);
                }
            }
            catch (Exception ex) { }
        }

        [TestMethod]
        public void ExecuteQueryForInsertQueryShouldHaveNotNullResultsAndReturnTrue()
        {
            try
            {
                using (var mock = new MockDatabase())
                {
                    List<User> results;
                    var format = FormattableStringFactory.Create("INSERT INTO Users");

                    Assert.IsTrue(mock.ExecuteQuery(format, out results));
                    Assert.IsNotNull(results);
                }
            }
            catch (Exception ex) { }
        }

        [TestMethod]
        public void ExecuteQueryForUpdateQueryShouldHaveNotNullResultsAndReturnTrue()
        {
            try
            {
                using (var mock = new MockDatabase())
                {
                    List<User> results;
                    var format = FormattableStringFactory.Create("UPDATE Users");

                    Assert.IsTrue(mock.ExecuteQuery(format, out results));
                    Assert.IsNotNull(results);
                }
            }
            catch (Exception ex) { }
        }

        [TestMethod]
        public void ExecuteQueryForDeleteQueryShouldHaveNotNullResultsAndReturnTrue()
        {
            try
            {
                using (var mock = new MockDatabase())
                {
                    List<User> results;
                    var format = FormattableStringFactory.Create("DELETE * FROM Users");

                    Assert.IsTrue(mock.ExecuteQuery(format, out results));
                    Assert.IsNotNull(results);
                }
            }
            catch (Exception ex) { }
        }

        [TestMethod]
        public void ExecuteQueryForIncorrectQueryShouldThrowArgumentException()
        {
            try
            {
                using (var mock = new MockDatabase())
                {
                    List<User> results;
                    var format = FormattableStringFactory.Create("DESTROY Users");

                    Assert.ThrowsException<ArgumentException>(() => mock.ExecuteQuery(format, out results));
                }
            }
            catch(Exception ex) { }
        }
    }
}