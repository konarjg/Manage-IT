using System.Runtime.CompilerServices;

namespace WebTests
{
    [TestClass]
    public class MockDatabaseAccessTests
    {
        [TestMethod]
        public void ProcessQueryInsertShouldHaveNullResultsAndReturnTrue()
        {
            MockDatabaseAccess mock = new MockDatabaseAccess();
            List<User> results;
            FormattableString format = FormattableStringFactory.Create("INSERT INTO Users");
            bool success = mock.ProcessQuery(format, out results);

            Assert.IsNull(results);
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void ProcessQueryInsertShouldAddTheQueryToBuffer()
        {
            MockDatabaseAccess mock = new MockDatabaseAccess();
            List<User> results;
            FormattableString format = FormattableStringFactory.Create("INSERT INTO Users");
            mock.ProcessQuery(format, out results);

            Assert.IsTrue(mock.QueryBuffer.Count != 0 && mock.QueryBuffer.Contains(format));
        }

        [TestMethod]
        public void ProcessQueryUpdateShouldHaveNullResultsAndReturnTrue()
        {
            MockDatabaseAccess mock = new MockDatabaseAccess();
            List<User> results;
            FormattableString format = FormattableStringFactory.Create("UPDATE Users");
            bool success = mock.ProcessQuery(format, out results);

            Assert.IsNull(results);
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void ProcessQueryUpdateShouldAddTheQueryToBuffer()
        {
            MockDatabaseAccess mock = new MockDatabaseAccess();
            List<User> results;
            FormattableString format = FormattableStringFactory.Create("UPDATE Users");
            mock.ProcessQuery(format, out results);

            Assert.IsTrue(mock.QueryBuffer.Count != 0 && mock.QueryBuffer.Contains(format));
        }

        [TestMethod]
        public void ProcessQueryDeleteShouldHaveNullResultsAndReturnTrue()
        {
            MockDatabaseAccess mock = new MockDatabaseAccess();
            List<User> results;
            FormattableString format = FormattableStringFactory.Create("DELETE * FROM Users");
            bool success = mock.ProcessQuery(format, out results);

            Assert.IsNull(results);
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void ProcessQueryDeleteShouldAddTheQueryToBuffer()
        {
            MockDatabaseAccess mock = new MockDatabaseAccess();
            List<User> results;
            FormattableString format = FormattableStringFactory.Create("DELETE * FROM Users");
            mock.ProcessQuery(format, out results);

            Assert.IsTrue(mock.QueryBuffer.Count != 0 && mock.QueryBuffer.Contains(format));
        }

        [TestMethod]
        public void ProcessQuerySelectShouldHaveNotNullResultsAndReturnTrue()
        {
            MockDatabaseAccess mock = new MockDatabaseAccess();
            List<User> results;
            FormattableString format = FormattableStringFactory.Create("SELECT * FROM Users");
            bool success = mock.ProcessQuery(format, out results);

            Assert.IsNotNull(results);
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void ProcessQuerySelectShouldNotAddTheQueryToBuffer()
        {
            MockDatabaseAccess mock = new MockDatabaseAccess();
            List<User> results;
            FormattableString format = FormattableStringFactory.Create("SELECT * FROM Users");
            mock.ProcessQuery(format, out results);

            Assert.IsTrue(mock.QueryBuffer.Count == 0);
        }

        [TestMethod]
        public void ProcessQueryIncorrectTypeShouldHaveNullResultsAndReturnFalse()
        {
            MockDatabaseAccess mock = new MockDatabaseAccess();
            List<User> results;
            FormattableString format = FormattableStringFactory.Create("DESTROY Users");
            bool success = mock.ProcessQuery(format, out results);

            Assert.IsNull(results);
            Assert.IsFalse(success);
        }
    }
}
