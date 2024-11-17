using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Task = EFModeling.EntityProperties.DataAnnotations.Annotations.Task;

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

        using (var database = new Database())
        {
            success = database.ExecuteQuery(query, out results);
        }

        return success;
    }
}
