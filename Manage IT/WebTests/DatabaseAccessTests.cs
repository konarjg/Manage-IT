using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WebTests
{
    [TestClass]
    public class DatabaseAccessTests
    {
        [TestMethod]
        public void ProcessQueryInsertShouldHaveNullResultsAndReturnTrue()
        {
            var mock = new MockDatabaseAccess();
            List<User> results;
            var format = FormattableStringFactory.Create("INSERT INTO Users");
            var success = mock.ProcessQuery(format, out results);

            Assert.IsNull(results);
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void ProcessQueryInsertShouldAddTheQueryToBuffer()
        {
            var mock = new MockDatabaseAccess();
            List<User> results;
            var format = FormattableStringFactory.Create("INSERT INTO Users");
            mock.ProcessQuery(format, out results);

            Assert.IsTrue(mock.QueryBuffer.Count != 0 && mock.QueryBuffer.Contains(format));
        }

        [TestMethod]
        public void ProcessQueryUpdateShouldHaveNullResultsAndReturnTrue()
        {
            var mock = new MockDatabaseAccess();
            List<User> results;
            var format = FormattableStringFactory.Create("UPDATE Users");
            var success = mock.ProcessQuery(format, out results);

            Assert.IsNull(results);
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void ProcessQueryUpdateShouldAddTheQueryToBuffer()
        {
            var mock = new MockDatabaseAccess();
            List<User> results;
            var format = FormattableStringFactory.Create("UPDATE Users");
            mock.ProcessQuery(format, out results);

            Assert.IsTrue(mock.QueryBuffer.Count != 0 && mock.QueryBuffer.Contains(format));
        }

        [TestMethod]
        public void ProcessQueryDeleteShouldHaveNullResultsAndReturnTrue()
        {
            var mock = new MockDatabaseAccess();
            List<User> results;
            var format = FormattableStringFactory.Create("DELETE * FROM Users");
            var success = mock.ProcessQuery(format, out results);

            Assert.IsNull(results);
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void ProcessQueryDeleteShouldAddTheQueryToBuffer()
        {
            var mock = new MockDatabaseAccess();
            List<User> results;
            var format = FormattableStringFactory.Create("DELETE * FROM Users");
            mock.ProcessQuery(format, out results);

            Assert.IsTrue(mock.QueryBuffer.Count != 0 && mock.QueryBuffer.Contains(format));
        }

        [TestMethod]
        public void ProcessQuerySelectShouldHaveNotNullResultsAndReturnTrue()
        {
            var mock = new MockDatabaseAccess();
            List<User> results;
            var format = FormattableStringFactory.Create("SELECT * FROM Users");
            var success = mock.ProcessQuery(format, out results);

            Assert.IsNotNull(results);
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void ProcessQuerySelectShouldNotAddTheQueryToBuffer()
        {
            var mock = new MockDatabaseAccess();
            List<User> results;
            var format = FormattableStringFactory.Create("SELECT * FROM Users");
            mock.ProcessQuery(format, out results);

            Assert.IsTrue(mock.QueryBuffer.Count == 0);
        }

        [TestMethod]
        public void ProcessQueryIncorrectTypeShouldHaveNullResultsAndReturnFalse()
        {
            var mock = new MockDatabaseAccess();
            List<User> results;
            var format = FormattableStringFactory.Create("DESTROY Users");
            var success = mock.ProcessQuery(format, out results);

            Assert.IsNull(results);
            Assert.IsFalse(success);
        }
    }
}
