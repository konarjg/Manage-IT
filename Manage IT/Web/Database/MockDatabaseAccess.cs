namespace Web.Database
{
    public class MockDatabaseAccess
    {
        public List<FormattableString> QueryBuffer { get; private set; }

        public MockDatabaseAccess()
        {
            QueryBuffer = new List<FormattableString>();
        }

        private void OnDatabaseUpdateRequested()
        {

        }

        public bool ProcessQuery<T>(FormattableString query, out List<T> results) where T : class
        {
            if (query.Format.Contains("INSERT") || query.Format.Contains("UPDATE") || query.Format.Contains("DELETE"))
            {
                QueryBuffer.Add(query);
                results = null;
                return true;
            }

            return ExecuteQuery(query, out results);
        }

        private bool ExecuteQuery<T>(FormattableString query, out List<T> results) where T : class
        {
            switch (query.Format)
            {
                case string s when s.Contains("INSERT"):
                    results = null;
                    return true;

                case string s when s.Contains("UPDATE"):
                    results = null;
                    return true;

                case string s when s.Contains("DELETE"):
                    results = null;
                    return true;

                case string s when s.Contains("SELECT"):
                    results = new List<T>();
                    return true;
            }

            results = null;
            return false;
        }
    }
}
