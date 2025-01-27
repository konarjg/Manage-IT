using EFModeling.EntityProperties.DataAnnotations.Annotations;

public class DatabaseAccess
{
    public static DatabaseAccess Instance { get; private set; }

    public DatabaseAccess()
    {

    }

    public static void Instantiate()
    {
        Instance = new DatabaseAccess();
    }

    public bool ExecuteQuery<T>(FormattableString query, out List<T> results) where T : class
    {
        bool success;

        using (Database database = new Database())
        {
            success = database.ExecuteQuery(query, out results);
        }

        return success;
    }
}
